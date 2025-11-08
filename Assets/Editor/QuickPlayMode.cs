using UnityEditor;
using UnityEditor.SceneManagement;

public class QuickPlayMode
{
    [MenuItem("Tools/Play Current Scene _F5")] // F5 运行当前场景
    public static void PlayCurrentScene()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);
            EditorApplication.isPlaying = true;
        }
    }
}