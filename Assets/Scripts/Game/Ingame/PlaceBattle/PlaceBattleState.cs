using Game.OutGame.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public abstract class PlaceBattleState
    {
        public PlaceBattleStatePhase PlaceBattleStatePhase
        {
            private set;
            get;
        }
        protected PlaceBattleStateMachine StateMachine;
        protected PlaceBattleMgr PlaceBattleMgr;

        public PlaceBattleState(PlaceBattleStateMachine stateMachine, PlaceBattleStatePhase battleStatePhase)
        {
            this.StateMachine = stateMachine;
            this.PlaceBattleStatePhase = battleStatePhase;
            this.PlaceBattleMgr = stateMachine.PlaceBattleMgr;
        }

        public void RunNext(float delayTime = 0)
        {
            var nextSate = GetNextPlaceBattleStatePhase();
            if (delayTime > 0)
            {
                this.StateMachine.DelayRun(nextSate, delayTime);
            }
            else
            {
                this.StateMachine.PlaceBattleStatePhase = nextSate;
            }
        }

        /// <summary>
        /// 获取下一个状态枚举
        /// </summary>
        /// <returns></returns>
        public abstract PlaceBattleStatePhase GetNextPlaceBattleStatePhase();

        /// <summary>
        /// 状态进入逻辑
        /// </summary>
        public virtual void OnEnter() { }
        /// <summary>
        /// 状态退出逻辑
        /// </summary>
        public virtual void OnExit() { }
        /// <summary>
        /// 释放销毁
        /// </summary>
        public virtual void Release() { }

        public virtual void Update() { }
    }
}
