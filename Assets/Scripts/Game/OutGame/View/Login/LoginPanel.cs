using Game.Common.Const;
using Game.Core.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Login
{
    public class LoginPanel : BasePanel<LoginPanel>
    {
        public Button startBtn;
        public Button changeServerBtn;
        public Button rigisterBtn;
        public TextMeshProUGUI tips;
        private RegisterPopup registerPopup;

        private void Start()
        {
            BackGroundManager.Instance.SetBackGroundImage(BgImageType.Login, GameConst.BgImageName.Login);
            BackGroundManager.Instance.SetBackGroundSpine(BgSpineType.Login,GameConst.UISpinePrefabName.LoginSpine);
            name = GameConst.UiPanel.LoginPanel;
            startBtn.GetComponent<Button>().enabled = true;
            startBtn.interactable = true;
            rigisterBtn.interactable = true;
            rigisterBtn.onClick.AddListener(OnRigister);
            startBtn.onClick.AddListener(OnLoginButtonClick);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnDestroy()
        {
        }

        private void OnDisable()
        {
            BackGroundManager.Instance.ClearBackGroundSpine();
        }

        // 定义事件，用于处理登录请求
        public event Action OnLoginRequest;

        private void OnRigister()
        {
            UiManager.Instance.OpenPopup(GameConst.Popup.RigitserPopup,
                popup => { registerPopup = popup.GetComponent<RegisterPopup>(); });
        }

        private void OnLoginButtonClick()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Windows);
            InActiveAllButton();
            OnLoginRequest.Invoke();
        }

        private void InActiveAllButton()
        {
            //startBtn.interactable = false;
            //rigisterBtn.interactable = false;
        }

        public void SetTips()
        {
            var openId = PlayerPrefs.GetString(UserPrefs.OPEN_ID, "");
            if (openId != "") tips.text = openId;
        }
    }
}