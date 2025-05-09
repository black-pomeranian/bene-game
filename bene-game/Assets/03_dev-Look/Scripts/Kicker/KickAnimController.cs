using UnityEditor;
using UnityEngine;

public class KickAnimController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("StartKick")]
    // isStartKick ‚ð true ‚É‚·‚é
    public void StartKick()
    {
        if (animator != null)
        {
            animator.SetBool("isStartKick", true);
        }
        int randomIndex = Random.Range(0, 6);
        animator.SetInteger("RandomIndex", randomIndex);
    }

    // isStartKick ‚ð false ‚É‚·‚é
    public void StopKick()
    {
        if (animator != null)
        {
            animator.SetBool("isStartKick", false);
        }
    }

    public void SetIsGoal(bool flag)
    {
        animator.SetBool("isGoal", flag);
    }
}
