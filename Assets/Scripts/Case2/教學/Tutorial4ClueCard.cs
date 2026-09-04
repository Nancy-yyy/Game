using UnityEngine;
using UnityEngine.EventSystems;

public class Tutorial4ClueCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("線索編號 (1~5)")]
    public int clueId = 1; 

    [HideInInspector] public Tutorial4_Manager manager;
    [HideInInspector] public Tutorial4DropSlot currentSlot;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;
    private bool isLocked = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        canvasGroup.blocksRaycasts = true;

        bool snapped = false;
        if (manager != null)
        {
            snapped = manager.TrySnapCardToNearestSlot(this);
        }

        if (!snapped)
        {
            ReturnToOriginal();
        }
    }

    public void LockPlaced()
    {
        isLocked = true;
        canvasGroup.blocksRaycasts = false;
    }

    public void ReturnToOriginal()
    {
        if (currentSlot != null)
        {
            currentSlot.currentCard = null;
            currentSlot = null;
        }

        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
}