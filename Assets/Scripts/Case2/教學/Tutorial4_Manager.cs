using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tutorial4_Manager : MonoBehaviour
{
    [Header("【常駐標題】")]
    public GameObject titleImage;

    [Header("【主角對話框 (第一階段)】")]
    public GameObject bottomDialoguePanel4;
    public TextMeshProUGUI dialogueContentText4;
    public Button dialogueNextBtn4;

    [Header("【System Block 系統解說框】")]
    public GameObject systemBlockPanel;
    public RectTransform systemBlockRect;
    public TextMeshProUGUI systemBlockText;
    public Button systemBlockNextBtn;

    [Header("【系統框尺寸與位置調節 (可直接在面板微調)】")]
    [Tooltip("第一階段：畫面中央解說框的位置與大小")]
    public Vector2 posCenter = new Vector2(0f, 0f);
    public Vector2 sizeCenter = new Vector2(1050f, 360f); // 加大以防字體溢位

    [Tooltip("左右對比階段：底部解說框的位置與大小")]
    public Vector2 posPostCards = new Vector2(0f, -345f);
    public Vector2 sizePostCards = new Vector2(1100f, 360f);

    [Tooltip("案例與線索階段：底部解說框的位置與大小")]
    public Vector2 posCaseQuiz = new Vector2(0f, -340f);
    public Vector2 sizeCaseQuiz = new Vector2(1050f, 340f);

    [Header("【錯誤警告彈窗】")]
    public GameObject warningBoxPanel;
    public TextMeshProUGUI warningText;
    public Button warningConfirmBtn;

    [Header("【第 1 幕：左右對比展示卡】")]
    public GameObject rentVsReuseGroup;

    [Header("【第 2 幕：偵測容器與提示 (Hint)】")]
    public GameObject detectorInteractionGroup;
    public RectTransform detectorHintRect;       // 拖入 HintPanel 的 RectTransform
    public Button detectorHintDismissBtn;        // 拖入全螢幕點擊關閉按鈕
    public float hintSlideDuration = 0.35f;
    public Tutorial4DropSlot slot1_Source;      // ① 資源來源 (Clue 5)
    public Tutorial4DropSlot slot2_Status;      // ② 資源狀態 (Clue 2)
    public Tutorial4DropSlot slot3_Usage;       // ③ 使用方式 (Clue 3)
    public Tutorial4ClueCard[] clueCards; 

    [Header("【第 3 幕：三選一案例選擇】")]
    public GameObject caseQuizGroup;
    public Button caseA_Btn; 
    public Button caseB_Btn; 
    public Button caseC_Btn; 

    [Header("【第 4 幕：自主輸入反思】")]
    public GameObject selfInputReflectionPanel;
    public TMP_InputField answerInputField; 
    public Button submitReflectionBtn;

    private int phase1SysStep = 0;
    private int phase1PostCardStep = 0;
    private int postClueStep = 0;
    private bool isPostCardsPhase = false;
    private bool isPostCluePhase = false;
    private bool isWaitingCaseQuizNext = false;

    private readonly string[] centerSysLines = new string[]
    {
        "不一定歐！",
        "『租借』描述的是使用方式，\n但共享經濟還需要進一步觀察",
        "像是資源從哪裡來，\n以及原本是否存在未被充分利用的資源",
        "所以先別急著把『租借』和『共享經濟』\n畫上等號！"
    };

    private readonly string[] postCardSysLines = new string[]
    {
        "上面的兩種方式都可能讓你『租到一本書』。",
        "但資源從哪裡來，以及它原本是否已經存在\n並處於未充分使用狀態，並不相同。",
        "現在不要只看『租』這個字。",
        "看看這些服務背後，資源是怎麼來的。",
        "你覺得共享經濟的資源從哪裡來？"
    };

    private readonly string[] postClueSysLines = new string[]
    {
        "你已經找到區分『一般租賃』與『閒置資源再利用』的重要線索。",
        "但不是所有『借東西』的服務，都符合這些條件。",
        "來看看這些條件吧！"
    };

    private void Awake()
    {
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
        if (rentVsReuseGroup != null) rentVsReuseGroup.SetActive(false);
        if (detectorInteractionGroup != null) detectorInteractionGroup.SetActive(false);
        if (caseQuizGroup != null) caseQuizGroup.SetActive(false);
        if (selfInputReflectionPanel != null) selfInputReflectionPanel.SetActive(false);
        if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
    }

    void Start()
    {
        if (titleImage != null) titleImage.SetActive(true);

        // ⭐ 自動啟用 TMP 防溢位設定
        if (systemBlockText != null)
        {
            systemBlockText.enableAutoSizing = true;
            systemBlockText.fontSizeMin = 22f;
            systemBlockText.fontSizeMax = 38f;
        }

        if (clueCards != null)
        {
            foreach (var card in clueCards)
            {
                if (card != null) card.manager = this;
            }
        }

        if (dialogueNextBtn4 != null)
        {
            dialogueNextBtn4.onClick.RemoveAllListeners();
            dialogueNextBtn4.onClick.AddListener(OnClickPlayerDialogueNext);
        }

        if (systemBlockNextBtn != null)
        {
            systemBlockNextBtn.onClick.RemoveAllListeners();
            systemBlockNextBtn.onClick.AddListener(OnClickSystemBlockNext);
        }

        if (warningConfirmBtn != null)
        {
            warningConfirmBtn.onClick.RemoveAllListeners();
            warningConfirmBtn.onClick.AddListener(OnCloseWarningBox);
        }

        if (caseA_Btn != null)
        {
            caseA_Btn.onClick.RemoveAllListeners();
            caseA_Btn.onClick.AddListener(() => OnSelectCase(false));
            caseA_Btn.interactable = true;
        }
        if (caseB_Btn != null)
        {
            caseB_Btn.onClick.RemoveAllListeners();
            caseB_Btn.onClick.AddListener(() => OnSelectCase(true));
            caseB_Btn.interactable = true;
        }
        if (caseC_Btn != null)
        {
            caseC_Btn.onClick.RemoveAllListeners();
            caseC_Btn.onClick.AddListener(() => OnSelectCase(false));
            caseC_Btn.interactable = true;
        }

        if (submitReflectionBtn != null)
        {
            submitReflectionBtn.onClick.RemoveAllListeners();
            submitReflectionBtn.onClick.AddListener(OnSubmitReflection);
        }

        if (detectorHintDismissBtn != null)
        {
            detectorHintDismissBtn.onClick.RemoveAllListeners();
            detectorHintDismissBtn.onClick.AddListener(DismissDetectorHint);
        }

        StartCoroutine(StartPhase1Routine());
    }

    private IEnumerator StartPhase1Routine()
    {
        yield return new WaitForSeconds(0.3f);
        if (bottomDialoguePanel4 != null)
        {
            bottomDialoguePanel4.SetActive(true);
            bottomDialoguePanel4.transform.SetAsLastSibling();
            if (dialogueContentText4 != null)
                dialogueContentText4.text = "所以只要把閒置的書租給別人，就算共享經濟嗎？";
        }
    }

    public void OnClickPlayerDialogueNext()
    {
        if (bottomDialoguePanel4 != null) bottomDialoguePanel4.SetActive(false);

        if (systemBlockPanel != null && systemBlockRect != null)
        {
            UpdateBlockTransform(posCenter, sizeCenter);
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();

            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            phase1SysStep = 0;
            if (systemBlockText != null) systemBlockText.text = centerSysLines[0];
        }
    }

    public void OnClickSystemBlockNext()
    {
        if (isWaitingCaseQuizNext)
        {
            isWaitingCaseQuizNext = false;
            if (caseQuizGroup != null) caseQuizGroup.SetActive(false);
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);

            if (selfInputReflectionPanel != null)
            {
                selfInputReflectionPanel.SetActive(true);
                selfInputReflectionPanel.transform.SetAsLastSibling();
            }
            return;
        }

        if (!isPostCardsPhase && !isPostCluePhase)
        {
            phase1SysStep++;
            if (phase1SysStep < centerSysLines.Length)
            {
                if (systemBlockText != null) systemBlockText.text = centerSysLines[phase1SysStep];
            }
            else
            {
                if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
                if (rentVsReuseGroup != null) rentVsReuseGroup.SetActive(true);
                StartCoroutine(ShowPostCardsSysDialogue());
            }
            return;
        }

        if (isPostCardsPhase)
        {
            phase1PostCardStep++;
            if (phase1PostCardStep < postCardSysLines.Length)
            {
                if (systemBlockText != null) systemBlockText.text = postCardSysLines[phase1PostCardStep];
            }
            else
            {
                isPostCardsPhase = false;
                if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
                if (rentVsReuseGroup != null) rentVsReuseGroup.SetActive(false);

                if (detectorInteractionGroup != null)
                {
                    detectorInteractionGroup.SetActive(true);
                    StartCoroutine(SlideInDetectorHint());
                }
            }
            return;
        }

        if (isPostCluePhase)
        {
            postClueStep++;
            if (postClueStep < postClueSysLines.Length)
            {
                if (systemBlockText != null) systemBlockText.text = postClueSysLines[postClueStep];
            }
            else
            {
                isPostCluePhase = false;
                if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
                if (caseQuizGroup != null) caseQuizGroup.SetActive(true);
            }
        }
    }

    private IEnumerator ShowPostCardsSysDialogue()
    {
        yield return new WaitForSeconds(0.3f);
        if (systemBlockPanel != null && systemBlockRect != null)
        {
            UpdateBlockTransform(posPostCards, sizePostCards);
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();

            isPostCardsPhase = true;
            phase1PostCardStep = 0;
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = postCardSysLines[0];
        }
    }

    private IEnumerator SlideInDetectorHint()
    {
        if (detectorHintRect == null) yield break;

        detectorHintRect.gameObject.SetActive(true);
        if (detectorHintDismissBtn != null) detectorHintDismissBtn.gameObject.SetActive(true);

        Vector2 startPos = new Vector2(0f, 1200f);
        Vector2 targetPos = new Vector2(0f, 400f);
        float elapsed = 0f;

        while (elapsed < hintSlideDuration)
        {
            elapsed += Time.deltaTime;
            detectorHintRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / hintSlideDuration);
            yield return null;
        }
        detectorHintRect.anchoredPosition = targetPos;
    }

    public void DismissDetectorHint()
    {
        if (detectorHintDismissBtn != null) detectorHintDismissBtn.gameObject.SetActive(false);
        StartCoroutine(SlideOutDetectorHint());
    }

    private IEnumerator SlideOutDetectorHint()
    {
        if (detectorHintRect == null) yield break;

        Vector2 startPos = detectorHintRect.anchoredPosition;
        Vector2 targetPos = new Vector2(0f, 1200f);
        float elapsed = 0f;

        while (elapsed < hintSlideDuration)
        {
            elapsed += Time.deltaTime;
            detectorHintRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / hintSlideDuration);
            yield return null;
        }
        detectorHintRect.anchoredPosition = targetPos;
        detectorHintRect.gameObject.SetActive(false);
    }

    public bool TrySnapCardToNearestSlot(Tutorial4ClueCard card)
    {
        Tutorial4DropSlot[] allSlots = new Tutorial4DropSlot[] { slot1_Source, slot2_Status, slot3_Usage };
        Tutorial4DropSlot nearestSlot = null;
        float minDistance = float.MaxValue;
        float snapThreshold = 150f;

        foreach (var slot in allSlots)
        {
            if (slot != null && (slot.currentCard == null || slot.currentCard == card))
            {
                float dist = Vector3.Distance(card.transform.position, slot.transform.position);
                if (dist < minDistance && dist <= snapThreshold)
                {
                    minDistance = dist;
                    nearestSlot = slot;
                }
            }
        }

        if (nearestSlot == null) return false;

        if (card.currentSlot != null && card.currentSlot != nearestSlot)
        {
            card.currentSlot.currentCard = null;
        }

        nearestSlot.currentCard = card;
        card.currentSlot = nearestSlot;
        card.transform.SetParent(nearestSlot.transform);
        card.transform.position = nearestSlot.transform.position;

        CheckAllSlotsFilled();
        return true;
    }

    private void CheckAllSlotsFilled()
    {
        if (slot1_Source.currentCard == null || 
            slot2_Status.currentCard == null || 
            slot3_Usage.currentCard == null)
        {
            return;
        }

        int id1 = slot1_Source.currentCard.clueId;
        int id2 = slot2_Status.currentCard.clueId;
        int id3 = slot3_Usage.currentCard.clueId;

        bool isCorrect = (id1 == 5 && id2 == 2 && id3 == 3);

        if (isCorrect)
        {
            slot1_Source.currentCard.LockPlaced();
            slot2_Status.currentCard.LockPlaced();
            slot3_Usage.currentCard.LockPlaced();
            StartCoroutine(OnAllCluesFoundRoutine());
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null)
                    warningText.text = "這些線索好像不太對歐...";
            }
        }
    }

    public void OnCloseWarningBox()
    {
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);

        Tutorial4DropSlot[] allSlots = new Tutorial4DropSlot[] { slot1_Source, slot2_Status, slot3_Usage };
        foreach (var slot in allSlots)
        {
            if (slot != null && slot.currentCard != null)
            {
                slot.currentCard.ReturnToOriginal();
                slot.currentCard = null;
            }
        }
    }

    private IEnumerator OnAllCluesFoundRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        if (systemBlockPanel != null && systemBlockRect != null)
        {
            UpdateBlockTransform(posCaseQuiz, sizeCaseQuiz);
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
            if (systemBlockText != null) systemBlockText.text = "找到所有線索！";
        }

        yield return new WaitForSeconds(1.5f);

        if (detectorInteractionGroup != null) detectorInteractionGroup.SetActive(false);

        isPostCluePhase = true;
        postClueStep = 0;

        if (systemBlockPanel != null)
        {
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = postClueSysLines[0];
        }
    }

    public void OnSelectCase(bool isCorrect)
    {
        if (isCorrect)
        {
            if (caseA_Btn != null) caseA_Btn.interactable = false;
            if (caseB_Btn != null) caseB_Btn.interactable = false;
            if (caseC_Btn != null) caseC_Btn.interactable = false;

            StartCoroutine(OnCaseCorrectRoutine());
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null) warningText.text = "好像不太對歐";
            }
        }
    }

    private IEnumerator OnCaseCorrectRoutine()
    {
        if (systemBlockPanel != null && systemBlockRect != null)
        {
            UpdateBlockTransform(posCaseQuiz, sizeCaseQuiz);
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
            if (systemBlockText != null) systemBlockText.text = "沒錯喲！";
        }

        yield return new WaitForSeconds(1.2f);

        if (systemBlockPanel != null)
        {
            if (systemBlockText != null)
                systemBlockText.text = "三種情況都可能讓使用者取得一本書，但它們的資源來源與運作方式不同\n其中，B 最符合『既有閒置資產重新被利用』的特徵。";

            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            isWaitingCaseQuizNext = true;
        }
    }

    public void OnSubmitReflection()
    {
        string userInput = answerInputField != null ? answerInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(userInput))
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null) warningText.text = "請輸入你的想法後再送出喔！";
            }
            return;
        }

        if (answerInputField != null) answerInputField.interactable = false;
        if (submitReflectionBtn != null) submitReflectionBtn.interactable = false;

        if (systemBlockPanel != null && systemBlockRect != null)
        {
            UpdateBlockTransform(posCaseQuiz, sizeCaseQuiz);
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
            if (systemBlockText != null) systemBlockText.text = "已記錄你的回答";
        }

        Case2State.StallPhase = 2;
        StartCoroutine(WaitAndBackToStallRoutine());
    }

    private IEnumerator WaitAndBackToStallRoutine()
    {
        yield return new WaitForSeconds(1.8f);

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.StartTransitionAndLoadScene("Case2_03_Stall");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Case2_03_Stall");
        }
    }

    // ⭐ 統一更新位置與尺寸，並即時強制重算佈局防止殘留與溢位
    private void UpdateBlockTransform(Vector2 targetPos, Vector2 targetSize)
    {
        if (systemBlockRect == null) return;
        systemBlockRect.anchoredPosition = targetPos;
        systemBlockRect.sizeDelta = targetSize;
        LayoutRebuilder.ForceRebuildLayoutImmediate(systemBlockRect);
    }
}