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

    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleJump();

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("IsGrounded", isGrounded);

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
        float targetSpeed = moveInput * moveSpeed;

        rb.linearVelocity = new Vector3(
            targetSpeed,
            rb.linearVelocity.y,
            0f
        );

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
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && jumpCount < maxJump)
        {
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
}