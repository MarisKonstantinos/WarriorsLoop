using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCombat : MonoBehaviour, IAttack
{
    private Rigidbody2D rb;
    [SerializeField] private AttackData meleeAttack;
    [SerializeField] private AttackData dashAttack;
    [SerializeField] private AttackData spinAttack;
    [SerializeField] private LayerMask enemyLayer;
    [Range(1, 10)] [SerializeField] private int meleeAttackOffset;

    private Vector2 attackPoint;
    private PlayerMovement playerMovement;
    private AnimatorController playerAnimator;
    private HashSet<HealthComponent> hitEnemies = new HashSet<HealthComponent>();
    private bool canAttack = true;
    private HealthComponent health;

    //Melee Attack Variables
    private float meleeCooldownTimer = 0;
    private bool hitPause = false; //Pause the game for a brief second to give weight to the player attack. 

    //Dash Attack Variables
    [SerializeField] private Image dashAttackCooldownImage;
    [SerializeField] private TextMeshProUGUI dashAttackCooldownText;
    [SerializeField] private ParticleSystem dashAttackParticle;
    private float dashAttackCooldownTimer;
    private float dashAttackDurationTimer = 0;
    private bool moveOverlapCircle = false;
    private Vector2 dashAttackPoint;
    public float dashAttackPower = 15;
    public float dashAttackDuration = 0.2f;

    //Spin Attack Variables
    [SerializeField] private Image spinAttackCooldownImage;
    [SerializeField] private TextMeshProUGUI spinAttackCooldownText;
    [SerializeField] private ParticleSystem spinAttackParticle;
    private float spinAttackCooldownTimer;
    private float spinAttackDurationTimer;
    public float spinAttackDuration;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<AnimatorController>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
    }

    private void OnEnable()
    {
        health.OnTakingDamage += DisableAttacksFor;
    }

    private void OnDisable()
    {
        health.OnTakingDamage -= DisableAttacksFor;
    }

    private void Update()
    {
        if (meleeCooldownTimer > 0)
            meleeCooldownTimer -= Time.deltaTime;

        CalculateDashAttackCd();

        CalculateSpinAttackCd();
    }

    private void FixedUpdate()
    {
        if (moveOverlapCircle) ContinousExecute();
    }

    private void ContinousExecute()
    {
        dashAttackPoint = gameObject.transform.localPosition;
        Execute(dashAttackPoint, dashAttack);
    }

    #region Attack Inputs
    public void OnMeleeAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!meleeAttack || !playerMovement || meleeCooldownTimer > 0 || dashAttackDurationTimer > 0
                || spinAttackDurationTimer > 0 || !canAttack) return;

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
            if (!dashAttack || !playerMovement || dashAttackCooldownTimer > 0 || meleeCooldownTimer > 0 
                || spinAttackDurationTimer > 0 || !canAttack) return;
            
            dashAttackDurationTimer = dashAttackDuration;

            //Dash
            playerMovement.DisableMovementFor(dashAttackDuration);
            rb.velocity = playerMovement.GetMoveDirection() * playerMovement.dashPower;

            playerMovement.ToggleInvincibilityFor(dashAttackDuration);
            dashAttackPoint = gameObject.transform.localPosition;
            hitEnemies.Clear();
            Execute(dashAttackPoint, dashAttack);

            if (dashAttackCooldownText && dashAttackCooldownImage)
            {
                dashAttackCooldownImage.fillAmount = 1;
                dashAttackCooldownText.text = dashAttackCooldownTimer.ToString();
            }

            if (playerAnimator)
                playerAnimator.PlayDashAttack();
            ParticleManager.Instance.PlaySimpleParticle(dashAttackParticle,gameObject.transform);
        }
    }

    /// <summary>
    /// Countdown and updating ui elements for dash attack cooldown 
    /// </summary>
    private void CalculateDashAttackCd()
    {
        //Dash attack cooldown
        if (dashAttackCooldownTimer > 0)
        {
            dashAttackCooldownTimer -= Time.deltaTime;
            if (dashAttackCooldownText && dashAttackCooldownImage)
            {
                dashAttackCooldownText.text = dashAttackCooldownTimer.ToString("#.#");
                dashAttackCooldownImage.fillAmount = dashAttackCooldownTimer / dashAttack.cooldown;
            }
        }

        //Dash duration. While in dash moveOverlapCircle is true which is calling execute in FixedUpdate to create the new overlap circle.
        if (dashAttackDurationTimer > 0)
        {
            dashAttackDurationTimer -= Time.deltaTime;
            moveOverlapCircle = true;

        }
        else
        {
            hitEnemies.Clear();
            moveOverlapCircle = false;
        }
    }

    public void OnSpinAttack(CallbackContext context)
    {
        if (context.performed)
        {
            if (!spinAttack || !playerMovement || spinAttackCooldownTimer > 0 || meleeCooldownTimer > 0 || dashAttackDurationTimer > 0
                || !canAttack) return;

            spinAttackDurationTimer = spinAttackDuration;
            
            playerMovement.DisableMovementFor(spinAttackDuration);
            playerMovement.ToggleInvincibilityFor(spinAttackDuration);

            attackPoint = gameObject.transform.localPosition;
            hitEnemies.Clear();
            Execute(attackPoint, spinAttack);

            if(spinAttackCooldownImage && spinAttackCooldownText)
            {
                spinAttackCooldownImage.fillAmount = 1;
                spinAttackCooldownText.text = spinAttackCooldownTimer.ToString();
            }

            if (playerAnimator)
                playerAnimator.PlaySpinAttack();
            ParticleManager.Instance.PlaySimpleParticle(spinAttackParticle,gameObject.transform);
        }
    }

    /// <summary>
    /// Countdown and updating ui elements for spin attack cooldown.
    /// </summary>
    private void CalculateSpinAttackCd()
    {
        //Spin attack cooldown
        if (spinAttackCooldownTimer > 0)
        {
            spinAttackCooldownTimer -= Time.deltaTime;
            if (spinAttackCooldownImage && spinAttackCooldownText)
            {
                spinAttackCooldownText.text = spinAttackCooldownTimer.ToString("#.#");
                spinAttackCooldownImage.fillAmount = spinAttackCooldownTimer / spinAttack.cooldown; ;
            }
        }

        //Spin attack duration
        if (spinAttackDurationTimer > 0)
        {
            spinAttackDurationTimer -= Time.deltaTime;
        }
    }

    #endregion

    /// <summary>
    /// Executes the given attack at a specific point by checking for enemies within range,
    /// calculating damage and knockback direction, and setting cooldowns for each attack type.
    /// </summary>
    /// <param name="attackPoint"> The world position from which the attack originates (usually the player or weapon position).</param>
    /// <param name="attack">The <see cref="AttackData"/> containing attack settings such as name, damage, range, knockback, and cooldown.</param>
    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint, attack.range,enemyLayer);
        //It should be different for each attack.
        if(attack.name == "MeleeAttack")
        {
            meleeCooldownTimer = meleeAttack.cooldown;
            hitPause = true;
            playerMovement.DisableMovementFor(0.2f);
           //DrawCircle(attackPoint, attack.range, Color.red,2);
        }
        else if(attack.name == "DashAttack")
        {
            dashAttackCooldownTimer = dashAttack.cooldown;
        }
        else if(attack.name == "SpinAttack")
        {
            spinAttackCooldownTimer = spinAttack.cooldown;
        }

        foreach (var enemy in hit)
        {
            if (enemy.TryGetComponent(out HealthComponent enemyHealth) && !hitEnemies.Contains(enemyHealth))
            {
                Vector2 hitPoint = enemy.transform.position;
                Vector2 sourcePoint = attackPoint;

                Vector2 knockbackDirection = (hitPoint - sourcePoint).normalized;
                
                enemyHealth.TakeDamage(attack.damage, knockbackDirection, attack.knockbackPower,hitPause);
                hitPause = false;
                hitEnemies.Add(enemyHealth);
            }
        }
    }

    private void DisableAttacksFor(float delay)
    {
        canAttack = false;
        StartCoroutine(EnableAttacks(delay));
    }

    private IEnumerator EnableAttacks(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }

    #region HelperFunctions
    private void DrawCircle(Vector3 center, float radius, Color color,float duration = 1f, int segments = 32)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color,duration);
            prevPoint = nextPoint;
        }
    }
    #endregion
}
