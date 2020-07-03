using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;

// 4일까지 플레이할수 있는 멀티게임
public class MultiTapBirdGameUi : MonoBehaviour
{
    // 화면 필드
    public Button buttonBack;
    public Button buttonTap;
    public Button buttonGameEnd;
    public Text textMyTapInfo;
    [SerializeField]
    public Text textUserTapInfo;

    private int tapCount = 0;

    UserAgent tapBirdUser;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 동작 리스너 연결
        buttonBack.onClick.AddListener(() => { OnClickBack(); });
        buttonTap.onClick.AddListener(() => { OnClickTap(); });
        buttonGameEnd.onClick.AddListener(() => { OnClickGameEnd(); });

        // ===========================================================================================>>> GameAnvil
        // 게임 유저 얻기
        tapBirdUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // 유저 점수 동기 처리
        tapBirdUser.AddListener((UserAgent userAgent, Com.Nhn.Gameanvil.Sample.Protocol.BroadcastTapBirdMsg msg) =>
            {
                if (msg != null)
                {
                    Debug.Log("BroadcastTapBirdMsg!!!!!! : " + msg);

                    string roomData = null;
                    foreach (var tapBirData in msg.TapBirdData)
                    {
                        var nickname = tapBirData.UserData.NickName;
                        var score = tapBirData.UserData.Score;

                        roomData += nickname + " :: " + score + "\n";
                    }

                    textUserTapInfo.text = roomData;
                }
            }
        );
        // ===========================================================================================>>> GameAnvil

        UpdateMyInfo(tapCount);
    }

    void UpdateMyInfo(int count)
    {
        textMyTapInfo.text = "My Tap Count : " + count;
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        buttonBack.onClick.RemoveAllListeners();
        buttonTap.onClick.RemoveAllListeners();
        buttonGameEnd.onClick.RemoveAllListeners();

        tapBirdUser.RemoveAllListeners();
    }

    void OnClickBack()
    {
        // 게임 중간 종료
        Debug.Log("OnClickBack!!!!!! : " + tapCount);

        // 게임 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndGiveUp);
    }

    void OnClickTap()
    {
        // 탭할때마다 카운트
        tapCount++;
        UpdateMyInfo(tapCount);
        Debug.Log("Tap!!!!!! : " + tapCount);

        // 전송할 패킷
        var scoreUp = new Com.Nhn.Gameanvil.Sample.Protocol.ScoreUpMsg
        {
            Score = tapCount
        };

        // ===========================================================================================>>> GameAnvil
        // 응답없이 서버로 데이터 성으로 전달하는 패킷
        tapBirdUser.Send(new Packet(scoreUp));
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
        // ===========================================================================================>>> GameAnvil
        // 게임룸 나가는 요청
        tapBirdUser.LeaveRoom((UserAgent userAgent, ResultCodeLeaveRoom result, bool force, int roomId, Payload payload) =>
        {
            Debug.Log("LeaveRoom " + result);

            if (result == ResultCodeLeaveRoom.LEAVE_ROOM_SUCCESS)
            {
                if (payload.contains<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>())
                {
                    Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes endGameRes = Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes.Parser.ParseFrom(payload.getPacket<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>().GetBytes());

                    UserInfo.Instance.Heart = endGameRes.UserData.Heart;
                    UserInfo.Instance.TotalScore = endGameRes.TotalScore;

                    Debug.Log("GameResult " + UserInfo.Instance.TotalScore);
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
}
