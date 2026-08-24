using UnityEngine;
using UnityEngine.EventSystems;

public class MatchingCard : MonoBehaviour
{
    [SerializeField] private RectTransform cardVisual;
    [SerializeField] private float hoverScale = 1.15f;

    [SerializeField] private string cardID;
    [SerializeField] private string cardDisplayText;

    [SerializeField] private Vector2 hoverOffset = new Vector2(0f, 60f);   // X=水平方向, Y=垂直方向

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private int originalSiblingIndex;

    private void Start()
    {
        originalScale = cardVisual.localScale;
        originalPosition = cardVisual.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void OnHoverEnter()
    {
        // 放大
        cardVisual.localScale = originalScale * hoverScale;
        
        // 往上浮
        cardVisual.anchoredPosition = originalPosition + hoverOffset;

        // 顯示在其他卡片最上層
        transform.SetAsLastSibling();
    }

    public void OnHoverExit()
    {
        // 回原大小
        cardVisual.localScale = originalScale;

        // 回原位置
        cardVisual.anchoredPosition = originalPosition;

        // 回原本疊牌順序
        transform.SetSiblingIndex(originalSiblingIndex);
    }

    public void SelectCard()
    {
        Case1MatchingManager manager =
            FindAnyObjectByType<Case1MatchingManager>();

        manager.SelectCard(cardID, cardDisplayText, gameObject);
    }

    public void ResetCardVisual()
    {
        cardVisual.localScale = originalScale;
        cardVisual.anchoredPosition = originalPosition;
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}