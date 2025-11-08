using Game.Common.Const;
using Game.Core.Manager;
using Game.Core.Net;
using Game.Ingame.Model;
using Game.Ingame.PlaceBattle;
using Game.Model;
using Game.OutGame.View.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.OutGame.Controller
{
    public class PlaceBattleMgr : MonoSingleton<PlaceBattleMgr>
    {
        public int WorldId { get; private set; }
        public int QuestId { get; private set; }
        private GuajiPanel guajiPanel;
        public PlaceBattleStateMachine PlaceBattleStateMachine;
        public List<PlayerUnitData> playerUnitDataList = new List<PlayerUnitData>();
        public List<EnemyUnitData> enemyUnitDataList = new List<EnemyUnitData>();

        public void Awake()
        {

        }

        public void Init(int worldId)
        {
            WorldId = worldId;
            QuestId = 10001;
            InitPlayerUnitData();
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.GuajiPanel, panel =>
            {
                guajiPanel = panel.GetComponent<GuajiPanel>();
            });
            PlaceBattleStateMachine = new PlaceBattleStateMachine(this);

        }

        public void Update()
        {
            if (PlaceBattleStateMachine != null)
            {
                PlaceBattleStateMachine.Update();
            }
        }

        public void InitPlayerUnitData()
        {
            PlayerInfoModel playerInfo = GameModelManager.Instance.PlayerInfoModel;
            foreach (BattleSlot battleSlot in playerInfo.Deck)
            {
                HeroModel heroModel = GameModelManager.Instance.GetHeroModelByHeroCd(battleSlot.HeroCd);
                playerUnitDataList.Add(new PlayerUnitData(heroModel, battleSlot.Position));
            }
        }
    }
}
