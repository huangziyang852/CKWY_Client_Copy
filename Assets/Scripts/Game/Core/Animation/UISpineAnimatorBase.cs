using Game.Core.Animation;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Animation
{
    /// <summary>
    /// UISpine动画控制类
    /// </summary>
    [RequireComponent(typeof(SkeletonGraphic))]
    public class UISpineAnimatorBase : MonoBehaviour
    {
        protected SkeletonGraphic _skeletonGraphic;
        protected SkeletonDataAsset _skeletonDataAsset;
        protected Spine.AnimationState _animationState;
        private TrackEntry _currentEntry;
        private Action _onComplete;
        protected string currentAnim;
        protected bool isLoop;
        protected int trackIndex = 0;

        protected virtual void Awake()
        {
            _skeletonGraphic = GetComponent<SkeletonGraphic>();
            _skeletonDataAsset = _skeletonGraphic.skeletonDataAsset;
            _animationState = _skeletonGraphic.AnimationState;
        }

        public void SetSpineAsset(SkeletonDataAsset skeletonDataAsset)
        {
            if (skeletonDataAsset == null)
            {
                Debug.LogError("SkeletonDataAsset is null！");
                return;
            }

            _skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
            _skeletonGraphic.Initialize(true);
            _animationState = _skeletonGraphic.AnimationState;
            if (_skeletonGraphic.Skeleton == null)
            {
                Debug.LogError("Skeleton  init failed！");
                return;
            }
            _animationState.Event += OnSpineEvent;
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
        /// 设置当前UI动画播放速度
        /// </summary>
        public virtual void SetAnimationSpeed(float speed)
        {
            if (_skeletonGraphic != null)
            {
                _skeletonGraphic.timeScale = speed;
            }
        }

        /// <summary>
        /// 播放指定动画
        /// </summary>
        public virtual void PlayAnimation(string animName, bool loop = true)
        {
            if (string.IsNullOrEmpty(animName) || _skeletonGraphic == null)
                return;

            if (currentAnim == animName && isLoop == loop)
                return;

            currentAnim = animName;
            isLoop = loop;

            Debug.Log("播放动画" + animName);
            TrackEntry entry = _animationState.SetAnimation(trackIndex, animName, loop);
        }
        /// <summary>
        /// 播放一次指定动画
        /// </summary>
        public virtual void PlayAnimationOnce(string animName, Action onComplete = null)
        {
            if (string.IsNullOrEmpty(animName) || _skeletonGraphic == null)
                return;
            // 取消上一次的监听（如果有）
            if (_currentEntry != null)
            {
                _currentEntry.Complete -= OnEntryComplete;
            }

            currentAnim = animName;

            Debug.Log("播放动画" + animName);
            TrackEntry entry = _animationState.SetAnimation(trackIndex, animName,false);

            _currentEntry = entry;
            _onComplete = onComplete;

            entry.Complete += OnEntryComplete;
        }

        /// <summary>
        /// 动画播放完成事件（子类可重写处理）
        /// </summary>
        public virtual void OnEntryComplete(TrackEntry entry)
        {
            _onComplete?.Invoke();
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