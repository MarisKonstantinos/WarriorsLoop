using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour, IAttack
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private AttackData touchAttack;

    private GameObject damagedObject;
    private Vector2 attackPoint;
    private float damageCooldownTimer = 0;

    private void Update()
    {
        if (damageCooldownTimer > 0)
        {
            damageCooldownTimer -= Time.deltaTime;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //TOUCH ATTACK
        if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

        if (damageCooldownTimer > 0 || !touchAttack) return;

        damagedObject = collision.gameObject;
        attackPoint = transform.position;
        Execute(attackPoint, touchAttack);
    }

    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        if (damagedObject.gameObject.layer.ToString() == "Invincible") return;

        damageCooldownTimer = attack.cooldown;

        Vector2 hitPoint = damagedObject.transform.position;
        Vector2 sourcePoint = transform.position;

        Vector2 knockbackDirection = (hitPoint - sourcePoint).normalized;
        if(damagedObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(attack.damage, knockbackDirection, attack.knockbackPower,true);
        }
    }

    public void OnDrawGizmosSelected()
    {
        //RED for melee attack
        if (touchAttack == null || attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint, touchAttack.range);
    }
}
