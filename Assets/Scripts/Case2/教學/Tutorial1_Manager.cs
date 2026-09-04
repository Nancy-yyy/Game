using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tutorial1_Manager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        [TextArea(2, 4)]
        public string questionText;
        public bool isOwnership; // true = 所有權, false = 使用權
    }

    [Header("【常駐標題】")]
    public GameObject titleImage; // 拖入 TitleImg_OwnershipVsUsage

    [Header("【第 1 幕：開場對話群組】")]
    public GameObject introDialogueGroup; // 拖入 IntroDialogueGroup
    public GameObject dialogue1_Group;    // 主角 1
    public GameObject dialogue2_Group;    // 鳥鳥 1
    public GameObject dialogue3_Group;    // 主角 2
    public Button screenClickAreaBtn;     // 全螢幕透明按鈕

    [Header("【第 2 幕：概念卡片】")]
    public RectTransform conceptCardsGroup; // 拖入 ConceptCardsGroup
    public Button cardOwnershipBtn;        
    public Button cardUseRightBtn;         
    public HoverCardEffect hoverOwnership; 
    public HoverCardEffect hoverUseRight;  

    [Header("【第 3 幕 & 結尾：System Block 系統講解框】")]
    public GameObject systemBlockPanel;    // 拖入 SystemBlockPanel
    public TextMeshProUGUI systemBlockText;// 拖入 SystemBlockText
    public Button systemBlockNextBtn;      // 拖入 SystemBlockNextBtn

    [Header("【第 4 幕：例題問答】")]
    public GameObject questionPanel;       // 拖入 QuestionPanel
    public TextMeshProUGUI questionIndexText;   
    public TextMeshProUGUI questionContentText; 
    public QuestionData[] questions = new QuestionData[]
    {
        new QuestionData { questionText = "小華向學長買下一台二手腳踏車，之後可以自己騎，也可以再賣給別人。", isOwnership = true },
        new QuestionData { questionText = "小明付費入住飯店一晚，隔天退房後不能繼續使用房間。", isOwnership = false },
        new QuestionData { questionText = "小美支付一個月的串流平台會員費，可以觀看平台上的影片，但會員到期後就不能繼續觀看。", isOwnership = false },
        new QuestionData { questionText = "小明支付一次性費用，取得某款 App 的「永久使用資格」，他可以一直使用這個 App，但不能把這款 App 的程式碼拿去販售。", isOwnership = false }
    };

    [Header("【錯誤警告彈窗】")]
    public GameObject warningBoxPanel;     
    public TextMeshProUGUI warningText;    
    public Button warningCloseBtn;         

    [Header("【第 5 幕：底部角色對話框】")]
    public GameObject bottomDialoguePanel; 
    public TextMeshProUGUI bottomSpeakerText; 
    public TextMeshProUGUI bottomContentText; 
    public Button bottomDialogueNextBtn;   

    private int introStep = 0;
    private int sysBlockStep = 0;
    private int currentQuestionIndex = 0;
    private bool isAnsweringPhase = false;
    private int endingDialogueStep = 0;
    private bool isEndingPraisePhase = false; 
    private bool isAllFinished = false; 

    private readonly string[] sysBlockDialogues = new string[]
    {
        "使用資產，不一定等於擁有資產",
        "所有權表示擁有資產本身，可以決定是否繼續持有、轉讓等",
        "例如購買一本 $3,500 的原文書，就可以取得這本書的所有權",
        "使用權是在特定期間或條件下使用資產",
        "例如租借一本原文書就可以取得租借期間的使用權，但必須要在租期結束後歸還",
        "現在我們來看看不同的例子吧，點擊你認為正確的權利！"
    };

    void Start()
    {
        if (titleImage != null) titleImage.SetActive(true);
        if (introDialogueGroup != null) introDialogueGroup.SetActive(true);
        if (dialogue1_Group != null) dialogue1_Group.SetActive(true);
        if (dialogue2_Group != null) dialogue2_Group.SetActive(false);
        if (dialogue3_Group != null) dialogue3_Group.SetActive(false);

        if (conceptCardsGroup != null) conceptCardsGroup.gameObject.SetActive(false);
        if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
        if (questionPanel != null) questionPanel.SetActive(false);
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
        if (bottomDialoguePanel != null) bottomDialoguePanel.SetActive(false);

        SetHoverInteractable(false);

        if (screenClickAreaBtn != null)
        {
            screenClickAreaBtn.onClick.RemoveAllListeners();
            screenClickAreaBtn.onClick.AddListener(OnClickScreenIntro);
        }

        if (systemBlockNextBtn != null)
        {
            systemBlockNextBtn.onClick.RemoveAllListeners();
            systemBlockNextBtn.onClick.AddListener(OnClickSystemBlockNext);
        }

        if (cardOwnershipBtn != null)
        {
            cardOwnershipBtn.onClick.RemoveAllListeners();
            cardOwnershipBtn.onClick.AddListener(() => OnSelectAnswer(true));
        }

        if (cardUseRightBtn != null)
        {
            cardUseRightBtn.onClick.RemoveAllListeners();
            cardUseRightBtn.onClick.AddListener(() => OnSelectAnswer(false));
        }

        if (warningCloseBtn != null)
        {
            warningCloseBtn.onClick.RemoveAllListeners();
            warningCloseBtn.onClick.AddListener(OnCloseWarningBox);
        }

        if (bottomDialogueNextBtn != null)
        {
            bottomDialogueNextBtn.onClick.RemoveAllListeners();
            bottomDialogueNextBtn.onClick.AddListener(OnClickBottomDialogueNext);
        }
    }

    public void OnClickScreenIntro()
    {
        introStep++;
        if (introStep == 1)
        {
            if (dialogue1_Group != null) dialogue1_Group.SetActive(false);
            if (dialogue2_Group != null) dialogue2_Group.SetActive(true);
        }
        else if (introStep == 2)
        {
            if (dialogue2_Group != null) dialogue2_Group.SetActive(false);
            if (dialogue3_Group != null) dialogue3_Group.SetActive(true);
        }
        else if (introStep == 3)
        {
            if (dialogue3_Group != null) dialogue3_Group.SetActive(false);
            if (introDialogueGroup != null) introDialogueGroup.SetActive(false);
            StartCoroutine(ShowConceptCardsAndSysBlock());
        }
    }

    private IEnumerator ShowConceptCardsAndSysBlock()
    {
        if (conceptCardsGroup != null) conceptCardsGroup.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f);

        if (systemBlockPanel != null)
        {
            sysBlockStep = 0;
            isEndingPraisePhase = false;
            systemBlockPanel.SetActive(true);
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = sysBlockDialogues[0];
        }
    }

    public void OnClickSystemBlockNext()
    {
        if (isAllFinished) return;

        if (isEndingPraisePhase)
        {
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            isEndingPraisePhase = false;

            if (bottomDialoguePanel != null)
            {
                endingDialogueStep = 0;
                bottomDialoguePanel.SetActive(true);
                ShowEndingDialogueLine();
            }
            return;
        }

        sysBlockStep++;
        if (sysBlockStep < sysBlockDialogues.Length)
        {
            if (systemBlockText != null) systemBlockText.text = sysBlockDialogues[sysBlockStep];
        }
        else
        {
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            StartQuestionPhase();
        }
    }

    private void StartQuestionPhase()
    {
        SetHoverInteractable(true);
        isAnsweringPhase = true;

        if (questionPanel != null) questionPanel.SetActive(true);
        currentQuestionIndex = 0;
        LoadCurrentQuestion();
    }

    private void LoadCurrentQuestion()
    {
        if (questions == null || currentQuestionIndex >= questions.Length) return;

        if (questionIndexText != null)
        {
            questionIndexText.text = $"例題 {currentQuestionIndex + 1}/{questions.Length}";
        }

        if (questionContentText != null)
        {
            questionContentText.text = questions[currentQuestionIndex].questionText;
        }
    }

    public void OnSelectAnswer(bool selectedOwnership)
    {
        if (!isAnsweringPhase) return;

        bool isCorrect = (selectedOwnership == questions[currentQuestionIndex].isOwnership);

        if (isCorrect)
        {
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Length)
            {
                LoadCurrentQuestion();
            }
            else
            {
                FinishAllQuestions();
            }
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                if (warningText != null) warningText.text = "好像不太對歐~再想想看吧！";
            }
        }
    }

    public void OnCloseWarningBox()
    {
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
    }

    private void FinishAllQuestions()
    {
        isAnsweringPhase = false;
        SetHoverInteractable(false);
        if (questionPanel != null) questionPanel.SetActive(false);

        if (systemBlockPanel != null)
        {
            isEndingPraisePhase = true;
            systemBlockPanel.SetActive(true);
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = "看來你對所有權與使用權都很了解囉！";
        }
    }

    private void ShowEndingDialogueLine()
    {
        if (endingDialogueStep == 0)
        {
            if (bottomSpeakerText != null) bottomSpeakerText.text = "主角";
            if (bottomContentText != null) bottomContentText.text = "所以買書是把書變成我的，而租書則是在一段時間內取得它的使用權嗎？";
        }
        else if (endingDialogueStep == 1)
        {
            if (bottomSpeakerText != null) bottomSpeakerText.text = "鳥鳥";
            if (bottomContentText != null) bottomContentText.text = "原來『可以用』跟『擁有它』不是同一件事！";
        }
    }

    public void OnClickBottomDialogueNext()
    {
        endingDialogueStep++;
        if (endingDialogueStep < 2)
        {
            ShowEndingDialogueLine();
        }
        else
        {
            // 教學 1 完成，更新狀態並切換回攤位
            Case2State.StallPhase = 1;
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.StartTransitionAndLoadScene("Case2_03_Stall");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Case2_03_Stall");
            }
        }
    }

    private void SetHoverInteractable(bool state)
    {
        if (hoverOwnership != null) hoverOwnership.isInteractable = state;
        if (hoverUseRight != null) hoverUseRight.isInteractable = state;
    }
}