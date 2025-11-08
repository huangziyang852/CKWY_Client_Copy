using Game.Common.Const;
using Game.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.OutGame.View;
using Game.Core.Net;

namespace Game.Core.Manager
{
    public class UiManager : MonoSingleton<UiManager>
    {
        private static UiManager instance;

        [SerializeField] private GameObject mainUI;

        [SerializeField] private GameObject subUI;

        [SerializeField] private GameObject popupUI;

        [SerializeField] private GameObject notificationUI;

        private readonly Dictionary<string, GameObject> panelCache = new();
        private readonly Dictionary<string, GameObject> subPanelCache = new();


        //public static UiManager Instance
        //{
        //    get
        //    {
        //        if (instance == null) instance = FindAnyObjectByType(typeof(UiManager)) as UiManager;
        //        return instance;
        //    }
        //}


        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        /// <summary>
        ///     直接从Resource目录加载，用于最开始的LoadingPanel
        /// </summary>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public GameObject InitUIPanel(string panelName)
        {
            if (!panelCache.TryGetValue(panelName, out var panelGameObject))
            {
                panelGameObject =
                    Instantiate(
                        Resources.Load<GameObject>(ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Ui, panelName)),
                        new Vector3(0, 0, 0), Quaternion.identity);
                panelGameObject.transform.SetParent(mainUI.transform, false);
                panelCache[panelName] = panelGameObject;
            }

            Debug.Log("init mainPanel" + panelName);
            panelGameObject.SetActive(true);
            return panelGameObject;
        }

        /// <summary>
        ///     使用Addressable从远程加载
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="onLoaded"></param>
        public void InitUIPanel(string panelName, Action<GameObject> onLoaded)
        {
            if (panelCache.TryGetValue(panelName, out var panelGameObject))
            {
                panelGameObject.SetActive(true);
                onLoaded?.Invoke(panelGameObject);
                return;
            }

            Debug.Log($"Loading UI Panel: {panelName}");

            ResourceLoader.Instance.LoadRemotePrefab(
                ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Ui, panelName),
                prefab =>
                {
                    if (prefab != null)
                    {
                        panelGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                        panelGameObject.transform.SetParent(mainUI.transform, false);
                        panelCache[panelName] = panelGameObject;
                        panelGameObject.SetActive(true);
                        Debug.Log($"✅ UiPanel 加载成功: {panelName}");

                        onLoaded?.Invoke(panelGameObject);
                    }
                    else
                    {
                        Debug.LogError($"❌ UiPanel 加载失败: {panelName}");
                    }
                }
            );
        }

        /// <summary>
        ///     加载附属界面
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="onLoaded"></param>
        public void InitSubPanel(string panelName, Action<GameObject> onLoaded)
        {
            if (subPanelCache.TryGetValue(panelName, out var panelGameObject))
            {
                panelGameObject.SetActive(true);
                onLoaded?.Invoke(panelGameObject);
                return;
            }

            Debug.Log($"Loading UI Panel: {panelName}");

            ResourceLoader.Instance.LoadRemotePrefab(
                ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Ui, panelName),
                prefab =>
                {
                    if (prefab != null)
                    {
                        panelGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                        panelGameObject.transform.SetParent(subUI.transform, false);
                        subPanelCache[panelName] = panelGameObject;
                        panelGameObject.SetActive(true);
                        Debug.Log($"✅ UiPanel 加载成功: {panelName}");

                        onLoaded?.Invoke(panelGameObject);
                    }
                    else
                    {
                        Debug.LogError($"❌ UiPanel 加载失败: {panelName}");
                    }
                }
            );
        }

        public void CloseUIPanel(string panelName)
        {
            if (!panelCache.ContainsKey(panelName))
            {
                Debug.LogWarning("can't find mainPanel：" + panelName);
                return;
            }

            var panelGameObject = panelCache[panelName];
            if (panelGameObject == null)
                return;

            Debug.Log("close mainPanel：" + panelName);

            panelGameObject.SetActive(false);

            Debug.Log("close mainPanel success：" + panelName);
        }

        public void closeSubPanel(string panelName)
        {
            if (!subPanelCache.ContainsKey(panelName))
            {
                Debug.LogWarning("can't find subPanel：" + panelName);
                return;
            }

            var panelGameObject = subPanelCache[panelName];

            Debug.Log("close subPanel：" + panelName);

            panelGameObject.SetActive(false);

            Debug.Log("close subPanel success：" + panelName);
        }

        public void OpenPopup(string popupName, Action<GameObject> onLoaded)
        {
            //if (popupCache.TryGetValue(popupName, out var popupGameObject))
            //{
            //    popupGameObject.SetActive(true);
            //    onLoaded?.Invoke(popupGameObject);
            //    return;
            //}

            Debug.Log($"Loading UI Popup: {popupName}");

            ResourceLoader.Instance.LoadRemotePrefab(
                ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Ui, popupName),
                prefab =>
                {
                    if (prefab != null)
                    {
                        GameObject popupGameObject = Instantiate(prefab, popupUI.transform);
                        //popupGameObject.transform.SetParent(popupUI.transform, false);
                        //popupCache[popupName] = popupGameObject;
                        popupGameObject.SetActive(true);
                        Debug.Log($"✅ Popup 加载成功: {popupName}");

                        onLoaded?.Invoke(popupGameObject);
                    }
                    else
                    {
                        Debug.LogError($"❌ Popup 加载失败: {popupName}");
                    }
                }
            );
        }

        /// <summary>
        ///     打开提示
        /// </summary>
        /// <param name="notificationName"></param>
        /// <param name="onLoaded"></param>
        public void OpenNotification(string notificationName, string notification, Action<GameObject> onLoaded = null)
        {
            Debug.Log($"Loading UI notification: {notificationName}");

            ResourceLoader.Instance.LoadRemotePrefab(
                ConfigManager.pathConfig.GetPath(GameConst.PathCategory.Ui, notificationName),
                prefab =>
                {
                    if (prefab != null)
                    {
                        // 实例化并显示物体
                        var notifyGameObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                        notifyGameObject.GetComponent<Notification>()?.setNotification(notification);
                        notifyGameObject.transform.SetParent(notificationUI.transform, false);
                        notifyGameObject.SetActive(true);
                        Debug.Log($"✅ notification 加载成功: {notificationName}");

                        // 调用回调方法
                        onLoaded?.Invoke(notifyGameObject);

                        // 启动协程延时摧毁物体
                        StartCoroutine(DestroyAfterDelay(notifyGameObject, 3f));
                    }
                    else
                    {
                        Debug.LogError($"❌ notification 加载失败: {notificationName}");
                    }
                }
            );
        }

        private IEnumerator DestroyAfterDelay(GameObject notificationGameObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(notificationGameObject);
        }

        public void OpenLoadingPanel()
        {
            InitUIPanel(GameConst.UiPanel.LoadingPanel);
        }

        public void CloseAllPanel()
        {
            foreach (var panel in panelCache.Values) panel.SetActive(false);

            foreach (var panel in subPanelCache.Values) panel.SetActive(false);
        }

        public void ClearAllPanel()
        {
            ClearAndDestroyAllPanels(panelCache);
            ClearAndDestroyAllPanels(subPanelCache);
        }

        public void HideSubPanel()
        {
            subUI.SetActive(false);
        }

        public void ShowSubPanel()
        {
            subUI.SetActive(true);
        }

        public void ClearAndDestroyAllPanels(Dictionary<string, GameObject> cache)
        {
            foreach (var kv in cache)
                if (kv.Value != null)
                    Destroy(kv.Value);
            cache.Clear();
        }
    }
}