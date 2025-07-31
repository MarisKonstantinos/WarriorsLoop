using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IDamageable damageable))
        {
            Vector2 hitPoint = collision.transform.position;
            Vector2 sourcePoint = transform.position;

            Vector2 knockbackDirection = (hitPoint - sourcePoint).normalized; 
            damageable.TakeDamage(20,knockbackDirection,10.0f);
        }
    }
}
