using Game.Common.Const;
using Game.Common.Utils;
using Game.Core.Manager;
using Game.Model;
using Game.OutGame.View.Common;
using Game.OutGame.View.Hero;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.OutGame.View.Item
{
    public class ItemListPanel : BasePanel<ItemListPanel>
    {
        public Transform contentTrans;
        public GameObject itemGridPrefab;
        public CurrencyItemComp goldCurrencyItemComp;
        public CurrencyItemComp diamondCurrencyItemComp;
        private List<ItemModel> itemList;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init()
        {
            InitHeader();
            CreateItemList();
        }

        private void InitHeader()
        {
            goldCurrencyItemComp.Init(GameConst.CurrencyId.Gold);
            diamondCurrencyItemComp.Init(GameConst.CurrencyId.Diamond);
        }

        private void CreateItemList()
        {
            foreach (Transform child in contentTrans) Destroy(child.gameObject);

            itemList = GameModelManager.Instance.ItemInfoModel.Items;
            Debug.Log(itemList.Count);
            foreach (var item in itemList)
            {
                var itemGridCom = Instantiate(itemGridPrefab, contentTrans).GetComponent<ItemGridCom>();
                itemGridCom.Init(item);
            }
        }
    }
}
