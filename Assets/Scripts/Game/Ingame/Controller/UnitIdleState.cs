using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public class UnitIdleState : IUnitState<UnitController>
    {
        private UnitController unit;

        public UnitIdleState()
        {

        }

        public void OnEnter(UnitController unitController)
        {
            this.unit = unitController;
            this.unit.RoleSpineAnimator.PlayIdle();
            Debug.Log(unit.name + " 开始待机");
        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {
            Debug.Log(unit.name + " 待机结束");
        }
    }
}
