using cfg.enemy;
using Game.Core.Animation;
using Game.Ingame.Model;
using Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Game.Ingame.BattleEnum;

namespace Game.Ingame.Effect
{
    public class BulletComp : MonoBehaviour
    {
        private EnemyUnitData _enemyUnitData;
        private Action<EnemyUnitData> _onHitCallback;  // 命中时执行的回调
        public EffectSpineAnimator effectSpineAnimator;
        public Transform targetTrans;      // 敌人目标
        public float hitRadius = 0.3f; // 命中距离半径

        private bool hasHit = false;  // 防止重复命中

        private void Update()
        {
            if (targetTrans == null || hasHit)
                return;

            // 移动到目标
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetTrans.position,
                BattleConst.BULLET_SPEED * Time.deltaTime
            );

            // 检测距离
            float distance = Vector2.Distance(transform.position, targetTrans.position);
            if (distance <= hitRadius)
            {
                hasHit = true;
                OnHit();
            }
        }

        public void Init(HeroModel heroModel, EnemyUnitData enemyUnitData, Action<EnemyUnitData> onHitCallback)
        {
            _enemyUnitData = enemyUnitData;
            _onHitCallback = onHitCallback;
            targetTrans = enemyUnitData.UnitController.attackEffectTrans;

            if (targetTrans != null)
            {
                float dirX = targetTrans.position.x - transform.position.x;
                Vector3 localScale = transform.localScale;
                localScale.x = dirX >= 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
                transform.localScale = localScale;
            }

            effectSpineAnimator.Init(heroModel,AttackEffectType.Bullet);
            effectSpineAnimator.PlayAnimation("animation");
        }

        private void OnHit()
        {
            _onHitCallback?.Invoke(_enemyUnitData); // 命中时执行回调
            Destroy(gameObject);
        }
    }
}
