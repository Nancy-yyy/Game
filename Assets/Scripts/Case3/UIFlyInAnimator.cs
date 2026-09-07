using System.Collections;
using UnityEngine;


public class UIFlyInAnimator : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 targetPos;          // 最終停靠的位置 (通常是 Vector2.zero)
    private Vector2 startPos;           // 起始位置 (畫面下方)

    [Header("動畫設定")]
    public float duration = 0.5f;       // 動畫持續時間（秒）

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            targetPos = rectTransform.anchoredPosition;
            // 計算畫面下方的起始 Y 座標（以 1080p 為例，往下位移 1200 像素）
            startPos = new Vector2(targetPos.x, targetPos.y - 1200f);
            
            // 一開始先把面板瞬間移到畫面下方，準備滑入
            rectTransform.anchoredPosition = startPos;
        }
    }

    void Start()
    {
        // 進入場景時自動觸發滑入動畫
        StartCoroutine(FlyInRoutine());
    }

    private IEnumerator FlyInRoutine()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 使用平滑插值（Mathf.SmoothStep）讓滑入動作有緩入緩出的質感
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothT);
            
            yield return null;
        }

        // 確保動畫結束時精準停在目標位置
        rectTransform.anchoredPosition = targetPos;
    }
}