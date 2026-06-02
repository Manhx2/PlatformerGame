using TMPro;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("Level")]
    public int currentLevel = 1;

    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Growth")]
    public float expRequirementMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;

        expToNextLevel =
            Mathf.RoundToInt(expToNextLevel * expRequirementMultiplier);

        playerStats.LevelUpStats();

        Debug.Log("Level Up! Lv." + currentLevel);
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = currentLevel.ToString();

        if (expText != null)
            expText.text = currentExp + "/" + expToNextLevel;
    }
}