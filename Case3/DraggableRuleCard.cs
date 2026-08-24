using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableRuleCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("是否為正確條款 (true: 正確, false: 違規干擾)")]
    public bool isCorrectRule;

    [HideInInspector] public Vector2 defaultDrawerPos;   // 記錄一開始你在場景中擺放的固定位置
    [HideInInspector] public int currentOccupiedSlot = -1; // -1: 在抽屜, 0~3: 在左邊清單格子

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
        originalParent = transform.parent;
        defaultDrawerPos = rectTransform.anchoredPosition; // 自動記錄你在編輯器排好的初始位置
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling(); // 拖曳時置頂顯示
        if (currentOccupiedSlot != -1)
        {
            RuleMiniGameManager.Instance.ReleaseSlot(currentOccupiedSlot);
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
        RuleMiniGameManager.Instance.HandleCardDropped(this);
    }

    public void ResetToDrawer()
    {
        currentOccupiedSlot = -1;
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = defaultDrawerPos;
    }
}