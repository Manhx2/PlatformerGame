using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float jumpForce = 30f;
    public float riseMultiplier = 10f;
    public float fallMultiplier = 10f;
    public int maxJump = 2;

    private Rigidbody rb;
    private int jumpCount;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y,
            0f
        );

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
        moveInput = value.Get<Vector2>();
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
                    break;
                }
            }
        }
    }
}