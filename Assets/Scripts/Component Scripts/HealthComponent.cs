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
    //public bool isInvincible { get; set; } = false;
    private void Awake()
    {
        currentHealth = maxHealth;
        if (healthBar)
            healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(float value, Vector2 knockbackDirection, float knockbackPower)
    {
        if(gameObject.layer.ToString() != "Invincibility")
        {
            currentHealth -= value;
            if(healthAmmountText)
                healthAmmountText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            if(healthBar)
            {
                healthBar.SetHealth(currentHealth);
            }

            Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
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
                enemy.SetIsDead(true);
            }
            
            GameManager.Instance.EnemyDied(gameObject, enemyScore);
        }
    }
}
