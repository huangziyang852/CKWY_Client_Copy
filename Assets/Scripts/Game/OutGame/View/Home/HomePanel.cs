using Game.Common.Const;
using Game.Core.Manager;
using Game.OutGame.Controller;
using TMPro;
using UnityEngine.UI;

namespace Game.OutGame.View.Home
{
    public class HomePanel : BasePanel<HomePanel>
    {
        public TextMeshProUGUI playerNameText;

        public TextMeshProUGUI playerLevel;

        public TextMeshProUGUI goldCount;

        public TextMeshProUGUI diamondCount;

        public Button gachaBtn;

        // Start is called before the first frame update

        private void OnEnable()
        {
            BackGroundManager.Instance.SetBackGroundImage(BgImageType.Home, GameConst.BgImageName.Home);
        }
        private void Start()
        {
            UiManager.Instance.CloseUIPanel(GameConst.UiPanel.LoadingPanel);
            SetPlayerInfo();

            gachaBtn.onClick.AddListener(TransitionToGacha);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void SetPlayerInfo()
        {
            playerNameText.text = GameModelManager.Instance.PlayerInfoModel.Name;
            playerLevel.text = GameModelManager.Instance.PlayerInfoModel.Level.ToString();
            goldCount.text = GameModelManager.Instance.PlayerInfoModel.Gold.ToString();
            diamondCount.text = GameModelManager.Instance.PlayerInfoModel.Diamond.ToString();
        }

        private void TransitionToGacha()
        {
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.Gacha);
            GachaMgr.Instance.Initialize();
        }
    }
}