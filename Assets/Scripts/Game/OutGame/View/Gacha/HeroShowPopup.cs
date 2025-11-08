using cfg.hero;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Common.Utils;
using Game.Core.Manager;
using Game.OutGame.View.Login;
using LaunchPB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Game.Common.Const.GameConst;

namespace Game.OutGame.View.Gacha
{
    public class HeroShowPopup : BasePanel<HeroShowPopup>,IPointerClickHandler
    {
        public Action onClickAnywhere;
        public Action OnClosed;
        public Image heroStand;
        public TextMeshProUGUI heroName1;
        public TextMeshProUGUI heroName2;
        public Image[] starGroup;
        private cfg.hero.Hero heroMaster;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClickAnywhere?.Invoke();
            OnClosed?.Invoke();
            Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public async Task Init(int heroId)
        {
            heroMaster = TableManager.Instance.MasterTables.TbHero.Get(heroId);
            Sprite sprite = await ResourceUtils.GetHeroSprite(HeroImageType.Stand, heroId);
            heroStand.sprite = sprite;
            heroStand.SetNativeSize();
            heroName1.text=heroMaster.Name;
            heroName2.text=heroMaster.Label;
            SetStarGroup(heroMaster.Star);
        }

        private void SetStarGroup(int count)
        {
            for(int i = 0; i < count; i++)
            {
                starGroup[i].gameObject.SetActive(true);
            }
        }

        public override void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}
