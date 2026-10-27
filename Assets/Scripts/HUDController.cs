using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject[] lifeIcons;

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


    public void ExitToStart()
    {
        SceneManager.LoadScene("StartScene");
    }
}
