using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComicDraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("這張卡片代表的正確順序 (0=閒置, 1=取得暫時使用權, 2=產生需求, 3=資產再次被使用)")]
    public int cardId = 0;

    [HideInInspector] public Transform initialAnchor; // 專屬的底座父物件
    [HideInInspector] public ComicDropSlot currentSlot = null;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 pointerOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (initialAnchor == null)
        {
            initialAnchor = transform.parent;
        }
    }

    // ⭐ 關鍵方法：由 Manager 換底座時呼叫，徹底換綁父物件與座標
    public void SetNewAnchor(Transform newAnchor)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        initialAnchor = newAnchor;
        transform.SetParent(newAnchor);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot != null)
        {
            currentSlot.placedCard = null;
            currentSlot = null;
        }

        transform.SetParent(canvas.transform); // 移至最上層
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            rectTransform.localPosition = localPoint - pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // 若沒有成功放進任何 Slot，彈回目前綁定的隨機底座
        if (currentSlot == null)
        {
            ReturnToInitialPos();
        }
    }

    public void PlaceIntoSlot(ComicDropSlot slot)
    {
        currentSlot = slot;
        slot.placedCard = this;
        transform.SetParent(slot.transform);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localPosition = Vector3.zero;
    }

    public void ReturnToInitialPos()
    {
        currentSlot = null;
        if (initialAnchor != null)
        {
            transform.SetParent(initialAnchor);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
    }
}