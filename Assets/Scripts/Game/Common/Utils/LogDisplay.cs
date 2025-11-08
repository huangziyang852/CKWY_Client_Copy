using TMPro;
using UnityEngine;

// �����ʹ�õ�����ͨ UI Text��ʹ����������ռ�

public class LogDisplay : MonoBehaviour
{
    public TextMeshProUGUI logText; // ���ʹ�� TextMeshPro����ĳ� TMP_Text
    private string logMessage = "";

    private void OnEnable()
    {
        // ���Ŀ���̨��־�¼�
        Application.logMessageReceived += LogCallback;
    }

    private void OnDisable()
    {
        // ȡ�����Ŀ���̨��־�¼�
        Application.logMessageReceived -= LogCallback;
    }

    private void LogCallback(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Error) return; // ����¼������־
        // ׷����־��Ϣ�� logMessage
        logMessage += logString + "\n";
        // ������־��������ֹ�ڴ�й©
        if (logMessage.Length > 1000)
            logMessage = logMessage.Substring(logMessage.Length - 1000); // ֻ������� 1000 ���ַ�
        // ���� Text ��ʾ
        logText.text = logMessage;
    }
}