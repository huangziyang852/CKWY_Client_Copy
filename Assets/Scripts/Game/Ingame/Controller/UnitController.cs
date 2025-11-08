using Game.Core.Animation;
using Game.Model;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public abstract class UnitController : MonoBehaviour
    {
        public GameObject unitGameObject;
        public GameObject roleSpine;
        public UnitStateMachine stateMachine;
        public RoleSpineAnimator RoleSpineAnimator;
        public EffectSpineAnimator EffectSpineAnimator;
        public Transform attackEffectTrans;
        public event Action<UnitController> OnArrived;

        protected virtual void Awake()
        {
            stateMachine = new UnitStateMachine(this);
            stateMachine.ChangeState(UnitState.IDLE);
        }

        protected virtual void Update()
        {
            stateMachine.Update();
        }
        /// <summary>
        /// 初始化(子类重写)
        /// </summary>
        public abstract void Init(IBaseModel unitModel);
        /// <summary>
        /// 奔跑(子类重写)
        /// </summary>
        public virtual void MoveToPosition(Vector3 position) { }

        /// <summary>
        /// 到达指定位置
        /// </summary>
        protected virtual void OnReachTarget()
        {
            OnArrived?.Invoke(this);
        } 
        /// <summary>
        /// 播放攻击动画
        /// </summary>
        public virtual void Attack()
        {
            stateMachine.ChangeState(UnitState.ATTACK);
        }
    }
}
