using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour 
{
    private Rigidbody2D rb;
    private bool isMovementDisabled = false;
    private bool isDead = false;
    [SerializeField] private float movementSpeed;
    
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private GameObject target;
    private AnimatorController enemyAnimator;
    

    //Line Of Sight
    private bool hasLOS;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<AnimatorController>();
    }

    private void OnEnable()
    {
        GetComponent<HealthComponent>().OnKnockback += DisableMovementFor;
    }

    private void OnDisable()
    {
        GetComponent<HealthComponent>().OnKnockback -= DisableMovementFor;
    }

    private void Start()
    {
        if (!target)
            target = GameManager.Instance.GetPlayer();
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
                enemyAnimator.PlayMove();

                float angle = Mathf.Atan2(rayDirection.y, rayDirection.x) * Mathf.Rad2Deg;
                rb.SetRotation(angle - 90);
            }
            else
            {
                enemyAnimator.PlayIdle();
                Debug.DrawRay(transform.position, rayDirection, Color.red);
                rb.velocity = Vector2.zero;
            }
        }
    }

    public void DisableMovement()
    {
        isMovementDisabled = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

    private void DisableMovementFor(float time)
    {
        StartCoroutine(DisableMovementCoroutine(time));
    }

    private IEnumerator DisableMovementCoroutine(float time)
    {
        isMovementDisabled = true;
        yield return new WaitForSeconds(time);
        if(!isDead)
        {
            isMovementDisabled = false;
        }
    }

    public void SetIsDead(bool _isDead)
    {
        isDead = _isDead;
    }
}
