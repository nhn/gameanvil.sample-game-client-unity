using System.Collections.Generic;
using GameAnvil;
using GameAnvil.Defines;
using GameAnvil.User;
using GameAnvilConnector;
using GameAnvil.User.Defines;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUi : MonoBehaviour
{
    // 화면 입력 필드
    public Button buttonDeckShuffle;
    public Text textDeckName;
    public Text textUUID;
    public Text textNickName;
    public Text textHeart;
    public Text textCoin;
    public Text textRuby;
    public Text textLevel;
    public Text textExp;
    public Text textHighScore;
    public Button buttonSingleStartGame;
    public Button buttonMultiTapBirdStartGame;
    public Button buttonMultiSnakeStartGame;
    public Button buttonSingleRanking;

    UserAgent gameUser;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 리스너 연결
        buttonDeckShuffle.onClick.AddListener(() => { OnClickDeckShuffle(); });
        buttonSingleStartGame.onClick.AddListener(() => { OnClickSingleStartGame(); });
        buttonMultiTapBirdStartGame.onClick.AddListener(() => { OnClickMultiTapBirdStartGame(); });
        buttonMultiSnakeStartGame.onClick.AddListener(() => { OnClickMultiSnakeStartGame(); });

        buttonSingleRanking.onClick.AddListener(() => { OnClickSingleRanking(); });

        // ===========================================================================================>>> GameAnvil
        // 커넥터로 부터 유저 객체 저장
        gameUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil

        gameUser.onErrorCommandListeners += (UserAgent userAgent, ErrorCode errorCode, Commands command) =>
        {
            Debug.Log("!!!!!!!!!!!!!!errorCode" + errorCode);
        };


        UserDataUiUpdate();
        UserInfo.Instance.IsMulti = false;
        UserInfo.Instance.gameState = UserInfo.GameState.None;
    }

    void RemoveAllListeners()
    {
        // 리스너 제거
        buttonDeckShuffle.onClick.RemoveAllListeners();
        buttonSingleStartGame.onClick.RemoveAllListeners();
        buttonSingleRanking.onClick.RemoveAllListeners();
    }

    void UserDataUiUpdate()
    {
        // 유저 데이터 갱신
        textDeckName.text = UserInfo.Instance.CurrentDeck;
        textUUID.text = UserInfo.Instance.Uuid;
        textNickName.text = UserInfo.Instance.Nickname;
        textHeart.text = UserInfo.Instance.Heart.ToString();
        textCoin.text = UserInfo.Instance.Coin.ToString();
        textRuby.text = UserInfo.Instance.Ruby.ToString();
        textLevel.text = UserInfo.Instance.Level.ToString();
        textExp.text = UserInfo.Instance.Exp.ToString();
        textHighScore.text = UserInfo.Instance.HighScore.ToString();
    }

    void OnClickDeckShuffle()
    {
        // 다중 클릭 막음
        buttonDeckShuffle.interactable = false;

        // 덱셔플 요청 프로토콜
        var shuffleDeckReq = new Com.Nhn.Gameanvil.Sample.Protocol.ShuffleDeckReq
        {
            CurrencyType = Com.Nhn.Gameanvil.Sample.Protocol.CurrencyType.CurrencyCoin,
            Usage = 1
        };
        Debug.Log("shuffleDeckReq " + shuffleDeckReq);

        // ===========================================================================================>>> GameAnvil
        // 게임유저가 서버로 request로 response를 받아 처리 한다.
        gameUser.Request<Com.Nhn.Gameanvil.Sample.Protocol.ShuffleDeckRes>(shuffleDeckReq, (userAgent, shuffleDeckRes) =>
        {
            Debug.Log("shuffleDeckRes" + shuffleDeckRes);

            if (shuffleDeckRes.ResultCode == Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode.None)
            {
                // 서버에 셔플 성공 하고 응답값 갱신
                UserInfo.Instance.CurrentDeck = shuffleDeckRes.Deck;
                UserInfo.Instance.Coin = shuffleDeckRes.BalanceCoin;
                UserInfo.Instance.Ruby = shuffleDeckRes.BalanceRuby;

                // 유저 정보 UI 갱신
                UserDataUiUpdate();
            }
            else
            {
                // 실패시 처리
            }
            buttonDeckShuffle.interactable = true;
        });
        // ===========================================================================================>>> GameAnvil
    }
    void OnClickSingleStartGame()
    {
        // 다중클릭 막음
        buttonSingleStartGame.interactable = false;

        // 게임 시작 프로토콜 데이터 정의
        var startGameReq = new Com.Nhn.Gameanvil.Sample.Protocol.StartGameReq
        {
            Deck = UserInfo.Instance.CurrentDeck,
            Difficulty = Com.Nhn.Gameanvil.Sample.Protocol.DifficultyType.DifficultyNormal
        };
        Debug.Log("startGameReq" + startGameReq);

        // ===========================================================================================>>> GameAnvil
        // 게임 싱글룸 생성
        gameUser.CreateRoom(Constants.SPACE_ROOM_TYPE_SINGLE, new Payload().Add(new Packet(startGameReq)), (UserAgent userAgent, ResultCodeCreateRoom result, int roomId, string roomName, Payload payload) =>
        {
            Debug.Log("CreateRoom " + result);

            if (result == ResultCodeCreateRoom.CREATE_ROOM_SUCCESS)
            {
                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();

                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_SINGLE);
            }
            else
            {
                // 실패 처리
            }
            buttonSingleStartGame.interactable = true;
        });
        // ===========================================================================================>>> GameAnvil
    }

    void OnClickMultiTapBirdStartGame()
    {
        // 다중클릭 막음
        buttonMultiTapBirdStartGame.interactable = false;

        // ===========================================================================================>>> GameAnvil
        // 만들어 져있는 방에 들어가는 룸매치 요청 - 혼자서도 플레이가 가능하다. 최대 인원수 까지 모두 입장
        gameUser.MatchRoom(Constants.SPACE_ROOM_TYPE_MULTI_ROOM_MATCH, "UNLIMITED_TAP", true, false, (UserAgent userAgent, ResultCodeMatchRoom result, int resultCode, int roomId, string roomName, bool created, Payload payload) =>
        {
            Debug.Log("!!!!!!!!!!!!!!MatchRoom " + result);
            if (result == ResultCodeMatchRoom.MATCH_ROOM_SUCCESS)
            {
                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_MULTI_TAPBIRD);
            }
            else
            {
                // 실패 처리
            }
        });
        // ===========================================================================================>>> GameAnvil

        buttonMultiTapBirdStartGame.interactable = true;
    }

    void OnClickMultiSnakeStartGame()
    {
        // 다중클릭 막음
        buttonMultiSnakeStartGame.interactable = false;

        // ===========================================================================================>>> GameAnvil
        // 타이밍 이슈상 리스너를 미리 등록,  유저가 게임방에 들어 갔을때 게임 레디 flag설정
        gameUser.onMatchUserDoneListeners += (UserAgent userAgent, ResultCodeMatchUserDone result, bool created, int roomId, Payload payload) =>
        {
            Debug.Log("onMatchUserDoneListeners!!!!!! " + userAgent.GetUserId());

            if (result == ResultCodeMatchUserDone.MATCH_USER_DONE_SUCCESS)
            {
                UserInfo.Instance.gameState = UserInfo.GameState.Wait;
            }
        };

        // 타이밍 이슈상 리스너 미리등록, 서버에서 게임룸에 두명이 모두 입장했을때 게임 설정데이터를 전송
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

                    var gameUser = new SnakeGameUser
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
                    gameUser.PositionList = snakePositionData;
                    userMap.Add(gameUser.Id, gameUser);
                }

                SnakeGameInfo.Instance.UserMapInfo = userMap;
                Debug.Log("UserMapInfo Count " + userMap.Count);
                UserInfo.Instance.gameState = UserInfo.GameState.Wait;
            }
        });

        // 유저 매치 두명이서 방을 만들어서 동시에 입장해서 게임진행
        gameUser.MatchUserStart(Constants.SPACE_ROOM_TYPE_MULTI_USER_MATCH, "SNAKE", (UserAgent userAgent, ResultCodeMatchUserStart result, Payload payload) =>
        {
            Debug.Log("MatchUser " + result);
            if (result == ResultCodeMatchUserStart.MATCH_USER_START_SUCCESS)
            {
                SnakeGameInfo.Instance.FoodList.Clear();
                SnakeGameInfo.Instance.UserMapInfo.Clear();

                // 성공시 게임신으로 변경, 등록된 리스너 제거
                RemoveAllListeners();
                UserInfo.Instance.MoveScene(Constants.SCENE_GAME_MULTI_SNAKE);
            }
            else
            {
                // 실패 처리
            }
        });
        // ===========================================================================================>>> GameAnvil

        buttonMultiSnakeStartGame.interactable = true;
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
        // 싱글 랭킹 요청
        gameUser.Request<Com.Nhn.Gameanvil.Sample.Protocol.ScoreRankingRes>(singleRankingReq, (userAgent, singleRankingRes) =>
        {
            Debug.Log("singleRankingRes" + singleRankingRes);

            if (singleRankingRes.ResultCode == Com.Nhn.Gameanvil.Sample.Protocol.ErrorCode.None)
            {
                // 성공시 랭킹 처리
            }
            else
            {
                // 실패시 처리
            }
            buttonSingleRanking.interactable = true;
        });
        // ===========================================================================================>>> GameAnvil
    }
}
