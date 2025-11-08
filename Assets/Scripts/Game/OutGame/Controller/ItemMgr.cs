using Game.Common.Const;
using Game.Core.Manager;
using Game.OutGame.View.Hero;
using Game.OutGame.View.Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.OutGame.Controller
{
    public class ItemMgr : BaseMgr<ItemMgr>
    {
        private ItemListPanel ItemListPanel { get; set; }
        public override void Initialize()
        {
            InitItemListPanel();
        }

        private void InitItemListPanel()
        {
            //UiManager.Instance.CloseUIPanel(GameConst.UiPanel.HomePanel);
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.ItemListPanel, panel =>
            {
                ItemListPanel = panel.GetComponent<ItemListPanel>();
                ItemListPanel.Init();
            });
            UiManager.Instance.InitSubPanel(GameConst.UiPanel.GuildPanel, panel => { });
        }
    }
}
