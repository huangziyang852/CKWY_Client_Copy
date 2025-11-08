using Game.Common.Const;
using Game.Core.Manager;
using Game.Ingame.Controller;
using Game.Ingame.Model;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateInit : PlaceBattleState
    {
        public PlaceBattleStateInit(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.INIT)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            return PlaceBattleStatePhase.ENEMY_ENTER;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            CreatePlayerUnitData();
        }

        public override void OnExit()
        {

        }

        /// <summary>
        /// 创建角色
        /// </summary>
        private void CreatePlayerUnitData()
        {

            ResourceLoader.Instance.LoadRemotePrefab(GameConst.PrefabPath.Batttle.Unit.PLAYER_UNIT, prefab =>
            {
                List<PlayerUnitData> playerUnitDatas = PlaceBattleMgr.playerUnitDataList;
                foreach (PlayerUnitData playerUnitData in playerUnitDatas)
                {
                    HeroModel heroModel = GameModelManager.Instance.GetHeroModelByHeroCd(playerUnitData.HeroModel.HeroCd);
                    GameObject instance = GameObject.Instantiate(prefab, BattleConst.PLAYER_UNIT_POSITION[playerUnitData.DeckPosition], Quaternion.identity, BattleRoot.Instance.unitLayer.transform);
                    instance.name = "Unit" + heroModel.HeroId;
                    PlayerUnitController controller = instance.GetComponent<PlayerUnitController>();
                    controller.Init(heroModel);
                    //这里给controller赋值
                    playerUnitData.UnitController = controller;
                }
                PlayerUnitData firstHero = playerUnitDatas.Find(unitData => unitData.DeckPosition == 0);
                BattleRoot.Instance.SetCameraPlayer(firstHero.UnitController.transform);
            });
            //加载敌人

            //进入下一步
            RunNext(1);
        }
    }
}