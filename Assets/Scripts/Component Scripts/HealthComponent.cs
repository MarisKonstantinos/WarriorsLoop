using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HealthComponent : MonoBehaviour , IDamageable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextMeshProUGUI healthAmmountText;
    private float currentHealth;
    [Tooltip("Set this to 0 for non enemy characters.") , Min(0), SerializeField] private int enemyScore = 0;
    private bool isDead = false;
    private AnimatorController animController;

    public event Action<float> OnKnockback;
    public event Action<GameObject> OnEnemyDeath;
    
    private void Awake()
    {
        SetMaxHealth(0f);
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetMaxHealth(float buffedPercentage)
    {
        buffedPercentage = buffedPercentage / 100;
        maxHealth = maxHealth + maxHealth* buffedPercentage;
    }

    public void HealDamage(float value)
    {
        currentHealth += value;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float value, Vector2 knockbackDirection, float knockbackPower,bool hitPause)
    {
        if(gameObject.layer.ToString() != "Invincibility")
        {
            currentHealth -= value;

            UpdateHealthBar();

            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if(hitPause)
                    CombatEffectsManager.Instance.HitPause(0.08f);
                OnKnockback?.Invoke(0.2f);
                rb.AddForce(knockbackDirection * knockbackPower, ForceMode2D.Impulse);
            }

            gameObject.TryGetComponent(out animController);
            if (animController)
            {
                animController.PlayHit();
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
            GameManager.Instance.TogglePlayerInput(false);
            GameManager.Instance.PlayerDied();

            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<AnimatorController>().PlayDie();
        }

        //Enemy layer
        if(gameObject.layer == 7)
        {
            if(gameObject.CompareTag("Enemy"))
            {
                OnEnemyDeath?.Invoke(gameObject);
                if(gameObject.TryGetComponent(out EnemyMovement enemy))
                {
                    enemy.DisableMovement();
                    enemy.SetIsDead(true);
                }
            
                GameManager.Instance.EnemyDied(gameObject, enemyScore);
            }
            
            if(gameObject.CompareTag("Healing item"))
            {
                //Play sound
                //Play particles
                ParticleManager.Instance.PlayBoxDestroyParticles(gameObject.transform.position);
                GameManager.Instance.HealPlayer(10);
                Destroy(gameObject);
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }
}
