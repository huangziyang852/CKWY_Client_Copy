using Cinemachine;
using Game.Common.Const;
using Game.Core.GameRoot;
using Game.Core.Manager;
using Game.Ingame.Comp;
using Game.OutGame.Controller;
using LaunchPB;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Game.Ingame
{
    public class BattleRoot : MonoBehaviour
    {
        public GameObject gameMap;
        public ParallaxManager parallaxManager;
        public GameObject stageLayer;
        public GameObject effectLayer;
        public Camera mainCamera;
        public Camera mapCamera;
        public float moveSpeed = 5f;
        public GameObject unitLayer;
        [SerializeField] private CinemachineVirtualCamera vcam;
        public Transform cameraTarget; // 角色
        public Vector3 offset;   // 相机和角色的相对偏移
        public static BattleRoot Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("BattleRoot is already exist");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            GameRoot.Instance.backGroundCanvas.gameObject.SetActive(false);
            Debug.Log("BattleRoot create");
        }

        // Start is called before the first frame update
        private void Start()
        {
            ResourceLoader.Instance.LoadRemotePrefab(GameConst.PrefabPath.Batttle.Com.BATTLE_MAP_PRE, prefab => {
                gameMap = GameObject.Instantiate(prefab, new Vector3(0, 0, 100), Quaternion.identity, this.gameObject.transform);
                gameMap.transform.localScale = new Vector3(40, 40, 0);
            });
            stageLayer.SetActive(false);
            SoundManager.Instance.PlayBGM(GameConst.AudioName.World);
            UiManager.Instance.InitUIPanel(GameConst.UiPanel.BattleMapPanel, panel => {
                UiManager.Instance.CloseUIPanel(GameConst.UiPanel.LoadingPanel);
            });
                //在这获取章节数据
            }

        // Update is called once per frame
        private void Update()
        {
            PlaceBattleMgr.Instance.Update();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                Debug.Log("BattleRoot destory");
            }
            GameRoot.Instance.backGroundCanvas.gameObject.SetActive(true);
        }

        /// <summary>
        /// 如果只想跟随X方向，可以在LateUpdate里锁定Y
        /// </summary>
        private void LateUpdate()
        {
            if (cameraTarget != null)
            {
                // 只跟随X方向
                Vector3 camPos = vcam.transform.position;
                camPos.y = 0; // 固定在某个Y（比如0）
                vcam.transform.position = camPos;
            }
        }


        /// <summary>
        /// 绑定玩家，相机开始跟随
        /// </summary>
        public void SetCameraPlayer(Transform playerTransform)
        {
            cameraTarget = playerTransform;
            vcam.Follow = cameraTarget;
            vcam.enabled = true;
        }

        /// <summary>
        /// 解绑玩家，相机停止跟随
        /// </summary>
        public void ClearCameraPlayer()
        {
            cameraTarget = null;
            vcam.Follow = null;
            vcam.enabled = false; // 禁用后MainCamera就不再跟随
        }


    }
}
