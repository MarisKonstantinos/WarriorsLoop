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

    [Header("Move")]
    [SerializeField] private float movementSpeed;
    private Vector2 moveInput;

    [Header("Dash")]
    [SerializeField] private float dashPower;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private float dashTimer = 0f;
    //Is used to check if the player can dash - cooldown ------------- DO NOT USE: or dead
    private bool canDash = true;
    //Is used to check the state of the player so he can't be dashing twice.
    private bool isDashing = false;

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
        if (!isDashing)
        {
            rb.velocity = moveInput * movementSpeed;
        }
    }

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

    private IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;
        rb.velocity = moveInput * dashPower;
        dashTimer = dashCooldown;
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}
