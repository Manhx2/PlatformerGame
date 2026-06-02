using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHP = 2000;
    public int currentHP;

    public int atk = 200;
    public int def = 30;

    [Header("Advanced Stats")]
    [Range(0f, 1f)] 
    public float critRate = 0.1f;      // 10%
    public float critDamage = 1.0f;    // 100%
    [Range(0f, 1f)] 
    public float effectResistance = 0.01f;

    [Header("Growth Per Level (%)")]
    public float hpGrowth = 10f;
    public float atkGrowth = 8f;
    public float defGrowth = 5f;

    public float critRateGrowth = 0.01f;
    public float critDamageGrowth = 0.05f;
    public float effectResGrowth = 0.01f;

    private DamageEffect damageEffect;

    private void Awake()
    {
        damageEffect = GetComponent<DamageEffect>();
    }

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - def, 1);
        currentHP -= finalDamage;

        StartCoroutine(DamageDelayed(finalDamage));

        Debug.Log("Player took " + finalDamage + " damage");

        if (currentHP <= 0)
        {
            Die();
        } 
    }

    private IEnumerator DamageDelayed(int damage)
    {
        yield return new WaitForSeconds(0.5f);

        DamagePopupManager.Instance.ShowDamage(
            transform.position + Vector3.up * 2f,
            damage
        );

        damageEffect?.PlayDamageEffect();
    }

    public int GetDamage()
    {
        int damage = atk;

        if (Random.value < critRate)
        {
            damage = Mathf.RoundToInt(damage * (1f + critDamage));
        }
        return damage;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public bool ResistEffect()
    {
        return Random.value < effectResistance;
    }

    void Die()
    {
        Debug.Log("Player died");
        gameObject.SetActive(false);
    }

    public void LevelUpStats()
    {
        maxHP = Mathf.RoundToInt(maxHP * (1f + hpGrowth / 100f));

        atk = Mathf.RoundToInt(atk * (1f + atkGrowth / 100f));

        def = Mathf.RoundToInt(def * (1f + defGrowth / 100f));

        critRate = Mathf.Min(1f, critRate + critRateGrowth);

        critDamage += critDamageGrowth;

        effectResistance =
            Mathf.Min(1f, effectResistance + effectResGrowth);

        currentHP = maxHP;
    }
}