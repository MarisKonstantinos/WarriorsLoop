using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    private ImpactResponseComponent impactComponent;
    public event Action<float> OnKnockback;
    public event Action<GameObject> OnEnemyDeath;
    public event Action<float> OnTakingDamage; 
    
    private void Awake()
    {
        SetMaxHealth(0f);
        currentHealth = maxHealth;
        UpdateHealthBar();
        impactComponent = GetComponent<ImpactResponseComponent>();
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
            
            OnTakingDamage?.Invoke(0.1f);
            UpdateHealthBar();

            if (hitPause)
                CombatEffectsManager.Instance.HitPause(0.08f);

            Knockback(knockbackDirection, knockbackPower);

            PlayHitEffects();
        }

        CheckIfDead();
    }

    /// <summary>
    /// Plays the animation and particles when the object was hit.
    /// </summary>
    private void PlayHitEffects()
    {
        gameObject.TryGetComponent(out animController);
        if (animController)
        {
            animController.PlayHit();
        }
        ParticleManager.Instance.PlayHitParticle(gameObject.transform.position);
    }

    /// <summary>
    /// Applies knockback on knockbackDirection with knockbackPower
    /// </summary>
    /// <param name="knockbackDirection"></param>
    /// <param name="knockbackPower"></param>
    private void Knockback(Vector2 knockbackDirection, float knockbackPower)
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            OnKnockback?.Invoke(0.2f);
            rb.AddForce(knockbackDirection * knockbackPower, ForceMode2D.Impulse);
        }
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            Die();
            if (impactComponent)
                impactComponent.PlayBreakFeedback();
        }
        else
        {
            if (impactComponent)
                impactComponent.PlayHitFeedback();
        }
    }

    void Die()
    {
        isDead = true;
        //Player layer
        if (gameObject.layer == 6)
        {
            PlayerDied();
        }

        //Enemy layer
        if(gameObject.layer == 7)
        {
            EnemyDied();
        }
    }

    /// <summary>
    /// Handles the death or destruction behavior of this object depending on its tag.
    /// 
    /// - If the object is an enemy:
    ///   • Invokes OnEnemyDeath event.  
    ///   • Disables movement and marks the enemy as dead.  
    ///   • Notifies the <see cref="GameManager"/> that an enemy has died, awarding score.  
    ///   • Schedules the object for delayed destruction.  
    /// 
    /// - If the object is a Healing item:  
    ///   • Plays destruction particles and healing sound.  
    ///   • Heals the player by a fixed amount.  
    ///   • Immediately destroys the object.
    /// </summary>
    private void EnemyDied()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            OnEnemyDeath?.Invoke(gameObject);
            if (gameObject.TryGetComponent(out EnemyMovement enemy))
            {
                enemy.DisableMovement();
                enemy.SetIsDead(true);
            }

            GameManager.Instance.EnemyDied(gameObject, enemyScore);
            StartCoroutine(LateDestroy());
        }

        if (gameObject.CompareTag("Healing item"))
        {
            //Play healing sound
            ParticleManager.Instance.PlayBoxDestroyParticles(gameObject.transform.position);
            GameManager.Instance.HealPlayer(10);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles the actions to be performed when the player dies.
    /// </summary>
    /// <remarks>This method disables player input, notifies the game manager of the player's death, disables
    /// the player's collider, and plays the death animation. It is intended to be called when the player's death
    /// condition is met.</remarks>
    private void PlayerDied()
    {
        GameManager.Instance.TogglePlayerInput(false);
        GameManager.Instance.PlayerDied();

        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        gameObject.GetComponent<AnimatorController>().PlayDie();
    }

    private void UpdateHealthBar()
    {
        if (healthBar)
        {
            if (currentHealth < 0)
                currentHealth = 0;
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    private IEnumerator LateDestroy()
    {
        gameObject.GetComponent<AnimatorController>().PlayDie();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
