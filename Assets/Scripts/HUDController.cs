using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject[] lifeIcons;
    [SerializeField] private GameObject scaredTimerGroup;
    [SerializeField] private TMP_Text scaredTimerText;
    [SerializeField] private GameObject countdownOverlay;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text gameTimerText;
    [SerializeField] private Button exitButton;
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString("D6");
    }

    public void UpdateLives(int lives)
    {
        for (int i = 0; i <  lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i < lives);
        }
    }

    public void ShowScaredTimer(bool show)
    {
        scaredTimerGroup.SetActive(show);
    }

    public void UpdateScaredTimer(int secondsLeft)
    {
        scaredTimerText.text = secondsLeft.ToString();
    }

    public void ExitToStart()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void ShowCountdown(bool show)
    {
        countdownOverlay.SetActive(show);
    }

    public void SetCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void UpdateGameTimer(float seconds)
    {
        gameTimerText.text = FormatTime(seconds);
    }

    public static string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60f);
        int secs = (int)(seconds % 60f);
        int hundredths = (int)(seconds * 100f) % 100;
        return $"{minutes:00}:{secs:00}:{hundredths:00}";

    }

    public void ShowGameOver()
    {
        countdownText.text = "GAME OVER";
        countdownOverlay.SetActive(true);
        exitButton.interactable = false;
    }
}
