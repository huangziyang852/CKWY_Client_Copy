using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public enum UnitState
    {
        /// <summary>
        /// ¾²Ö¹×´Ì¬
        /// </summary>
        IDLE,
        /// <summary>
        /// ±¼ÅÜ×´Ì¬
        /// </summary>
        MOVE,
        /// <summary>
        /// ¹¥»÷×´Ì¬
        /// </summary>
        ATTACK
    }


    public class UnitStateMachine
    {
        private UnitController owner;
        private IUnitState<UnitController> currentState;
        private IUnitState<UnitController>[] unitStates = new IUnitState<UnitController>[Enum.GetValues(typeof(UnitState)).Length];

        public UnitStateMachine(UnitController owner)
        {
            this.owner = owner;
            InitStates();
        }

        private void InitStates()
        {
            Debug.Log("init unit state");
            unitStates[(int)UnitState.IDLE] = new UnitIdleState();
            unitStates[(int)UnitState.MOVE] = new UnitMoveState();
            unitStates[(int)UnitState.ATTACK] = new UnitAttackState();
        }

        public void ChangeState(UnitState unitState)
        {
            currentState?.OnExit();
            currentState = unitStates[(int)unitState];
            currentState?.OnEnter(owner);
        }

        public void Update()
        {
            currentState?.OnUpdate();
        }

        public void SetMoveTarget(Vector3 position)
        {
            UnitMoveState moveState = unitStates[(int)UnitState.MOVE] as UnitMoveState;
            if (moveState != null)
            {
                moveState.SetTarget(position);
            }
        }
    }
}
