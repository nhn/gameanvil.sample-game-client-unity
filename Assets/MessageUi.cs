using UnityEngine;
using UnityEngine.UI;

// 게임화면 테스트 시에 필요에 의해서 UI에 얼럿을 띄울용도로 만든 객체
public class MessageUi : MonoBehaviour
{
    private static MessageUi messageUi;

    public Text TextMessage;
    public GameObject panel;

    public static MessageUi Instance
    {
        get
        {
            return messageUi;
        }
    }

    private void Awake()
    {
        if (messageUi != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        messageUi = this;

    }

    // 화면에 메세지 표시
    public void SetTextMessage(string message)
    {
        TextMessage.text = message;
        panel.SetActive(true);
    }
}
