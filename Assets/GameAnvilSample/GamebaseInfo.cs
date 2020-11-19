using UnityEngine;
using UnityEngine.UI;

// Gamebase 정보 객체
public class GamebaseInfo : MonoBehaviour
{
    private static GamebaseInfo gamebaseInfo;

    public Text TextGamebaseInfo;

    public string GameServerIp { get; set; }
    public string GameServerPort { get; set; }
    public string GamebaseUserId { get; set; }
    public string GamebaseToken { get; set; }

    public static GamebaseInfo Instance
    {
        get
        {
            return gamebaseInfo;
        }
    }

    private void Awake()
    {
        if (gamebaseInfo != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        gamebaseInfo = this;

    }

    public void SetGamebaseInfo(string info)
    {
        TextGamebaseInfo.text = info;
    }

    public void SetActive(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }
}
