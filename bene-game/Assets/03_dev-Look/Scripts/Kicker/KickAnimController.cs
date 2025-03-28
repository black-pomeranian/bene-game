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
    }

    // isStartKick ‚ð false ‚É‚·‚é
    public void StopKick()
    {
        if (animator != null)
        {
            animator.SetBool("isStartKick", false);
        }
    }
}
