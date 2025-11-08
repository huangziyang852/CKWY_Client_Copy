using System;
using System.Text.RegularExpressions;
using Game.Common.Const;
using Game.Common.Utils;
using Game.Config;
using Game.Core.Manager;
using Game.Core.Net;
using Game.Core.Net.HttpProto;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Login
{
    public class RegisterPopup : BasePanel<RegisterPopup>
    {
        public Button closeBtn;
        public Button cancelBtn;
        public Button RegisterBtn;
        public TMP_Text accountInput;
        public TMP_Text passwordInput;

        // Start is called before the first frame update
        private void Start()
        {
            name = GameConst.Popup.RigitserPopup;
            closeBtn.onClick.AddListener(ClosePanel);
            cancelBtn.onClick.AddListener(ClosePanel);
            RegisterBtn.onClick.AddListener(OnRegister);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnRegister()
        {
            var account = Regex.Replace(accountInput.text, @"\u200B", "");
            var password = Regex.Replace(passwordInput.text, @"\u200B", "");

            if (checkInput(account, password)) GetLoginToken(account, password);
        }

        private bool checkInput(string account, string password)
        {
            var pattern = @"^[a-zA-Z0-9_]+$";
            var input = "10001";

            var isValid = Regex.IsMatch(input, pattern);
            Console.WriteLine($"Is '{input}' valid? {isValid}");

            var isAccountValid = CommonUtils.ValidateAccount(account);
            var isPasswordValid = CommonUtils.ValidatePassword(password);
            // 账号或密码不合法
            if (!isAccountValid)
            {
                var notification = "账号为6到15个字符,允许使用字母,数字和下划线";
                UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, notification);
                return false;
            }

            if (!isPasswordValid)
            {
                var notification = "密码为8到20个字符";
                UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, notification);
                return false;
            }

            // 账号和密码都合法
            UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, "login success");
            Debug.Log("账号和密码验证通过");

            return true;
        }

        public void GetLoginToken(string account, string password)
        {
            var LoginRoot_Url = ConfigManager.NetInfo.LoginRootUrl;
            PlayerPrefs.SetString(UserPrefs.LAST_USER_ID, account);
            var input = account + password;
            var sign = CommonUtils.CalculateMD5(input);
            var sendUrl = string.Format("{0}/UserLogin", LoginRoot_Url);
            Debug.Log("send request:" + sendUrl);
            var form = new WWWForm();
            form.AddField("account", account);
            form.AddField("sign", sign);
            NetWorkService.Instance.SendPostHttp(sendUrl, form, OnLoginReceive, OnLoginFailed);
        }

        private void OnLoginReceive(string jsonData)
        {
            var response = JsonConvert.DeserializeObject<ApiResponse<UserLoginResponse>>(jsonData);
            if (response.Success)
            {
                var openId = response.Data.OpenId;
                var token = response.Data.LoginToken;
                var refreshToken = response.Data.RefreshToken;
                PlayerPrefs.SetString(UserPrefs.LOGIN_TOKEN, token);
                PlayerPrefs.SetString(UserPrefs.OPEN_ID, openId);
                PlayerPrefs.SetString(UserPrefs.REFRESH_TOKEN, refreshToken);
                ClosePanel();
            }

            if (response.Success == false)
            {
                UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, response.Message);
                Debug.Log(response.Message);
            }
        }

        private void OnLoginFailed()
        {
            UiManager.Instance.OpenNotification(GameConst.Notification.SystemNotification, "Login Failed");
        }

        public override void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}