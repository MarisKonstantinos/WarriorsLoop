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
*/  [SerializeField] private AttackData[] attackList;
    [SerializeField] private LayerMask enemyLayer;
    private Vector2 attackPoint;
    private PlayerMovement playerMovement;
    private Vector2 attackPointGizmo;
    private int currentAttack = -1;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMeleeAttack(CallbackContext context)
    {
        if(context.performed)
        {
            Attack(0,attackPoint);
        }
    }

    private void Attack(int attackIndex, Vector2 attackPoint)
    {
        if (attackList == null || attackIndex > attackList.Length - 1 || !playerMovement) return;

        Debug.LogError("ATTACK!");
        attackPoint = (Vector2)gameObject.transform.localPosition + playerMovement.GetMoveDirection() / 2;
        Execute(attackPoint, attackList[attackIndex]);
    }

    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        attackPointGizmo = attackPoint;
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPointGizmo, attack.range,enemyLayer);
        Debug.LogError("Enemies scanned: " + hit.Length);
        foreach(var enemy in hit)
        {
            Debug.LogError("enemy name: " + enemy.name);
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
        if (attackPointGizmo == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPointGizmo, attackList[0].range);
    }
}
