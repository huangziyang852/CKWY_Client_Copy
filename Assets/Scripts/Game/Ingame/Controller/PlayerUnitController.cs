using cfg.hero;
using cfg.skill;
using Game.Common.Utils;
using Game.Model;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Assets.Scripts.Game.Ingame.BattleEnum;

namespace Game.Ingame.Controller
{
    public class PlayerUnitController : UnitController
    {
        // Start is called before the first frame update
        private bool isMoving = false;
        private Vector3 targetPosition = Vector3.zero;


        void Start()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (!isMoving) return;
            Vector3 currentPos = unitGameObject.transform.position;
            if (Mathf.Abs(currentPos.x - targetPosition.x) < 0.01f)
            {
                isMoving = false;
                OnReachTarget();
            }
        }

        public override void Init(IBaseModel unitModel)
        {
            HeroModel heroModel = unitModel as HeroModel;
            if (heroModel != null)
            {
                RoleSpineAnimator.Init(heroModel.HeroId);
                EffectSpineAnimator.Init(heroModel,AttackEffectType.Normal);
            }
        }

        public override void MoveToPosition(Vector3 position)
        {
            isMoving = true;
            stateMachine.ChangeState(UnitState.MOVE);
            targetPosition = position;
            stateMachine.SetMoveTarget(position);
        }

        protected override void OnReachTarget()
        {
            Debug.Log($"{unitGameObject.name} 到达目标位置");
            base.OnReachTarget();
            isMoving = false;
            stateMachine.ChangeState(UnitState.IDLE);
        }
    }
}
