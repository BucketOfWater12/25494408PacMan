using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{

    [SerializeField] private HUDController hud;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private GhostController[] ghosts;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PacStudentController pacStudent;
    [SerializeField] private float deathSequenceDuration = 2f;
    [SerializeField] private float deadGhostDuration = 3f;
    [SerializeField] private string levelKey = "Level1";
    [SerializeField] private float gameOverDuration = 3f;

    public int Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public bool CanMove { get; private set; } = true;
    private bool isGameOver;
    private int pelletsRemaining;

    private const float ScaredDuration = 10f;
    private const float RecoveringAt = 3f;
    private float scaredTimer;

    private float gameTime;
    private bool timerRunning;


    public GhostState GlobalGhostState { get; private set; } = GhostState.Normal;
    void Start()
    {
        pelletsRemaining = levelGenerator.CountPellets();

        hud.UpdateScore(Score);
        hud.UpdateLives(Lives);
        hud.UpdateGameTimer(0f);
        StartCoroutine(RoundStart());
    }

    public void AddScore(int amount)
    {
        Score += amount;
        hud.UpdateScore(Score);
    }

    public void PelletEaten(int points)
    {
        AddScore(points);
        pelletsRemaining--;

        if (pelletsRemaining <= 0)
        {
            GameOver();
        }
    }

    void Update()
    {
        if (isGameOver) return;

        if (timerRunning)
        {
            gameTime += Time.deltaTime;
            hud.UpdateGameTimer(gameTime);
        }

        if (GlobalGhostState == GhostState.Normal)
        {
            return;
        }
        
        scaredTimer -= Time.deltaTime;

        if (scaredTimer <= 0f)
        {
            SetGlobalGhostState(GhostState.Normal);
            hud.ShowScaredTimer(false);
            UpdateMusic();
            return;
        }

        if (scaredTimer <= RecoveringAt && GlobalGhostState == GhostState.Scared)
        {
            SetGlobalGhostState(GhostState.Recovering);
        }

        hud.UpdateScaredTimer(Mathf.CeilToInt(scaredTimer));

    }

    public void PowerPelletEaten()
    {
        scaredTimer = ScaredDuration;
        SetGlobalGhostState(GhostState.Scared);

        hud.ShowScaredTimer(true);
        hud.UpdateScaredTimer(Mathf.CeilToInt(scaredTimer));
        UpdateMusic();
    }

    void SetGlobalGhostState(GhostState state)
    {
        GlobalGhostState = state;

        foreach (GhostController ghost in ghosts)
        {
            if (ghost.State != GhostState.Dead)
            {
                ghost.SetState(state);
            }
        }
    }

    public void UpdateMusic()
    {
        foreach (GhostController ghost in ghosts)
        {
            if (ghost.State == GhostState.Dead)
            {
                audioManager.PlayDead();
                return;
            }
        }

        if (GlobalGhostState == GhostState.Normal)
        {
            audioManager.PlayNormal();
        }
        else
        {
            audioManager.PlayScared();
        }
    }

    public void PacStudentDied()
    {
        CanMove = false;
        Lives--;
        hud.UpdateLives(Lives);

        pacStudent.PlayDeath();
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(deathSequenceDuration);

        if (Lives <= 0)
        {
            GameOver();
            yield break;
        }

        pacStudent.Respawn();

        foreach (GhostController ghost in ghosts)
        {
            ghost.ResetGhost();
        }

        scaredTimer = 0f;
        GlobalGhostState = GhostState.Normal;
        hud.ShowScaredTimer(false);
        UpdateMusic();

        CanMove = true;
    }

    public void GhostEaten(GhostController ghost)
    {
        ghost.SetState(GhostState.Dead);
        AddScore(300);
        UpdateMusic();
        StartCoroutine(ReviveGhost(ghost));
    }

    IEnumerator ReviveGhost(GhostController ghost)
    {
        yield return new WaitForSeconds(deadGhostDuration);

        if (ghost.State == GhostState.Dead)
        {
            ghost.SetState(GlobalGhostState);
            UpdateMusic();
        }
    }

    IEnumerator RoundStart()
    {
        CanMove = false;
        hud.ShowCountdown(true);
        audioManager.PlayIntro();

        string[] steps = { "3", "2", "1", "GO!" };
        foreach (string step in steps)
        {
            hud.SetCountdownText(step);
            yield return new WaitForSeconds(1f);
        }

        hud.ShowCountdown(false);
        audioManager.PlayNormal();
        CanMove = true;
        timerRunning = true;
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        CanMove = false;
        timerRunning = false;

        hud.ShowGameOver();
        audioManager.StopMusic();
        SaveHighScore();

        StartCoroutine(ReturnToStart());
        pacStudent.StopMovement();
    }

    void SaveHighScore()
    {
        string scoreKey = levelKey + "_HighScore";
        string timeKey = levelKey + "_BestTime";

        bool hasRecord = PlayerPrefs.HasKey(scoreKey);
        int bestScore = PlayerPrefs.GetInt(scoreKey, 0);
        float bestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);

        if(!hasRecord || Score > bestScore || (Score == bestScore && gameTime < bestTime))
        {
            PlayerPrefs.SetInt(scoreKey, Score);
            PlayerPrefs.SetFloat(timeKey, gameTime);
            PlayerPrefs.Save();
        }
    }

    IEnumerator ReturnToStart()
    {
        yield return new WaitForSeconds(gameOverDuration);
        SceneManager.LoadScene("StartScene");
    }
}
