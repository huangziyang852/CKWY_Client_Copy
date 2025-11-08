using Game.Common.Const;
using Game.Core.Manager;
using Game.OutGame.Controller;
using UnityEngine;

namespace Game.Ingame.Comp
{
    public class BattleMapCom : MonoBehaviour
    {
        public Camera mapCamera;
        public Camera mainCamera;
        public GameObject worldParent;

        // Start is called before the first frame update

        private void Awake()
        {
            mapCamera = BattleRoot.Instance.mapCamera;
            mainCamera = BattleRoot.Instance.mainCamera;
        }
        private void Start()
        {
            mapCamera.enabled = true;
            mainCamera.enabled = false;
            CreateWorldIcon();
            SetMapCameraPositon(new Vector3(-160, -180, 0));
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void CreateWorldIcon()
        {
            var worldMaster = TableManager.Instance.MasterTables.TbBattleMap.DataList;
            ResourceLoader.Instance.LoadRemotePrefab(GameConst.PrefabPath.Batttle.Com.CHAPTER_ITEM_PRE,
                prefab =>
                {
                    foreach (var world in worldMaster)
                    {
                        var instance = Instantiate(prefab, worldParent.transform);
                        instance.transform.localPosition = new Vector3(world.Pos.X, world.Pos.Y, 0);
                        var chapterItemPre = instance.GetComponent<ChapterItemPre>();
                        chapterItemPre.Init(world);
                        chapterItemPre.clickableObject.SetOnClick(() => { Debug.Log("点击了" + world.Id); OpenStage(world.Id); });
                    }
                });
        }

        public void SetMapCameraPositon(Vector3 position)
        {
            var mapCameraController = mapCamera.GetComponent<MapCameraController>();
            mapCameraController.SetPosition(position);
        }

        private void OpenStage(int worldId)
        {
            UiManager.Instance.CloseAllPanel();
            PlaceBattleMgr.Instance.Init(worldId);
            BattleRoot.Instance.gameMap.SetActive(false);
            BattleRoot.Instance.stageLayer.SetActive(true);
            mapCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}