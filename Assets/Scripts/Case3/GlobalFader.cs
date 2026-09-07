using System;
using System.Collections;
using UnityEngine;

public class GlobalFader : MonoBehaviour
{
    public static GlobalFader Instance;

    [Header("轉場黑幕 CanvasGroup")]
    public CanvasGroup curtainCanvasGroup;

    [Header("預設淡入淡出秒數")]
    public float defaultFadeDuration = 0.3f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (curtainCanvasGroup != null)
        {
            curtainCanvasGroup.alpha = 0f;
            curtainCanvasGroup.blocksRaycasts = false;
        }
    }

    public void FadeTransition(Action onBlackout, float duration = -1f)
    {
        float dur = duration > 0 ? duration : defaultFadeDuration;
        StartCoroutine(FadeTransitionRoutine(onBlackout, dur));
    }

    private IEnumerator FadeTransitionRoutine(Action onBlackout, float duration)
    {
        if (curtainCanvasGroup == null)
        {
            onBlackout?.Invoke();
            yield break;
        }

        curtainCanvasGroup.blocksRaycasts = true;

        // 淡入全黑
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            curtainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        curtainCanvasGroup.alpha = 1f;

        // 執行畫面切換
        onBlackout?.Invoke();

        yield return new WaitForSeconds(0.05f);

        // 淡出回透明
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            curtainCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }
        curtainCanvasGroup.alpha = 0f;
        curtainCanvasGroup.blocksRaycasts = false;
    }
}