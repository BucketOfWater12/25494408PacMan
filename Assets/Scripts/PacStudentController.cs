using UnityEngine;
using UnityEngine.Rendering;

public class PacStudentController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource moveAudio;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private GameManager gameManager;

    [Header("Movement Audio")]
    [SerializeField] private AudioClip moveClip;
    [SerializeField] private AudioClip eatClip;
    [SerializeField] private float movePitch = 1.5f;
    [SerializeField] private float eatPitch = 1.0f;

    [Header("Movement")]
    [SerializeField] private float tilesPerSecond = 5f;

    private Vector2Int lastInput = Vector2Int.zero;
    private Vector2Int currentInput = Vector2Int.zero;

    private Vector2Int currentCell;
    private Vector2Int targetCell;
    private bool isLerping;
    private Vector3 lerpStart;
    private Vector3 lerpEnd;
    private float lerpStartTime;
    private float lerpDuration;

    void Start()
    {
        currentCell = levelGenerator.WorldToCell(transform.position);
        transform.position = levelGenerator.CellToWorld(currentCell);

        lerpDuration = 1f / tilesPerSecond;

        SetFacing(Vector2Int.right);
        SetMovingEffects(false);
    }

    void Update()
    {
        GatherInput();

        if (isLerping)
        {
            ContinueLerp();
        }

        if (!isLerping)
        {
            TryStartMove();
        }
    }

    void GatherInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2Int.right;
    }

    void TryStartMove()
    {
        if (lastInput != Vector2Int.zero && levelGenerator.IsWalkable(currentCell + lastInput))
        {
            currentInput = lastInput;
            StartLerp(currentCell + currentInput);
        }

        else if (currentInput != Vector2Int.zero && levelGenerator.IsWalkable(currentCell + currentInput))
        {
            StartLerp(currentCell + currentInput);
        }

        else
        {
            SetMovingEffects(false);
        }
    }

    void StartLerp(Vector2Int newTarget)
    {
        targetCell = newTarget;
        lerpStart = levelGenerator.CellToWorld(currentCell);
        lerpEnd = levelGenerator.CellToWorld(targetCell);
        lerpStartTime = Time.time;
        isLerping = true;

        SetFacing(currentInput);
        SetMovingEffects(true);
        UpdateMoveAudio();
    }

    void ContinueLerp()
    {
        float t = (Time.time - lerpStartTime) / lerpDuration;

        if(t >= 1f)
        {
            transform.position = lerpEnd;
            currentCell = targetCell;
            isLerping = false;
        }

        else
        {
            transform.position = Vector3.Lerp(lerpStart, lerpEnd, t);
        }
    }

    void SetFacing(Vector2Int direction)
    {
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }

    void SetMovingEffects(bool moving)
    {
        animator.speed = moving ? 1f : 0f;

        if (dustParticles != null)
        {
            if (moving && !dustParticles.isPlaying)
            {
                dustParticles.Play();
            }
            else if (!moving && dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }
        }
        if (!moving && moveAudio.isPlaying)
        {
            moveAudio.Stop();
        }
    }

    void UpdateMoveAudio()
    {
        int tile = levelGenerator.GetTile(targetCell);
        AudioClip wanted = (tile == 5 || tile == 6) ? eatClip : moveClip;

        if (moveAudio.clip != wanted)
        {
            moveAudio.clip = wanted;
            moveAudio.pitch = (wanted == eatClip) ? eatPitch : movePitch;
            moveAudio.Play();
        }

        else if (!moveAudio.isPlaying)
        {
            moveAudio.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pellet"))
        {
            EatPellet(other.gameObject, 10);
        }

        else if (other.CompareTag("PowerPellet"))
        {
            EatPellet(other.gameObject, 50);
        }
    }

    void EatPellet(GameObject pellet, int points)
    {
        levelGenerator.SetTile(levelGenerator.WorldToCell(pellet.transform.position), 0);
        Destroy(pellet);
        gameManager.PelletEaten(points);
    }
}