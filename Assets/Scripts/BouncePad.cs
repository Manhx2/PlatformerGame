using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 60f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );

            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

            player.ResetJump();
        }
    }
}