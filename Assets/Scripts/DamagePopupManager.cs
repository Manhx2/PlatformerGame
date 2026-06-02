using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    public GameObject damagePopupPrefab;
    public Canvas canvas;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 worldPos, int damage)
    {
        GameObject popup =
            Instantiate(damagePopupPrefab, canvas.transform);

        Vector3 screenPos =
            Camera.main.WorldToScreenPoint(worldPos);

        popup.transform.position = screenPos;

        popup.GetComponent<DamagePopup>()
             .Setup(damage);
    }
}