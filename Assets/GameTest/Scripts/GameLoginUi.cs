using System.Collections;
using System.Collections.Generic;
using Tardis;
using Tardis.Defines;
using Tardis.Session;
using Tardis.User;
using TardisConnector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameLoginUi : MonoBehaviour
{
    // 화면과 연결되는 필드들
    public Text textUUID;
    public Button buttonGenerateUUID;
    public InputField inputFieldLaunching;
    public InputField inputFieldID;
    public Button buttonLogin;

    // Start is called before the first frame update
    void Start()
    {
        // 기본값 지정
        textUUID.text = PlayerPrefs.GetString(Constants.KEY_UUID);
        if (string.IsNullOrWhiteSpace(textUUID.text))
        {
            textUUID.text = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(Constants.KEY_UUID, textUUID.text);
        }

        string launching = PlayerPrefs.GetString(Constants.KEY_LAUNCHING);
        if (string.IsNullOrWhiteSpace(launching))
        {
            inputFieldLaunching.text = "http://127.0.0.1:10080";
        }
        else
        {
            inputFieldLaunching.text = launching;
        }

        string id = PlayerPrefs.GetString(Constants.KEY_USER_ID);
        if (string.IsNullOrWhiteSpace(id))
        {
            inputFieldID.text = "SampleID";
        }
        else
        {
            inputFieldID.text = id;
        }

        // 버튼 클릭에 대한 리스너등록
        buttonGenerateUUID.onClick.AddListener(() => { OnClickGenerateUUID(); });
        buttonLogin.onClick.AddListener(() => { OnClickLogin(); });

        Debug.Log("Initialize complete!!!! " + textUUID.text);

        // ===========================================================================================>>> Tardis
        // 연결 끊기는 부분 처리 리스너 등록
        ConnectHandler.Instance.GetSessionAgent().onDisconnectListeners += (SessionAgent sessionAgent, ResultCodeDisconnect result, bool force, Payload payload) =>
        {
            Debug.LogFormat("onDisconnect - {0}", result);
            UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOGIN);
        };

        // 세션 연결 리스너
        ConnectHandler.Instance.GetSessionAgent().onConnectListeners += (SessionAgent sessionAgent, ResultCodeConnect result) =>
        {
            if (result == ResultCodeConnect.CONNECT_SUCCESS)
            {
                // 인증에 필요한 프로토콜 데이터 설정
                var authenticationReq = new Com.Nhn.Tardis.Sample.Protocol.AuthenticationReq
                {
                    AccessToken = Constants.AUTH_ACCESS_TOKEN
                };
                Debug.Log("authenticationReq " + authenticationReq);

                // 서버에 인증 시도. 현재는 deviceid, id, pw 모두 uuid값으로 전달
                ConnectHandler.Instance.GetSessionAgent().Authenticate(textUUID.text, inputFieldID.text, inputFieldID.text, new Payload().add(new Packet(authenticationReq)));
            }
            else
            {
                // 서버 접속 실패 처리
                buttonLogin.interactable = true;
            }
        };

        // 인증 리스너
        ConnectHandler.Instance.GetSessionAgent().onAuthenticationListeners +=
            (SessionAgent sessionAgent, ResultCodeAuth result, List<SessionAgent.LoginedUserInfo> loginedUserInfoList, string message, Payload payload) =>
            {
                Debug.Log("Auth " + result);

                // 성공인 경우 다음 단계로 진행.
                if (result == ResultCodeAuth.AUTH_SUCCESS)
                {
                    PlayerPrefs.SetString(Constants.KEY_USER_ID, inputFieldID.text);

                    // 성공시 로그인 요청
                    TardisLogin();
                }
                else
                {
                    // 실패시 처리
                    buttonLogin.interactable = true;
                }

            };
        // ===========================================================================================>>> Tardis
    }

    void OnClickGenerateUUID()
    {
        // uuid 갱신 처리
        textUUID.text = System.Guid.NewGuid().ToString();
        PlayerPrefs.SetString(Constants.KEY_UUID, textUUID.text);
        Debug.Log("OnClickGenerateUUID " + textUUID.text);
    }

    void OnClickLogin()
    {
        string url = inputFieldLaunching.text;
        PlayerPrefs.SetString(Constants.KEY_LAUNCHING, url);
        url += "/launching?platform=" + Application.platform + "&appStore=NONE&appVersion=" + Application.version + "&deviceId=" + textUUID.text;
        //        string url = "http://10.77.35.47:10080/launching?platform=Editor&appStore=GOOGLE&appVersion=1.2.0&deviceId=4D34C127-9C56-5BAB-A3C2-D8F18C0B7B6E";

        StartCoroutine(GetRequestLaunching(url));
    }

    IEnumerator GetRequestLaunching(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // json파싱해서 객체로 매핑
            LaunchingInfo launchingInfo = JsonUtility.FromJson<LaunchingInfo>(www.downloadHandler.text);

            inputFieldLaunching.enabled = false;

            // ===========================================================================================>>> Tardis
            // 런칭 정보로 tcp connect요청
            ConnectHandler.Instance.GetSessionAgent().Connect(launchingInfo.serverUrl, launchingInfo.port);
            // ===========================================================================================>>> Tardis
        }
    }

    void TardisLogin()
    {
        // 로그인에 필요한 프로토콜 데이터 설정
        var loginReq = new Com.Nhn.Tardis.Sample.Protocol.LoginReq
        {
            Uuid = textUUID.text,
            LoginType = Com.Nhn.Tardis.Sample.Protocol.LoginType.LoginGuest,
            AppVersion = "0.0.1",
            AppStore = "None",
            DeviceModel = SystemInfo.deviceModel,
            DeviceCountry = "KR",
            DeviceLanguage = "ko"

        };
        Debug.Log("loginReq " + loginReq);

        // ===========================================================================================>>> Tardis
        // 서버에 로그인
        ConnectHandler.Instance.CreateUserAgent(Constants.GAME_SPACE_NAME, string.Empty).Login(Constants.SPACE_USER_TYPE, string.Empty, new Payload().add(new Packet(loginReq)),
            (UserAgent userAgent, ResultCodeLogin result, UserAgent.LoginInfo loginInfo) =>
            {
                Debug.Log("Login " + result + ", " + loginInfo);

                // 성공시 다음 단계.
                if (result == ResultCodeLogin.LOGIN_SUCCESS)
                {
                    if (loginInfo.Payload.contains<Com.Nhn.Tardis.Sample.Protocol.LoginRes>())
                    {
                        // 로그인 응답 프로토콜 처리
                        Com.Nhn.Tardis.Sample.Protocol.LoginRes loginRes = Com.Nhn.Tardis.Sample.Protocol.LoginRes.Parser.ParseFrom(loginInfo.Payload.getPacket<Com.Nhn.Tardis.Sample.Protocol.LoginRes>().GetBytes());
                        Debug.Log("LoginRes " + loginRes);

                        // 서버에서 받은 게임 데이터 설정
                        UserInfo.Instance.Uuid = textUUID.text;
                        UserInfo.Instance.Nickname = loginRes.Userdata.Nickname;
                        UserInfo.Instance.Heart = loginRes.Userdata.Heart;
                        UserInfo.Instance.Coin = loginRes.Userdata.Coin;
                        UserInfo.Instance.Ruby = loginRes.Userdata.Ruby;
                        UserInfo.Instance.Level = loginRes.Userdata.Level;
                        UserInfo.Instance.Exp = loginRes.Userdata.Exp;
                        UserInfo.Instance.HighScore = loginRes.Userdata.HighScore;
                        UserInfo.Instance.CurrentDeck = loginRes.Userdata.CurrentDeck;

                        // 신전환신 버튼 리스너 모두 해재
                        buttonGenerateUUID.onClick.RemoveAllListeners();
                        buttonLogin.onClick.RemoveAllListeners();
                        UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOBBY);
                    }
                }
                else
                {
                    // 실패 처리
                }
                buttonLogin.interactable = true;
            }
       );
        // ===========================================================================================>>> Tardis
    }
}
