using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Animation
{
    /// <summary>
    /// Spine动画控制类
    /// </summary>
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineAnimatorBase : MonoBehaviour
    {
        protected SkeletonAnimation _skeletonAnimation;
        protected SkeletonDataAsset _skeletonDataAsset;
        protected Spine.AnimationState _animationState;
        protected string currentAnim;
        protected bool isLoop;
        protected int trackIndex = 0;

        protected virtual void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _skeletonDataAsset = _skeletonAnimation.skeletonDataAsset;
            _animationState = _skeletonAnimation.AnimationState;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(_animationState.GetCurrent(0));
        }

        public void SetSpineAsset(SkeletonDataAsset skeletonDataAsset)
        {
            if (skeletonDataAsset == null)
            {
                Debug.LogError("SkeletonDataAsset is null！");
                return;
            }

            _skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
            _skeletonAnimation.Initialize(true);
            _animationState = _skeletonAnimation.AnimationState;
            if (_skeletonAnimation.Skeleton == null)
            {
                Debug.LogError("Skeleton  init failed！");
                return;
            }
            _animationState.Event += OnSpineEvent;
            _animationState.Complete += OnAnimationComplete;
        }

        /// <summary>
        /// 播放指定动画
        /// </summary>
        public virtual void PlayAnimation(string animName, bool loop = true)
        {
            if (string.IsNullOrEmpty(animName) || _skeletonAnimation == null)
                return;

            if (currentAnim == animName && isLoop == loop)
                return;

            currentAnim = animName;
            isLoop = loop;

            Debug.Log("播放动画" + animName);
            TrackEntry entry = _animationState.SetAnimation(trackIndex, animName, loop);
        }

        /// <summary>
        /// 停止当前动画（可切换为空动画或保持最后一帧）
        /// </summary>
        public virtual void StopAnimation()
        {
            _animationState.SetEmptyAnimation(trackIndex, 0.2f);
            currentAnim = null;
        }

        /// <summary>
        /// 动画播放完成事件（子类可重写处理）
        /// </summary>
        public virtual void OnAnimationComplete(TrackEntry trackEntry)
        {
            
        }

        /// <summary>
        /// 设置当前角色动画播放速度（支持战斗加速）
        /// </summary>
        public virtual void SetAnimationSpeed(float speed)
        {
            if (_skeletonAnimation != null)
            {
                _skeletonAnimation.timeScale = speed;
            }
        }
        /// <summary>
        /// 触发动画事件
        /// </summary>
        protected virtual void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            Debug.Log($"BaseUnit SpineEvent: {e.Data.Name}");
        }

        protected virtual void OnDestroy()
        {
            if (_animationState != null)
                _animationState.Event -= OnSpineEvent;
        }
    }
}
