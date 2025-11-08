using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public class UnitMoveState : IUnitState<UnitController>
    {
        private UnitController unit;
        private Vector3? targetPosition;
        private Transform targetEnemy;

        public UnitMoveState()
        {

        }

        public void SetTarget(Transform target)
        {
            this.targetEnemy = target;
        }

        public void SetTarget(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public void OnEnter(UnitController unitController)
        {
            unit = unitController;
            unit.RoleSpineAnimator.PlayRun();
            Debug.Log(unit.name + " 开始移动");
        }

        public void OnUpdate()
        {
            Transform self = this.unit.transform;
            if (targetEnemy != null)
            {
                Vector3 target = targetEnemy.position;
                self.position = Vector3.MoveTowards(self.position, target, 5 * Time.deltaTime);
            }
            else if (targetPosition.HasValue)
            {
                self.position = Vector3.MoveTowards(self.position, targetPosition.Value, 5 * Time.deltaTime);
            }
        }

        public void OnExit()
        {
            unit.RoleSpineAnimator.PlayIdle();
            Debug.Log(unit.name + " 停止移动");
        }
    }
}
