using UnityEngine;

public enum GhostState { Normal, Scared, Recovering, Dead }
public class GhostController : MonoBehaviour
{
    private Animator animator;
    private Vector3 startPosition;

    public GhostState State{ get; private set; } = GhostState.Normal;

    void Awake()
    {
        animator = GetComponent<Animator>();

        startPosition = transform.position;
    }

    public void SetState(GhostState newState)
    {
        State = newState;
        animator.SetInteger("State", (int)newState);
    }

    public void ResetGhost()
    {
        transform.position = startPosition;
        SetState(GhostState.Normal);
    }
}
