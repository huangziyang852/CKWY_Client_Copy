using Game.Core.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public class UnitAttackState : IUnitState<UnitController>
    {
        private UnitController unit;
        public void OnEnter(UnitController unitController)
        {
            this.unit = unitController;
            this.unit.RoleSpineAnimator.PlayAttack();
            this.unit.EffectSpineAnimator.PlayAttackEffect();
            Debug.Log(unit.name + " ¿ªÊ¼¹¥»÷");
        }

        public void OnExit()
        {
            this.unit.RoleSpineAnimator.PlayIdle();
        }

        public void OnUpdate()
        {
            
        }
    }
}
