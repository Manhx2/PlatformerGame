using System.Collections;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeIntensity = 0.08f;

    private Color[] originalColors;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        if (renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    public void PlayDamageEffect()
    {
        StopAllCoroutines();
        StartCoroutine(DamageEffectCoroutine());
    }

    private IEnumerator DamageEffectCoroutine()
    {
        Vector3 startPos = transform.localPosition;

        // Flash
        foreach (Renderer r in renderers)
        {
            r.material.color = flashColor;
        }

        float timer = 0f;

        // Shake
        while (timer < shakeDuration)
        {
            transform.localPosition =
                startPos +
                Random.insideUnitSphere * shakeIntensity;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPos;

        yield return new WaitForSeconds(
            Mathf.Max(0, flashDuration - shakeDuration)
        );

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
}