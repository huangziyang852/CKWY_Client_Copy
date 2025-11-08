using UnityEditor;
using UnityEngine;

public class PrefabDependenciesFinder
{
    [MenuItem("Tools/Find Prefab Dependencies")]
    public static void FindDependencies()
    {
        var prefabPath = EditorUtility.OpenFilePanel("Select Prefab", "Assets", "prefab");
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.Log("δѡ�� Prefab");
            return;
        }

        prefabPath = "Assets" + prefabPath.Substring(Application.dataPath.Length);
        var dependencies = AssetDatabase.GetDependencies(prefabPath, true);

        Debug.Log("Prefab ��������Դ�б���");
        foreach (var dependency in dependencies) Debug.Log(dependency);
    }
}