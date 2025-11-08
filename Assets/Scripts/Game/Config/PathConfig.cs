using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "PathConfig", menuName = "Config/PathConfig")]
    public class PathConfig : ScriptableObject
    {
        public List<CategoryPaths> pathsCategories; // �� List ���� Dictionary

        private Dictionary<string, Dictionary<string, string>> _pathDict;

        private void Init()
        {
            _pathDict = new Dictionary<string, Dictionary<string, string>>();

            foreach (var category in pathsCategories)
            {
                var categoryDict = new Dictionary<string, string>();
                foreach (var entry in category.entries) categoryDict[entry.key] = entry.path;
                _pathDict[category.categoryName] = categoryDict;
            }
        }

        public string GetPath(string categoryKey, string key)
        {
            if (_pathDict == null) Init();

            if (_pathDict.TryGetValue(categoryKey, out var categoryDict))
                return categoryDict.TryGetValue(key, out var path) ? path : null;

            return null;
        }

        [Serializable]
        public class PathEntry
        {
            public string key;
            public string path;
        }

        [Serializable]
        public class CategoryPaths
        {
            public string categoryName; // ���ڴ��� Dictionary �� key
            public List<PathEntry> entries;
        }
    }
}