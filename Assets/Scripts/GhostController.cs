using UnityEngine;
using System.Collections.Generic;
public enum GhostState { Normal, Scared, Recovering, Dead }
public class GhostController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform pacStudent;

    [Header("Ghost")]
    [SerializeField] private int ghostNumber = 1;
    [SerializeField] private float pacStudentSpeed = 5f;

    private Animator animator;
    public GhostState State { get; private set; } = GhostState.Normal;
    private Vector3 startPosition;
    private Vector2Int startCell;
    private Vector2Int currentCell;
    private Vector2Int targetCell;
    private Vector2Int lastDirection;
    private bool isLerping;
    private Vector3 lerpStart;
    private Vector3 lerpEnd;
    private float lerpStartTime;
    private float lerpDuration;

    private bool inSpawn = true;
    private bool returningToSpawn;

    private int waypointIndex;
    private bool followingLoop;

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private static readonly Vector2Int[] OuterLoop =
    {
        new Vector2Int(1, -1),   new Vector2Int(12, -1),  new Vector2Int(12, -5),
        new Vector2Int(15, -5),  new Vector2Int(15, -1),  new Vector2Int(26, -1),
        new Vector2Int(26, -8),  new Vector2Int(21, -8),  new Vector2Int(21, -20),
        new Vector2Int(26, -20), new Vector2Int(26, -27), new Vector2Int(15, -27),
        new Vector2Int(15, -23), new Vector2Int(12, -23), new Vector2Int(12, -27),
        new Vector2Int(1, -27),  new Vector2Int(1, -20),  new Vector2Int(6, -20),
        new Vector2Int(6, -8),   new Vector2Int(1, -8)
    };

    void Awake()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Start()
    {
        startCell = levelGenerator.WorldToCell(startPosition);
        currentCell = startCell;
    }

    void Update()
    {
        if (!gameManager.CanMove) return;

        if (isLerping)
        {
            ContinueLerp();
        }

        if (!isLerping && State != GhostState.Dead)
        {
            Vector2Int direction = inSpawn ? SpawnExitDirection() : ChooseDirection();
            StartStep(direction);
        }
    }

    float CurrentSpeed()
    {
        float normalSpeed = pacStudentSpeed * 0.9f;
        return State == GhostState.Normal ? normalSpeed : normalSpeed * 0.5f;
    }

    void StartStep(Vector2Int direction)
    {
        targetCell = currentCell + direction;
        lastDirection = direction;

        lerpStart = levelGenerator.CellToWorld(currentCell);
        lerpEnd = levelGenerator.CellToWorld(targetCell);
        lerpStartTime = Time.time;
        lerpDuration = 1f / CurrentSpeed();
        isLerping = true;

        SetFacing(direction);
    }

    void ContinueLerp()
    {
        float t = (Time.time - lerpStartTime) / lerpDuration;

        if (t < 1f)
        {
            transform.position = Vector3.Lerp(lerpStart, lerpEnd, t);
            return;
        }

        transform.position = lerpEnd;
        isLerping = false;

        if (returningToSpawn)
        {
            returningToSpawn = false;
            currentCell = startCell;
            lastDirection = Vector2Int.zero;
            inSpawn = true;
            gameManager.GhostReturnedToSpawn(this);
            return;
        }
        currentCell = targetCell;

        if (inSpawn && currentCell == SpawnExitCell())
        {
            inSpawn = false;
        }
    }

    void SetFacing(Vector2Int direction)
    {
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
    }

    Vector2Int SpawnExitCell()
    {
        int gapX = startCell.x <= 13 ? 13 : 14;
        bool exitTop = ghostNumber == 1 || ghostNumber == 3;
        return new Vector2Int(gapX, exitTop ? -11 : -17);
    }

    Vector2Int SpawnExitDirection()
    {
        Vector2Int exit = SpawnExitCell();

        if (currentCell.x != exit.x)
        {
            return currentCell.x < exit.x ? Vector2Int.right : Vector2Int.left;
        }

        return currentCell.y < exit.y ? Vector2Int.up : Vector2Int.down;
    }

    Vector2Int ChooseDirection()
    {
        List<Vector2Int> options = new List<Vector2Int>();
        foreach (Vector2Int d in Directions)
        {
            if (d == -lastDirection) continue;
            if (levelGenerator.IsWalkable(currentCell + d)) options.Add(d);
        }

        if (options.Count == 0)
        {
            followingLoop = false;
            return -lastDirection;
        }

        bool scaredBehaviour = State == GhostState.Scared || State == GhostState.Recovering;

        if (scaredBehaviour || ghostNumber == 1)
        {
            followingLoop = false;
            return PickByDistance(options, true);
        }

        if (ghostNumber == 2)
        {
            return PickByDistance(options, false);
        }

        if (ghostNumber == 3)
        {
            return options[Random.Range(0, options.Count)];
        }

        return FollowOuterLoop(options);
    }

    Vector2Int PickByDistance(List<Vector2Int> options, bool further)
    {
        Vector2 pacPosition = pacStudent.position;
        float currentDistance = Vector2.Distance(levelGenerator.CellToWorld(currentCell), pacPosition);

        List<Vector2Int> matching = new List<Vector2Int>();
        foreach (Vector2Int d in options)
        {
            float newDistance = Vector2.Distance(levelGenerator.CellToWorld(currentCell + d), pacPosition);

            if (further ? newDistance >= currentDistance : newDistance <= currentDistance)
            {
                matching.Add(d);
            }
        }

        List<Vector2Int> pool = matching.Count > 0 ? matching : options;
        return pool[Random.Range(0, pool.Count)];
    }

    Vector2Int FollowOuterLoop(List<Vector2Int> options)
    {
        if (!followingLoop)
        {
            waypointIndex = (NearestWaypoint() + 1) % OuterLoop.Length;
            followingLoop = true;
        }

        if (currentCell == OuterLoop[waypointIndex])
        {
            waypointIndex = (waypointIndex + 1) % OuterLoop.Length;
        }

        Vector2 target = levelGenerator.CellToWorld(OuterLoop[waypointIndex]);

        Vector2Int best = options[0];
        float bestDistance = float.MaxValue;
        foreach (Vector2Int d in options)
        {
            float distance = Vector2.Distance(levelGenerator.CellToWorld(currentCell + d), target);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = d;
            }
        }

        return best;
    }

    int NearestWaypoint()
    {
        int nearest = 0;
        float nearestDistance = float.MaxValue;
        for (int i = 0; i < OuterLoop.Length; i++)
        {
            float distance = Vector2Int.Distance(currentCell, OuterLoop[i]);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = i;
            }
        }
        return nearest;
    }

    public void SetState(GhostState newState)
    {
        State = newState;
        animator.SetInteger("State", (int)newState);

        if (newState == GhostState.Dead)
        {
            StartReturnToSpawn();
        }
    }

    void StartReturnToSpawn()
    {
        returningToSpawn = true;
        followingLoop = false;

        lerpStart = transform.position;
        lerpEnd = startPosition;
        lerpStartTime = Time.time;
        lerpDuration = Mathf.Max(Vector3.Distance(lerpStart, lerpEnd) / CurrentSpeed(), 0.01f);
        isLerping = true;
    }

    public void ResetGhost()
    {
        transform.position = startPosition;
        currentCell = startCell;
        lastDirection = Vector2Int.zero;

        isLerping = false;
        returningToSpawn = false;
        inSpawn = true;
        followingLoop = false;

        SetState(GhostState.Normal);
        SetFacing(Vector2Int.right);
    }
}
