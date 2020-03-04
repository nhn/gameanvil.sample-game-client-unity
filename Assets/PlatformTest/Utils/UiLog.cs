using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 로그 UI
public class UiLog : MonoBehaviour
{
    private static UiLog uiLog;
    private string logText;
    private Queue logQueue = new Queue();

    // 화면 객체
    private Text logTextUi = null;
    private ScrollRect scrollRect = null;

    private void Awake()
    {
        if (uiLog != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        uiLog = this;
    }

    public static UiLog Instance
    {
        get
        {
            return uiLog;
        }
    }

    void Start()
    {
        logTextUi = GameObject.Find("TextLog").GetComponent<Text>();
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
    }

    void Update()
    {
        if (logTextUi.text.Length > 15000)
        {
            ResetLog();
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logText = logString;
        string newString = "\n [" + type + "] : " + logText;
        logQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            logQueue.Enqueue(newString);
        }
        logText = string.Empty;
        foreach (string log in logQueue)
        {
            logText += log;
        }
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0.0f;
        }
    }

    void ResetLog()
    {
        logText = "";
        logQueue.Clear();
    }

    void OnGUI()
    {
        logTextUi.text = logText;
    }
}
