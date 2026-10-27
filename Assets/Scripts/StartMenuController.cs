using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class StartMenuController : MonoBehaviour
{

    [SerializeField] private TMP_Text level1HighScoreText;
    [SerializeField] private TMP_Text level1TimeText;
    [SerializeField] private TMP_Text level2HighScoreText;
    [SerializeField] private TMP_Text level2TimeText;

    void Start()
    {
        ShowRecord("Level1", level1HighScoreText, level1TimeText);
        ShowRecord("Level2", level2HighScoreText, level2TimeText);
    }

    void ShowRecord(string levelKey, TMP_Text scoreText, TMP_Text timeText)
    {
        int score = PlayerPrefs.GetInt(levelKey + "_HighScore", 0);
        float time = PlayerPrefs.GetFloat(levelKey + "_BestTime", 0f);

        scoreText.text = "High Score: " + score;
        timeText.text = "Best Time: " + HUDController.FormatTime(time);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Recreated Level");
    }

    public void LoadLevel2()
    {

    }
}
