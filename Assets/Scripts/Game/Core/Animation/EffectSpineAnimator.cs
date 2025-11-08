using cfg.hero;
using cfg.skill;
using Game.Common.Utils;
using Game.Core.Animation;
using Game.Core.Manager;
using Game.Model;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Game.Ingame.BattleEnum;

namespace Game.Core.Animation
{
    public class EffectSpineAnimator : SpineAnimatorBase
    {
        private string SfxName { get; set; }

        public async void Init(HeroModel heroModel,AttackEffectType attackEffectType)
        {
            Skill skillMaster = MasterUtils.GetSkillMasterByHeroModel(heroModel);
            SkeletonDataAsset skeletonDataAsset = null;
            switch (attackEffectType)
            {
                case AttackEffectType.Normal:
                    SfxName = skillMaster.FireSound;
                    skeletonDataAsset = await ResourceUtils.GetHeroEffectSpineSkeletonData(heroModel.HeroId, skillMaster.AttackEffect);
                    SetSpineAsset(skeletonDataAsset);
                    break;
                case AttackEffectType.Bullet:
                    SfxName = skillMaster.FlySound;
                    skeletonDataAsset = await ResourceUtils.GetHeroEffectSpineSkeletonData(heroModel.HeroId, skillMaster.FlyEffect);
                    SetSpineAsset(skeletonDataAsset);
                    break;
            }  
        }

        public override void PlayAnimation(string animName, bool loop = true)
        {
            if (string.IsNullOrEmpty(animName) || _skeletonAnimation == null)
                return;

            currentAnim = animName;
            isLoop = loop;

            Debug.Log("²¥·Å¶¯»­" + animName);
            TrackEntry entry = _animationState.SetAnimation(trackIndex, animName, loop);
        }

        public void PlayAttackEffect()
        {
            PlayAnimation("animation", false);
            if(SfxName != null)
            {
                SoundManager.Instance.PlaySFX(SfxName);
            }
        }
    }
}
