using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour 
{
    private Rigidbody2D rb;
    [SerializeField] private bool isMovementDisabled = false;
    private bool isDead = false;
    [SerializeField] private float movementSpeed;
    private Vector2 moveDirection = Vector2.zero;
    
    [SerializeField] private LayerMask lineOfSightLayer;
    [SerializeField] private GameObject target;
    private AnimatorController enemyAnimator;

    //Line Of Sight
    private bool hasLOS;

    [Header("UI")]
    public int numOfRays = 1;
    public float radius = 1;
    public float angleSpread = 30f;
    Vector2 tempMoveDirection;
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

    private void Update()
    {
        DrawCircle(transform.position, radius, Color.red);
    }

    private void FixedUpdate()
    {
        if (!target || isMovementDisabled || numOfRays < 1) return;

        DrawRays(numOfRays);
        MoveEnemy(moveDirection);
    }

    private void DrawRays(int numOfRays)
    {
        hasLOS = false;
        //Caluclating the length by multiplying with radius for Physics2D.Raycast
        for( int i =0; i < numOfRays; i++)
        {
            float angleOffset = ((float)i / numOfRays  - 0.5f) * angleSpread;
            Vector2 rayDirection = Quaternion.Euler(0, 0, angleOffset) * ((target.transform.position - transform.position).normalized * radius);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, rayDirection, radius, lineOfSightLayer);
            
            if (ray.collider != null)
            {
                if (ray.collider.CompareTag("Player"))
                {
                    hasLOS = true;
                    tempMoveDirection = rayDirection;
                    DrawRay(rayDirection, Color.green);
                }
                else
                {
                    DrawRay(rayDirection, Color.red);
                }
            }
            else
            {
                DrawRay(rayDirection, Color.red);
            }
        }
        moveDirection = hasLOS ? tempMoveDirection : Vector2.zero;
    }

    private void DrawRay(Vector3 direction, Color col)
    {
        Debug.DrawRay(transform.position, direction, col);
    }

    private void MoveEnemy(Vector2 direction)
    {
        if(direction != Vector2.zero)
        {
            rb.velocity = direction.normalized * movementSpeed;
            enemyAnimator.PlayMove();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.SetRotation(angle - 90);
        }
        else
        {
            enemyAnimator.PlayIdle();
            rb.velocity = Vector2.zero;
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

    private void DrawCircle(Vector3 center, float radius, Color color, int segments = 32)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }
}
