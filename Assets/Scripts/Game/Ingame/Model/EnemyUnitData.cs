using cfg.enemy;
using Game.Core.Manager;
using Game.Ingame.Controller;
using Game.Model;
using Game.OutGame.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Model
{
    public class EnemyUnitData : UnitData
    {
        public EnemyModel EnemyModel { get; private set; }

        public cfg.enemy.Enemy EnemyMaster { get; private set; }

        public cfg.enemy.EnemyAttrs EnemyAttrsMaster { get; private set; }

        public int Def {  get; private set; }

        public int Hp { get; private set; }

        public int Atk {  get; private set; }

        public int Spd {  get; private set; }

        public EnemyUnitData(EnemyModel enemyModel) : base()
        {
            this.EnemyModel = enemyModel;
            EnemyMaster = TableManager.Instance.MasterTables.TbEnemy.Get(enemyModel.EnemyId);
            cfg.enemy.EnemyGroup enemyGroup = TableManager.Instance.MasterTables.TbEnemyGroup.Get(PlaceBattleMgr.Instance.QuestId);
            if (enemyGroup.AttrID.TryGetValue(enemyModel.EnemyId, out int enemyAttrId))
            {
                EnemyAttrsMaster = TableManager.Instance.MasterTables.TbEnemyAttrs.Get(enemyAttrId);
                if (EnemyAttrsMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int def))
                {
                    Def = def;
                }
                if (EnemyAttrsMaster.Attrs.TryGetValue(BattleConst.HP_KEY, out int hp))
                {
                    Hp = hp;
                }
                if (EnemyAttrsMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int atk))
                {
                    Atk = atk;
                }
                if (EnemyAttrsMaster.Attrs.TryGetValue(BattleConst.DEF_KEY, out int spd))
                {
                    Spd = spd;
                }
            }
            else
            {
                Debug.LogError("获取enemyAttr失败");
            }   
        }

        public void receiveDamage(int damage)
        {
            Hp = Hp - damage;
            bool isDead = IsDead();
            if (isDead)
            {
                // 从全局管理器移除
                PlaceBattleMgr.Instance.enemyUnitDataList.Remove(this);
                UnitController.RoleSpineAnimator.OnDieComplete += Dispose;
                UnitController.RoleSpineAnimator.PlayDie();
            }
        }
        public bool IsDead()
        {
            return Hp<=0; 
        }

        private void Dispose()
        {
            // 1销毁模型对象
            if (EnemyModel != null)
            {
                EnemyModel = null;
            }

            // 2销毁控制器
            if (UnitController != null)
            {
                UnitController.RoleSpineAnimator.OnDieComplete -= Dispose;
                GameObject.Destroy(UnitController.gameObject);
                UnitController = null;
            }

            // 清除其他引用，等待 GC
            EnemyMaster = null;
            EnemyAttrsMaster = null;
        }
    }
}