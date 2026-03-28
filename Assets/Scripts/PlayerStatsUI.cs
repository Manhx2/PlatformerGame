using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerStats playerStats;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI critRateText;
    public TextMeshProUGUI critDmgText;
    public TextMeshProUGUI effectResText;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        hpText.text = playerStats.currentHP + "/" + playerStats.maxHP;
        atkText.text = (playerStats.atk).ToString("F0");
        defText.text = (playerStats.def).ToString("F0");

        critRateText.text = (playerStats.critRate * 100f).ToString("F1") + "%";
        critDmgText.text = (playerStats.critDamage * 100f).ToString("F1") + "%";
        effectResText.text = (playerStats.effectResistance * 100f).ToString("F1") + "%";
    }
}