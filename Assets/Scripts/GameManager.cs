using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private HUDController hud;
    [SerializeField] private LevelGenerator levelGenerator;

    public int Score {  get; private set; }
    public int Lives { get; private set; } = 3;

    private int pelletsRemaining;
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

}
