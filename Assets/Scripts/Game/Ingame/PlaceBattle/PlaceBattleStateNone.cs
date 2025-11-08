using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateNone : PlaceBattleState
    {
        public PlaceBattleStateNone(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.NONE)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            Debug.Log("get next state");
            return PlaceBattleStatePhase.INIT;
        }
    }
}
