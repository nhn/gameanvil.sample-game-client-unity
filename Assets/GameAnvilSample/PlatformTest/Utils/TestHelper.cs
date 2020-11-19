using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 유저 객체 데이터
public class TestHelper
{
    public static string GenerateUUID()
    {
        // uuid 갱신 처리
        string uuid = System.Guid.NewGuid().ToString();
        PlayerPrefs.SetString(Constants.KEY_UUID, uuid);
        Debug.Log("OnClickGenerateUUID " + uuid);
        return uuid;
    }
}
