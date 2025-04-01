using UnityEngine;

public class UnscaledAnimator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.Update(Time.unscaledDeltaTime);
    }
}
