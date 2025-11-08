using Game.OutGame.Controller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public enum PlaceBattleStatePhase
    {
        /// <summary>
        /// 无效状态
        /// </summary>
        NONE,
        /// <summary>
        /// 战斗初始化
        /// </summary>
        INIT,
        /// <summary>
        /// 卡牌入场
        /// </summary>
        CARD_ENTER,
        /// <summary>
        /// 怪物入场
        /// </summary>
        ENEMY_ENTER,
        /// <summary>
        /// 卡牌移动
        /// </summary>
        HERO_RUN,
        /// <summary>
        /// 战斗
        /// </summary>
        BATTLE,
        /// <summary>
        /// 战斗开始
        /// </summary>
        START,
        /// <summary>
        /// 回合开始
        /// </summary>
        BEGIN,
        /// <summary>
        /// 战斗回合
        /// </summary>
        CIRCLE,
        /// <summary>
        /// 结束游戏
        /// </summary>
        OVER,
        /// <summary>
        /// 剧情对话
        /// </summary>
        DRAMMA,
        /// <summary>
        /// 回合清理
        /// </summary>
        CLEAR,
        /// <summary>
        /// 状态延时
        /// </summary>
        DELAY,
        /// <summary>
        /// 直接结束
        /// </summary>
        FINISH,
        MAX,
    }


    public class PlaceBattleStateMachine
    {
        private static PlaceBattleStateMachine _instance;
        private static GameObject _gameObject;
        public PlaceBattleMgr PlaceBattleMgr;
        private PlaceBattleState[] battleStates = new PlaceBattleState[(int)PlaceBattleStatePhase.MAX];
        private PlaceBattleStatePhase _placeBattleStatePhase = PlaceBattleStatePhase.NONE;
        public bool Enable { get; private set; }
        public PlaceBattleState PlaceBattleState
        {
            get
            {
                return battleStates[(int)this._placeBattleStatePhase];
            }
        }
        public PlaceBattleStatePhase PlaceBattleStatePhase
        {
            get { return _placeBattleStatePhase; }
            set
            {
                if (this._placeBattleStatePhase != value)
                {
                    PlaceBattleStatePhase prePhase = this._placeBattleStatePhase;
                    if (this.PlaceBattleState != null)
                    {
                        this.PlaceBattleState.OnExit();
                    }


                    this._placeBattleStatePhase = value;
                    if (this.PlaceBattleState != null)
                        this.PlaceBattleState.OnEnter();
                }
            }
        }

        private void InitStates()
        {
            Debug.Log("init state machine");
            battleStates[(int)PlaceBattleStatePhase.NONE] = new PlaceBattleStateNone(this);
            battleStates[(int)PlaceBattleStatePhase.INIT] = new PlaceBattleStateInit(this);
            battleStates[(int)PlaceBattleStatePhase.DELAY] = new PlaceBattleStateDelay(this);
            battleStates[(int)PlaceBattleStatePhase.HERO_RUN] = new PlaceBattleStateHeroRun(this);
            battleStates[(int)PlaceBattleStatePhase.ENEMY_ENTER] = new PlaceBattleStateEnemyEnter(this);
            battleStates[(int)PlaceBattleStatePhase.BATTLE] = new PlaceBattleStateBattle(this);
        }

        public PlaceBattleStateMachine(PlaceBattleMgr placeBattleMgr)
        {
            PlaceBattleMgr = placeBattleMgr;
            Enable = true;
            InitStates();
            this.PlaceBattleState.RunNext();
        }

        public void DelayRun(PlaceBattleStatePhase targetPhhase, float time)
        {
            (battleStates[(int)PlaceBattleStatePhase.DELAY] as PlaceBattleStateDelay).SetInfo(targetPhhase, time);
            PlaceBattleStatePhase = PlaceBattleStatePhase.DELAY;
        }

        public void Update()
        {
            if (this.PlaceBattleState != null)
            {
                this.PlaceBattleState.Update();
            }
        }

    }
}


