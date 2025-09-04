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

    //Melee Attack Variables
    private float meleeCooldownTimer = 0;

    //Dash Attack Variables
    [SerializeField] private Image dashAttackCooldownImage;
    [SerializeField] private TextMeshProUGUI dashAttackCooldownText;
    private float dashAttackCooldownTimer;
    private float dashAttackDurationTimer = 0;
    private bool moveOverlapCircle = false;
    private Vector2 dashAttackPoint;
    public float dashAttackPower = 15;
    public float dashAttackDuration = 0.2f;

    //Spin Attack Variables
    [SerializeField] private Image spinAttackCooldownImage;
    [SerializeField] private TextMeshProUGUI spinAttackCooldownText;
    private float spinAttackCooldownTimer;
    private float spinAttackDurationTimer;
    public float spinAttackDuration;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<AnimatorController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (meleeCooldownTimer > 0)
            meleeCooldownTimer -= Time.deltaTime;

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

        //Dash duration
        if(dashAttackDurationTimer > 0)
        {
            dashAttackDurationTimer -= Time.deltaTime;
            moveOverlapCircle = true;
            
        }
        else
        {
            hitEnemies.Clear();
            moveOverlapCircle = false;
        }

        //Spin attack cooldown
        if(spinAttackCooldownTimer > 0)
        {
            spinAttackCooldownTimer -= Time.deltaTime;
            if (spinAttackCooldownImage && spinAttackCooldownText)
            {
                spinAttackCooldownText.text = spinAttackCooldownTimer.ToString("#.#");
                spinAttackCooldownImage.fillAmount = spinAttackCooldownTimer / spinAttack.cooldown; ;
            }
        }

        //Spin duration
        if (spinAttackDurationTimer > 0)
        {
            spinAttackDurationTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (moveOverlapCircle)
        {
            dashAttackPoint = gameObject.transform.localPosition;
            Execute(dashAttackPoint, dashAttack);
        }
    }

    #region Attack Inputs
    public void OnMeleeAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!meleeAttack || !playerMovement || meleeCooldownTimer > 0 || dashAttackDurationTimer > 0 || spinAttackDurationTimer > 0) return;

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
            if (!dashAttack || !playerMovement || dashAttackCooldownTimer > 0 || meleeCooldownTimer > 0 || spinAttackDurationTimer > 0) return;
            
            dashAttackDurationTimer = dashAttackDuration;

            //Dash
            playerMovement.DisableMovementFor(dashAttackDuration);
            rb.velocity = playerMovement.GetMoveDirection() * playerMovement.dashPower;

            //Change it to dashAttackPoint. attackPoint is global var for DrawGizmos.
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
        }
    }

    public void OnSpinAttack(CallbackContext context)
    {
        if(context.performed)
        {
            if (!spinAttack || !playerMovement || spinAttackCooldownTimer > 0 || meleeCooldownTimer > 0 || dashAttackDurationTimer > 0) return;

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
        }
    }

    #endregion

    public void Execute(Vector2 attackPoint, AttackData attack)
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint, attack.range,enemyLayer);
        //It should be different for each attack.
        if(attack.name == "MeleeAttack")
        {
            meleeCooldownTimer = meleeAttack.cooldown;
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

                enemyHealth.TakeDamage(attack.damage, knockbackDirection, attack.knockbackPower);
                hitEnemies.Add(enemyHealth);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        //RED for melee attack
        if (meleeAttack == null || attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dashAttackPoint, dashAttack.range);
    }
}
