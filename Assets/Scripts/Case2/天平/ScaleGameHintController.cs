using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleGameHintController : MonoBehaviour
{
    [Header("Hint UI 設定")]
    public RectTransform hintPanelRect;
    public Button screenDismissButton;
    public float slideDuration = 0.35f;

    void Start()
    {
        if (screenDismissButton != null)
        {
            screenDismissButton.onClick.RemoveAllListeners();
            screenDismissButton.onClick.AddListener(DismissHint);
        }

        StartCoroutine(SlideInRoutine());
    }

    // ⭐ 從 (0, 1200) 滑入到 (0, 400)
    private IEnumerator SlideInRoutine()
    {
        if (hintPanelRect == null) yield break;

        hintPanelRect.gameObject.SetActive(true);
        if (screenDismissButton != null) screenDismissButton.gameObject.SetActive(true);

        Vector2 startPos = new Vector2(0f, 1200f);
        Vector2 targetPos = new Vector2(0f, 400f);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            hintPanelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / slideDuration);
            yield return null;
        }
        hintPanelRect.anchoredPosition = targetPos;
    }

    public void DismissHint()
    {
        if (screenDismissButton != null) screenDismissButton.gameObject.SetActive(false);
        StartCoroutine(SlideOutRoutine());
    }

    // ⭐ 從 (0, 400) 滑回 (0, 1200)
    private IEnumerator SlideOutRoutine()
    {
        if (hintPanelRect == null) yield break;

        Vector2 startPos = hintPanelRect.anchoredPosition;
        Vector2 targetPos = new Vector2(0f, 1200f);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            hintPanelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / slideDuration);
            yield return null;
        }
        hintPanelRect.anchoredPosition = targetPos;
        hintPanelRect.gameObject.SetActive(false);
    }
}