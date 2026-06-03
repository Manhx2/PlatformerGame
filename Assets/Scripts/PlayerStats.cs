using UnityEngine;
using UnityEngine.InputSystem;
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
    public float critRate = 0.1f;
    public float critDamage = 1.0f;
    [Range(0f, 1f)]
    public float effectResistance = 0.01f;

    [Header("Growth Per Level (%)")]
    public float hpGrowth = 10f;
    public float atkGrowth = 8f;
    public float defGrowth = 5f;

    public float critRateGrowth = 0.01f;
    public float critDamageGrowth = 0.05f;
    public float effectResGrowth = 0.01f;

    [Header("Shield Skill")]
    public bool hasShieldSkill = false;

    [SerializeField] private float shieldCooldown = 5f;
    [SerializeField] private float shieldDuration = 4f;
    [SerializeField] private GameObject shieldSkillObject;

    private bool shieldReady = true;
    private bool shieldActive = false;

    private DamageEffect damageEffect;

    private void Awake()
    {
        damageEffect = GetComponent<DamageEffect>();
    }

    private void Start()
    {
        currentHP = maxHP;

        if (shieldSkillObject != null)
            shieldSkillObject.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
        Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E Pressed");
        }

        if (hasShieldSkill &&
            shieldReady &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Activate Shield");

            ActivateShield();
        }
    }

    public void TakeDamage(int damage)
    {
        if (shieldActive)
        {
            shieldActive = false;

            if (shieldSkillObject != null)
                shieldSkillObject.SetActive(false);

            Debug.Log("Shield blocked damage!");

            return;
        }

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

    public void UnlockShieldSkill()
    {
        if (hasShieldSkill)
            return;

        hasShieldSkill = true;

        Debug.Log("Unlocked Shield Skill!");
    }

    private void ActivateShield()
    {
        shieldActive = true;
        shieldReady = false;

        if (shieldSkillObject != null)
            shieldSkillObject.SetActive(true);

        Debug.Log("Shield Activated!");

        StartCoroutine(ShieldDuration());
        StartCoroutine(ShieldCooldown());
    }

    private IEnumerator ShieldCooldown()
    {
        yield return new WaitForSeconds(shieldCooldown);

        shieldReady = true;
    }

    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(shieldDuration);

        if (shieldActive)
        {
            shieldActive = false;

            if (shieldSkillObject != null)
                shieldSkillObject.SetActive(false);
        }
    }
}