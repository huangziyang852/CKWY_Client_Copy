using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class LubanToolWindow : EditorWindow
{
    private StringBuilder logBuilder = new StringBuilder();
    private Vector2 scrollPos;

    private bool isRunning = false;
    private float progress = 0f;

    // 配置路径
    private static readonly string WORKSPACE = @"E:\C#\master";
    private static readonly string LUBAN_DLL = Path.Combine(WORKSPACE, "Luban", "Luban.dll");
    private static readonly string CONF_ROOT = @"E:\C#\master";
    private static readonly string OUTPUT_CODE_DIR = Path.Combine(WORKSPACE, "code");
    private static readonly string OUTPUT_JSON_DIR = Path.Combine(WORKSPACE, "json");

    // Unity 目标路径
    private static readonly string UNITY_PROJECT_ROOT = @"E:\Unity\Project\ckwy";
    private static readonly string UNITY_CODE_DIR = Path.Combine(UNITY_PROJECT_ROOT, @"Assets\Scripts\Table\code");
    private static readonly string UNITY_JSON_DIR = Path.Combine(UNITY_PROJECT_ROOT, @"Assets\Download\Master");

    [MenuItem("Tools/Luban/打开生成工具")]
    public static void ShowWindow()
    {
        GetWindow<LubanToolWindow>("Luban 配置生成器");
    }

    private void OnGUI()
    {
        GUILayout.Label("Luban 配置生成工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (isRunning)
        {
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), progress, "正在生成中...");
            GUILayout.Space(10);
            if (GUILayout.Button("强制停止", GUILayout.Height(25)))
            {
                isRunning = false;
            }
        }
        else
        {
            if (GUILayout.Button("生成配置表并同步到 Unity", GUILayout.Height(30)))
            {
                _ = RunLubanAsync();
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("输出日志：", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));
        GUILayout.TextArea(logBuilder.ToString(), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        GUILayout.Space(5);
        if (GUILayout.Button("清空日志")) logBuilder.Clear();
    }

    private async Task RunLubanAsync()
    {
        isRunning = true;
        progress = 0f;
        logBuilder.Clear();

        AppendLog("开始执行 Luban 配置生成...");

        try
        {
            await Task.Run(() =>
            {
                string command = $"dotnet \"{LUBAN_DLL}\" -t all -c cs-simple-json -d json --conf \"{CONF_ROOT}\\luban.conf\" -x outputCodeDir=\"{OUTPUT_CODE_DIR}\" -x outputDataDir=\"{OUTPUT_JSON_DIR}\"";

                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    WorkingDirectory = WORKSPACE,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process())
                {
                    process.StartInfo = psi;

                    process.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLogThreadSafe(e.Data); };
                    process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLogThreadSafe("[Error] " + e.Data); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            });

            progress = 0.6f;
            AppendLog("Luban 生成完成，开始复制文件...");

            CopyDirectory(OUTPUT_CODE_DIR, UNITY_CODE_DIR, true);
            progress = 0.85f;

            CopyDirectory(OUTPUT_JSON_DIR, UNITY_JSON_DIR, true);
            progress = 1.0f;

            AssetDatabase.Refresh();
            AppendLog("配置表生成并同步到 Unity 成功！");
        }
        catch (System.Exception ex)
        {
            AppendLog($"生成失败：{ex.Message}");
        }
        finally
        {
            isRunning = false;
        }
    }

    private void CopyDirectory(string sourceDir, string destDir, bool overwrite)
    {
        if (!Directory.Exists(sourceDir))
        {
            AppendLog($"源目录不存在: {sourceDir}");
            return;
        }

        Directory.CreateDirectory(destDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destPath = Path.Combine(destDir, fileName);
            File.Copy(file, destPath, overwrite);
        }

        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string dirName = Path.GetFileName(dir);
            string destSubDir = Path.Combine(destDir, dirName);
            CopyDirectory(dir, destSubDir, overwrite);
        }
    }

    private void AppendLog(string msg)
    {
        logBuilder.AppendLine(msg);
        Repaint();
    }

    private void AppendLogThreadSafe(string msg)
    {
        lock (logBuilder)
        {
            logBuilder.AppendLine(msg);
        }
    }
}
