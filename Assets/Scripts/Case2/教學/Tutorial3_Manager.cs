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

    [Header("【音效設定】")]
    public AudioSource audioSource;
    public AudioClip correctSFX;    
    public AudioClip wrongSFX;      

    private int sysStep = 0;
    private bool isComicPhase = false;
    private bool isInitialized = false;
    private bool isWaitingComicCorrectNext = false;

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

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
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
            scenarioOptA_Btn.onClick.AddListener(() => OnSelectScenario("A", false));
            scenarioOptA_Btn.interactable = true;
        }
        if (scenarioOptB_Btn != null)
        {
            scenarioOptB_Btn.onClick.RemoveAllListeners();
            scenarioOptB_Btn.onClick.AddListener(() => OnSelectScenario("B", true));
            scenarioOptB_Btn.interactable = true;
        }
        if (scenarioOptC_Btn != null)
        {
            scenarioOptC_Btn.onClick.RemoveAllListeners();
            scenarioOptC_Btn.onClick.AddListener(() => OnSelectScenario("C", false));
            scenarioOptC_Btn.interactable = true;
        }

        StartCoroutine(InitialShuffleRoutine());
    }

    private void OnEnable()
    {
        StartCoroutine(InitialShuffleRoutine());
    }

    private IEnumerator InitialShuffleRoutine()
    {
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
        // ======================================
        // 手動點擊控制：漫畫答對提示 → 下一段系統訊息
        // ======================================
        if (isWaitingComicCorrectNext)
        {
            isWaitingComicCorrectNext = false;

            if (comicDragDropGroup != null)
                comicDragDropGroup.SetActive(false);

            sysStep = 0;

            if (systemBlockText != null)
                systemBlockText.text = postComicDialogues[0];

            return;
        }
        // ======================================
        
        if (!isComicPhase)
        {
            isComicPhase = true;
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            
            if (comicDragDropGroup != null) comicDragDropGroup.SetActive(true);
            ShuffleCardsToAnchors();

            // 研究紀錄：正式進入四格漫畫排序題
            GameData.StartTask(GameData.TaskIds.C2_REUSE_ORDER);

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

            // 研究紀錄：正式進入資產再利用情境題
            GameData.StartTask(GameData.TaskIds.C2_REUSE_CASE);
        }
    }

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

        for (int i = 0; i < dragCards.Length; i++)
        {
            if (dragCards[i] != null && i < randomizedAnchors.Count)
            {
                dragCards[i].SetNewAnchor(randomizedAnchors[i]);
            }
        }
    }

    private string GetCurrentComicOrderAnswer()
    {
        List<string> order = new List<string>();

        for (int i = 0; i < dropSlots.Length; i++)
        {
            if (dropSlots[i] != null && dropSlots[i].placedCard != null)
            {
                order.Add(dropSlots[i].placedCard.cardId.ToString());
            }
            else
            {
                order.Add("");
            }
        }

        return string.Join(">", order);
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

        string submittedOrder = GetCurrentComicOrderAnswer();
        GameData.RecordAnswer(submittedOrder, isAllCorrect);

        if (isAllCorrect)
        {
            PlaySFX(correctSFX);
            StartCoroutine(OnAllCardsPlacedCorrectly());
        }
        else
        {
            PlaySFX(wrongSFX);

            // ⭐ 累計四格漫畫排序錯誤次數
            GameData.Case2_Tutorial3_Errors++;

            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();

                // ⭐【此為 Tutorial 3A 四格漫畫排序錯誤 - 高低資訊回饋內容】
                if (GameData.IsHighInfo)
                {
                    if (warningText != null)
                        warningText.text = "順序不太對喔！請注意：必須先存在『閒置資產』與『新需求出現』，才能進入『媒合取得使用權』，最後達成『資產再次被使用』。";
                }
                else
                {
                    if (warningText != null)
                        warningText.text = "順序好像不太對歐，再試一次吧！";
                }

                GameData.RecordFeedbackShown("C2_REUSE_ORDER_WRONG");
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

        ShuffleCardsToAnchors();
    }

    private IEnumerator OnAllCardsPlacedCorrectly()
    {
        yield return new WaitForSeconds(0.4f);

        if (systemBlockPanel != null)
        {
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;

            // ⭐【此為 Tutorial 3A 漫畫排序完成 - 高低資訊回饋內容】
            if (GameData.IsHighInfo)
            {
                if (systemBlockText != null)
                    systemBlockText.text = "沒錯就是這樣！閒置資源透過媒合重新流動，讓不需要買斷的人也能在對的時間滿足需求。";
            }
            else
            {
                if (systemBlockText != null)
                    systemBlockText.text = "看來你對共享經濟已經有初步了解囉！";
            }

            GameData.RecordFeedbackShown("C2_REUSE_ORDER_CORRECT");
        }

        GameData.CompleteTask();

        isWaitingComicCorrectNext = true;
        if (systemBlockNextBtn != null)
            systemBlockNextBtn.interactable = true;
    }

    public void OnSelectScenario(string answer, bool isCorrect)
    {
        GameData.RecordAnswer(answer, isCorrect);

        if (isCorrect)
        {
            PlaySFX(correctSFX);

            if (scenarioOptA_Btn != null) scenarioOptA_Btn.interactable = false;
            if (scenarioOptB_Btn != null) scenarioOptB_Btn.interactable = false;
            if (scenarioOptC_Btn != null) scenarioOptC_Btn.interactable = false;

            if (systemBlockPanel != null)
            {
                systemBlockPanel.SetActive(true);
                systemBlockPanel.transform.SetAsLastSibling();
                if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;

                // ⭐【此為 Tutorial 3B 情境選擇答對 - 高低資訊回饋內容】
                if (GameData.IsHighInfo)
                {
                    if (systemBlockText != null)
                        systemBlockText.text = "沒錯！B 選項讓同一本已存在的實體書在不同時間被多位同學重複使用，最能將閒置容量發揮到極致。\n我們繼續往下看！";
                }
                else
                {
                    if (systemBlockText != null)
                        systemBlockText.text = "判斷正確！\n點擊右邊的箭頭我們繼續往下看！";
                }

                GameData.RecordFeedbackShown("C2_REUSE_CASE_CORRECT");
            }

            GameData.CompleteTask();

            if (TutorialCarouselManager.Instance != null)
            {
                TutorialCarouselManager.Instance.UnlockNextPage(3);
            }
            // ======================================
            // AI 防暴雷：玩家已完成閒置資產再利用教學
            AIProgress.SetStoryStep("case2_idle_reuse_completed");
            // ======================================
        }
        else
        {
            PlaySFX(wrongSFX);

            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();

                // ⭐【此為 Tutorial 3B 情境選擇錯誤 - 高低資訊回饋內容】
                if (GameData.IsHighInfo)
                {
                    if (warningText != null)
                        warningText.text = "好像不太對呢...購買新書或長期放著沒有在提高資產利用率，請比較哪一個選項真正讓『既有閒置資源』被循環利用！";
                }
                else
                {
                    if (warningText != null)
                        warningText.text = "好像不太對呢...再想一想吧！";
                }

                GameData.RecordFeedbackShown("C2_REUSE_CASE_WRONG");
            }
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}