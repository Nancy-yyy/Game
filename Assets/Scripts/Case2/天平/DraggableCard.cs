using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string cardType; // Inspector 填入 A, B, C, D
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;
    private bool isPlacedOnScale = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        originalPos = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isPlacedOnScale = false;
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
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

        GameObject rightPan = GameObject.Find("天秤右_0");
        if (rightPan == null) rightPan = GameObject.Find("天右_0");

        if (rightPan != null)
        {
            Vector3 panScreenPos = Camera.main.WorldToScreenPoint(rightPan.transform.position);
            if (Vector2.Distance(eventData.position, panScreenPos) < 180f)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(), 
                    panScreenPos, 
                    canvas.worldCamera, 
                    out localPoint
                );
                rectTransform.anchoredPosition = localPoint;
                isPlacedOnScale = true;

                ScaleController scale = FindObjectOfType<ScaleController>();
                if (scale != null) scale.TriggerReaction(cardType);
                return;
            }
        }

        rectTransform.anchoredPosition = originalPos;
    }

    public void ResetToOriginalPos()
    {
        isPlacedOnScale = false;
        rectTransform.anchoredPosition = originalPos;
    }
}