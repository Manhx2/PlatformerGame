using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int maxHP = 500;
    public int currentHP;

    public int atk = 100;
    public int def = 10;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - def, 1);
        currentHP -= finalDamage;

        Debug.Log(name + " took " + finalDamage + " damage");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(name + " died");
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return atk;
    }
}