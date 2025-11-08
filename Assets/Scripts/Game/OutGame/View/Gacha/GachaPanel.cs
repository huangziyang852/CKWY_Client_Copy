using cfg.item;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Common.Utils;
using Game.Core.Animation;
using Game.Core.Manager;
using Game.Model;
using Game.OutGame.UIComp;
using Game.OutGame.View.Common;
using Game.OutGame.View.Login;
using LaunchPB;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Gacha
{
    public class GachaPanel : BasePanel<GachaPanel>
    {
        private static readonly Vector3[] cardEffectPos = new Vector3[] { new Vector3(-389, 22, 0), new Vector3(-645, 22, 0), new Vector3(-507, 22, 0) };
        public GameObject header;
        public GameObject leftButtonGroup;
        public GameObject rightButtonGroup;
        public GameObject Card;
        public Button backBtn;
        public CurrencyItemComp diamondCurrencyItemComp;
        public CurrencyItemComp ticketCurrencyItemComp;
        public TabBase gachaTab;
        public Button oneTimeBtn;
        public Button tenTimesBtn;
        public UISpineAnimatorBase bgSpine;
        public UISpineAnimatorBase cardSpine;
        public UISpineAnimatorBase cardEffectSpine;
        public UISpineAnimatorBase gachaEffect;
        public UISpineAnimatorBase gachaEffectFront;
        public UISpineAnimatorBase gachaEffectBack;
        public UISpineAnimatorBase flashEffect;
        private int selectedGachaId;

        private void OnEnable()
        {
            EventBus<GachaResultEvent>.Subscribe(OnGachaResult);
            BackGroundManager.Instance.SetBackGroundImage(BgImageType.Gacha, GameConst.BgImageName.Gacha);
            SetCardSpine();
            SetDiamondCurrency();
            SetTicketCurreny();
        }

        private void OnDisable()
        {
            BackGroundManager.Instance.ClearBackGroundSpine();
            EventBus<GachaResultEvent>.Unsubscribe(OnGachaResult);
        }

        // Start is called before the first frame update
        private void Start()
        {
            gachaTab.onTabChanged = OnTabChanged;
            oneTimeBtn.onClick.AddListener(() => GachaExcute(1));
            tenTimesBtn.onClick.AddListener(() => GachaExcute(10));
            backBtn.onClick.AddListener(() => { TransitionToHomePanel(); });
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void SetDiamondCurrency()
        {
            diamondCurrencyItemComp.Init(GameConst.CurrencyId.Diamond);
        }

        private void SetTicketCurreny()
        {
            switch (selectedGachaId)
            {
                case (int)GachaType.Normal:
                    ticketCurrencyItemComp.Init(GameConst.CurrencyId.NormalGachaTicket);
                    break;
                case (int)GachaType.Friend:
                    ticketCurrencyItemComp.Init(GameConst.CurrencyId.FriendGachaTicket);
                    break;
                case (int)GachaType.Super:
                    ticketCurrencyItemComp.Init(GameConst.CurrencyId.SuperGachaTicket);
                    break;
            }
        }

        public event Action<int, int> OnGachaRequest;

        private void OnTabChanged(int newId)
        {
            if(newId == selectedGachaId) return;
            cardEffectSpine.gameObject.SetActive(false);
            selectedGachaId = newId;
            flashEffect.PlayAnimationOnce("animation");
            SetCardSpine();
            SetTicketCurreny();
        }

        private void GachaExcute(int count)
        {
            Debug.Log("gacha " + count + "times");
            EventBus<GachaRequestEvent>.Publish(new GachaRequestEvent(selectedGachaId, count));
        }

        private void TransitionToHomePanel()
        {
            UiManager.Instance.CloseAllPanel();
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.HomePanel, panel => { });
            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel, panel => { });
        }

        private void OnGachaResult(GachaResultEvent e)
        {
            Debug.Log("收到抽卡结果"+e.GachaResult);
            PlayGachaAnimation(e.GachaResult);
        }

        private async void SetCardSpine()
        {
            SkeletonDataAsset skeletonDataAsset = null;
            switch (selectedGachaId)
            {
                case (int)GachaType.Normal:
                    skeletonDataAsset = await ResourceUtils.GetGachaCardSpineSkeletonData(GachaType.Normal);
                    break;
                case (int)GachaType.Friend:
                    skeletonDataAsset = await ResourceUtils.GetGachaCardSpineSkeletonData(GachaType.Friend);
                    break;
                case (int)GachaType.Super:
                    skeletonDataAsset = await ResourceUtils.GetGachaCardSpineSkeletonData(GachaType.Super);
                    break;
            }
            cardSpine.SetSpineAsset(skeletonDataAsset);
            bgSpine.PlayAnimationOnce("enter", () =>
            {
                cardEffectSpine.gameObject.transform.localPosition = cardEffectPos[selectedGachaId];
                cardEffectSpine.gameObject.SetActive(true);
            });
        }

        private void SetAllUIGroup(bool active)
        {
            if (active == false)
            {
                UiManager.Instance.HideSubPanel();
            }
            else
            {
                UiManager.Instance.ShowSubPanel();
            }

            header.SetActive(active);
            leftButtonGroup.SetActive(active);
            rightButtonGroup.SetActive(active);
            Card.SetActive(active);
        }

        private void PlayGachaAnimation(GachaResultResp gachaResult)
        {
            SetAllUIGroup(false);
            gachaEffect.gameObject.SetActive(true);
            gachaEffectFront.gameObject.SetActive(true);
            gachaEffectBack.gameObject.SetActive(true);
            gachaEffectFront.PlayAnimationOnce("take");
            gachaEffectBack.PlayAnimationOnce("take");
            SoundManager.Instance.PlayClickSound(GameConst.AudioName.UIAudioName.GachaExcute);
            List<int> heroResultList = gachaResult.GachaHeroResult.ToList();
            gachaEffect.PlayAnimationOnce("take", () =>
            {
                ShowHeroResults(heroResultList, () =>
                {
                    OpenRewardPopup(gachaResult);
                });
            });

        }

        public void ShowHeroResults(List<int> heroList, Action onHeroPopupClosed)
        {
            if (heroList == null || heroList.Count == 0) return;

            if(heroList.Count > 1)
            {

            }
            else
            {
                OpenHeroPopup(heroList[0], onHeroPopupClosed);
            }
        }

        private void OpenHeroPopup(int heroId, Action onClosed)
        {
            UiManager.Instance.OpenPopup(GameConst.Popup.HeroShowPopup, async popup =>
            {
                HeroShowPopup heroShowPopup = popup.GetComponent<HeroShowPopup>();
                heroShowPopup.OnClosed = onClosed;
                await heroShowPopup.Init(heroId);
            });
        }

        private void OpenRewardPopup(GachaResultResp gachaResultResp)
        {
            SetAllUIGroup(true);
            List<ItemModel> items = gachaResultResp.AddItem.Select(i => new ItemModel(i.ItemId, i.ItemCount)).ToList();
            if (items.Count > 0)
            {
                UiManager.Instance.OpenPopup(GameConst.Popup.RewardCommonPopup, popup =>
                {
                    RewardCommonPopup rewardCommonPopup = popup.GetComponent<RewardCommonPopup>();
                    rewardCommonPopup.Init(items,"星数が3以下の英雄が自動的に交換されます。");
                });
            }
        }
    }
}