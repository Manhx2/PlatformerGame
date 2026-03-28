using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterPanelController : MonoBehaviour
{
    public GameObject characterPanel;

    private bool isOpen = false;

    public void OnTogglePanel(InputValue value)
    {
        if (value.isPressed && !isOpen)
        {
            OpenPanel();
        }
    }

    public void OnClosePanel(InputValue value)
    {
        if (value.isPressed && isOpen)
        {
            ClosePanel();
        }
    }

    void OpenPanel()
    {
        isOpen = true;
        characterPanel.SetActive(true);

        Time.timeScale = 0f; // pause
    }

    void ClosePanel()
    {
        isOpen = false;
        characterPanel.SetActive(false);

        Time.timeScale = 1f; // resume
    }
}