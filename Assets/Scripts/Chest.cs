using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    [Header("Reward")]
    public int minExp = 20;
    public int maxExp = 100;

    [Header("UI")]
    [SerializeField] private GameObject openPanel;

    [Header("Chest Models")]
    [SerializeField] private GameObject closedChestModel;
    [SerializeField] private GameObject openedChestModel;

    private bool playerInRange;
    private bool opened;

    private PlayerLevel playerLevel;

    private void Start()
    {
        if (closedChestModel != null)
            closedChestModel.SetActive(true);

        if (openedChestModel != null)
            openedChestModel.SetActive(false);

        if (openPanel != null)
            openPanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange &&
            !opened &&
            Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        opened = true;

        int expReward = Random.Range(minExp, maxExp + 1);

        if (playerLevel != null)
        {
            playerLevel.AddExp(expReward);
        }

        Debug.Log($"Opened Chest! +{expReward} EXP");

        if (closedChestModel != null)
            closedChestModel.SetActive(false);

        if (openedChestModel != null)
            openedChestModel.SetActive(true);

        if (openPanel != null)
            openPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            playerLevel = other.GetComponent<PlayerLevel>();

            if (openPanel != null)
                openPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (openPanel != null)
                openPanel.SetActive(false);
        }
    }
}