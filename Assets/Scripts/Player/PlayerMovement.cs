using System.Collections;
using System.Collections.Generic;
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
    private Vector2 mousePos;

    [Header("Dash")]
    [SerializeField] private float dashPower;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float dashTimer = 0f;
    //Is used to check if the player can dash - cooldown ------------- DO NOT USE: or dead
    private bool canDash = true;
    //Is used to check the state of the player so he can't be dashing twice.
    private bool isDashing = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        healthComponent = GetComponent<HealthComponent>();
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
                Debug.LogError("DASH READY!");
            }
        }
        
    }

    //Fixed update is good for physics.
    private void FixedUpdate()
    {
        if (isDashing || isMovementDisabled) return;

        rb.velocity = moveInput * movementSpeed;
    }

    #region Input Functions
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (isDashing || !canDash || moveInput == Vector2.zero) return;

            StartCoroutine(Dash(moveInput));
        }
    }

    #endregion

    #region Movement
    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;
        ToggleInvincibility(true);
        rb.velocity = moveInput * dashPower;
        dashTimer = dashCooldown;
        yield return new WaitForSeconds(dashDuration);

        ToggleInvincibility(false);
        isDashing = false;
    }

    private void ToggleInvincibility(bool _isInvincible)
    {
        if(_isInvincible)
        {
            gameObject.layer = LayerMask.NameToLayer("Invincible");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }

    private void DisableMovementFor(float time)
    {
        StartCoroutine(DisableMovementCoroutine(time));
    }

    private IEnumerator DisableMovementCoroutine(float time)
    {
        isMovementDisabled = true;
        yield return new WaitForSeconds(time);
        isMovementDisabled = false;
    }

    #endregion
}
