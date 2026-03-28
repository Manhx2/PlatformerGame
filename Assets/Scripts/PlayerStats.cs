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


    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - def, 1);
        currentHP -= finalDamage;

        Debug.Log("Player took " + finalDamage + " damage");

        if (currentHP <= 0)
        {
            Die();
        }
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
}