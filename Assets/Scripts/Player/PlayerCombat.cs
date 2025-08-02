using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCombat : MonoBehaviour, IAttack
{
/*
    - 0 -> Melee Attack
    - 1 -> Dash Attack
    - 2 -> AOE Attack
*//*  [SerializeField] private AttackData[] attackList;*/
    [SerializeField] private AttackData meleeAttack;
    [SerializeField] private AttackData dashAttack;
    [SerializeField] private AttackData aoeAttack;

    [SerializeField] private LayerMask enemyLayer;
    private Vector2 attackPoint;
    private PlayerMovement playerMovement;

    [Range(1, 10)] [SerializeField] private int meleeAttackOffset;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMeleeAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!meleeAttack || !playerMovement) return;

            Debug.LogError("ATTACK!");
            if (meleeAttackOffset == 0) meleeAttackOffset = 1;
            attackPoint = (Vector2)gameObject.transform.localPosition + playerMovement.GetMoveDirection()  / meleeAttackOffset;

            Execute(attackPoint, meleeAttack);
        }
    }

    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint, attack.range,enemyLayer);
        
        foreach(var enemy in hit)
        {
            if (enemy.TryGetComponent(out HealthComponent enemyHealth))
            {
                Vector2 hitPoint = enemy.transform.position;
                Vector2 sourcePoint = attackPoint;

                Vector2 knockbackDirection = (hitPoint - sourcePoint).normalized;

                enemyHealth.TakeDamage(attack.damage, knockbackDirection, attack.knockbackPower);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        //RED for melee attack
        if (meleeAttack == null || attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint, meleeAttack.range);
    }
}
