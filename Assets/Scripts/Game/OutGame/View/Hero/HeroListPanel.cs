using System.Collections.Generic;
using Game.Core.Manager;
using Game.Model;
using UnityEngine;

namespace Game.OutGame.View.Hero
{
    public class HeroListPanel : BasePanel<HeroListPanel>
    {
        public GameObject heroGridPrefab;
        public Transform contentTrans;
        private List<HeroModel> heroList;

        // Start is called before the first frame update
        private void Start()
        {
            //CreateHeroList();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Init()
        {
            CreateHeroList();
        }

        private void CreateHeroList()
        {
            foreach (Transform child in contentTrans) Destroy(child.gameObject);

            heroList = GameModelManager.Instance.HeroInfoModel.Heroes;
            Debug.Log(heroList.Count);
            foreach (var hero in heroList)
            {
                var heroGridCom = Instantiate(heroGridPrefab, contentTrans).GetComponent<HeroGridCom>();
                heroGridCom.Init(hero.HeroId);
            }
        }
    }
}