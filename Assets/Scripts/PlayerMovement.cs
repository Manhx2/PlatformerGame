using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;

    [Header("Jump")]
    public float jumpForce = 30f;
    public float riseMultiplier = 10f;
    public float fallMultiplier = 10f;
    public int maxJump = 2;

    [SerializeField] private Animator animator;

    private Rigidbody rb;

    private int jumpCount;
    private float moveInput;
    private float depthInput;

    private bool isGrounded;
    private bool isInStairsZone = false;

    [Header("Slope")]
    public float playerHeight = 4f;
    public float maxSlopeAngle = 50f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Bow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();

        animator.SetBool("IsGrounded", isGrounded);

        if (!OnSlope())
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        else
            animator.SetFloat("yVelocity", 0f);

        if (isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }
    }

    void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput,
                                       0f,
                                       isInStairsZone ? depthInput : 0f);

        //Debug.Log(isInStairsZone);

        if (OnSlope() && !exitingSlope)
        {
            Vector3 slopeDir = GetSlopeMoveDirection(inputDir);

            rb.AddForce(slopeDir * moveSpeed * 5f, ForceMode.Force);

            //Debug.Log(rb.linearVelocity.y);
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 100f, ForceMode.Force);

            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;

            Vector3 velocity = inputDir.normalized * moveSpeed;
            velocity.y = rb.linearVelocity.y;

            rb.linearVelocity = velocity;
        }

        if (moveInput > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            animator.SetBool("IsRunning", true);
        }
        else if (moveInput < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            animator.SetBool("IsRunning", true);
        }
        else if (depthInput > 0 || depthInput < 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }
    void HandleJump()
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y *
                                 (riseMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y *
                                 (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x; 
        depthInput = input.y;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && jumpCount < maxJump)
        {
            exitingSlope = true;

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpForce,
                rb.linearVelocity.z
            );
            jumpCount++;

            isGrounded = false;
        }
    }

    public void ResetJump()
    {
        jumpCount = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.8f)
                {
                    jumpCount = 0;
                    isGrounded = true;
                    break;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0 && angle < maxSlopeAngle;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 inputDir)
    {
        return Vector3.ProjectOnPlane(inputDir, slopeHit.normal).normalized;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("StairsZone"))
        {
            isInStairsZone = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("StairsZone"))
        {
            isInStairsZone = false;
        }
    }

    public void OnShoot(InputValue value)
    {
        if (!value.isPressed) return;

        GameObject arrow = Instantiate(
            arrowPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Arrow arrowScript = arrow.GetComponent<Arrow>();

        Vector3 shootDir;

        if (transform.eulerAngles.y == 180)
            shootDir = Vector3.left;
        else
            shootDir = Vector3.right;

        arrowScript.SetDirection(shootDir);
    }
}