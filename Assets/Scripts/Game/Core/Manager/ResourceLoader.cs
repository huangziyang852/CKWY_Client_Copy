using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using Game.Config;
using Spine.Unity;
using UnityEngine;
using YooAsset;

namespace Game.Core.Manager
{
    public class ResourceLoader : MonoBehaviour
    {
        private static ResourceLoader _instance;

        public static ResourceLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("ResourceLoader");
                    _instance = obj.AddComponent<ResourceLoader>();
                    DontDestroyOnLoad(obj); // 确保切换场景时不被销毁
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject); // 防止创建多个实例
            }
        }

        public void InitYooAssets()
        {
            YooAssets.Initialize();
        }

        public async UniTask InitPackage(string packageName)
        {
            //创建资源包
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }


            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                var initParameters = new EditorSimulateModeParameters();
                initParameters.EditorFileSystemParameters = editorFileSystemParams;
                var initOperation = package.InitializeAsync(initParameters);
                await initOperation;
                if (initOperation.Status == EOperationStatus.Succeed)
                {
                    Debug.Log("资源包初始化成功！");
                    // 2. 请求资源清单的版本信息
                    var operation = package.RequestPackageVersionAsync();
                    await operation;
                    if (operation.Status == EOperationStatus.Succeed)
                    {
                        Debug.Log("请求版本成功！");
                        // 3. 传入的版本信息更新资源清单
                        var updateOperation = package.UpdatePackageManifestAsync(operation.PackageVersion);
                        await updateOperation;

                        if (initOperation.Status == EOperationStatus.Succeed)
                        {
                            Debug.Log("资源包更新成功！");
                            await Download(packageName);
                        }
                        else
                        {
                            Debug.LogError($"资源包更新失败：{initOperation.Error}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"请求版本失败：{initOperation.Error}");
                    }
                }
                else
                {
                    Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                }
            }
            else if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                string defaultHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/WebGL/" + packageName;
                string fallbackHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/WebGL/" + packageName;

                Debug.Log("WebGL下载地址defaultHostServer:"+ defaultHostServer);
                Debug.Log("WebGL下载地址fallbackHostServer:" + fallbackHostServer);

                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载
                var createParameters = new WebPlayModeParameters();
                createParameters.WebServerFileSystemParameters = webServerFileSystemParams;
                createParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;

                var initOperation = package.InitializeAsync(createParameters);
                await initOperation;

                if (initOperation.Status == EOperationStatus.Succeed)
                {
                    Debug.Log("资源包初始化成功！");
                    await RequestPackageVersion(packageName);
                }
                else
                {
                    Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                }  
            }
            else
            {
                var defaultHostServer = string.Empty;
                var fallbackHostServer = string.Empty;

                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                        defaultHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Windows/" + packageName;
                        fallbackHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Windows/" + packageName;
                        break;
                    case RuntimePlatform.Android:
                        defaultHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Android/" + packageName;
                        fallbackHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Android/" + packageName;
                        break;
                    case RuntimePlatform.WindowsEditor:
                        defaultHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Windows/" + packageName;
                        fallbackHostServer = ConfigManager.NetInfo.FileDownloadUrl + "/Windows/" + packageName;
                        break;
                }

                if (ConfigManager.NetInfo.UseRemoteResource == false)
                {
                    defaultHostServer = ConfigManager.NetInfo.LocalFileDownLoadUrl + "/Windows/" + packageName;
                    fallbackHostServer = ConfigManager.NetInfo.LocalFileDownLoadUrl + "/Windows/" + packageName;
                }

                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                var buildinFileSystemParams =
                    FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices);

                var initParameters = new HostPlayModeParameters();
                initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                initParameters.CacheFileSystemParameters = cacheFileSystemParams;

                var initOperation = package.InitializeAsync(initParameters);
                await initOperation;

                if (initOperation.Status == EOperationStatus.Succeed)
                {
                    Debug.Log("资源包初始化成功！");
                    await RequestPackageVersion(packageName);
                }
                else
                {
                    Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                }
            }
        }

        private IEnumerator Download(string packageName)
        {
            var downloadingMaxNum = 10;
            var failedTryAgain = 3;
            var package = YooAssets.GetPackage(packageName);
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0) yield return true;

            //需要下载的文件总数和总大小
            var totalDownloadCount = downloader.TotalDownloadCount;
            var totalDownloadBytes = downloader.TotalDownloadBytes;
            Debug.Log("download file count:" + totalDownloadCount + ",download file sizes" + totalDownloadBytes);

            //注册回调方法
            downloader.DownloadFinishCallback = OnDownloadFinishFunction; //当下载器结束（无论成功或失败）
            downloader.DownloadErrorCallback = OnDownloadErrorFunction; //当下载器发生错误
            downloader.DownloadUpdateCallback = OnDownloadUpdateFunction; //当下载进度发生变化
            downloader.DownloadFileBeginCallback = OnDownloadFileBeginFunction; //当开始下载某个文件

            //开启下载
            downloader.BeginDownload();
            yield return downloader;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
                //下载成功
                yield return true;
            else
                //下载失败
                yield return false;
        }

        private void OnDownloadFinishFunction(DownloaderFinishData downloaderFinishData)
        {
            Debug.Log("download finished");
        }

        private void OnDownloadErrorFunction(DownloadErrorData downloadErrorData)
        {
            Debug.Log("download error");
        }

        private void OnDownloadUpdateFunction(DownloadUpdateData downloadUpdateData)
        {
            Debug.Log("download update");
        }

        private void OnDownloadFileBeginFunction(DownloadFileData downloadFileData)
        {
            Debug.Log("download begin");
        }


        private async UniTask<bool> RequestPackageVersion(string packageName)
        {
            Debug.Log("开始更新资源包:" + packageName);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            var package = YooAssets.TryGetPackage(packageName);

            var operation = package.RequestPackageVersionAsync();

            await operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                var packageVersion = operation.PackageVersion;

                await package.UpdatePackageManifestAsync(operation.PackageVersion);
                Debug.Log($"Request package Version : {packageVersion}");
                return true;
            }

            //更新失败
            Debug.LogError(operation.Error);
            return false;
        }

        /// <summary>
        ///     加载预制体
        /// </summary>
        /// <param name="address"></param>
        /// <param name="callback"></param>
        public void LoadRemotePrefab(string address, Action<GameObject> callback)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<GameObject>(address);

            package.LoadAssetAsync<GameObject>(address).Completed += handle =>
            {
                if (handle.Status == EOperationStatus.Succeed)
                {
                    var prefab = handle.AssetObject as GameObject;
                    callback?.Invoke(prefab);
                }
                else
                {
                    Debug.LogError($"❌ 资源加载失败: {address}");
                }
            };
        }

        public async UniTask<GameObject> LoadRemotePrefabAsync(string path)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<GameObject>(path);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed) return handle.AssetObject as GameObject;

            Debug.LogError($"❌ 预制体加载失败: {path}");
            return null;
        }

        /// <summary>
        ///     加载图片
        /// </summary>
        /// <param name="address"></param>
        /// <param name="callback"></param>
        public void LoadRemoteSprite(string address, Action<Sprite> callback)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<Sprite>(address);

            package.LoadAssetAsync<Sprite>(address).Completed += handle =>
            {
                if (handle.Status == EOperationStatus.Succeed)
                {
                    var sprite = handle.AssetObject as Sprite;
                    callback?.Invoke(sprite);
                }
                else
                {
                    Debug.LogError($"❌ 资源加载失败: {address}");
                }
            };
        }

        /// <summary>
        ///     异步加载无回调
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask<Sprite> LoadRemoteSpriteAsync(string path)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<Sprite>(path);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed) return handle.AssetObject as Sprite;

            Debug.LogError($"❌ 资源加载失败: {path}");
            return null;
        }

        /// <summary>
        ///     加载音频
        /// </summary>
        /// <param name="address"></param>
        /// <param name="callback"></param>
        public void LoadRemoteAudio(string address, Action<AudioClip> callback)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<AudioClip>(address);

            package.LoadAssetAsync<AudioClip>(address).Completed += handle =>
            {
                if (handle.Status == EOperationStatus.Succeed)
                {
                    var audioClip = handle.AssetObject as AudioClip;
                    callback?.Invoke(audioClip);
                }
                else
                {
                    Debug.LogError($"❌ 资源加载失败: {address}");
                }
            };
        }

        public async UniTask<AudioClip> LoadRemoteAudio(string path)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<AudioClip>(path);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed) return handle.AssetObject as AudioClip;

            Debug.LogError($"❌ 资源加载失败: {path}");
            return null;
        }

        public void LoadRemoteSkeletonData(string path, Action<SkeletonDataAsset> callback)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<SkeletonDataAsset>(path);

            package.LoadAssetAsync<SkeletonDataAsset>(path).Completed += handle =>
            {
                if (handle.Status == EOperationStatus.Succeed)
                {
                    var skeletonDataAsset = handle.AssetObject as SkeletonDataAsset;
                    callback?.Invoke(skeletonDataAsset);
                }
                else
                {
                    Debug.LogError($"❌ 资源加载失败: {path}");
                }
            };
        }

        /// <summary>
        ///     异步加载spine，无回调
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask<SkeletonDataAsset> LoadRemoteSkeletonData(string path)
        {
            var package = YooAssets.TryGetPackage("RemoteResource");
            var handle = package.LoadAssetAsync<SkeletonDataAsset>(path);
            await handle.ToUniTask();

            if (handle.Status == EOperationStatus.Succeed) return handle.AssetObject as SkeletonDataAsset;

            Debug.LogError($"❌ 资源加载失败: {path}");
            return null;
        }

        /// <summary>
        ///     远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }

            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }

            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }
}