using System.Collections;
using System.Collections.Generic;
using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.Connection;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// 인증 화면
public class AuthUi : MonoBehaviour
{
    // 화면과 연결되는 필드들
    public InputField inputFieldUUID;
    public Button buttonGenerateUUID;
    public InputField inputFieldLaunching;
    public Button buttonGetLaunching;
    public Text textIP;
    public Text textPort;
    public InputField inputFieldID;
    public Text textID;
    public Button buttonConnect;
    public Button buttonAuth;
    public Button buttonLogin;

    // Start is called before the first frame update
    void Start()
    {
        // 기본값 지정
        inputFieldUUID.text = PlayerPrefs.GetString(Constants.KEY_UUID);
        if (string.IsNullOrWhiteSpace(inputFieldUUID.text))
        {
            OnClickGenerateUUID();
        }

        string launching = PlayerPrefs.GetString(Constants.KEY_LAUNCHING);
        if (string.IsNullOrWhiteSpace(launching))
        {
            inputFieldLaunching.text = "http://127.0.0.1:18600";
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
        buttonGetLaunching.onClick.AddListener(() => { OnClickGetLaunching(); });
        buttonConnect.onClick.AddListener(() => { OnClickConnect(); });
        buttonAuth.onClick.AddListener(() => { OnClickAuth(); });
        buttonLogin.onClick.AddListener(() => { OnClickLogin(); });

        UiReset();

        GamebaseInfo.Instance.SetActive(false);

        Debug.Log("Initialize complete!!!! " + inputFieldUUID.text);

        // ===========================================================================================>>> GameAnvil
        // 연결 끊기는 부분 처리 리스너 등록
        ConnectHandler.Instance.GetConnectionAgent().onDisconnectListeners += (ConnectionAgent connectionAgent, ResultCodeDisconnect result, bool force, Payload payload) =>
        {
            Debug.LogFormat("onDisconnect - {0}", result);
            UserInfo.Instance.MoveScene(Constants.SCENE_AUTH);
        };
        // ===========================================================================================>>> GameAnvil
    }


    // 버튼과 일력제한 기본값 설정
    void UiReset()
    {
        buttonAuth.gameObject.SetActive(false);
        buttonLogin.gameObject.SetActive(false);
        buttonConnect.gameObject.SetActive(false);
        textID.gameObject.SetActive(false);

        buttonGetLaunching.gameObject.SetActive(true);
        inputFieldLaunching.enabled = true;
    }

    void OnClickGenerateUUID()
    {
        // uuid 갱신 처리
        inputFieldUUID.text = TestHelper.GenerateUUID();
    }

    void OnClickGetLaunching()
    {
        // 다중 클릭 막음
        buttonGetLaunching.interactable = false;

        // sample url "http://10.77.35.47:18600/launching?platform=Editor&appStore=GOOGLE&appVersion=1.2.0&deviceId=4D34C127-9C56-5BAB-A3C2-D8F18C0B7B6E";

        string url = inputFieldLaunching.text;
        PlayerPrefs.SetString(Constants.KEY_LAUNCHING, url);
        url += "/launching?platform=" + Application.platform + "&appStore=NONE&appVersion=" + Application.version + "&deviceId=" + inputFieldUUID.text;

        StartCoroutine(GetRequestLaunching(url));
    }

    void OnClickConnect()
    {
        // 다중 클릭 막음
        buttonConnect.interactable = false;

        // ===========================================================================================>>> GameAnvil
        // 서버에 접속 시도.
        ConnectHandler.Instance.GetConnectionAgent().Connect(textIP.text, int.Parse(textPort.text),
            (ConnectionAgent connectionAgent, ResultCodeConnect result) =>
            {
                Debug.Log("Connect " + textID.text + ":" + textPort.text + " " + result);

                if (result == ResultCodeConnect.CONNECT_SUCCESS)
                {
                    // 성공시 입력 UI 처리
                    buttonConnect.gameObject.SetActive(false);
                    buttonAuth.gameObject.SetActive(true);
                    textID.gameObject.SetActive(true);
                }
                else
                {
                    // 서버 접속 실패 처리
                }
                buttonConnect.interactable = true;
            }
        );
        // ===========================================================================================>>> GameAnvil
    }

    void OnClickAuth()
    {
        if (string.IsNullOrWhiteSpace(inputFieldID.text))
        {
            Debug.Log("ID field is empty!!!! ");
        }
        else if (inputFieldID.text.Length < 4)
        {
            Debug.Log("Please enter at least 4 digits.");
        }
        else
        {
            PlayerPrefs.SetString(Constants.KEY_USER_ID, inputFieldID.text);
            PlayerPrefs.SetString(Constants.KEY_UUID, inputFieldUUID.text);

            // 다중 클릭 막음
            buttonAuth.interactable = false;

            // 인증에 필요한 프로토콜 데이터 설정
            var authenticationReq = new Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReq
            {
                AccessToken = Constants.AUTH_ACCESS_TOKEN
            };
            Debug.Log("authenticationReq " + authenticationReq);

            // ===========================================================================================>>> GameAnvil
            // 서버에 인증 시도. 현재는 deviceid, id, pw 모두 uuid값으로 전달
            ConnectHandler.Instance.GetConnectionAgent().Authenticate(inputFieldUUID.text, inputFieldID.text, inputFieldID.text, new Payload().Add(new Packet(authenticationReq)),
                (ConnectionAgent connectionAgent, ResultCodeAuth result, List<ConnectionAgent.LoginedUserInfo> loginedUserInfoList, string message, Payload payload) =>
                {
                    Debug.Log("Auth " + result);

                    // 성공인 경우 다음 단계로 진행.
                    if (result == ResultCodeAuth.AUTH_SUCCESS)
                    {
                        // 성공시 입력 UI 처리
                        buttonAuth.gameObject.SetActive(false);
                        buttonLogin.gameObject.SetActive(true);
                        inputFieldID.enabled = false;
                    }
                    else
                    {
                        // 실패시 처리
                    }
                    buttonAuth.interactable = true;
                }
            );
            // ===========================================================================================>>> GameAnvil
        }
    }

    void OnClickLogin()
    {
        // 다중 클릭 막음
        buttonLogin.interactable = false;

        // 로그인에 필요한 프로토콜 데이터 설정
        var loginReq = new Com.Nhn.Gameanvil.Sample.Protocol.LoginReq
        {
            // 임의로 필요한 데이터 설정
            Uuid = inputFieldUUID.text,
            LoginType = Com.Nhn.Gameanvil.Sample.Protocol.LoginType.LoginGuest,
            AppVersion = Application.version,
            AppStore = "None",
            DeviceModel = SystemInfo.deviceModel,
            DeviceCountry = "KR",
            DeviceLanguage = "ko"

        };
        Debug.Log("loginReq " + loginReq);

        // ===========================================================================================>>> GameAnvil
        // 서버에 로그인
        ConnectHandler.Instance.CreateUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId).Login(Constants.SPACE_USER_TYPE, string.Empty, new Payload().Add(new Packet(loginReq)),
            (UserAgent userAgent, ResultCodeLogin result, UserAgent.LoginInfo loginInfo) =>
            {
                Debug.Log("Login " + result + ", " + loginInfo);

                // 성공시 다음 단계.
                if (result == ResultCodeLogin.LOGIN_SUCCESS)
                {
                    if (loginInfo.Payload.Contains<Com.Nhn.Gameanvil.Sample.Protocol.LoginRes>())
                    {
                        // 로그인 응답 프로토콜 처리
                        Com.Nhn.Gameanvil.Sample.Protocol.LoginRes loginRes = Com.Nhn.Gameanvil.Sample.Protocol.LoginRes.Parser.ParseFrom(loginInfo.Payload.GetPacket<Com.Nhn.Gameanvil.Sample.Protocol.LoginRes>().GetBytes());
                        Debug.Log("LoginRes " + loginRes);

                        // 서버에서 받은 게임 데이터 설정
                        UserInfo.Instance.Uuid = inputFieldUUID.text;
                        UserInfo.Instance.Nickname = loginRes.Userdata.Nickname;
                        UserInfo.Instance.Heart = loginRes.Userdata.Heart;
                        UserInfo.Instance.Coin = loginRes.Userdata.Coin;
                        UserInfo.Instance.Ruby = loginRes.Userdata.Ruby;
                        UserInfo.Instance.Level = loginRes.Userdata.Level;
                        UserInfo.Instance.Exp = loginRes.Userdata.Exp;
                        UserInfo.Instance.HighScore = loginRes.Userdata.HighScore;
                        UserInfo.Instance.CurrentDeck = loginRes.Userdata.CurrentDeck;

                        // 신전환시 버튼 리스너 모두 해재
                        buttonConnect.onClick.RemoveAllListeners();
                        buttonGenerateUUID.onClick.RemoveAllListeners();
                        buttonAuth.onClick.RemoveAllListeners();
                        buttonLogin.onClick.RemoveAllListeners();

                        // 기본으로 로비 설정
                        UserInfo.Instance.MoveScene(Constants.SCENE_LOBBY);

                        if (loginInfo.IsJoinedRoom)
                        {
                            if (loginInfo.RoomPayload.Contains<Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg>())
                            {
                                Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg roomInfoMsg = Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg.Parser.ParseFrom(loginInfo.RoomPayload.GetPacket<Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg>().GetBytes());
                                Debug.Log("RoomInfoMsg " + roomInfoMsg.RoomType.ToString());
                                if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomSingle)
                                {
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_SINGLE); // 싱글
                                }
                                else if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomSnake)
                                {
                                    UserInfo.Instance.gameState = UserInfo.GameState.Wait;
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_MULTI_SNAKE); // 유저매치 
                                }
                                else if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomTap)
                                {
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_MULTI_TAPBIRD);    // 룸 매치
                                }

                            }
                        }
                    }
                }
                else
                {
                    // 실패 처리
                }
                buttonLogin.interactable = true;
            }
       );
        // ===========================================================================================>>> GameAnvil
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

            // 런칭 응답 정보 파싱
            LaunchingInfo launchingInfo = JsonUtility.FromJson<LaunchingInfo>(www.downloadHandler.text);

            textIP.text = launchingInfo.serverUrl;
            textPort.text = launchingInfo.port.ToString();
            buttonConnect.gameObject.SetActive(true);

            inputFieldLaunching.enabled = false;
            buttonGetLaunching.gameObject.SetActive(false);
        }

        buttonGetLaunching.enabled = true;
        buttonGetLaunching.interactable = true;
    }
}

