using Game.Core.Manager;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.Controller
{
    public class EnemyUnitController : UnitController
    {
        void Update()
        {
            base.Update();
        }

        public override void Init(IBaseModel unitModel)
        {
            EnemyModel enemyModel = unitModel as EnemyModel;
            var enemyMaster = TableManager.Instance.MasterTables.TbEnemy.Get(enemyModel.EnemyId);
            if (enemyModel != null)
            {
                RoleSpineAnimator.Init(enemyMaster.Body);
                roleSpine.transform.localScale = Vector3.one;
            }
        }
    }
}
