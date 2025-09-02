using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCombat : MonoBehaviour, IAttack
{
    [SerializeField] private AttackData meleeAttack;
    [SerializeField] private AttackData dashAttack;
    [SerializeField] private AttackData aoeAttack;
    [SerializeField] private LayerMask enemyLayer;
    [Range(1, 10)] [SerializeField] private int meleeAttackOffset;

    private Vector2 attackPoint;
    private PlayerMovement playerMovement;
    private float meleeCooldownTimer = 0;
    private AnimatorController playerAnimator;

    [SerializeField] private float dashAttackCooldownTimer;
    [SerializeField] private Image dashAttackCooldownImage;
    [SerializeField] private TextMeshProUGUI dashAttackCooldownText;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<AnimatorController>();
    }

    private void Update()
    {
        if (meleeCooldownTimer > 0)
            meleeCooldownTimer -= Time.deltaTime;
    }

    public void OnMeleeAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!meleeAttack || !playerMovement || meleeCooldownTimer > 0) return;

            if (meleeAttackOffset == 0) meleeAttackOffset = 1;
            Vector2 meleeAttackPoint = (Vector2)gameObject.transform.localPosition + playerMovement.GetMoveDirection()  / meleeAttackOffset;

            Execute(meleeAttackPoint, meleeAttack);

            if (!playerAnimator) return;

            playerAnimator.PlayAttack();
        }
    }

    public void OnDashAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!dashAttack || !playerMovement || dashAttackCooldownTimer > 0) return;
            //Change it to dashAttackPoint. attackPoint is global var for DrawGizmos.
            attackPoint = gameObject.transform.localPosition;
            Execute(attackPoint, dashAttack);
        }
    }

    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint, attack.range,enemyLayer);
        //It should be different for each attack.
        if(attack.name == "Melee Attack")
        {
            meleeCooldownTimer = meleeAttack.cooldown;
        }
        else if(attack.name == "Dash Attack")
        {
            dashAttackCooldownTimer = dashAttack.cooldown;
        }

        foreach (var enemy in hit)
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
