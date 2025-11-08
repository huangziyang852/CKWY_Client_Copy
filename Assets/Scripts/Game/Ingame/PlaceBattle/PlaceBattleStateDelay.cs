using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateDelay : PlaceBattleState
    {
        private PlaceBattleStatePhase targetPhase;
        private float delayTime;
        public PlaceBattleStateDelay(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.DELAY)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            return targetPhase;
        }

        public void SetInfo(PlaceBattleStatePhase target, float time)
        {
            targetPhase = target;
            delayTime = time;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public override void Update()
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0)
            {
                RunNext();
            }
        }
    }
}
