using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Tutorial3_Manager : MonoBehaviour
{
    [Header("【常駐標題】")]
    public GameObject titleImage;

    [Header("【System Block 系統解說框】")]
    public GameObject systemBlockPanel;
    public TextMeshProUGUI systemBlockText;
    public Button systemBlockNextBtn;

    [Header("【第 1 階段：四格漫畫排序】")]
    public GameObject comicDragDropGroup;
    public Transform[] cardAnchors;         // 4 個底座 (Anchor_1 ~ Anchor_4)
    public ComicDropSlot[] dropSlots;      // 4 個目標放置格 (Slot_1 ~ Slot_4)
    public ComicDraggableCard[] dragCards; // 4 張拖曳卡片 (Card_1 ~ Card_4)

    [Header("【第 2 階段：三選一情境選項卡】")]
    public GameObject scenarioQuizGroup;   
    public Button scenarioOptA_Btn;        // A. 各自購買新書 (錯)
    public Button scenarioOptB_Btn;        // B. 輪流使用同一本書 (對)
    public Button scenarioOptC_Btn;        // C. 買下一本三年後使用 (錯)

    [Header("【錯誤警告彈窗】")]
    public GameObject warningBoxPanel;
    public TextMeshProUGUI warningText;
    public Button warningCloseBtn;

    private int sysStep = 0;
    private bool isComicPhase = false;
    private bool isInitialized = false;

    private readonly string[] postComicDialogues = new string[]
    {
        "現在有以下三種情況，都有一本已經存在的書。",
        "如果三位學生的需求發生在不同時間，你認為哪一種最能體現『共享經濟中的資產再利用』？"
    };

    private void Awake()
    {
        Transform bottomDialogue = transform.Find("BottomDialoguePanel3");
        if (bottomDialogue != null) bottomDialogue.gameObject.SetActive(false);

        if (comicDragDropGroup != null) comicDragDropGroup.SetActive(false);
        if (scenarioQuizGroup != null) scenarioQuizGroup.SetActive(false);
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
        if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
    }

    void Start()
    {
        if (titleImage != null) titleImage.SetActive(true);
        if (scenarioQuizGroup != null) scenarioQuizGroup.SetActive(false);
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
        if (systemBlockPanel != null) systemBlockPanel.SetActive(false);

        if (dropSlots != null)
        {
            foreach (var slot in dropSlots)
            {
                if (slot != null) slot.managerTutorial3 = this;
            }
        }

        if (systemBlockNextBtn != null)
        {
            systemBlockNextBtn.onClick.RemoveAllListeners();
            systemBlockNextBtn.onClick.AddListener(OnClickSystemBlockNext);
        }

        if (warningCloseBtn != null)
        {
            warningCloseBtn.onClick.RemoveAllListeners();
            warningCloseBtn.onClick.AddListener(OnCloseWarningBox);
        }

        if (scenarioOptA_Btn != null)
        {
            scenarioOptA_Btn.onClick.RemoveAllListeners();
            scenarioOptA_Btn.onClick.AddListener(() => OnSelectScenario(false));
            scenarioOptA_Btn.interactable = true;
        }
        if (scenarioOptB_Btn != null)
        {
            scenarioOptB_Btn.onClick.RemoveAllListeners();
            scenarioOptB_Btn.onClick.AddListener(() => OnSelectScenario(true));
            scenarioOptB_Btn.interactable = true;
        }
        if (scenarioOptC_Btn != null)
        {
            scenarioOptC_Btn.onClick.RemoveAllListeners();
            scenarioOptC_Btn.onClick.AddListener(() => OnSelectScenario(false));
            scenarioOptC_Btn.interactable = true;
        }

        // 啟動開場打亂協程
        StartCoroutine(InitialShuffleRoutine());
    }

    // ⭐ 只要面板被啟用 (輪播切換到這頁) 就確保執行一次強制打亂
    private void OnEnable()
    {
        StartCoroutine(InitialShuffleRoutine());
    }

    private IEnumerator InitialShuffleRoutine()
    {
        // 等待一幀讓 RectTransform 錨點與坐標完全運算就緒
        yield return null;
        ShuffleCardsToAnchors();
    }

    public void StartTutorial3Phase()
    {
        if (isInitialized) return;
        isInitialized = true;

        isComicPhase = false;
        sysStep = 0;

        if (systemBlockPanel != null)
        {
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null)
                systemBlockText.text = "如果這本書之後又被其他人需要，你認為接下來會發生什麼？";
        }
    }

    public void OnClickSystemBlockNext()
    {
        if (!isComicPhase)
        {
            isComicPhase = true;
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            
            if (comicDragDropGroup != null) comicDragDropGroup.SetActive(true);
            ShuffleCardsToAnchors(); // 進入拖曳階段再次刷新打亂
            return;
        }

        sysStep++;
        if (sysStep < postComicDialogues.Length)
        {
            if (systemBlockText != null) systemBlockText.text = postComicDialogues[sysStep];
        }
        else
        {
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            if (scenarioQuizGroup != null) scenarioQuizGroup.SetActive(true);
        }
    }

    // ⭐ 強制打亂且「絕不出現正解順序」的洗牌演算法
    public void ShuffleCardsToAnchors()
    {
        if (dragCards == null || cardAnchors == null || dragCards.Length == 0 || cardAnchors.Length == 0) return;

        List<Transform> randomizedAnchors = new List<Transform>(cardAnchors);

        bool isSequential = true;
        int maxAttempts = 15;

        while (isSequential && maxAttempts > 0)
        {
            maxAttempts--;

            for (int i = 0; i < randomizedAnchors.Count; i++)
            {
                int rnd = Random.Range(i, randomizedAnchors.Count);
                Transform temp = randomizedAnchors[i];
                randomizedAnchors[i] = randomizedAnchors[rnd];
                randomizedAnchors[rnd] = temp;
            }

            isSequential = true;
            for (int i = 0; i < randomizedAnchors.Count; i++)
            {
                if (randomizedAnchors[i] != cardAnchors[i])
                {
                    isSequential = false;
                    break;
                }
            }
        }

        // ⭐ 關鍵修正：呼叫 SetNewAnchor，直接重設父物件與 AnchoredPosition
        for (int i = 0; i < dragCards.Length; i++)
        {
            if (dragCards[i] != null && i < randomizedAnchors.Count)
            {
                dragCards[i].SetNewAnchor(randomizedAnchors[i]);
            }
        }
    }

    public void CheckAllSlotsPlaced()
    {
        if (dropSlots == null || dropSlots.Length < 4) return;

        int filledCount = 0;
        for (int i = 0; i < dropSlots.Length; i++)
        {
            if (dropSlots[i].placedCard != null) filledCount++;
        }

        if (filledCount < 4) return;

        bool isAllCorrect = true;
        for (int i = 0; i < dropSlots.Length; i++)
        {
            if (dropSlots[i].placedCard.cardId != dropSlots[i].slotIndex)
            {
                isAllCorrect = false;
            }
        }

        if (isAllCorrect)
        {
            StartCoroutine(OnAllCardsPlacedCorrectly());
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null) warningText.text = "順序好像不太對歐，再試一次吧！";
            }
        }
    }

    public void OnCloseWarningBox()
    {
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);

        for (int i = 0; i < dropSlots.Length; i++)
        {
            if (dropSlots[i].placedCard != null)
            {
                dropSlots[i].placedCard.ReturnToInitialPos();
                dropSlots[i].placedCard = null;
            }
        }

        ShuffleCardsToAnchors(); // 答錯後重新隨機洗牌
    }

    private IEnumerator OnAllCardsPlacedCorrectly()
    {
        yield return new WaitForSeconds(0.4f);

        if (systemBlockPanel != null)
        {
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
            if (systemBlockText != null) systemBlockText.text = "看來你對共享經濟已經有初步了解囉！";
        }

        yield return new WaitForSeconds(1.5f);

        if (comicDragDropGroup != null) comicDragDropGroup.SetActive(false);

        sysStep = 0;
        if (systemBlockPanel != null)
        {
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = postComicDialogues[0];
        }
    }

    public void OnSelectScenario(bool isCorrect)
    {
        if (isCorrect)
        {
            if (scenarioOptA_Btn != null) scenarioOptA_Btn.interactable = false;
            if (scenarioOptB_Btn != null) scenarioOptB_Btn.interactable = false;
            if (scenarioOptC_Btn != null) scenarioOptC_Btn.interactable = false;

            if (systemBlockPanel != null)
            {
                systemBlockPanel.SetActive(true);
                systemBlockPanel.transform.SetAsLastSibling();
                if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
                if (systemBlockText != null) systemBlockText.text = "判斷正確！\n我們繼續往下看！";
            }

            if (TutorialCarouselManager.Instance != null)
            {
                TutorialCarouselManager.Instance.UnlockNextPage(3);
            }
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null) warningText.text = "再想想看吧";
            }
        }
    }
}