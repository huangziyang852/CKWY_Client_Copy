using cfg.enemy;
using Game.Common.Const;
using Game.Core.Manager;
using Game.Ingame.Controller;
using Game.Ingame.Model;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ingame.PlaceBattle
{
    public class PlaceBattleStateEnemyEnter : PlaceBattleState
    {
        private readonly float CREATE_ENEMY_DISTANCE = 40f;
        public PlaceBattleStateEnemyEnter(PlaceBattleStateMachine stateMachine) : base(stateMachine, PlaceBattleStatePhase.ENEMY_ENTER)
        {
        }

        public override PlaceBattleStatePhase GetNextPlaceBattleStatePhase()
        {
            return PlaceBattleStatePhase.HERO_RUN;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            CreateEnemyUnitData();
            RunNext(1);
        }

        private void CreateEnemyUnitData()
        {
            var enemyGroupMaster = TableManager.Instance.MasterTables.TbEnemyGroup.Get(PlaceBattleMgr.QuestId);
            ResourceLoader.Instance.LoadRemotePrefab(GameConst.PrefabPath.Batttle.Unit.ENEMY_UNIT, prefab =>
            {
                foreach (var enemy in enemyGroupMaster.Member)
                {
                    var enemyMaster = TableManager.Instance.MasterTables.TbEnemy.Get(enemy.Key);
                    EnemyModel enemyModel = new EnemyModel(enemyMaster.EnemyID, enemyMaster.Star);
                    PlaceBattleMgr.enemyUnitDataList.Add(new EnemyUnitData(enemyModel));
                }
                List<EnemyUnitData> enemyUnitDatas = PlaceBattleMgr.enemyUnitDataList;
                for (int i = 0; i < enemyUnitDatas.Count; i++)
                {
                    EnemyModel enemyModel = enemyUnitDatas[i].EnemyModel;
                    var mainCamera = BattleRoot.Instance.mainCamera;
                    float camHalfHeight = mainCamera.orthographicSize;
                    float camHalfWidth = camHalfHeight * mainCamera.aspect;
                    float camRight = mainCamera.transform.position.x + camHalfWidth;
                    Vector3 enemyPosition = new Vector3(camRight + CREATE_ENEMY_DISTANCE, BattleConst.PLAYER_UNIT_POSITION[i].y, BattleConst.PLAYER_UNIT_POSITION[i].z);

                    GameObject instance = GameObject.Instantiate(prefab, enemyPosition, Quaternion.identity, BattleRoot.Instance.unitLayer.transform);
                    instance.name = "EnemyUnit" + enemyModel.EnemyId;
                    EnemyUnitController controller = instance.GetComponent<EnemyUnitController>();
                    controller.Init(enemyModel);
                    //这里给controller赋值
                    enemyUnitDatas[i].UnitController = controller;
                };
            });
        }
    }
}
