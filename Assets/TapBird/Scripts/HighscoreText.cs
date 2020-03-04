using UnityEngine;
using UnityEngine.UI;

// 하이스코어 스크립트
public class HighscoreText : MonoBehaviour {

	Text score;

	void OnEnable() {
		score = GetComponent<Text>();
		score.text = "High Score: " +PlayerPrefs.GetInt("HighScore").ToString();
	}
}
