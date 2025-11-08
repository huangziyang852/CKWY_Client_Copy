using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Common.Utils;
using Game.Core.Manager;
using Game.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.OutGame.View.Hero
{
    public class HeroGridCom : MonoBehaviour
    {
        public Image heroIcon;
        public Image campIcon;
        public Image rarityIcon;
        public TextMeshProUGUI levelText;
        public GameObject[] starGroup;
        private HeroModel _hero;
        private int _heroId;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Init(int heroId)
        {
            _heroId = heroId;
            _hero = GameModelManager.Instance.HeroInfoModel.Heroes.Find(h => h.HeroId == heroId);
            SetHeroInfo();
        }

        private void SetHeroInfo()
        {
            var heroMaster = TableManager.Instance.MasterTables.TbHero.Get(_hero.HeroId);
            ResourceUtils.GetHeroSprite(HeroImageType.Head, _heroId, sprite => { heroIcon.sprite = sprite; });
            levelText.text = _hero.Level.ToString();
            SetStarGroup(_hero.Star);
            SetCampIcon(heroMaster.Camp).Forget();
            SetRarityIcon(heroMaster.Grade).Forget();
        }

        private async UniTaskVoid SetCampIcon(int camp)
        {
            campIcon.sprite = await ResourceUtils.GetCampIconSprite(camp);
        }

        private async UniTaskVoid SetRarityIcon(int rarity)
        {
            rarityIcon.sprite = await ResourceUtils.GetRarityIconSprite(rarity);
        }

        private void SetStarGroup(int star)
        {
            for(int i = 0; i < star; i++)
            {
                starGroup[i].SetActive(true);
            }
        }
    }
}