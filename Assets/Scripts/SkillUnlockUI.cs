using System.Collections;
using TMPro;
using UnityEngine;

public class SkillUnlockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unlockText;

    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeTime = 1f;

    private Coroutine currentRoutine;

    private void Start()
    {
        unlockText.gameObject.SetActive(false);
    }

    public void ShowUnlockMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine =
            StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        unlockText.text = message;
        unlockText.gameObject.SetActive(true);

        Color color = unlockText.color;
        color.a = 1f;
        unlockText.color = color;

        yield return new WaitForSeconds(displayTime);

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(
                1f,
                0f,
                timer / fadeTime
            );

            unlockText.color = color;

            yield return null;
        }

        color.a = 0f;
        unlockText.color = color;

        unlockText.gameObject.SetActive(false);
    }
}