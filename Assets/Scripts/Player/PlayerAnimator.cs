using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;


    public void PlayIdle()
    {
        playerAnimator.SetBool("isMoving", false);
    }

    public void PlayMove()
    {
        playerAnimator.SetBool("isMoving", true);
    }

    public void PlayDie()
    {
        playerAnimator.SetTrigger("isDead");
    }

    public void PlayAttack()
    {
        playerAnimator.SetTrigger("isAttacking");
    }
}
