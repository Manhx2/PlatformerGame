using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Enemy : MonoBehaviour
{
    public Transform player;

    [Header("Range")]
    public float patrolRange = 14f;
    public float detectionRange = 20f;
    public float attackRange = 4f;

    [Header("Speed")]
    public float moveSpeed = 3f;

    [Header("Attack")]
    public float attackCooldown = 3f;
    private float lastAttackTime = -999f;

    [Header("Patrol")]
    public Transform patrolCenter;
    public float patrolWaitTime = 2f;
    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    [Header("Vision")]
    public LayerMask visionMask;

    private float stateDelay = 1f;
    private float stateTimer;
    private State nextState;

    private enum State { Patrol, Chase, Attack, Wait }
    private State currentState;

    private Rigidbody rb;

    [SerializeField] private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        SetNewPatrolPoint();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        currentState = State.Patrol;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        State detectedState = GetStateFromCondition();

        if (currentState != State.Wait && detectedState != currentState)
        {
            nextState = detectedState;
            currentState = State.Wait;
            stateTimer = stateDelay;

            if (nextState == State.Chase)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                Flip(dir.x);
                animator.SetTrigger("Detected");
            }
            else if (nextState == State.Patrol)
            {
                animator.SetTrigger("NotDetected");
            }
        }

        switch (currentState)
        {
            case State.Wait:
                Wait();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Patrol:
                Patrol();
                break;
        }
    }

    State GetStateFromCondition()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && CanSeePlayer())
            return State.Attack;

        if (distance <= detectionRange &&
            Vector3.Distance(patrolCenter.position, player.position) <= patrolRange &&
            CanSeePlayer())
            return State.Chase;

        return State.Patrol;
    }

    void Wait()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        animator.SetBool("IsWaiting", true);

        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
        {
            animator.SetBool("IsWaiting", false);
            currentState = nextState;
        }
    }

    void Chase()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Flip(dir.x);

        rb.linearVelocity = new Vector3(
            dir.x * moveSpeed,
            rb.linearVelocity.y,
            0f
        );
    }

    void Patrol()
    {
        float dist = Vector3.Distance(transform.position, patrolTarget);

        if (dist < 0.2f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            patrolTimer += Time.fixedDeltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
        else
        {
            Vector3 dir = (patrolTarget - transform.position).normalized;

            rb.linearVelocity = new Vector3(
                dir.x * moveSpeed,
                rb.linearVelocity.y,
                0f
            );

            Flip(dir.x);
        }
    }

    void SetNewPatrolPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);

        patrolTarget = new Vector3(
            patrolCenter.position.x + randomX,
            patrolCenter.position.y,
            patrolCenter.position.z
        );
    }

    void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        animator.SetTrigger("Attack");
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        DealDamage();
        Debug.Log("Enemy Attack!");
    }

    public void DealDamage()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            EnemyStats es = GetComponent<EnemyStats>();

            if (ps != null && es != null)
            {
                int rawDamage = es.GetDamage();
                ps.TakeDamage(rawDamage);
            }
        }
    }

    void Flip(float x)
    {
        if (x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 direction = (player.position - origin).normalized;
        float distance = Vector3.Distance(origin, player.position);

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, distance, visionMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        if (patrolCenter == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolCenter.position, patrolRange);
    }
}