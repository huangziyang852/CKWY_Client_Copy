using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneDependenciesFinder
{
    [MenuItem("Tools/Find Scene Dependencies")]
    private static void FindDependencies()
    {
        var scenePath = EditorSceneManager.GetActiveScene().path;
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError("���ȴ�һ��������");
            return;
        }

        var allObjects = Object.FindObjectsOfType<GameObject>();
        var dependencies = new HashSet<Object>();

        foreach (var obj in allObjects)
        {
            var objDependencies = EditorUtility.CollectDependencies(new Object[] { obj });
            dependencies.UnionWith(objDependencies);
        }

        Debug.Log($"���� {scenePath} ʹ�õ�����Դ:");
        foreach (var dep in dependencies)
        {
            var assetPath = AssetDatabase.GetAssetPath(dep);
            if (!string.IsNullOrEmpty(assetPath)) Debug.Log(assetPath, dep);
        }
    }
}