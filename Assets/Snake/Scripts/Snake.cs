using System.Collections.Generic;
using System.Linq;
using GameAnvil;
using GameAnvil.User;
using GameAnvilConnector;
using UnityEngine;

// 게임 유저 스크립트
public class Snake : MonoBehaviour
{
    // 유저 스네이크
    Vector2 dir = Vector2.right;
    List<Transform> tail = new List<Transform>();

    bool ate = false;

    // Tail Prefab
    public GameObject tailPrefab;

    UserAgent snakeUser;

    // Start is called before the first frame update
    void Start()
    {
        // ===========================================================================================>>> GameAnvil
        // 게임 유저 얻기
        snakeUser = ConnectHandler.Instance.GetUserAgent(Constants.GAME_SPACE_NAME, Constants.userSubId);
        // ===========================================================================================>>> GameAnvil

        InvokeRepeating("Move", 0.3f, 0.3f);
    }

    void Update()
    {
        if (UserInfo.Instance.gameState == UserInfo.GameState.Start)
        {
            // 키 처리
            if (Input.GetKey(KeyCode.RightArrow))
                dir = Vector2.right;
            else if (Input.GetKey(KeyCode.DownArrow))
                dir = -Vector2.up;    // 밑으로 이동
            else if (Input.GetKey(KeyCode.LeftArrow))
                dir = -Vector2.right; // 왼쪽으로 이동
            else if (Input.GetKey(KeyCode.UpArrow))
                dir = Vector2.up;

        }
    }

    void Move()
    {
        if (UserInfo.Instance.gameState == UserInfo.GameState.Start)
        {
            Vector2 v = transform.position;
            transform.Translate(dir);

            List<Com.Nhn.Gameanvil.Sample.Protocol.SnakePositionData> userPositionList = new List<Com.Nhn.Gameanvil.Sample.Protocol.SnakePositionData>();
            // 유저 snake 위치 저장
            var userPosition = new Com.Nhn.Gameanvil.Sample.Protocol.SnakePositionData
            {
                Idx = 0,
                X = (int)transform.position.x,
                Y = (int)transform.position.y
            };
            userPositionList.Add(userPosition);

            if (ate)
            {
                // food 먹을때 유저 꼬리 추가
                GameObject g = (GameObject)Instantiate(tailPrefab, v, Quaternion.identity);
                tail.Insert(0, g.transform);
                SnakeGameInfo.Instance.AddScore(UserInfo.Instance.Uuid);
                ate = false;
            }
            else if (tail.Count > 0)
            {
                // 꼬리 이동
                tail.Last().position = v;
                tail.Insert(0, tail.Last());
                tail.RemoveAt(tail.Count - 1);
            }

            // 꼬리들의 위치 저장 - 전송 프로코록 만듬
            foreach (var tailObject in tail)
            {
                var tailPosition = new Com.Nhn.Gameanvil.Sample.Protocol.SnakePositionData
                {
                    Idx = 0,
                    X = (int)tailObject.position.x,
                    Y = (int)tailObject.position.y
                };
                userPositionList.Add(tailPosition);
            }

            // 전송할 데이터 만듬
            var baseData = SnakeGameInfo.Instance.GetBaseUserDataByProto(UserInfo.Instance.Uuid);
            var snakeUserData = new Com.Nhn.Gameanvil.Sample.Protocol.SnakeUserData
            {
                BaseData = baseData

            };
            snakeUserData.UserPositionListData.AddRange(userPositionList);

            var SnakeUserMsg = new Com.Nhn.Gameanvil.Sample.Protocol.SnakeUserMsg
            {
                UserData = snakeUserData
            };

            // ===========================================================================================>>> GameAnvil
            // 응답없이 서버로 데이터 성으로 전달하는 패킷
            snakeUser.Send(new Packet(SnakeUserMsg));
            // ===========================================================================================>>> GameAnvil

        }
    }

    // 충돌확인
    void OnTriggerEnter2D(Collider2D coll)
    {
        // food에 닫는경우
        if (coll.name.StartsWith("FoodPrefab"))
        {

            Debug.Log("Chap Chap!!!!!!!! " + coll.transform.position.z + ":" + coll.transform.position.x + ":" + coll.transform.position.y);
            ate = true;

            foreach (var food in SnakeGameInfo.Instance.FoodList.ToList<GameObject>())
            {
                if (food.transform.position.z == coll.transform.position.z)
                {
                    SnakeGameInfo.Instance.FoodList.Remove(food);

                    var removeFoodMsg = new Com.Nhn.Gameanvil.Sample.Protocol.SnakePositionData
                    {
                        Idx = (int)food.transform.position.z,
                        X = (int)food.transform.position.x,
                        Y = (int)food.transform.position.x
                    };

                    var snakeRemoveFoodMsg = new Com.Nhn.Gameanvil.Sample.Protocol.SnakeFoodMsg
                    {
                        IsDelete = true,
                        FoodData = removeFoodMsg
                    };

                    // ===========================================================================================>>> GameAnvil
                    // 응답없이 서버로 데이터 성으로 전달하는 패킷
                    snakeUser.Send(new Packet(snakeRemoveFoodMsg));
                    // ===========================================================================================>>> GameAnvil
                }
            }

            Destroy(coll.gameObject);
        }
        // 벽이나 자신에게 닿았을때 게임 종료 처리
        else
        {
            UserInfo.Instance.gameState = UserInfo.GameState.GameOver;
            Debug.Log("Game End!!!!!!!!");
        }
    }
}
