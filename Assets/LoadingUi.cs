using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 로딩신 관리
public class LoadingUi : MonoBehaviour
{
    // 화면과 연될된 객체
    public Text textTime;
    public Text textMoveScene;

    private float elapsedDelta;
    private string moveScene;

    // Start is called before the first frame update
    void Start()
    {
        // 로딩 시간확인
        elapsedDelta = Constants.LOADING_TIME;

        moveScene = UserInfo.Instance.Scene;

        updateTimeUI(elapsedDelta);
        textMoveScene.text = "Scene Lodding... move to " + moveScene;

        Debug.Log(textMoveScene.text);

        if(UiLog.Instance != null)
        {
            UiLog.Instance.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 로딩 신에서 시간 확인
        if (elapsedDelta > 0)
        {
            elapsedDelta -= Time.deltaTime;

            if (int.Parse(textTime.text) != Mathf.RoundToInt(elapsedDelta))
            {
                updateTimeUI(elapsedDelta);
            }
        }
        else
        {
            if (moveScene != Constants.SCENE_GAME_TAPBIRD && moveScene != Constants.SCENE_GAME_SNAKE) {
                if (UiLog.Instance != null)
                {
                    UiLog.Instance.gameObject.SetActive(true);
                }
            }
            // 신이동
            SceneManager.LoadScene(moveScene);
        }
    }

    void updateTimeUI(float time)
    {
        // 로딩 시간 갱신
        textTime.text = Mathf.RoundToInt(time).ToString();
    }
}
