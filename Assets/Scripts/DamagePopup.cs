using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;

    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float moveSpeed = 1f;

    private Color textColor;
    private float timer;

    private void Awake()
    {
        timer = lifeTime;
        textColor = damageText.color;
    }

    public void Setup(int damage)
    {
        damageText.text = damage.ToString();
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        float alpha = timer / lifeTime;

        Color c = textColor;
        c.a = alpha;
        damageText.color = c;

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}