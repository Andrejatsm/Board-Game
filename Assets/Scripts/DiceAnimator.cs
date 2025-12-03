using UnityEngine;

public class DiceAnimator : MonoBehaviour
{
    Animator animator;



    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RollDice()
    {
        animator.SetBool("IsRolling", true);
    }

    public void StopDice()
    {
        animator.SetBool("IsRolling", false);
    }
}
