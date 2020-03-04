using System.Collections.Generic;
using System.Linq;
using Tardis;
using Tardis.Defines;
using Tardis.User;
using TardisConnector;
using UnityEngine;
using UnityEngine.UI;

// 탬버드 게임 스크립트
public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;
    public Text userInfoText;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver
    }

    int score = 0;
    bool gameOver = false;

    UserAgent tapBirdGameUser;

    public bool GameOver { get { return gameOver; } }

    void Awake()
    {
        Instance = this;

        // ===========================================================================================>>> Tardis
        // 게임 유저 얻기
        tapBirdGameUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, string.Empty);
        // ===========================================================================================>>> Tardis

        SendScore();

    }

    void OnEnable()
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;

        if (UserInfo.Instance.IsMulti)
        {
            // ===========================================================================================>>> Tardis
            tapBirdGameUser.AddListener((UserAgent userAgent, Com.Nhn.Tardis.Sample.Protocol.BroadcastTapBirdMsg msg) =>
            {
                if (msg != null)
                {

                    string roomData = "";
                    Dictionary<string, long> userDic = new Dictionary<string, long>();
                    foreach (var tapBirData in msg.TapBirdData)
                    {
                        userDic.Add(tapBirData.UserData.NickName, tapBirData.UserData.Score);
                    }
                    var sortDic = userDic.OrderByDescending(dic => dic.Value);

                    foreach (var data in sortDic)
                    {
                        roomData += data.Key + " :: " + data.Value + "\n";
                    }

                    Debug.Log("BroadcastTapBirdMsg!!!!!! : " + roomData);
                    userInfoText.text = roomData;
                }
            });
            // ===========================================================================================>>> Tardis
        }
    }

    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;

        tapBirdGameUser.RemoveAllListeners();
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;

        SendScore();
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();


        if (UserInfo.Instance.IsMulti)
        {
            SendScore();
        }
        else
        {
            // 전송할 패킷
            var tapMsg = new Com.Nhn.Tardis.Sample.Protocol.TapMsg
            {
                Combo = score,
                SelectCardName = UserInfo.Instance.CurrentDeck + "_0" + 1,
                TapScore = 100
            };
            // ===========================================================================================>>> Tardis
            // 응답없이 서버로 데이터 성으로 전달하는 패킷
            tapBirdGameUser.Send(new Packet(tapMsg));
            // ===========================================================================================>>> Tardis
        }
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
    }

    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }
    public void OnClickGameEnd()
    {
        // 게임종료 프로토콜 정의
        var endGameReq = new Com.Nhn.Tardis.Sample.Protocol.EndGameReq
        {
            EndType = Com.Nhn.Tardis.Sample.Protocol.EndType.GameEndGiveUp
        };
        // ===========================================================================================>>> Tardis
        // 게임룸 나가는 요청
        tapBirdGameUser.LeaveRoom(new Payload().add(new Packet(endGameReq)), (UserAgent userAgent, ResultCodeLeaveRoom result, bool force, string roomId, Payload payload) =>
        {
            Debug.Log("LeaveRoom " + result);

            if (result == ResultCodeLeaveRoom.LEAVE_ROOM_SUCCESS)
            {
                if (payload.contains<Com.Nhn.Tardis.Sample.Protocol.EndGameRes>())
                {
                    Com.Nhn.Tardis.Sample.Protocol.EndGameRes endGameRes = Com.Nhn.Tardis.Sample.Protocol.EndGameRes.Parser.ParseFrom(payload.getPacket<Com.Nhn.Tardis.Sample.Protocol.EndGameRes>().GetBytes());

                    UserInfo.Instance.Heart = endGameRes.UserData.Heart;
                    UserInfo.Instance.TotalScore = endGameRes.TotalScore;

                    UserInfo.Instance.Coin = endGameRes.UserData.Coin;
                    UserInfo.Instance.CurrentDeck = endGameRes.UserData.CurrentDeck;
                    UserInfo.Instance.Exp = endGameRes.UserData.Exp;
                    UserInfo.Instance.HighScore = endGameRes.UserData.HighScore;
                    UserInfo.Instance.Level = endGameRes.UserData.Level;
                    UserInfo.Instance.Nickname = endGameRes.UserData.Nickname;
                    UserInfo.Instance.Ruby = endGameRes.UserData.Ruby;

                    Debug.Log("GameResult " + UserInfo.Instance.TotalScore);
                }

                // 로비신으로 이동
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_LOBBY);

            }
            else
            {
                // 실패시 처리
            }
        });
        // ===========================================================================================>>> Tardis
    }

    void SendScore()
    {
        // 전송할 패킷
        var scoreUp = new Com.Nhn.Tardis.Sample.Protocol.ScoreUpMsg
        {
            Score = score
        };

        // ===========================================================================================>>> Tardis
        // 응답없이 서버로 데이터 성으로 전달하는 패킷
        tapBirdGameUser.Send(new Packet(scoreUp));
        // ===========================================================================================>>> Tardis
    }
}
