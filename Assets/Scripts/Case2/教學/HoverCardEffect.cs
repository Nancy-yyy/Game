using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverCardEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("縮放設定")]
    public float hoverScale = 1.08f;      // 懸停放大倍率
    public float transitionSpeed = 0.15f; // 縮放過渡時間 (秒)

    [Header("引號圖示 (滑鼠移入時顯示)")]
    public GameObject quoteTopLeft;       // 拖入 左上引號物件
    public GameObject quoteBottomRight;   // 拖入 右下引號物件

    private Vector3 originalScale;
    private Coroutine scaleRoutine;
    public bool isInteractable = true;    // 是否允許懸停互動

    void Awake()
    {
        originalScale = transform.localScale;
        SetQuotesActive(false); // 遊戲剛載入時強制隱藏引號
    }

    void OnEnable()
    {
        transform.localScale = originalScale;
        SetQuotesActive(false); // 物件被啟用時確保引號是隱藏的
    }

    // 滑鼠移入：放大並顯示引號
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;

        SetQuotesActive(true);

        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleTo(originalScale * hoverScale));
    }

    // 滑鼠移出：還原大小並隱藏引號
    public void OnPointerExit(PointerEventData eventData)
    {
        SetQuotesActive(false);

        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleTo(originalScale));
    }

    private void SetQuotesActive(bool state)
    {
        if (quoteTopLeft != null) quoteTopLeft.SetActive(state);
        if (quoteBottomRight != null) quoteBottomRight.SetActive(state);
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        float time = 0;
        Vector3 startScale = transform.localScale;

        while (time < transitionSpeed)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / transitionSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}