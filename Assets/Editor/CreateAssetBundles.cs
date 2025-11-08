using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Tools/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        var bundlePath = "Assets/AssetBundles";
        if (!Directory.Exists(bundlePath))
            Directory.CreateDirectory(bundlePath);

        BuildPipeline.BuildAssetBundles(bundlePath, BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }
}