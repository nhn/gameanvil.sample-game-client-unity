using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;

// 유저 매치 두명이서 게임데이터 동기화 하면서 게임 처리
public class MultiSnakeGameUi : MonoBehaviour
{
    // 화면 필드
    public Button buttonBack;
    public Button buttonGameEnd;
    public Text textFoodList;
    public Text textGameInfoList;
    public Text textUserInfoList;

    UserAgent snakeGameUser;

    string myName;
    string peerName;

    void Start()
    {
        // ===========================================================================================>>> GameAnvil
        // 게임 유저 얻기
        snakeGameUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil

        // 버튼 동작 리스너 연결
        buttonBack.onClick.AddListener(() => { OnClickBack(); });
        buttonGameEnd.onClick.AddListener(() => { OnClickGameEnd(); });

        // ===========================================================================================>>> GameAnvil
        // 유저 매치 요청 타임 아웃 리스너
        snakeGameUser.onMatchUserTimeoutListeners += (UserAgent userAgent) =>
        {
            Debug.Log("onMatchUserTimeoutListeners!!!!!! " + userAgent.GetUserId());

            // 로비신으로 이동
            RemoveAllListeners();
            UserInfo.Instance.MoveScene(Constants.SCENE_LOBBY);

        };

        // 사용자가 방을 나갈때 처리 되는 리스너 - 서버에서 강제로 내보내는 처리
        snakeGameUser.onLeaveRoomListeners += (UserAgent userAgent, ResultCodeLeaveRoom result, bool force, int roomId, Payload payload) =>
        {
            Debug.Log("onLeaveRoomListeners!!!!!! " + userAgent.GetUserId() + "isForce : " + force);
            // 게임 나가기
            OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndTimeUp);
        };

        snakeGameUser.AddListener((UserAgent userAgent, Com.Nhn.Gameanvil.Sample.Protocol.SnakeFoodMsg msg) =>
        {
            if (msg != null)
            {
                Debug.Log("<<<< SnakeFoodMsg!!!!!! : " + msg);


                if (msg.IsDelete)
                {
                    foreach (var currentFood in SnakeGameInfo.Instance.FoodList)
                    {
                        if (msg.FoodData.Idx == currentFood.transform.position.z)
                        {
                            Debug.Log("Delete Food !!!!! : " + currentFood.transform.position.z + ":" + currentFood.transform.position.x + ", " + currentFood.transform.position.y);
                            Destroy(currentFood);
                            SnakeGameInfo.Instance.FoodList.Remove(currentFood);
                            break;
                        }
                    }
                }
                else
                {
                    textFoodList.text = msg.FoodData.ToString();
                }
            }
        });
        // ===========================================================================================>>> GameAnvil
    }

    // Update is called once per frame
    void Update()
    {
        // 유저 매치가 되면 게임 패널 On
        if (transform.Find("PanelWait").gameObject.activeSelf && UserInfo.Instance.gameState == UserInfo.GameState.Wait)
        {
            transform.Find("PanelWait").gameObject.SetActive(false);
            transform.Find("PanelMultiSnakeGame").gameObject.SetActive(true);
            UserInfo.Instance.gameState = UserInfo.GameState.Start;
        }

        // 사용자 게임 데이터 설정
        textGameInfoList.text = "X:" + SnakeGameInfo.Instance.BoarderLeft + " to " + SnakeGameInfo.Instance.BoarderRight + ", Y:" + SnakeGameInfo.Instance.BoarderBottom + " to " + SnakeGameInfo.Instance.BoarderTop;


        // 유저 동기 정보 처리
        if (SnakeGameInfo.Instance.UserMapInfo != null)
        {
            foreach (var userData in SnakeGameInfo.Instance.UserMapInfo)
            {
                if (userData.Value.Nickname.Equals(UserInfo.Instance.Nickname))
                {
                    myName = userData.Value.Nickname;
                }
                else
                {
                    peerName = userData.Value.Nickname;
                }
            }
            textUserInfoList.text = myName + ", " + peerName;
        }
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        buttonBack.onClick.RemoveAllListeners();
        buttonGameEnd.onClick.RemoveAllListeners();

        snakeGameUser.RemoveAllListeners();
    }

    void OnClickBack()
    {
        // 게임 중간 종료
        Debug.Log("OnClickBack!!!!!! ");

        // 게임 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndGiveUp);
    }

    void OnClickGameEnd()
    {
        // 정상 게임 종료
        Debug.Log("OnClickGameEnd!!!!!!");

        // 게임 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndTimeUp);

    }

    void OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType gameEndType)
    {
        // ===========================================================================================>>> GameAnvil
        // 게임룸 나가는 요청
        snakeGameUser.LeaveRoom((UserAgent userAgent, ResultCodeLeaveRoom result, bool force, int roomId, Payload payload) =>
        {
            Debug.Log("LeaveRoom " + result);

            if (result == ResultCodeLeaveRoom.LEAVE_ROOM_SUCCESS)
            {
                if (payload.Contains<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>())
                {
                    Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes endGameRes = Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes.Parser.ParseFrom(payload.GetPacket<Com.Nhn.Gameanvil.Sample.Protocol.EndGameRes>().GetBytes());

                    UserInfo.Instance.Heart = endGameRes.UserData.Heart;
                    UserInfo.Instance.TotalScore = endGameRes.TotalScore;

                    UserInfo.Instance.gameState = UserInfo.GameState.GameOver;

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
