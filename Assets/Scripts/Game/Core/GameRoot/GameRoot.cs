using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Common.Const;
using Game.Config;
using Game.Core.Manager;
using Game.Core.Net;
using Game.OutGame.Controller;
using Game.OutGame.View;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.GameRoot
{
    public class GameRoot : MonoBehaviour
    {
        private static GameRoot instance;

        public static GameRoot Instance
        {
            get
            {
                if (instance == null) instance = new GameRoot();
                return instance;
            }
        }

        public static UiManager UiManager => UiManager.Instance;

        public static NetWorkService NetWorkService => NetWorkService.Instance;

        public static SoundManager SoundManager => SoundManager.Instance;

        public static ConfigManager configManager => ConfigManager.Instance;

        public static EventManager eventManager => EventManager.Instance;

        public Canvas backGroundCanvas;


        private void Awake()
        {
            // 确保只有一个 GameRoot 实例存在
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 保持 GameRoot 不被销毁
            }
            else
            {
                Destroy(gameObject); // 如果已经存在实例，则销毁新实例
            }
        }

        private void Start()
        {
            //Shader.SetGlobalFloat("SceneScale", 1.5f);
            Debug.Log("GameStart");
            PlayerPrefs.SetInt("gameStart", 2);
            ConfigManager.Instance.Init();
            SoundManager.Instance.Init();
            Initialize();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Initialize()
        {
            //获取服务器信息
            LoginMgr.Instance.RequestServerList();
            var bgmClip = Resources.Load<AudioClip>("Sounds/BGM/loading");
            SoundManager.PlayAudioClip(bgmClip);
        }

        public async UniTask TransiationToScene(string sceneName, Action onComplete, List<UniTask> preloadTasks = null)
        {
            // 关闭当前所有 UI 面板
            UiManager.Instance.CloseAllPanel();

            // 显示加载面板
            var loadingPanel = UiManager.Instance
                .InitUIPanel(GameConst.UiPanel.LoadingPanel)
                .GetComponent<LoadingPanel>();

            loadingPanel.SetProgress(0f);

            // 1. 执行预加载任务（资源、配置、对象等）
            if (preloadTasks != null && preloadTasks.Count > 0)
            {
                Debug.Log($"开始执行 {preloadTasks.Count} 个预加载任务...");
                await UniTask.WhenAll(preloadTasks);
                Debug.Log("所有预加载任务已完成。");
            }

            //2. 异步加载场景
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false; // 暂不切换

            while (!asyncLoad.isDone)
            {
                // 加载进度（0~0.9），0.9之后是等待激活
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                loadingPanel.SetProgress(progress);

                // 到达90%后，等待进入场景
                if (asyncLoad.progress >= 0.9f)
                {
                    loadingPanel.SetProgress(1f);

                    // 这里可以等待一段时间或者让UI显示“点击继续”
                    await UniTask.Delay(TimeSpan.FromSeconds(3));

                    // 激活场景
                    asyncLoad.allowSceneActivation = true;
                }

                await UniTask.Yield(); // 等下一帧
            }
            //UiManager.Instance.ClearAllPanel();
            // 3. 场景加载完成回调
            onComplete?.Invoke();
        }
    }
}
