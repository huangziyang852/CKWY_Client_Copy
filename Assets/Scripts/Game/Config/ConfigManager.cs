using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Config
{
    public class ConfigManager
    {
        private const string configFile = "config.json";
        private static ConfigManager _instance;

        public static PathConfig pathConfig;

        public static MConfig NetInfo;

        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ConfigManager();

                return _instance;
            }
        }

        public void Init()
        {
            try
            {
                var configText = Resources.Load<TextAsset>("Config/" + Path.GetFileNameWithoutExtension(configFile));
                if (configText != null)
                {
                    var jsonContent = configText.text;
                    NetInfo = JsonConvert.DeserializeObject<MConfig>(jsonContent);
                }
                else
                {
                    Debug.LogError("can not find config file");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            LoadPathConfig();
        }

        public static void LoadPathConfig()
        {
            pathConfig = Resources.Load<PathConfig>("Config/PathConfig");
        }
    }

    public class MConfig
    {
        public bool UseRemoteResource { get; set; }
        public string LoginRootUrl { get; set; }
        public string FileDownloadUrl { get; set; }
        public string LocalLoginRootUrl { get; set; }
        public string LocalFileDownLoadUrl { get; set; }
        public string LoginRootChannel { get; set; }
        public string LoginRootSubChannel { get; set; }
        public string LoginRootVersion { get; set; }
    }
}