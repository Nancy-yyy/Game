using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string cardType; // Inspector 填入 A, B, C, D
    
    [Header("吸附高度微調 (數值越小越往下，建議 -80 ~ -100)")]
    public float yOffset = -90f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPos;
    private Transform originalParent;
    
    private Transform targetPan;
    private bool isPlaced = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalParent = transform.parent;
    }

    void Start()
    {
        // 在 Start 階段確實記錄右側選單的原始 AnchoredPosition
        originalPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // 只有被放在天平右托盤時，每幀貼齊圓盤位置
        if (isPlaced && targetPan != null && canvas != null)
        {
            Vector3 panScreenPos = Camera.main.WorldToScreenPoint(targetPan.position);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                panScreenPos,
                canvas.worldCamera,
                out localPoint
            );
            rectTransform.anchoredPosition = localPoint + new Vector2(0, yOffset);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isPlaced = false;
        targetPan = null;
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

        GameObject rightPanObj = GameObject.Find("天秤右_0");
        if (rightPanObj == null) rightPanObj = GameObject.Find("天右_0");

        if (rightPanObj != null)
        {
            Vector3 panScreenPos = Camera.main.WorldToScreenPoint(rightPanObj.transform.position);
            // 距離右托盤 200 像素以內判定為成功放入
            if (Vector2.Distance(eventData.position, panScreenPos) < 200f)
            {
                targetPan = rightPanObj.transform;
                isPlaced = true;

                ScaleController scale = FindObjectOfType<ScaleController>();
                if (scale != null) scale.TriggerReaction(cardType);
                return;
            }
        }

        // 未放準自動彈回
        ResetToOriginalPos();
    }

    // 強制卡片解除吸附並飛回右側卡片列
    public void ResetToOriginalPos()
    {
        isPlaced = false;
        targetPan = null;
        if (canvasGroup != null) canvasGroup.blocksRaycasts = true;
        
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPos;
    }
}