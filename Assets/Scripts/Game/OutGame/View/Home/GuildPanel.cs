using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Core.GameRoot;
using Game.Core.Manager;
using Game.OutGame.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace Game.OutGame.View.Home
{
    public class GuildPanel : BasePanel<GuildPanel>
    {
        public Button heroBtn;
        public Button countryBtn;
        public Button inventoryBtn;
        public Button adventureBtn;

        private RectTransform rectTransform;

        // Start is called before the first frame update
        private void Start()
        {
            heroBtn.onClick.AddListener(TransitionToHeroPanel);
            inventoryBtn.onClick.AddListener(TransitionToItemPanel);
            countryBtn.onClick.AddListener(TransitionToHomePanel);
            adventureBtn.onClick.AddListener(TranstionToBattleScene);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void TransitionToHeroPanel()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Hero);
            UiManager.Instance.CloseAllPanel();
            HeroMgr.Instance.Initialize();
        }

        private void TransitionToItemPanel()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Item);
            UiManager.Instance.CloseAllPanel();
            ItemMgr.Instance.Initialize();
        }

        private void TransitionToHomePanel()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Main);
            UiManager.Instance.CloseAllPanel();
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.HomePanel, panel => { });
            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel, panel => { });
        }

        private async void TranstionToBattleScene()
        {
            Debug.Log("transition to battle scene");
            var package = YooAssets.TryGetPackage("RemoteResource");
            AssetInfo[] assetInfos = package.GetAssetInfos("Battle");
            List<UniTask> preloadTasks = new List<UniTask>();

            foreach (var assetInfo in assetInfos)
            {
                // 调用异步加载
                var handle = YooAssets.LoadAssetAsync(assetInfo);

                // 把 handle 转换为 UniTask（等待加载完成）
                preloadTasks.Add(handle.ToUniTask());
            }

            await GameRoot.Instance.TransiationToScene("Battle", () => {},preloadTasks);
        }
    }
}