using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int attackAnimations; //2 different attack animations cycling through
    private int currentAttackAnim;

    private void Start()
    {
        currentAttackAnim = 0;
    }

    private void Awake()
    {
        if(!animator)
        {
            animator = gameObject.GetComponent<Animator>();
        }
    }

    public void PlayIdle()
    {
        animator.SetBool("isMoving", false);
    }

    public void PlayMove()
    {
        animator.SetBool("isMoving", true);
    }

    public void PlayHit()
    {
        animator.SetTrigger("isHit");
    }

    public void PlayDie()
    {
        animator.SetTrigger("isDead");
    }

    public void PlayAttack()
    {
        SetCurrentAttackAnim();
        animator.SetTrigger("isAttacking");
    }

    private void SetCurrentAttackAnim()
    {
        if (currentAttackAnim < attackAnimations - 1)
        {
            currentAttackAnim++;
        }
        else
        {
            currentAttackAnim = 0;
        }
        animator.SetInteger("attack", currentAttackAnim);
    }
}
