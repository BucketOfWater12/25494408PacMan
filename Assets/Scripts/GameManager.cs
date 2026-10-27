using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{

    [SerializeField] private HUDController hud;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private GhostController[] ghosts;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PacStudentController pacStudent;
    [SerializeField] private float deathSequenceDuration = 2f;
    [SerializeField] private float deadGhostDuration = 3f;

    public int Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public bool CanMove { get; private set; } = true;

    private int pelletsRemaining;

    private const float ScaredDuration = 10f;
    private const float RecoveringAt = 3f;
    private float scaredTimer;


    public GhostState GlobalGhostState { get; private set; } = GhostState.Normal;
    void Start()
    {
        pelletsRemaining = levelGenerator.CountPellets();

        hud.UpdateScore(Score);
        hud.UpdateLives(Lives);
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
            //Gameover
        }
    }

    void Update()
    {
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
}
