using Game.Common.Utils;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Animation
{
    public class RoleSpineAnimator : SpineAnimatorBase
    {
        public event Action OnAttackHit;
        public event Action OnDieComplete;
        // Start is called before the first frame update
        void Start()
        {
            //PlayIdle();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public async void Init(int heroId)
        {
            var skeletonDataAsset = await ResourceUtils.GetHeroSpineSkeletonData(heroId);
            SetSpineAsset(skeletonDataAsset);
            this.PlayIdle();
        }


        public void PlayAttack() => PlayAnimation("attack", false);
        public void PlayDie() => PlayAnimation("die", false);
        public void PlayHit() => PlayAnimation("hit", false);
        public void PlayIdle() => PlayAnimation("idle", true);
        public void PlayJump1() => PlayAnimation("jump1", false);
        public void PlayJump2() => PlayAnimation("jump2", false);
        public void PlayRun() => PlayAnimation("run", true);
        public void PlaySkill1() => PlayAnimation("skill1", false);
        public void PlaySkill2() => PlayAnimation("skill2", false);
        public void PlayStun() => PlayAnimation("stun", false);
        public void PlayVictory() => PlayAnimation("victory", false);

        public override void OnAnimationComplete(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == "attack")
            {
                _animationState.SetAnimation(0, "idle", true);
            }

            if (trackEntry.Animation.Name == "die")
            {
                OnDieComplete?.Invoke();
            }
        }

        protected override void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "attackHit_tx")
            {
                Debug.Log(e.Data);
                OnAttackHit?.Invoke();
            }
        }
    }
}
