using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text targetText;

    [SerializeField]
    private float fadeDuration = 0.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float minAlpha = 0.2f;

    private void Start()
    {
        StartCoroutine(FadeLoop());
    }

    private IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return Fade(1f, minAlpha);
            yield return Fade(minAlpha, 1f);
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;

        Color color = targetText.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(
                from,
                to,
                time / fadeDuration
            );

            color.a = alpha;
            targetText.color = color;

            yield return null;
        }

        color.a = to;
        targetText.color = color;
    }
}