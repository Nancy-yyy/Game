using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableTrustCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("卡片類型 (0: 身分驗證, 1: 押金, 2: 損壞, 3: 預付款)")]
    public int cardType;

    [HideInInspector] public Vector2 originalAnchoredPosition;
    [HideInInspector] public int currentOccupiedSlot = -1; // -1 表示在底層木盒，0~3 代表被放在哪個插槽

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling(); // 拖曳時置頂
        // 開始拖曳時，如果原本在某個插槽，先釋放該插槽
        if (currentOccupiedSlot != -1)
        {
            TrustMiniGameManager.Instance.ReleaseSlot(currentOccupiedSlot);
            currentOccupiedSlot = -1;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // 由管理器判定自由吸附到最靠近的插槽，或留在原位/回到底座
        TrustMiniGameManager.Instance.HandleCardDropped(this);
    }

    public void ResetToOriginalPos()
    {
        currentOccupiedSlot = -1;
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}