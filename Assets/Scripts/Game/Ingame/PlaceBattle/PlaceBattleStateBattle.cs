using Game.Ingame.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateBattle : PlaceBattleState
    {
        public PlaceBattleStateBattle(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.BATTLE)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            return PlaceBattleStatePhase.ENEMY_ENTER;
        }

        public override void OnEnter()
        {
            Debug.Log("½øÈëÕ½¶·×´Ì¬");
            base.OnEnter();
            UnitBattle();
        }

        private void UnitBattle()
        {
            //RunNext(1);
            foreach (PlayerUnitData playerUnitData in PlaceBattleMgr.playerUnitDataList)
            {
                playerUnitData.StartBattle();
            }
        }
    }
}
