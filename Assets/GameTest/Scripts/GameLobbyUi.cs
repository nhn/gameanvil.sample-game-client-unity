using System.Collections.Generic;
using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;
using UnityEngine.UI;

// 게임 로비신 UI 제어 스크립트
public class GameLobbyUi : MonoBehaviour
{
    // 화면 입력 필드
    public InputField inputFieldNickname;
    public Button buttonNicknameChange;
    public Button buttonTapBirdGame;
    public Button buttonMultiTapBirdGame;
    public Button buttonMultiSnakeGame;
    public Button buttonSingleRanking;
    public Text textRanking;
    public Button buttonUserInfo;
    // 유저 정보 패널
    public GameObject panelUserInfo;
    public Text textUUID;
    public Text textNickName;
    public Text textHeart;
    public Text textCoin;
    public Text textRuby;
    public Text textLevel;
    public Text textExp;
    public Text textHighScore;
    // 싱글 랭킹 패널
    public GameObject panelSingleRankingInfo;

    // 접속 유저 객체
    UserAgent gameUser;

    void Start()
    {
        inputFieldNickname.text = UserInfo.Instance.Nickname;

        // 버튼 리스너 연결
        buttonNicknameChange.onClick.AddListener(() => { OnClickNicknameChange(); });
        buttonTapBirdGame.onClick.AddListener(() => { OnClickTapBirdGame(); });
        buttonMultiTapBirdGame.onClick.AddListener(() => { OnClickMultiTapBirdGame(); });
        buttonMultiSnakeGame.onClick.AddListener(() => { OnClickMultiSnakeGame(); });
        buttonSingleRanking.onClick.AddListener(() => { OnClickSingleRanking(); });
        buttonUserInfo.onClick.AddListener(() => { OnClickUserInfo(); });

        // ===========================================================================================>>> GameAnvil
        // 커넥터로 부터 유저 객체 저장
        gameUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil

        // 유저 게임 정보 초기화
        UserInfo.Instance.IsMulti = false;
        UserInfo.Instance.gameState = UserInfo.GameState.None;

        // ===========================================================================================>>> GameAnvil
        // 타이밍 이슈상 리스너를 미리 등록, 유저 매치가 되었을때 불리는 리스너  : snake 게임 server to client
        gameUser.onMatchUserDoneListeners += (UserAgent userAgent, ResultCodeMatchUserDone result, bool created, int roomId, Payload payload) =>
        {
            Debug.Log("onMatchUserDoneListeners!!!!!! " + userAgent.GetUserId());
        };

        // 타이밍 이슈상 리스너 미리등록, 서버에서 게임룸에 두명이 모두 입장했을때 게임 설정데이터를 전송 : snake 게임 server to client
        gameUser.AddListener((UserAgent userAgent, Com.Nhn.Gameanvil.Sample.Protocol.SnakeGameInfoMsg msg) =>
        {
            if (msg != null)
            {
                Debug.Log("SnakeGameInfoMsg!!!!!! : " + msg);
                SnakeGameInfo.Instance.BoarderLeft = msg.BoarderLeft;
                SnakeGameInfo.Instance.BoarderRight = msg.BoarderRight;
                SnakeGameInfo.Instance.BoarderBottom = msg.BoarderBottom;
                SnakeGameInfo.Instance.BoarderTop = msg.BoarderTop;

                Dictionary<string, SnakeGameUser> userMap = new Dictionary<string, SnakeGameUser>();
                foreach (var user in msg.Users)
                {

                    var snakeGameUser = new SnakeGameUser
                    {
                        Id = user.BaseData.Id,
                        Nickname = user.BaseData.NickName,
                        Score = user.BaseData.Score
                    };

                    List<SnakePositionInfo> snakePositionData = new List<SnakePositionInfo>();
                    foreach (var userPosition in user.UserPositionListData)
                    {
                        var positionData = new SnakePositionInfo
                        {
                            Index = userPosition.Idx,
                            X = userPosition.X,
                            Y = userPosition.Y
                        };
                        snakePositionData.Add(positionData);
                    }
                    snakeGameUser.PositionList = snakePositionData;
                    userMap.Add(snakeGameUser.Id, snakeGameUser);
                }

                SnakeGameInfo.Instance.UserMapInfo = userMap;
                Debug.Log("UserMapInfo Count " + userMap.Count);
                UserInfo.Instance.gameState = UserInfo.GameState.Wait;
            }
        });
        // ===========================================================================================>>> GameAnvil
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        buttonNicknameChange.onClick.RemoveAllListeners();
        buttonMultiTapBirdGame.onClick.RemoveAllListeners();
        buttonMultiSnakeGame.onClick.RemoveAllListeners();
        buttonTapBirdGame.onClick.RemoveAllListeners();
        buttonSingleRanking.onClick.RemoveAllListeners();
        buttonUserInfo.onClick.RemoveAllListeners();
    }

    void OnClickNicknameChange()
    {
        buttonNicknameChange.interactable = false;

        // 닉네임변경 요청 프로토콜
        var changeNicknameReq = new Com.Nhn.Gameanvil.Sample.Protocol.ChangeNicknameReq
        {
            Nickname = inputFieldNickname.text
        };
        Debug.Log("changeNicknameReq " + changeNicknameReq);

        // ===========================================================================================>>> GameAnvil
        // 게임유저가 서버로 request로 response를 받아 처리 한다.
        gameUser.Request<Com.Nhn.Gameanvil.Sample.Protocol.ChangeNicknameRes>(changeNicknameReq, (userAgent, changeNicknameRes) =>
        {
            Debug.Log("changeNicknameRes" + changeNicknameRes);

            if (changeNicknameRes.ResultCode == Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode.None)
            {
                // 서버에 닉네임 변경 후 성공 하고 응답값 갱신
                UserInfo.Instance.Nickname = changeNicknameRes.UserData.Nickname;
                UserInfo.Instance.Heart = changeNicknameRes.UserData.Heart;
                UserInfo.Instance.Coin = changeNicknameRes.UserData.Coin;
                UserInfo.Instance.Ruby = changeNicknameRes.UserData.Ruby;
                UserInfo.Instance.Level = changeNicknameRes.UserData.Level;
                UserInfo.Instance.Exp = changeNicknameRes.UserData.Exp;
                UserInfo.Instance.HighScore = changeNicknameRes.UserData.HighScore;
                UserInfo.Instance.CurrentDeck = changeNicknameRes.UserData.CurrentDeck;
            }
            else
            {
                // 실패시 처리
            }
            buttonNicknameChange.interactable = true;
        });
        // ===========================================================================================>>> GameAnvil
    }

    void OnClickTapBirdGame()
    {
        // 다중클릭 막음
        buttonTapBirdGame.interactable = false;

        // ===========================================================================================>>> GameAnvil
        // 게임 시작 프로토콜 데이터 정의
        var startGameReq = new Com.Nhn.Gameanvil.Sample.Protocol.StartGameReq
        {
            Deck = UserInfo.Instance.CurrentDeck,
            Difficulty = Com.Nhn.Gameanvil.Sample.Protocol.DifficultyType.DifficultyNormal
        };
        Debug.Log("startGameReq" + startGameReq);
        gameUser.CreateRoom(Constants.SPACE_ROOM_TYPE_SINGLE, new Payload().add(new Packet(startGameReq)), (UserAgent userAgent, ResultCodeCreateRoom result, int roomId, string roomName, Payload payload) =>
        {
            Debug.Log("CreateRoom " + result);

            if (result == ResultCodeCreateRoom.CREATE_ROOM_SUCCESS)
            {
                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_TAPBIRD);
            }
            else
            {
                // 실패 처리
                MessageUi.Instance.SetTextMessage("onCreateRoom Fail..... " + result);
            }
        });
        // ===========================================================================================>>> GameAnvil

        buttonTapBirdGame.interactable = true;
    }

    void OnClickMultiTapBirdGame()
    {
        // 다중클릭 막음
        buttonMultiTapBirdGame.interactable = false;

        // ===========================================================================================>>> GameAnvil
        // 만들어 져있는 방에 들어가는 룸매치 요청 - 혼자서도 플레이가 가능하다. 최대 인원수 까지 모두 입장
        gameUser.MatchRoom(Constants.SPACE_ROOM_TYPE_MULTI_ROOM_MATCH, true, false, (UserAgent userAgent, ResultCodeMatchRoom result, int roomId, string roomName, bool created, Payload payload) =>
        {
            Debug.Log("MatchRoom " + result);
            if (result == ResultCodeMatchRoom.MATCH_ROOM_SUCCESS)
            {
                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();
                UserInfo.Instance.IsMulti = true;
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_TAPBIRD);
            }
            else
            {
                // 실패 처리
                MessageUi.Instance.SetTextMessage("onMatchRoom Fail..... " + result);
            }
        });
        // ===========================================================================================>>> GameAnvil

        buttonMultiTapBirdGame.interactable = true;
    }

    void OnClickMultiSnakeGame()
    {
        // 다중클릭 막음
        buttonMultiSnakeGame.interactable = false;

        // 유저 매치 두명이서 방을 만들어서 동시에 입장해서 게임진행
        gameUser.MatchUserStart(Constants.SPACE_ROOM_TYPE_MULTI_USER_MATCH, (UserAgent userAgent, ResultCodeMatchUserStart result, Payload payload) =>
        {
            Debug.Log("MatchUser " + result);
            if (result == ResultCodeMatchUserStart.MATCH_USER_START_SUCCESS)
            {
                SnakeGameInfo.Instance.FoodList.Clear();
                SnakeGameInfo.Instance.UserMapInfo.Clear();

                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_SNAKE);
            }
            else
            {
                // 실패 처리
                MessageUi.Instance.SetTextMessage("onMatchUserStart Fail..... " + result);
            }
        });
        // ===========================================================================================>>> GameAnvil

        buttonMultiSnakeGame.interactable = true;
    }

    void OnClickSingleRanking()
    {
        // 다중 클릭 막음
        buttonSingleRanking.interactable = false;

        var singleRankingReq = new Com.Nhn.Gameanvil.Sample.Protocol.ScoreRankingReq
        {
            Start = 1,
            End = 100
        };
        Debug.Log("singleRankingReq " + singleRankingReq);

        // ===========================================================================================>>> GameAnvil
        // 게임에서 등록한 랭킹 리스트 요청
        gameUser.Request<Com.Nhn.Gameanvil.Sample.Protocol.ScoreRankingRes>(singleRankingReq, (userAgent, singleRankingRes) =>
        {
            Debug.Log("singleRankingRes" + singleRankingRes);

            if (singleRankingRes.ResultCode == Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode.None)
            {
                var rankings = singleRankingRes.Rankings;

                textRanking.text = "RankingList";
                int rankingNumber = 0;
                foreach (var ranking in rankings)
                {
                    textRanking.text += "\n" + rankingNumber++ + ":" + ":" + ranking.Nickname + ":" + ranking.Score;
                }

                panelSingleRankingInfo.SetActive(true);
            }
            else
            {
                // 실패시 처리
                MessageUi.Instance.SetTextMessage("singleRankingReq  Fail..... " + singleRankingRes);
            }
            buttonSingleRanking.interactable = true;
        });
        // ===========================================================================================>>> GameAnvil

    }

    void OnClickUserInfo()
    {
        // 다중 클릭 막음
        buttonUserInfo.interactable = false;

        textUUID.text = UserInfo.Instance.Uuid;
        textNickName.text = UserInfo.Instance.Nickname;
        textHeart.text = UserInfo.Instance.Heart.ToString();
        textCoin.text = UserInfo.Instance.Coin.ToString();
        textRuby.text = UserInfo.Instance.Ruby.ToString();
        textLevel.text = UserInfo.Instance.Level.ToString();
        textExp.text = UserInfo.Instance.Exp.ToString();
        textHighScore.text = UserInfo.Instance.HighScore.ToString();

        panelUserInfo.SetActive(true);
        buttonUserInfo.interactable = true;
    }
}
