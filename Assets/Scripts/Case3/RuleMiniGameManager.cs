using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; // 用於控制門禁卡動畫影片

public class RuleMiniGameManager : MonoBehaviour
{
    public static RuleMiniGameManager Instance;

    [Header("=== 介面面板 ===")]
    public GameObject ruleGamePanel;         // 規範清單.jpg 面板
    public GameObject ruleSuccessPanel;      // 獲得電子門禁.jpg 面板
    public Button btnResetGame;              // 左側上一步按鈕
    public Button btnStartUsingSpace;        // 開始使用空間按鈕

    [Header("=== 門禁卡影片播放器 ===")]
    public VideoPlayer cardVideoPlayer;      // 播放門禁卡 MP4 的 VideoPlayer

    [Header("=== 錯誤提示 (共用) ===")]
    public GameObject wrongPromptBackdrop;
    public Button btnCloseWrongPrompt;

    [Header("=== 左側 4 個清單插槽 (由上到下) ===")]
    public RectTransform[] listSlots = new RectTransform[4];

    [Header("=== 7 張可拖曳規範條款 ===")]
    public DraggableRuleCard[] allRuleCards = new DraggableRuleCard[7];

    private DraggableRuleCard[] slotsOccupied = new DraggableRuleCard[4] { null, null, null, null };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (ruleGamePanel != null) ruleGamePanel.SetActive(false);
        if (ruleSuccessPanel != null) ruleSuccessPanel.SetActive(false);
        if (btnResetGame != null) btnResetGame.onClick.AddListener(ResetAllCards);
        if (btnStartUsingSpace != null) btnStartUsingSpace.onClick.AddListener(OnStartUsingSpaceClicked);
        if (btnCloseWrongPrompt != null) btnCloseWrongPrompt.onClick.AddListener(() => wrongPromptBackdrop.SetActive(false));
    }

    public void StartMiniGame()
    {
        if (ruleGamePanel != null) ruleGamePanel.SetActive(true);
        if (ruleSuccessPanel != null) ruleSuccessPanel.SetActive(false);
        ResetAllCards(); // 保持你在編輯器擺放好的固定順序
    }

    public void ReleaseSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 4)
        {
            slotsOccupied[slotIndex] = null;
        }
    }

    public void HandleCardDropped(DraggableRuleCard card)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        int closestSlot = -1;
        float minDistance = float.MaxValue;

        // 檢查是否拖到左側 4 個清單格
        for (int i = 0; i < listSlots.Length; i++)
        {
            if (listSlots[i] != null && IsOverlapping(cardRect, listSlots[i]))
            {
                float dist = Vector2.Distance(cardRect.position, listSlots[i].position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestSlot = i;
                }
            }
        }

        if (closestSlot != -1)
        {
            // 如果該格已有卡片，將原本的卡片踢回抽屜原位
            if (slotsOccupied[closestSlot] != null && slotsOccupied[closestSlot] != card)
            {
                slotsOccupied[closestSlot].ResetToDrawer();
            }

            cardRect.position = listSlots[closestSlot].position;
            card.currentOccupiedSlot = closestSlot;
            slotsOccupied[closestSlot] = card;

            CheckAllSlotsFilled();
        }
        else
        {
            // 沒放進清單格 ➔ 回到自己的固定抽屜位置
            card.ResetToDrawer();
        }
    }

    private void CheckAllSlotsFilled()
    {
        for (int i = 0; i < 4; i++)
        {
            if (slotsOccupied[i] == null) return;
        }

        // 檢查放進去的 4 張是否全為正確條款
        bool allCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (!slotsOccupied[i].isCorrectRule)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            OnGameSuccess();
        }
        else
        {
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

    private void OnGameSuccess()
    {
        if (ruleGamePanel != null) ruleGamePanel.SetActive(false);
        
        if (ruleSuccessPanel != null)
        {
            ruleSuccessPanel.SetActive(true);
        }

        // 過關時自動開始播放門禁卡影片與音效
        if (cardVideoPlayer != null)
        {
            cardVideoPlayer.Play();
        }
    }

    public void ResetAllCards()
    {
        for (int i = 0; i < 4; i++) slotsOccupied[i] = null;
        foreach (var card in allRuleCards)
        {
            if (card != null) card.ResetToDrawer();
        }
    }

    private void OnStartUsingSpaceClicked()
    {
        Debug.Log("【成功點擊】開始使用空間，準備回到裊裊對話！");

        // 1. 停止影片與音效
        if (cardVideoPlayer != null)
        {
            cardVideoPlayer.Stop();
        }

        // 2. 關閉獲得門禁卡面板
        if (ruleSuccessPanel != null)
        {
            ruleSuccessPanel.SetActive(false);
        }

        // 3. 呼叫裊裊與玩家的後續對話
        if (PeopleSelectionManager.Instance != null)
        {
            Debug.Log("【流程切換】觸發 PeopleSelectionManager.StartPostGameStorySchemeC()");
            PeopleSelectionManager.Instance.StartPostGameStorySchemeC();
        }
        else
        {
            Debug.LogError("【報錯】找不到 PeopleSelectionManager.Instance！請確認該管理器已在場景中啟用。");
        }
    }
}