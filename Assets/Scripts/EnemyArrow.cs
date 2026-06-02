using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [Header("Arrow")]
    public float speed = 15f;
    public int damage = 50;
    public float lifeTime = 5f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        transform.rotation = Quaternion.LookRotation(direction) *
                             Quaternion.Euler(0f, 90f, 0f);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats =
                other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}