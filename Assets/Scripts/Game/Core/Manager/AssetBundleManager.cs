using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Game.Config;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Core.Manager
{
    public class AssetBundleManager
    {
        private static AssetBundleManager _instance;

        private static readonly string LocalPath = Application.persistentDataPath;
        private static readonly Dictionary<string, AssetBundle> _loadedBundles = new();

        public static AssetBundleManager Instance
        {
            get
            {
                if (_instance == null) _instance = new AssetBundleManager();
                return _instance;
            }
        }

        public async UniTask<bool> DownloadAssetBundle(string assetBundleName)
        {
            var loginRootUrl = ConfigManager.NetInfo.LoginRootUrl;
            var localFilePath = Path.Combine(LocalPath, assetBundleName);
            var manifestUrl = string.Format("{0}/api/files/{1}.manifest", loginRootUrl, assetBundleName);

            // **1 先下载 master.manifest**
            var manifestRequest = UnityWebRequest.Get(manifestUrl);
            await manifestRequest.SendWebRequest();
            if (manifestRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("下载 master.manifest 失败: " + manifestRequest.error);
                return false;
            }

            // **2 解析 master.manifest 获取最新版本**
            var manifestData = manifestRequest.downloadHandler.data;
            var manifestContent = Encoding.UTF8.GetString(manifestData);
            var latestHash = ExtractHashFromManifest(manifestContent);
            if (string.IsNullOrEmpty(latestHash))
            {
                Debug.LogError("无法解析 master.manifest");
                return false;
            }

            // **3 检查本地是否有旧版本**
            var localHash = GetLocalMasterBundleHash(assetBundleName);
            if (localHash == latestHash)
            {
                Debug.Log("本地 master AssetBundle 已是最新版本，无需更新");
                return true;
            }

            // **4 下载最新的 master**
            var assetUrl = string.Format("{0}/api/files/{1}", loginRootUrl, assetBundleName);
            var request = UnityWebRequestAssetBundle.GetAssetBundle(assetUrl);
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("master AssetBundle 下载失败: " + request.error);
                return false;
            }

            // **5 缓存到本地**
            File.WriteAllBytes(localFilePath, request.downloadHandler.data);
            Debug.Log("已缓存新版本: " + assetBundleName);
            Debug.Log("缓存路径" + Application.persistentDataPath);

            return true;
        }

        // **解析 master.manifest 获取 Hash**
        private string ExtractHashFromManifest(string manifestContent)
        {
            var lines = manifestContent.Split('\n');
            foreach (var line in lines)
                if (line.Trim().StartsWith("Hash:"))
                    return line.Split(':')[1].Trim();

            return null;
        }

        // **获取本地 master AssetBundle 的 Hash**
        private string GetLocalMasterBundleHash(string assetBundleName)
        {
            var localManifestPath = Path.Combine(LocalPath, assetBundleName + ".manifest");
            if (!File.Exists(localManifestPath)) return null;

            var manifestContent = File.ReadAllText(localManifestPath);
            return ExtractHashFromManifest(manifestContent);
        }

        /// <summary>
        ///     从本地加载 AssetBundle，避免重复加载
        /// </summary>
        public static AssetBundle LoadAssetBundle(string assetBundleName)
        {
            var localFilePath = Path.Combine(LocalPath, assetBundleName);
            if (_loadedBundles.TryGetValue(localFilePath, out var bundle)) return bundle; // 直接返回已加载的 bundle

            if (!File.Exists(localFilePath))
            {
                Debug.LogError($"AssetBundle 文件不存在: {localFilePath}");
                return null;
            }

            bundle = AssetBundle.LoadFromFile(localFilePath);
            if (bundle == null)
            {
                Debug.LogError($"加载 AssetBundle 失败: {localFilePath}");
                return null;
            }

            _loadedBundles[localFilePath] = bundle; // 缓存
            return bundle;
        }

        /// <summary>
        ///     卸载 AssetBundle，释放内存
        /// </summary>
        public static void UnloadBundle(string localFilePath, bool unloadAllLoadedObjects)
        {
            if (_loadedBundles.TryGetValue(localFilePath, out var bundle))
            {
                bundle.Unload(unloadAllLoadedObjects);
                _loadedBundles.Remove(localFilePath);
            }
        }
    }
}