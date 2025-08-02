using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
    private Rigidbody2D rb;
    private bool isMovementDisabled = false;
    [SerializeField] private float movementSpeed;
    
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private GameObject target;
    
    //Line Of Sight
    private bool hasLOS;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        GetComponent<HealthComponent>().OnKnockback += DisableMovementFor;
    }

    private void OnDisable()
    {
        GetComponent<HealthComponent>().OnKnockback -= DisableMovementFor;
    }

    private void FixedUpdate()
    {
        if (!target || isMovementDisabled) return;

        Vector2 rayDirection = target.transform.position - transform.position;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, rayDirection,Mathf.Infinity,lineOfSightLayer);
        if (ray.collider != null)
        {
            hasLOS = ray.collider.CompareTag("Player");
            if (hasLOS)
            {
                Debug.DrawRay(transform.position, rayDirection, Color.green);
                rb.velocity = rayDirection.normalized * movementSpeed;
            }
            else
            {
                Debug.DrawRay(transform.position, rayDirection, Color.red);
                rb.velocity = Vector2.zero;
            }
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
}
