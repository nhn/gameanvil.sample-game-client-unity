using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Toast.Gamebase;

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
            // Gamebase 정보가 없다면 진행안되게 팝업 창 노출
            if (string.IsNullOrWhiteSpace(GamebaseInfo.Instance.GamebaseUserId) || string.IsNullOrWhiteSpace(GamebaseInfo.Instance.GamebaseToken))
            {
                MessageUi.Instance.SetTextMessage("Gamebase is not initialize..... pleaze Wait....");
            }
            else
            {
                // 게임 테스트 신으로 이동
                SceneManager.LoadScene(Constants.SCENE_GAME_LOGIN);
            }
        });

        GamebaseInfo.Instance.SetActive(true);

        GamebaseInitialize();
    }

    public void GamebaseInitialize()
    {
        Gamebase.Initialize((launchingInfo, error) =>
        {
            if (Gamebase.IsSuccess(error) == true)
            {
                Debug.Log("Initialization succeeded.");

                var serverInfo = launchingInfo.launching.app.accessInfo.serverAddress;
                if(serverInfo != null)
                {
                    var serverData = serverInfo.Split(':');
                    GamebaseInfo.Instance.GameServerIp = serverData[0];
                    GamebaseInfo.Instance.GameServerPort = serverData[1];

                    GamebaseInfo.Instance.SetGamebaseInfo(string.Format("GameServerIP:{0}, GameServerPort:{1}", GamebaseInfo.Instance.GameServerIp, GamebaseInfo.Instance.GameServerPort));
                }

                GamebaseLogin();

                //Following notices are registered in the Gamebase Console
                var notice = launchingInfo.launching.notice;
                if (notice != null)
                {
                    if (string.IsNullOrEmpty(notice.message) == false)
                    {
                        Debug.Log(string.Format("title:{0}", notice.title));
                        Debug.Log(string.Format("message:{0}", notice.message));
                        Debug.Log(string.Format("url:{0}", notice.url));
                    }
                }

                //Status information of game app version set in the Gamebase Unity SDK initialization.
                var status = launchingInfo.launching.status;

                // Game status code (e.g. Under maintenance, Update is required, Service has been terminated)
                // refer to GamebaseLaunchingStatus
                if (status.code == GamebaseLaunchingStatus.IN_SERVICE)
                {
                    // Service is now normally provided.
                    
                }
                else
                {
                    switch (status.code)
                    {
                        case GamebaseLaunchingStatus.RECOMMEND_UPDATE:
                            {
                                // Update is recommended.
                                break;
                            }
                        // ... 
                        case GamebaseLaunchingStatus.INTERNAL_SERVER_ERROR:
                            {
                                // Error in internal server.
                                break;
                            }
                    }
                }
            }
            else
            {
                // Check the error code and handle the error appropriately.
                Debug.Log(string.Format("Initialization failed. error is {0}", error));
            }
        });
    }

    public void GamebaseLogin()
    {
        Gamebase.Login(GamebaseAuthProvider.GUEST, (authToken, error) =>
        {
            if (Gamebase.IsSuccess(error))
            {
                Debug.Log(string.Format("Login succeeded. Gamebase userId is {0}, token {1}", authToken.member.userId, authToken.token.accessToken));

                GamebaseInfo.Instance.GamebaseUserId = authToken.member.userId;
                GamebaseInfo.Instance.GamebaseToken = authToken.token.accessToken;
                GamebaseInfo.Instance.SetGamebaseInfo(string.Format("GameServerIP:{0}, GameServerPort:{1}, Gamebase UserId:{2}", GamebaseInfo.Instance.GameServerIp, GamebaseInfo.Instance.GameServerPort, GamebaseInfo.Instance.GamebaseUserId));
            }
            else
            {
                Debug.Log(string.Format("Login failed. error is {0}", error));
            }
        });
    }
}
