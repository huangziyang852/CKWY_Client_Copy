using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using YooAsset;

namespace Game.Core.Manager
{
    public class TableManager
    {
        private static TableManager _instance;

        private Dictionary<string, string> jsonCache = new();

        public Tables MasterTables { get; private set; }

        public static TableManager Instance
        {
            get
            {
                if (_instance == null) _instance = new TableManager();
                return _instance;
            }
        }

        /// <summary>
        ///     缓存所有数据表
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> LoadMasterTableAsync()
        {
            var tempCache = new Dictionary<string, string>();

            //加载所有 Master 组的 JSON
            var package = YooAssets.TryGetPackage("Master");
            var handle = package.LoadAllAssetsAsync<TextAsset>("Assets/Download/Master/hero_tbhero.json");
            await handle;

            if (handle.Status != EOperationStatus.Succeed || handle.AllAssetObjects.Count == 0)
            {
                Debug.LogError("❌ 加载 Master 组的所有 JSON 失败");
                return false;
            }

            // 遍历加载的 JSON 资源，并存入缓存
            foreach (var assetObjet in handle.AllAssetObjects)
            {
                var jsonAsset = assetObjet as TextAsset;
                if (jsonAsset == null) continue;

                var fileName = jsonAsset.name; // 获取 JSON 文件名（不包含路径和扩展名）
                tempCache[fileName] = jsonAsset.text;
            }

            // 初始化 masterTables
            MasterTables = new Tables(file =>
            {
                if (!tempCache.TryGetValue(file, out var jsonString))
                {
                    Debug.LogError($"JSON 缓存中未找到文件: {file}");
                    return null;
                }

                return JSON.Parse(jsonString);
            });


            var hero = MasterTables.TbHero.Get(101001);
            Debug.Log(hero.Name);
            // **释放 Addressables 资源**
            handle.Release();
            return true;
        }
    }
}