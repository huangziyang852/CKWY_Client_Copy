using Cysharp.Threading.Tasks;
using Game.Ingame.Controller;
using Game.Ingame.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateHeroRun : PlaceBattleState
    {
        private float ENEMY_UNIT_OFFSET = 15.0f;
        private int arrivedUnitCount = 0;
        public PlaceBattleStateHeroRun(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.HERO_RUN)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            if (this.PlaceBattleMgr.enemyUnitDataList.Count == 0)
            {
                return PlaceBattleStatePhase.NONE;
            }
            return PlaceBattleStatePhase.BATTLE;
        }

        public override void OnEnter()
        {
            Debug.Log("进入移动状态");
            base.OnEnter();
            if (PlaceBattleMgr.enemyUnitDataList.Count == 0)
            {
                RunNext(1);
            }
            else
            {
                float enemyPosition = FindFirstEnemyPositionX();
                UnitRun(enemyPosition);
            }
        }

        public override void Update()
        {

        }

        private void UnitRun(float distance)
        {
            Debug.Log("开始移动,距离:"+distance);
            arrivedUnitCount = 0;
            foreach (PlayerUnitData playerUnit in PlaceBattleMgr.playerUnitDataList)
            {
                //清除已填加的事件
                playerUnit.UnitController.OnArrived -= HandleUnitArrived;
                playerUnit.UnitController.OnArrived += HandleUnitArrived;
                playerUnit.UnitController.MoveToPosition(new Vector3(distance- ENEMY_UNIT_OFFSET, playerUnit.UnitController.unitGameObject.transform.position.y, 0));
            }
        }

        private float FindFirstEnemyPositionX()
        {
            EnemyUnitData enemyUnit = PlaceBattleMgr.enemyUnitDataList.First();
            PlayerUnitData playerUnit = PlaceBattleMgr.playerUnitDataList.First();
            return enemyUnit.UnitController.unitGameObject.transform.position.x;
        }

        private void HandleUnitArrived(UnitController unit)
        {
            arrivedUnitCount++;
            Debug.Log($"{unit.unitGameObject.name} 抵达，当前抵达数量: {arrivedUnitCount}/{PlaceBattleMgr.playerUnitDataList.Count}");
            if (arrivedUnitCount == PlaceBattleMgr.playerUnitDataList.Count)
            {
                Debug.Log("所有玩家单位到达！进入战斗状态");
                RunNext(1);
            }
        }
    }
}
