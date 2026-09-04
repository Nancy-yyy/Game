using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustMiniGameManager : MonoBehaviour
{
    public static TrustMiniGameManager Instance;

    [Header("=== 遊戲主介面 ===")]
    public GameObject unlockGamePanel;         // 解鎖.jpg 的主面板
    public GameObject unlockSuccessPanel;      // 解鎖成功.jpg 的面板
    public Button btnResetGame;                // 上一步按鈕（重置所有卡片）
    public Button btnEnterClassroom;           // 解鎖成功上的【進入教室】按鈕

    [Header("=== 錯誤提示彈窗 ===")]
    public GameObject wrongPromptBackdrop;     // 答錯時的遮罩提示
    public Button btnCloseWrongPrompt;         // 點擊空白處關閉提示

    [Header("=== 4 個插槽判定區域 (RectTransform) ===")]
    public RectTransform slot_Identity;        // 疑慮1: 槽位 0 (正確應放身分驗證)
    public RectTransform slot_Deposit;         // 疑慮2左: 槽位 1 (正確應放押金)
    public RectTransform slot_Damage;          // 疑慮2右: 槽位 2 (正確應放損壞)
    public RectTransform slot_Prepayment;      // 疑慮3: 槽位 3 (正確應放預付款)

    [Header("=== 4 張可拖曳卡牌 ===")]
    public DraggableTrustCard[] allCards;

    // 記錄 4 個槽位目前被哪張卡牌佔用（存放卡牌的 cardType，-1 代表該槽位是空的）
    private int[] slotsOccupied = new int[4] { -1, -1, -1, -1 };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (unlockGamePanel != null) unlockGamePanel.SetActive(false);
        if (unlockSuccessPanel != null) unlockSuccessPanel.SetActive(false);
        if (wrongPromptBackdrop != null) wrongPromptBackdrop.SetActive(false);

        if (btnResetGame != null) btnResetGame.onClick.AddListener(ResetAllCards);
        if (btnCloseWrongPrompt != null) btnCloseWrongPrompt.onClick.AddListener(() => wrongPromptBackdrop.SetActive(false));
        if (btnEnterClassroom != null) btnEnterClassroom.onClick.AddListener(OnEnterClassroomClicked);
    }

    public void StartMiniGame()
    {
        if (unlockGamePanel != null) unlockGamePanel.SetActive(true);
        if (unlockSuccessPanel != null) unlockSuccessPanel.SetActive(false);
        ResetAllCards();
    }

    // 釋放某個插槽
    public void ReleaseSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 4)
        {
            slotsOccupied[slotIndex] = -1;
        }
    }

    // 處理玩家放開卡片時的邏輯：自由吸附到重疊的卡槽（不論對錯）
    public void HandleCardDropped(DraggableTrustCard card)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        RectTransform[] targetSlots = new RectTransform[4] { slot_Identity, slot_Deposit, slot_Damage, slot_Prepayment };

        int closestSlot = -1;
        float minDistance = float.MaxValue;

        // 檢查卡片是否放在 4 個插槽中的任何一個
        for (int i = 0; i < targetSlots.Length; i++)
        {
            if (targetSlots[i] != null && IsOverlapping(cardRect, targetSlots[i]))
            {
                float dist = Vector2.Distance(cardRect.position, targetSlots[i].position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestSlot = i;
                }
            }
        }

        if (closestSlot != -1)
        {
            // 如果該槽位已經有其他卡片，將原有的卡片推回底座
            foreach (var otherCard in allCards)
            {
                if (otherCard != card && otherCard.currentOccupiedSlot == closestSlot)
                {
                    otherCard.ResetToOriginalPos();
                }
            }

            // 吸附到該槽位中心
            cardRect.position = targetSlots[closestSlot].position;
            card.currentOccupiedSlot = closestSlot;
            slotsOccupied[closestSlot] = card.cardType;

            // 檢查 4 個槽位是否全部放滿了
            CheckAllSlotsFilled();
        }
        else
        {
            // 沒放到任何插槽 ➔ 留在放開的位置，或彈回原位（這裡讓它彈回原位更整齊）
            card.ResetToOriginalPos();
        }
    }

    private void CheckAllSlotsFilled()
    {
        // 檢查是否 4 個槽位都有卡片
        for (int i = 0; i < 4; i++)
        {
            if (slotsOccupied[i] == -1) return; // 還沒放滿，不做結算，讓玩家繼續放
        }

        // 放滿 4 張卡片了，進行答案核對
        bool isCorrect = false;

        // 正確對應：
        // 槽位 0 (疑慮1) ➔ 身分驗證 (cardType 0)
        // 槽位 3 (疑慮3) ➔ 預付款 (cardType 3)
        // 槽位 1 與 2 (疑慮2) ➔ 押金 (1) 與 損壞 (2) 互換也算對
        bool identityCorrect = (slotsOccupied[0] == 0);
        bool prepaymentCorrect = (slotsOccupied[3] == 3);
        bool middleCorrect = (slotsOccupied[1] == 1 && slotsOccupied[2] == 2) || (slotsOccupied[1] == 2 && slotsOccupied[2] == 1);

        if (identityCorrect && prepaymentCorrect && middleCorrect)
        {
            isCorrect = true;
        }

        if (isCorrect)
        {
            // 答對 ➔ 進入解鎖成功
            OnGameComplete();
        }
        else
        {
            // 答錯 ➔ 跳出錯誤提示框，不彈回卡片，讓玩家點擊空白處後直接在畫面上手動調整
            if (wrongPromptBackdrop != null)
            {
                wrongPromptBackdrop.transform.SetAsLastSibling();
                wrongPromptBackdrop.SetActive(true);
            }
        }
    }

    private bool IsOverlapping(RectTransform rectA, RectTransform rectB)
    {
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);

        Rect rA = new Rect(cornersA[0].x, cornersA[0].y, cornersA[2].x - cornersA[0].x, cornersA[2].y - cornersA[0].y);
        Rect rB = new Rect(cornersB[0].x, cornersB[0].y, cornersB[2].x - cornersB[0].x, cornersB[2].y - cornersB[0].y);

        return rA.Overlaps(rB);
    }

    private void OnGameComplete()
    {
        if (unlockGamePanel != null) unlockGamePanel.SetActive(false);
        if (unlockSuccessPanel != null) unlockSuccessPanel.SetActive(true);
    }

    public void ResetAllCards()
    {
        for (int i = 0; i < 4; i++) slotsOccupied[i] = -1;
        foreach (var card in allCards)
        {
            if (card != null) card.ResetToOriginalPos();
        }
    }

    private void OnEnterClassroomClicked()
    {
        if (unlockSuccessPanel != null) unlockSuccessPanel.SetActive(false);
        if (PeopleSelectionManager.Instance != null)
        {
            PeopleSelectionManager.Instance.StartPostGameStory();
        }
    }
}