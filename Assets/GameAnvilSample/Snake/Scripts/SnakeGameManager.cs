using System.Collections.Generic;
using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;

// snake 게임 관리
public class SnakeGameManager : MonoBehaviour
{
    // 화면과 연결
    public Button backButton;
    public GameObject foodPrefab;
    public GameObject userPrefab;
    public GameObject enemyPrefab;
    public Transform borderTop;
    public Transform borderBottom;
    public Transform borderLeft;
    public Transform borderRight;
    public Text playerScoreText;
    public Text enemyScoreText;
    public Text foodText;

    List<GameObject> enemySnake;

    UserAgent snakeGameUser;

    SnakeGameUser playerInfo;

    void Awake()
    {
        // ===========================================================================================>>> GameAnvil
        // 게임 유저 얻기
        snakeGameUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil
    }

    void Start()
    {
        // 버튼 동작 리스너 연결
        backButton.onClick.AddListener(() => { OnClickGameEnd(); });

        SetBoarderActive(false);

        enemySnake = new List<GameObject>();

        playerScoreText.text = UserInfo.Instance.Nickname + " : 0";

        // ===========================================================================================>>> GameAnvil
        // 유저 매치 요청 타임 아웃 리스너
        snakeGameUser.onMatchUserTimeoutListeners += (UserAgent userAgent) =>
        {
            Debug.Log("onMatchUserTimeoutListeners!!!!!! " + userAgent.GetUserId());

            // 로비신으로 이동
            RemoveAllListeners();
            UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOBBY);
        };

        // 사용자가 방을 나갈때 처리 되는 리스너 - 서버에서 강제로 내보내는 처리
        snakeGameUser.onLeaveRoomListeners += (UserAgent userAgent, ResultCodeLeaveRoom result, bool force, int roomId, Payload payload) =>
        {
            Debug.Log("onLeaveRoomListeners!!!!!! " + userAgent.GetUserId() + "isForce : " + force);
            UserInfo.Instance.gameState = UserInfo.GameState.GameOver;
        };

        // 상대편 유저 데이터 처리
        snakeGameUser.AddListener((UserAgent userAgent, ResultCode resultCode, Com.Nhn.Gameanvil.Sample.Protocol.SnakeUserMsg msg) =>
        {
            Debug.Log("<<<< SnakeUserMsg!!!!!! : " + msg);
            if (msg != null)
            {
                enemyScoreText.text = msg.UserData.BaseData.NickName + " : " + msg.UserData.BaseData.Score;
                if (msg.UserData.UserPositionListData != null)
                {
                    for (int i = 0; i < msg.UserData.UserPositionListData.Count; i++)
                    {
                        if (i < enemySnake.Count)
                        {
                            enemySnake[i].transform.position = new Vector3(msg.UserData.UserPositionListData[i].X, msg.UserData.UserPositionListData[i].Y, msg.UserData.UserPositionListData[i].Idx);
                        }
                        else
                        {
                            enemySnake.Add(Instantiate(enemyPrefab, new Vector3(msg.UserData.UserPositionListData[i].X, msg.UserData.UserPositionListData[i].Y, msg.UserData.UserPositionListData[i].Idx), Quaternion.identity));
                        }

                    }
                }
            }
        });

        // 서버에서 전달 받은 food 상태 처리
        snakeGameUser.AddListener((UserAgent userAgent, ResultCode resultCode, Com.Nhn.Gameanvil.Sample.Protocol.SnakeFoodMsg msg) =>
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
                    SnakeGameInfo.Instance.FoodList.Add(Instantiate(foodPrefab, new Vector3(msg.FoodData.X, msg.FoodData.Y, msg.FoodData.Idx), Quaternion.identity));

                }
            }
        });
        // ===========================================================================================>>> GameAnvil
    }

    void Update()
    {
        // 유저 매치가 되면 게임 패널 On
        if (transform.Find("PanelWait").gameObject.activeSelf && UserInfo.Instance.gameState == UserInfo.GameState.Wait)
        {
            transform.Find("PanelWait").gameObject.SetActive(false);

            borderLeft.position = new Vector3(SnakeGameInfo.Instance.BoarderLeft, 0, 0);
            borderRight.position = new Vector3(SnakeGameInfo.Instance.BoarderRight, 0, 0);
            borderBottom.position = new Vector3(0, SnakeGameInfo.Instance.BoarderBottom, 0);
            borderTop.position = new Vector3(0, SnakeGameInfo.Instance.BoarderTop, 0);

            SetBoarderActive(true);

            // 유저 위치들 설정
            foreach (var snakeGameUser in SnakeGameInfo.Instance.UserMapInfo.Values)
            {
                if (UserInfo.Instance.Uuid.Equals(snakeGameUser.Id))
                {
                    Instantiate(userPrefab, new Vector2(snakeGameUser.PositionList[0].X, snakeGameUser.PositionList[0].Y), Quaternion.identity);
                }
                else
                {
                    if (enemySnake.Count == 0)
                    {
                        enemySnake.Add(Instantiate(enemyPrefab, new Vector2(snakeGameUser.PositionList[0].X, snakeGameUser.PositionList[0].Y), Quaternion.identity));
                    }
                }
            }

            UserInfo.Instance.gameState = UserInfo.GameState.Start;
        }

        if (playerInfo == null)
        {
            playerInfo = SnakeGameInfo.Instance.GetSnakeGameUser(UserInfo.Instance.Uuid);
        }
        else
        {
            playerScoreText.text = playerInfo.Nickname + " : " + playerInfo.Score;
        }

        if (SnakeGameInfo.Instance.FoodList != null)
        {
            foodText.text = "Food Count : " + SnakeGameInfo.Instance.FoodList.Count;
        }

        if (UserInfo.Instance.gameState == UserInfo.GameState.GameOver)
        {
            transform.Find("PanelGameEnd").gameObject.SetActive(true);
        }
    }

    // 화면 설정
    void SetBoarderActive(bool isActive)
    {
        borderTop.gameObject.SetActive(isActive);
        borderBottom.gameObject.SetActive(isActive);
        borderLeft.gameObject.SetActive(isActive);
        borderRight.gameObject.SetActive(isActive);
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        backButton.onClick.RemoveAllListeners();

        snakeGameUser.RemoveAllListeners();
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

                    Debug.Log("GameResult " + UserInfo.Instance.TotalScore);
                }

                // 로비신으로 이동
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOBBY);

            }
            else
            {
                // 실패시 처리
            }
        });
        // ===========================================================================================>>> GameAnvil
    }

    public void OnClickGameEnd()
    {
        // 게임 중간 종료
        Debug.Log("OnClickBack!!!!!! ");

        // 게임 나가기
        OnLeaveRoom(Com.Nhn.Gameanvil.Sample.Protocol.EndType.GameEndGiveUp);
    }
}
