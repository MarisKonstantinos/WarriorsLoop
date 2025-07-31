using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour , IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private bool isDead = false;

    public event Action<float> OnKnockback;

    private void Awake()
    {
        currentHealth = maxHealth;
    }



    //Negative value = healing
    //Positive value = damaging
    public void TakeDamage(float value, Vector2 knockbackDirection, float knockbackPower)
    {
        currentHealth -= value;
        if(this.tag == "Player")
        {
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                OnKnockback.Invoke(0.2f);
                //Direction? Mouse direction?


                rb.AddForce(knockbackDirection * knockbackPower, ForceMode2D.Impulse);
            }
            Debug.LogError("Current health of " + this.name + ": " + currentHealth);
        }
        
    }

    void Die()
    {
        
    }
}
