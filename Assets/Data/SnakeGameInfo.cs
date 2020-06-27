using UnityEngine;
using System.Collections.Generic;

// snake 게임 정보 객체
public class SnakeGameInfo : MonoBehaviour
{
    private static SnakeGameInfo snakeGameInfo;

    // 게임 화면 사이즈
    public int BoarderLeft { get; set; }
    public int BoarderRight { get; set; }
    public int BoarderBottom { get; set; }
    public int BoarderTop { get; set; }

    // 유저 리스트
    public Dictionary<string, SnakeGameUser> UserMapInfo { get; set; }

    // food 리스트
    public List<GameObject> FoodList { get; set; }

    public static SnakeGameInfo Instance
    {
        get{
            return snakeGameInfo;
        }
    }

    private void Awake()
    {
        if(snakeGameInfo != null)
        {
            Destroy(this.gameObject);
       }

        DontDestroyOnLoad(this.gameObject);
        snakeGameInfo = this;

        // 데이터 초기화
        FoodList = new List<GameObject>();
        UserMapInfo = new Dictionary<string, SnakeGameUser>();
        
    }

    // food를 먹었을때 점수 추가
    public void AddScore(string findId)
    {
        foreach (var snakeGameUser in UserMapInfo.Values)
        {
            if (findId.Equals(snakeGameUser.Id))
            {
                snakeGameUser.Score++;
            }
        }
    }

    // 아이디로 점수 찾기
    public long GetScore(string findId)
    {
        foreach (var snakeGameUser in UserMapInfo.Values)
        {
            if (findId.Equals(snakeGameUser.Id))
            {
                return snakeGameUser.Score;
            }
        }

        return 0;
    }

    // 아이디로 게임유저 객체 찾기
    public SnakeGameUser GetSnakeGameUser(string findId)
    {
        foreach (var snakeGameUser in UserMapInfo.Values)
        {
            if (findId.Equals(snakeGameUser.Id))
            {
                return snakeGameUser;
            }
        }
        return null;
    }

    // 전달할 프로토콜 생성
    public Com.Nhn.Gameflex.Sample.Protocol.RoomUserData GetBaseUserDataByProto(string findId)
    {
        foreach (var snakeGameUser in UserMapInfo.Values)
        {
            if (findId.Equals(snakeGameUser.Id))
            {
                var baseData = new Com.Nhn.Gameflex.Sample.Protocol.RoomUserData
                {
                    Id = snakeGameUser.Id,
                    NickName = snakeGameUser.Nickname,
                    Score = snakeGameUser.Score
                };
                return baseData;
            }
        }
        return null;
    }
}
