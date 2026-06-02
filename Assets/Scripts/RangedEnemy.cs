using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Range")]
    public float patrolRange = 4f;
    public float attackRange = 25f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Attack")]
    public float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("Patrol")]
    public Transform patrolCenter;
    public float patrolWaitTime = 2f;

    private Vector3 patrolTarget;
    private float patrolTimer;

    [Header("Vision")]
    public LayerMask visionMask;

    private Rigidbody rb;

    [SerializeField] private Animator animator;

    private enum State
    {
        Patrol,
        Attack
    }

    private State currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetNewPatrolPoint();

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        currentState = State.Patrol;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && CanSeePlayer())
        {
            currentState = State.Attack;
        }
        else
        {
            currentState = State.Patrol;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        float dist =
            Vector3.Distance(transform.position, patrolTarget);

        if (dist < 0.2f)
        {
            rb.linearVelocity =
                new Vector3(0, rb.linearVelocity.y, 0);

            patrolTimer += Time.fixedDeltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
        else
        {
            Vector3 dir =
                (patrolTarget - transform.position).normalized;

            rb.linearVelocity =
                new Vector3(
                    dir.x * moveSpeed,
                    rb.linearVelocity.y,
                    0f
                );

            Flip(dir.x);
        }
    }

    private void Attack()
    {
        rb.linearVelocity =
            new Vector3(0, rb.linearVelocity.y, 0);

        Vector3 dir =
            (player.position - transform.position).normalized;

        Flip(dir.x);

        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack");

        ShootArrow();
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
            return;

        GameObject arrow =
            Instantiate(
                arrowPrefab,
                firePoint.position,
                Quaternion.identity
            );

        EnemyArrow arrowScript =
            arrow.GetComponent<EnemyArrow>();

        EnemyStats enemyStats =
            GetComponent<EnemyStats>();

        if (arrowScript != null)
        {
            Vector3 dir =
                (player.position + Vector3.up -
                 firePoint.position).normalized;

            arrowScript.SetDirection(dir);

            if (enemyStats != null)
            {
                arrowScript.damage =
                    enemyStats.GetDamage();
            }
        }
    }

    private void SetNewPatrolPoint()
    {
        float randomX =
            Random.Range(-patrolRange, patrolRange);

        patrolTarget =
            new Vector3(
                patrolCenter.position.x + randomX,
                patrolCenter.position.y,
                patrolCenter.position.z
            );
    }

    private void Flip(float x)
    {
        if (x > 0)
        {
            transform.rotation =
                Quaternion.Euler(0, 0, 0);
        }
        else if (x < 0)
        {
            transform.rotation =
                Quaternion.Euler(0, 180, 0);
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 origin =
            transform.position + Vector3.up;

        Vector3 direction =
            (player.position - origin).normalized;

        float distance =
            Vector3.Distance(origin, player.position);

        RaycastHit hit;

        if (Physics.Raycast(
            origin,
            direction,
            out hit,
            distance,
            visionMask))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolCenter == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            patrolCenter.position,
            patrolRange
        );
    }
}