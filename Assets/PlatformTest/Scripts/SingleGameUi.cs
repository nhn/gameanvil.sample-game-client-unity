using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;

// 싱글 게임 
public class SingleGameUi : MonoBehaviour
{
    // 화면 필드
    public Button buttonBack;
    public Text textTime;
    public Text textTapCount;
    public Button buttonTap;
    public Button buttonGameEnd;

    private int tapCount = 0;
    private float elapsedDelta;

    UserAgent tapBirdUser;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 동작 리스너 연결
        buttonBack.onClick.AddListener(() => { OnClickBack(); });
        buttonTap.onClick.AddListener(() => { OnClickTap(); });
        buttonGameEnd.onClick.AddListener(() => { OnClickGameEnd(); });

        // 게임 데이터 초기화
        tapCount = 0;
        elapsedDelta = Constants.GAME_PLAY_TIME;

        // 종료버튼 숨김
        buttonGameEnd.gameObject.SetActive(false);

        // ===========================================================================================>>> GameAnvil
        // 게임 유저 얻기
        tapBirdUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil
    }

    // Update is called once per frame
    void Update()
    {
        // 게임신에서 시간 확인
        if (elapsedDelta > 0)
        {
            elapsedDelta -= Time.deltaTime;

            if (int.Parse(textTime.text) != (int)elapsedDelta)
            {
                textTime.text = ((int)elapsedDelta).ToString();
            }
        }
        else
        {
            // 게임 종료
            buttonGameEnd.gameObject.SetActive(true);
            buttonTap.gameObject.SetActive(false);
        }
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        buttonBack.onClick.RemoveAllListeners();
        buttonTap.onClick.RemoveAllListeners();
        buttonGameEnd.onClick.RemoveAllListeners();
    }

    void OnClickBack()
    {
        // 게임 중간 종료
        Debug.Log("OnClickBack!!!!!! : " + tapCount);

        // 게임 룸에서 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndGiveUp);
    }
    void OnClickTap()
    {
        // 탭할때마다 카운트
        tapCount++;
        Debug.Log("Tap!!!!!! : " + tapCount);

        // 전송할 패킷
        var tapMsg = new Com.Nhn.Gameanvil.Sample.Protocol.TapMsg
        {
            Combo = tapCount,
            SelectCardName = UserInfo.Instance.CurrentDeck + "_0" + 1,
            TapScore = 100
        };

        // ===========================================================================================>>> GameAnvil
        // 응답없이 서버로 데이터 성으로 전달하는 패킷
        tapBirdUser.Send(new Packet(tapMsg));
        // ===========================================================================================>>> GameAnvil
    }

    void OnClickGameEnd()
    {
        // 정상 게임 종료
        Debug.Log("OnClickGameEnd!!!!!! : " + tapCount);

        // 게임 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndTimeUp);

    }

    void OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType gameEndType)
    {
        // 게임종료 프로토콜 정의
        var endGameReq = new Com.Nhn.Gameanvil.Sample.Protocol.EndGameReq
        {
            EndType = gameEndType
        };

        // ===========================================================================================>>> GameAnvil
        // 게임룸 나가는 요청
        tapBirdUser.LeaveRoom(new Payload().add(new Packet(endGameReq)), (UserAgent userAgent, ResultCodeLeaveRoom result, bool force, int roomId, Payload payload) =>
        {
            Debug.Log("LeaveRoom " + result);

            if (result == ResultCodeLeaveRoom.LEAVE_ROOM_SUCCESS)
            {
                if (payload.contains<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>())
                {
                    Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes endGameRes = Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes.Parser.ParseFrom(payload.getPacket<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>().GetBytes());

                    UserInfo.Instance.Heart = endGameRes.UserData.Heart;
                    UserInfo.Instance.TotalScore = endGameRes.TotalScore;

                    UserInfo.Instance.Coin = endGameRes.UserData.Coin;
                    UserInfo.Instance.CurrentDeck = endGameRes.UserData.CurrentDeck;
                    UserInfo.Instance.Exp = endGameRes.UserData.Exp;
                    UserInfo.Instance.HighScore = endGameRes.UserData.HighScore;
                    UserInfo.Instance.Level = endGameRes.UserData.Level;
                    UserInfo.Instance.Nickname = endGameRes.UserData.Nickname;
                    UserInfo.Instance.Ruby = endGameRes.UserData.Ruby;
   
                    Debug.Log("GameResult " + endGameRes);
                }

                // 로비신으로 이동
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_LOBBY);
            }
            else
            {
                // 실패시 처리
            }
        });
        // ===========================================================================================>>> GameAnvil
    }

    void OnGUI()
    {
        // 화면에 탭 카운트 표시
        textTapCount.text = tapCount.ToString();
    }
}
