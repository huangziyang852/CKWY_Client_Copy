using cfg;
using cfg.enemy;
using cfg.hero;
using cfg.skill;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Core.Manager;
using Game.Ingame.Effect;
using Game.Ingame.PlaceBattle;
using Game.Model;
using Game.OutGame.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Game.Common.Const.GameConst.ImagesPath.UI;

namespace Game.Ingame.Model
{
    public class PlayerUnitData : UnitData
    {
        public HeroModel HeroModel { get; private set; }

        public int DeckPosition { get; private set; }

        private cfg.hero.Hero HeroMaster { get; set; }

        private cfg.hero.HeroAttr HeroAttrMaster { get; set; }

        private cfg.skill.Skill SkillMaster { get; set; }

        public int Def { get; private set; }

        public int Hp { get; private set; }

        public int Atk { get; private set; }

        public int Spd { get; private set; }

        private bool isAttacking = false;

        public EnemyUnitData Target { get; set; }

        public Event OnReceDamage { get; set; }

        public PlayerUnitData(HeroModel heroModel, int position) : base()
        {
            this.HeroModel = heroModel;
            HeroMaster = TableManager.Instance.MasterTables.TbHero.Get(heroModel.HeroId);
            HeroAttrMaster = TableManager.Instance.MasterTables.TbHeroAttr.DataList.Find((attr) =>
            {
                return attr.HeroID == heroModel.HeroId;
            });
            SkillMaster = TableManager.Instance.MasterTables.TbSkill.Get(HeroAttrMaster.NormalAtkID);
            if (HeroMaster == null)
            {
                Debug.LogError("获取heroAttr失败");
            }
            else
            {
                if (HeroAttrMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int def))
                {
                    Def = def;
                }
                if (HeroAttrMaster.Attrs.TryGetValue(BattleConst.HP_KEY, out int hp))
                {
                    Hp = hp;
                }
                if (HeroAttrMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int atk))
                {
                    Atk = atk;
                }
                if (HeroAttrMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int spd))
                {
                    Spd = spd;
                }
            }
        }

        private void SelectEnemy(List<EnemyUnitData> enemyUnitDatas)
        {
            if (enemyUnitDatas == null || enemyUnitDatas.Count == 0)
            {
                Target = null;
                return;
            }

            int index = UnityEngine.Random.Range(0, enemyUnitDatas.Count);
            Target = enemyUnitDatas[index];
        }

        public void StartBattle()
        {

            //选择敌人
            SelectEnemy(PlaceBattleMgr.Instance.enemyUnitDataList);

            if (Target == null)
            {
                Debug.Log("没有选择目标");
                PlaceBattleMgr.Instance.PlaceBattleStateMachine.PlaceBattleState.RunNext(1);
                return;
            }

            if (!isAttacking)
            {
                isAttacking = true;
                UnitController.RoleSpineAnimator.OnAttackHit -= PlayBulletEffect;
                UnitController.RoleSpineAnimator.OnAttackHit += PlayBulletEffect;
                StartAttackAsync().Forget();
            }

        }

        private async UniTask StartAttackAsync(CancellationToken? token = null)
        {
            var ct = token ?? default; // 如果外部没传，就默认不取消

            while (Target != null && !Target.IsDead())
            {
                UnitController.Attack();

                // 等待 1.5 秒，不会卡主线程
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(BattleConst.ATTACK_INTERVAL), cancellationToken: ct);
                }
                catch (OperationCanceledException)
                {
                    // 安全退出
                    return;
                }
            }
            isAttacking = false;
            UnitController.RoleSpineAnimator.OnAttackHit -= PlayBulletEffect;
            Debug.Log("敌人死亡");

            var nextEnemy = PlaceBattleMgr.Instance.enemyUnitDataList.Find(e => !e.IsDead());
            if(nextEnemy != null)
            {
                Debug.Log("继续攻击下一个敌人");
                //Target = nextEnemy;
                StartBattle();
            }
            else
            {
                Debug.Log("所有敌人都已死亡，战斗结束");
                PlaceBattleMgr.Instance.PlaceBattleStateMachine.PlaceBattleState.RunNext(1);
            }

        }

        private void OnBulletHit(EnemyUnitData enemy)
        {
            int damage = Atk - Target.Def;
            Target.receiveDamage(damage);
            Debug.Log("给" + Target + "造成了" + damage + "伤害");
            if (Target.IsDead())
            {
                Target = null;
            }
        }

        private void PlayBulletEffect()
        {
            ResourceLoader.Instance.LoadRemotePrefab(GameConst.PrefabPath.Batttle.Effect.BULLET, (prefab) =>
            {
                GameObject instance = GameObject.Instantiate(prefab,UnitController.attackEffectTrans.position, Quaternion.identity, BattleRoot.Instance.effectLayer.transform);
                BulletComp bulletComp = instance.GetComponent<BulletComp>();
                bulletComp.Init(HeroModel,Target, OnBulletHit);
            });
        }
    }
}
