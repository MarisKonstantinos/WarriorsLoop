using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    
    private HealthComponent healthComponent;
    private bool isMovementDisabled = false;
    PlayerInput playerInput;
    [SerializeField] Camera _Camera;

    [Header("Move")]
    [SerializeField] private float movementSpeed;
    private Vector2 moveInput;
    private Vector2 lastLookAtDirection;

    [Header("Dash")]
    public float dashPower;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private UnityEngine.UI.Image dashCooldownImage;
    [SerializeField] private TextMeshProUGUI dashCooldownText;

    private float dashTimer = 0f;
    //Is used to check if the player can dash - cooldown ------------- DO NOT USE: or dead
    private bool canDash = true;

    //Is used to check the state of the player so he can't be dashing twice.
    private bool isDashing = false;

    [SerializeField] private float diagonalBuffer = 1f; //seconds
    private float lastXPressTime;
    private float lastYPressTime;
    Vector2 bufferedInput = Vector2.zero;

    AnimatorController playerAnimator;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        healthComponent = GetComponent<HealthComponent>();
        playerAnimator = GetComponent<AnimatorController>();
    }

    private void OnEnable()
    {
        playerInput.enabled = true;
        GetComponent<HealthComponent>().OnKnockback += DisableMovementFor;
    }
    private void OnDisable()
    {
        playerInput.enabled = false;
        GetComponent<HealthComponent>().OnKnockback -= DisableMovementFor;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //UI Shows smoother in simple Update
        if (!canDash && dashTimer > 0)
        {
            
            dashTimer -= Time.deltaTime;
            
            //Update dash feedback icon/text
            if (dashTimer <= 0f)
            {
                canDash = true;
                dashTimer = 0f;
            }

            if (dashCooldownText && dashCooldownImage)
            {
                dashCooldownText.text = dashTimer.ToString("#.#");
                dashCooldownImage.fillAmount = dashTimer / dashCooldown;
            }
        }

        //Buffered Input for sticky diagonal movement.
        bufferedInput = moveInput;

        //If we were moving diagonally and lifted up/down first. So we need to keep diagonal
        if (Mathf.Abs(bufferedInput.x) > 0.01 && Mathf.Abs(bufferedInput.y) < 0.01)
        {
            if (Time.time - lastYPressTime <= diagonalBuffer)
            {
                bufferedInput.y = Mathf.Sign(lastLookAtDirection.y);
            }
        }

        if (Mathf.Abs(bufferedInput.y) > 0.01 && Mathf.Abs(bufferedInput.x) < 0.01)
        {
            if (Time.time - lastXPressTime <= diagonalBuffer)
            {
                bufferedInput.x = Mathf.Sign(lastLookAtDirection.x);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isMovementDisabled) return;

        rb.velocity = bufferedInput.normalized * movementSpeed;

        if (moveInput != Vector2.zero)
        {
            //For attacks when player is not giving input.
            lastLookAtDirection = bufferedInput;

            //Because if moving right (1,0). tan(1,0) = 0.
            //Angle to 0 means the player looks up instead of right. That's why I reduce 90 degrees.
            float angle = Mathf.Atan2(lastLookAtDirection.y, lastLookAtDirection.x) * Mathf.Rad2Deg;
            rb.SetRotation(angle - 90);
        }

    }

    #region Input Functions
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (Mathf.Abs(moveInput.x) > 0.01)
            lastXPressTime = Time.time;

        if (Mathf.Abs(moveInput.y) > 0.01)
            lastYPressTime = Time.time;

        if (!playerAnimator) return;

        if (moveInput != Vector2.zero)
        {
            playerAnimator.PlayMove();
        }
        else
        {
            playerAnimator.PlayIdle();
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (isDashing || !canDash || moveInput == Vector2.zero) return;

            StartCoroutine(Dash(moveInput));
        }
    }

    /// <summary>
    /// This function is used for testing purposes. E.g. kill the player immidietly.
    /// </summary>
    /// <param name="context"></param>
    public void OnTest(InputAction.CallbackContext context)
    {
        if(context.performed)
            gameObject.GetComponent<HealthComponent>().TakeDamage(20, Vector2.zero, 0);
    }

    #endregion

    #region Movement Functions
    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;
        rb.velocity = moveInput * dashPower;
        dashTimer = dashCooldown;
        if (dashCooldownText && dashCooldownImage)
        {
            dashCooldownImage.fillAmount = 1;
            dashCooldownText.text = dashTimer.ToString();
        }
        ToggleInvincibilityFor(dashDuration);
        yield return new WaitForSeconds(dashDuration);
       
        isDashing = false;
    }

    public void ToggleInvincibilityFor(float duration)
    {
        StartCoroutine(ToggleInvincibility(duration));
    }

    public IEnumerator ToggleInvincibility(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        yield return new WaitForSeconds(duration);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void DisableMovement()
    {
        isMovementDisabled = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

    public void EnableMovement()
    {
        isMovementDisabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    public void DisableMovementFor(float time)
    {
        StartCoroutine(DisableMovementCoroutine(time));
    }

    private IEnumerator DisableMovementCoroutine(float time)
    {
        isMovementDisabled = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(time);
        isMovementDisabled = false;
    }

    /// <summary>
    /// Returns the last move direction if the player is not moving or the current move direction.
    /// It is used at attacking to get the direction of attack.
    /// </summary>
    /// <returns>
    /// A Vector 2 representing either:
    /// - The current move input direction if moving.
    /// - The last look-at direction if idle.
    /// </returns>
    public Vector2 GetMoveDirection()
    {
        if (moveInput == Vector2.zero)
        {
            return lastLookAtDirection;
        }
        else
        {
            return moveInput;
        }
    }

    #endregion
}
