using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 유저 객체 데이터
public class UserInfo : MonoBehaviour
{
    public enum GameState
    {
        None,
        Wait,
        Start,
        GameOver
    }

    private static UserInfo userData;

    public string Uuid { get; set; }
    public string Nickname { get; set; }
    public int Heart { get; set; }
    public long Coin { get; set; }
    public long Ruby { get; set; }
    public int Level { get; set; }
    public long Exp { get; set; }
    public long HighScore { get; set; }
    public long TotalScore { get; set; }

    public string CurrentDeck { get; set; }

    public string Scene { get; set; }
    
    public GameState gameState { get; set; }

    public bool IsMulti { get; set; }

    public static UserInfo Instance
    {
        get{
            return userData;
        }
    }

    private void Awake()
    {
        if(userData != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        userData = this;
        
    }

    public void MoveScene(string moveScene)
    {
        Scene = moveScene;
        SceneManager.LoadScene(Constants.SCENE_LOADING);
    }
}
