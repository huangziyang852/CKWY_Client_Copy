using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Config;
using Game.Core.GameRoot;
using Game.Core.Manager;
using Game.Core.Net;
using Game.Core.Net.HttpProto;
using Game.Core.Net.Service;
using Game.OutGame.View;
using Game.OutGame.View.Login;
using Google.Protobuf;
using LaunchPB;
using Newtonsoft.Json;
using UnityEngine;
using YooAsset;

namespace Game.OutGame.Controller
{
    public class LoginMgr : BaseMgr<LoginMgr>
    {
        public ServerInfo gameServer;
        private LoadingPanel loadingPanel;
        private LoginPanel loginPanel;
        private ServerInfo selectedServer;

        public override void Initialize()
        {
            Debug.Log("LoginMgr initialized.");
        }

        public void StartDownloadResource()
        {
        }

        public async void RequestServerList()
        {
            loadingPanel = UiManager.Instance.InitUIPanel(GameConst.UiPanel.LoadingPanel).GetComponent<LoadingPanel>();
            loadingPanel.SetProgress(0.1f);
            //这里先检查新版本
            ResourceLoader.Instance.InitYooAssets();
            //下载资源
            await ResourceLoader.Instance.InitPackage("Master");
            loadingPanel.SetProgress(0.3f);
            //加载数据表
            var success = await TableManager.Instance.LoadMasterTableAsync();
            await ResourceLoader.Instance.InitPackage("RemoteResource");
            loadingPanel.SetProgress(0.5f);

            //加载资源
            List<UniTask> preloadTasks = new List<UniTask>();
            var package = YooAssets.TryGetPackage("RemoteResource");
            AssetInfo[] assetInfos = package.GetAssetInfos("UI");
            foreach (var assetInfo in assetInfos)
            {
                var handle = YooAssets.LoadAssetAsync(assetInfo);
                preloadTasks.Add(handle.ToUniTask());
            }

            await UniTask.WhenAll(preloadTasks);

            loadingPanel.SetProgress(0.7f);

            var LoginRoot_Url = ConfigManager.NetInfo.LoginRootUrl;
            var LoginRoot_Channel = ConfigManager.NetInfo.LoginRootChannel;
            var LoginRoot_SubChannel = ConfigManager.NetInfo.LoginRootSubChannel;
            var LoginRoot_Version = ConfigManager.NetInfo.LoginRootVersion;
            var sendUrl =
                string.Format("{0}/getServerList?channel={1}&plat=android&sub_channel={2}&server_version={3}",
                    LoginRoot_Url, LoginRoot_Channel, LoginRoot_SubChannel, LoginRoot_Version);
            Debug.Log(sendUrl);
            NetWorkService.Instance.SendGetHttp(sendUrl, OnReceiveServerList, OnReceiveServerListFailed);
        }

        private void OnReceiveServerList(string jsonData)
        {
            Debug.Log(jsonData);
            //UiManager.Instance.CloseUIPanel(GameConst.UiPanel.LoadingPanel);
            if (!string.IsNullOrEmpty(jsonData))
            {
                // �����յ���JSON�ַ���������һ���б�
                var response = JsonConvert.DeserializeObject<ApiResponse<ServerInfo>>(jsonData);

                gameServer = response.Data;

                if (gameServer != null)
                {
                    RequestConnectTOGameServer(gameServer);
                    loadingPanel.SetProgress(1);
                    UiManager.Instance.InitUIPanel(GameConst.UiPanel.LoginPanel, panel =>
                    {
                        UiManager.Instance.CloseUIPanel(GameConst.UiPanel.LoadingPanel);
                        loginPanel = panel.GetComponent<LoginPanel>();
                        loginPanel.SetTips();
                        loginPanel.OnLoginRequest += LoginToGameServer;
                    });
                }
            }
            else
            {
                Debug.LogError("JSON data is null or empty!");
                UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, "获取服务器列表失败");
            }
        }

        private void OnReceiveServerListFailed()
        {
            Debug.LogError("receive server list failed");
        }

        private void RequestConnectTOGameServer(ServerInfo serverInfo)
        {
            Debug.Log("request socket login");
            UiManager.Instance.OpenLoadingPanel();

            TcpService.Instance.TCPConnect(serverInfo);
        }

        private void LoginToGameServer()
        {
            var loginToken = PlayerPrefs.GetString(UserPrefs.LOGIN_TOKEN, "");
            var openId = PlayerPrefs.GetString(UserPrefs.OPEN_ID, "");
            if (loginToken == "" || openId == "")
                UiManager.Instance.OpenPopup(GameConst.Popup.RigitserPopup, popup =>
                {
                    //registerPopup = popup.GetComponent<RegisterPopup>();
                });
            Login(openId, loginToken);
        }


        private void Login(string openId, string token)
        {
            Debug.Log("login into gameserver:openId" + openId);
            var openId_long = long.Parse(openId);
            var loginRequest = new Login { OpenId = openId_long, LoginToken = token };

            var serializedData = loginRequest.ToByteArray();

            TcpService.Instance.SendMessageWithCallBack((int)ProtoCode.ELogin, (int)ProtoCode.ELoginResp,
                serializedData, OnLoginResponse);
        }

        private void OnLoginResponse(NetPacket response)
        {
            try
            {
                IMessage message = new LoginResp();
                var login = message.Descriptor.Parser.ParseFrom(response.PacketBodyBytes, 0,
                    response.PacketBodyBytes.Length) as LoginResp;
                if (login != null)
                {
                    if (login.Result == LoginResult.ETokenWrong || login.Result == LoginResult.EOpenIdWrong)
                    {
                        var refreshToken = PlayerPrefs.GetString(UserPrefs.REFRESH_TOKEN, "");
                        if (string.IsNullOrEmpty(refreshToken))
                            UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification,
                                "token is wrong or out of time");
                        else
                            RefreshLoginToken(refreshToken);
                    }
                    else
                    {
                        _ = OnLoginSuccess(login); //这里不关心什么时候完成，什么时候完成什么时候跳转
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse LoginResp message.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing LoginResp: {e.Message}");
            }
        }

        private void RefreshLoginToken(string refreshToken)
        {
            var LoginRoot_Url = ConfigManager.NetInfo.LoginRootUrl;
            var sendUrl = string.Format("{0}/refreshToken", LoginRoot_Url);
            Debug.Log("send request:" + sendUrl);
            var form = new WWWForm();
            var openId = PlayerPrefs.GetString(UserPrefs.OPEN_ID, "");
            form.AddField("openId", openId);
            form.AddField("refreshToken", refreshToken);
            NetWorkService.Instance.SendPostHttp(sendUrl, form, OnRefreshTokenReceive, OnRefreshTokenFailed);
        }

        private void OnRefreshTokenReceive(string jsonData)
        {
            var response = JsonConvert.DeserializeObject<ApiResponse<RefreshTokenResponse>>(jsonData);
            if (response.Success)
            {
                var token = response.Data.LoginToken;
                var refreshToken = response.Data.RefreshToken;
                PlayerPrefs.SetString(UserPrefs.LOGIN_TOKEN, token);
                PlayerPrefs.SetString(UserPrefs.REFRESH_TOKEN, refreshToken);

                var openId = PlayerPrefs.GetString(UserPrefs.OPEN_ID, "");
                Login(openId, token);
            }
            else
            {
                if (response.Success == false)
                {
                    UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, response.Message);
                    Debug.Log(response.Message);
                }
            }
        }

        private void OnRefreshTokenFailed()
        {
            UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, "refresh token Failed");
        }

        public async Task OnLoginSuccess(LoginResp loginResp)
        {
            await InitPlayerData();
        }

        private async Task InitPlayerData()
        {
            Debug.Log("设置用户信息");

            List<UniTask> preloadTasks = new List<UniTask>();
            preloadTasks.Add(GameModelManager.Instance.GetPlayerInfoFromServerAsync());
            preloadTasks.Add(GameModelManager.Instance.GetHeroInfoFromServerAsync());
            preloadTasks.Add(GameModelManager.Instance.GetWorldInfoFromServerAsync());
            preloadTasks.Add(GameModelManager.Instance.GetItemInfoFromServerAsync());

            var package = YooAssets.TryGetPackage("RemoteResource");
            AssetInfo[] assetInfos = package.GetAssetInfos("UI");
            foreach (var assetInfo in assetInfos)
            {
                var handle = YooAssets.LoadAssetAsync(assetInfo);
                preloadTasks.Add(handle.ToUniTask());
            }

            //await UniTask.SwitchToMainThread(); //切换回主线程！！！
            HeartBeatService.Instance.Start(); //开始发送心跳包
            await GameRoot.Instance.TransiationToScene("Home", () => { HomeMgr.Instance.Initialize(); },preloadTasks);
        }
    }
}