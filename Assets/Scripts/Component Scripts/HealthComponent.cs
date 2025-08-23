using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthComponent : MonoBehaviour , IDamageable
{
    [SerializeField] private float maxHealth;
    private float currentHealth;
    [Min(0),SerializeField]
    private int enemyScore = 0;
    private bool isDead = false;
    public event Action<float> OnKnockback;
    public event Action<GameObject> OnEnemyDeath;
    //public bool isInvincible { get; set; } = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    //Negative value = healing
    //Positive value = damaging
    public void TakeDamage(float value, Vector2 knockbackDirection, float knockbackPower)
    {
        if(gameObject.layer.ToString() != "Invincibility")
        {
            currentHealth -= value;
            
            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                CombatEffectsManager.Instance.HitPause(0.08f);
                OnKnockback?.Invoke(0.2f);
                rb.AddForce(knockbackDirection * knockbackPower, ForceMode2D.Impulse);
            }
            Debug.LogError("Current health of " + this.name + ": " + currentHealth);

            //if is player
            if(gameObject.layer == 6)
            {
                gameObject.GetComponent<AnimatorController>().PlayHit();
            }
            else
            {
                gameObject.GetComponent<AnimatorController>().PlayHit();
            }
        }

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        
        //Player layer
        if(gameObject.layer == 6)
        {
            if(gameObject.TryGetComponent(out PlayerInput input))
            {
                input.DeactivateInput();
            }
            GameManager.Instance.PlayerDied();

            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<AnimatorController>().PlayDie();
        }

        //Enemy layer
        if(gameObject.layer == 7)
        {
            OnEnemyDeath?.Invoke(gameObject);
            if(gameObject.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.DisableMovement();
            }
            
            GameManager.Instance.EnemyDied(gameObject, enemyScore);
        }
    }
}
