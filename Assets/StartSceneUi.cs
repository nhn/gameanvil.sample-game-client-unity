using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneUi : MonoBehaviour
{
    // 화명과 연동된 객체
    public Button buttonPlatformTestStart;
    public Button buttonGameTestStart;

    public void Start()
    {
        buttonPlatformTestStart.onClick.AddListener(() =>
        {
            // 플랫폼 테스트 신으로 이동
            SceneManager.LoadScene(Constants.SCENE_AUTH);
        });

        buttonGameTestStart.onClick.AddListener(() =>
        {
            // 게임 테스트 신으로 이동
            SceneManager.LoadScene(Constants.SCENE_GAME_LOGIN);
        });
    }
}
