using UnityEngine;

public enum GhostState { Normal, Scared, Recovering, Dead }
public class GhostController : MonoBehaviour
{
    private Animator animator;

    public GhostState State{ get; private set; } = GhostState.Normal;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetState(GhostState newState)
    {
        State = newState;
        animator.SetInteger("State", (int)newState);
    }
}
