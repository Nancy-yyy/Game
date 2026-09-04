using UnityEngine;
using UnityEngine.EventSystems;

public class ComicDropSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex = 0; // 0, 1, 2, 3 代表由左至右第 1 到第 4 個框
    [HideInInspector] public Tutorial3_Manager managerTutorial3; // 專門對接案例三的管理器
    [HideInInspector] public ComicDraggableCard placedCard = null;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        ComicDraggableCard card = droppedObj.GetComponent<ComicDraggableCard>();
        if (card != null)
        {
            // 若格子內已有卡片，彈回其專屬底座
            if (placedCard != null && placedCard != card)
            {
                placedCard.ReturnToInitialPos();
            }

            // 吸附新卡片
            card.PlaceIntoSlot(this);

            // 通知 Tutorial3 檢查是否 4 格皆放滿
            if (managerTutorial3 != null)
            {
                managerTutorial3.CheckAllSlotsPlaced();
            }
        }
    }
}