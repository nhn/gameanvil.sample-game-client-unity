using System.Collections.Generic;
using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.Connection;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;
using Toast.Gamebase;

public class GameLoginUi : MonoBehaviour
{
    // 화면과 연결되는 필드들
    public Text textUUID;
    public Button buttonGenerateUUID;
    public Text textGamebaseUserId;
    public Text textGameServerIp;
    public Text textGameServerPort;

    public Button buttonLogin;

    // Start is called before the first frame update
    void Start()
    {
        // 기본값 지정
//        textUUID.text = PlayerPrefs.GetString(Constants.KEY_UUID);
        textUUID.text = "123456789999";
        if (string.IsNullOrWhiteSpace(textUUID.text))
        {
            OnClickGenerateUUID();
        }
        textGamebaseUserId.text = GamebaseInfo.Instance.GamebaseUserId;
        textGameServerIp.text = GamebaseInfo.Instance.GameServerIp;
        textGameServerPort.text = GamebaseInfo.Instance.GameServerPort;

        // 버튼 클릭에 대한 리스너등록
        buttonLogin.onClick.AddListener(() => { OnClickLogin(); });
        buttonGenerateUUID.onClick.AddListener(() => { OnClickGenerateUUID(); });

        // ===========================================================================================>>> GameAnvil
        // 연결 끊기는 부분 처리 리스너 등록
        ConnectHandler.Instance.GetConnectionAgent().onDisconnectListeners += (ConnectionAgent connectionAgent, ResultCodeDisconnect result, bool force, Payload payload) =>
        {
            Debug.LogFormat("onDisconnect - {0}", result);
            MessageUi.Instance.SetTextMessage("onDisconnect..... " + result);
            UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOGIN);
        };

        // 세션 연결 리스너
        ConnectHandler.Instance.GetConnectionAgent().onConnectListeners += (ConnectionAgent connectionAgent, ResultCodeConnect result) =>
        {
            if (result == ResultCodeConnect.CONNECT_SUCCESS)
            {
                // 인증에 필요한 프로토콜 데이터 설정
                var authenticationReq = new Com.Nhn.Gameanvil.Sample.Protocol.AuthenticationReq
                {
                    AccessToken = GamebaseInfo.Instance.GamebaseToken
                };
                Debug.Log("authenticationReq " + authenticationReq);

                // 서버에 인증 시도. 현재는 deviceid uuid, id, pw Gamebase userId 값으로 전달
                ConnectHandler.Instance.GetConnectionAgent().Authenticate(textUUID.text, textGamebaseUserId.text, textGamebaseUserId.text, new Payload().Add(new Packet(authenticationReq)));
            }
            else
            {
                // 서버 접속 실패 처리
                buttonLogin.interactable = true;
                MessageUi.Instance.SetTextMessage("onConnect Fail..... " + result);
            }
        };

        // 인증 리스너
        ConnectHandler.Instance.GetConnectionAgent().onAuthenticationListeners +=
            (ConnectionAgent connectionAgent, ResultCodeAuth result, List<ConnectionAgent.LoginedUserInfo> loginedUserInfoList, string message, Payload payload) =>
            {
                Debug.Log("Auth " + result);

                // 성공인 경우 다음 단계로 진행.
                if (result == ResultCodeAuth.AUTH_SUCCESS)
                {
                    // 성공시 로그인 요청
                    GameAnvilLogin();
                }
                else
                {
                    // 실패시 처리
                    buttonLogin.interactable = true;
                    MessageUi.Instance.SetTextMessage("onAutentication Fail..... " + result);
                }

            };
        // ===========================================================================================>>> GameAnvil
    }

    void OnClickGenerateUUID()
    {
        // uuid 갱신 처리
        textUUID.text = TestHelper.GenerateUUID();
    }

    void OnClickLogin()
    {
        // ===========================================================================================>>> GameAnvil
        // 런칭 정보로 tcp connect요청
        ConnectHandler.Instance.GetConnectionAgent().Connect(GamebaseInfo.Instance.GameServerIp, int.Parse(GamebaseInfo.Instance.GameServerPort));
        // ===========================================================================================>>> GameAnvil
    }

    void GameAnvilLogin()
    {
        // 로그인에 필요한 프로토콜 데이터 설정
        var loginReq = new Com.Nhn.Gameanvil.Sample.Protocol.LoginReq
        {
            Uuid = textGamebaseUserId.text,
            LoginType = Com.Nhn.Gameanvil.Sample.Protocol.LoginType.LoginGuest,
            AppVersion = Application.version,
            AppStore = "None",
            DeviceModel = SystemInfo.deviceModel,
            DeviceCountry = Gamebase.GetCountryCodeOfDevice(),
            DeviceLanguage = Gamebase.GetDeviceLanguageCode()

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
                        UserInfo.Instance.Uuid = textGamebaseUserId.text;
                        UserInfo.Instance.Nickname = loginRes.Userdata.Nickname;
                        UserInfo.Instance.Heart = loginRes.Userdata.Heart;
                        UserInfo.Instance.Coin = loginRes.Userdata.Coin;
                        UserInfo.Instance.Ruby = loginRes.Userdata.Ruby;
                        UserInfo.Instance.Level = loginRes.Userdata.Level;
                        UserInfo.Instance.Exp = loginRes.Userdata.Exp;
                        UserInfo.Instance.HighScore = loginRes.Userdata.HighScore;
                        UserInfo.Instance.CurrentDeck = loginRes.Userdata.CurrentDeck;

                        // 신전환신 버튼 리스너 모두 해재
                        buttonLogin.onClick.RemoveAllListeners();

                        UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOBBY);

                        if (loginInfo.IsJoinedRoom)
                        {
                            if (loginInfo.RoomPayload.Contains<Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg>())
                            {
                                Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg roomInfoMsg = Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg.Parser.ParseFrom(loginInfo.RoomPayload.GetPacket<Com.Nhn.Gameanvil.Sample.Protocol.RoomInfoMsg>().GetBytes());
                                Debug.Log("RoomInfoMsg " + roomInfoMsg.RoomType.ToString());
                                if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomSingle)
                                {
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_TAPBIRD); // 싱글
                                }
                                else if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomSnake)
                                {
                                    UserInfo.Instance.gameState = UserInfo.GameState.Wait;
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_SNAKE); // 유저매치 
                                }
                                else if (roomInfoMsg.RoomType == Com.Nhn.Gameanvil.Sample.Protocol.RoomType.RoomTap)
                                {
                                    UserInfo.Instance.IsMulti = true;
                                    UserInfo.Instance.MoveScene(Constants.SCENE_GAME_TAPBIRD);    // 룸 매치
                                }

                            }
                        }
                    }
                }
                else
                {
                    // 실패 처리
                    MessageUi.Instance.SetTextMessage("onLogin Fail..... " + result);
                }
                buttonLogin.interactable = true;
            }
       );
        // ===========================================================================================>>> GameAnvil
    }
}
