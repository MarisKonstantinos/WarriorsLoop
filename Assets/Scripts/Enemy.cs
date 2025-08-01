using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float damageValue = 10;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private float knockbackPower = 10;
    [SerializeField] private LayerMask targetLayer;
    private float damageCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageCooldownTimer = damageCooldown;
    }

    private void Update()
    {
        if(damageCooldownTimer > 0)
        {
            damageCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //On Trigger Stay instead of Enter to check more regularly but I need to check for cooldown.
        if (((1 << collision.gameObject.layer) & targetLayer) == 0) return;

        if (damageCooldownTimer > 0) return;
        
        if(collision.TryGetComponent(out IDamageable damageable))
        {
            if (collision.gameObject.layer.ToString() == "Invincible") return;
            
            damageCooldownTimer = damageCooldown;

            Vector2 hitPoint = collision.transform.position;
            Vector2 sourcePoint = transform.position;

            Vector2 knockbackDirection = (hitPoint - sourcePoint).normalized;
            damageable.TakeDamage(damageValue, knockbackDirection, knockbackPower);
        }
    }
}
