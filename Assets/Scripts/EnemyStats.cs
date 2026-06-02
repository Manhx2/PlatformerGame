using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHP = 500;
    public int currentHP;

    public int atk = 100;
    public int def = 10;

    [SerializeField]
    private int expReward = 20;

    private DamageEffect damageEffect;

    private void Awake()
    {
        damageEffect = GetComponent<DamageEffect>();
    }

    void Start()
    {
        PlayerLevel player =
        FindFirstObjectByType<PlayerLevel>();

        if (player != null)
        {
            ScaleByPlayerLevel(player.currentLevel);
        }

        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - def, 1);
        currentHP -= finalDamage;

        Debug.Log(name + " took " + finalDamage + " damage");

        damageEffect?.PlayDamageEffect();

        if (currentHP <= 0)
        {
            Die();
        }

        DamagePopupManager.Instance.ShowDamage(
            transform.position + Vector3.up * 2f,
            finalDamage
        );
    }

    void Die()
    {
        PlayerLevel player =
        FindFirstObjectByType<PlayerLevel>();

        if (player != null)
        {
            player.AddExp(expReward);
        }

        Debug.Log(name + " died");
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return atk;
    }

    public void ScaleByPlayerLevel(int level)
    {
        maxHP = Mathf.RoundToInt(
            maxHP * Mathf.Pow(1.25f, level - 1)
        );

        atk = Mathf.RoundToInt(
            atk * Mathf.Pow(1.20f, level - 1)
        );

        def = Mathf.RoundToInt(
            def * Mathf.Pow(1.15f, level - 1)
        );

        expReward = Mathf.RoundToInt(
            expReward * Mathf.Pow(1.18f, level - 1)
        );

        currentHP = maxHP;
    }
}