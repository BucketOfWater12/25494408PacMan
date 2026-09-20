using UnityEngine;

public class AstroMovement : MonoBehaviour
{

    [SerializeField] private Vector2[] waypoints;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    private int targetIndex = 1;
    private Vector2 MoveStart;
    private float MoveDuration;

    private float MoveElapsed;

    void Start()
    {
        transform.position = waypoints[0];
        BeginMove();

        audioSource.loop = true;
        audioSource.Play();
    }

    void BeginMove()
    {
        MoveStart = transform.position;
        Vector2 direction = (waypoints[targetIndex] - MoveStart).normalized;
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        float distance = Vector2.Distance(MoveStart, waypoints[targetIndex]);
        MoveDuration = distance / speed;
        MoveElapsed = 0f;
    }

    void Update()
    {
        MoveElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(MoveElapsed / MoveDuration);
        transform.position = Vector2.Lerp(MoveStart, waypoints[targetIndex], t);

        if (t >= 1f)
        {
            targetIndex = (targetIndex + 1) % waypoints.Length;
            BeginMove();
        }
    }
}
