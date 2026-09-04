using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Case1MatchingManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private GameObject playerDialogueUI;
    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private GameObject systemIntroPanel;
    [SerializeField] private TMP_Text systemIntroText;
    [SerializeField] private GameObject systemIntroNextTriangle;
    [SerializeField] private GameObject systemIntroConfirmButton;

    private int systemIntroStep = 0;

    [SerializeField]
    private GameObject matchingOrderPanel;

    [SerializeField] private Image answerSlot1;
    [SerializeField] private Image answerSlot2;
    [SerializeField] private Image answerSlot3;
    [SerializeField] private Image answerSlot4;

    [SerializeField] private TMP_Text answerSlot1Text;
    [SerializeField] private TMP_Text answerSlot2Text;
    [SerializeField] private TMP_Text answerSlot3Text;
    [SerializeField] private TMP_Text answerSlot4Text;

    [SerializeField] private GameObject wrongPanel;
    
    [Header("Feedback Version")]
    [SerializeField] private TMP_Text wrongText;

    [SerializeField] private GameObject systemHintPanel;

    [SerializeField] private GameObject systemAnalysisPanel;
    [SerializeField] private TMP_Text systemAnalysisText;

    [SerializeField] private GameObject systemMessagePanel;

    [SerializeField] private AudioSource wrongAudio;
    [SerializeField] private AudioSource correctAudio;

    [SerializeField] private GameObject playerSeatDialogueUI;
    [SerializeField] private TMP_Text playerSeatNameText;

    [SerializeField] private GameObject supplyQuestionPanel;
    [SerializeField] private GameObject questionWrongPanel;
    [Header("Supply Feedback")]
    [SerializeField] private TMP_Text questionWrongText;

    [SerializeField] private GameObject supplyHintPanel;
    [SerializeField] private GameObject supplyAnalysisPanel;
    [SerializeField] private GameObject supplyMessagePanel;

    [SerializeField] private GameObject matchingSuccessPanel;
    [SerializeField] private TMP_Text matchingSuccessText;
    [SerializeField] private GameObject matchingSuccessNextTriangle;
    [SerializeField] private GameObject matchingSuccessConfirmButton;
    [SerializeField] private GameObject sustainabilityWarningPanel;

    [Header("Case 1 Summary")]
    [SerializeField] private GameObject case1SummaryPanel;
    [SerializeField] private GameObject case1SummaryAnalysisPanel;
    [SerializeField] private GameObject case1SummaryMessagePanel;

    private int selectedCount = 0;

    private List<string> selectedOrder = new List<string>();
    private List<GameObject> selectedCards = new List<GameObject>();

    private enum MatchingFeedbackFlow
    {
        None,
        Wrong,
        Correct
    }

    private MatchingFeedbackFlow matchingFeedbackFlow = MatchingFeedbackFlow.None;

    private void Start()
    {
        playerDialogueUI.SetActive(true);

        playerNameText.text = GameData.PlayerName;
        playerSeatNameText.text = GameData.PlayerName;

        systemIntroPanel.SetActive(false);
        systemIntroConfirmButton.SetActive(false);

        matchingOrderPanel.SetActive(false);

        wrongPanel.SetActive(false);
        systemHintPanel.SetActive(false);
        systemAnalysisPanel.SetActive(false);
        systemMessagePanel.SetActive(false);
        playerSeatDialogueUI.SetActive(false);
        supplyQuestionPanel.SetActive(false);

        questionWrongPanel.SetActive(false);
        supplyHintPanel.SetActive(false);
        supplyAnalysisPanel.SetActive(false);
        supplyMessagePanel.SetActive(false);
        matchingSuccessPanel.SetActive(false);
        matchingSuccessConfirmButton.SetActive(false);
        sustainabilityWarningPanel.SetActive(false);

        case1SummaryPanel.SetActive(false);
        case1SummaryAnalysisPanel.SetActive(false);
        case1SummaryMessagePanel.SetActive(false);
    }

    public void ShowSystemIntro()
    {
        playerDialogueUI.SetActive(false);

        systemIntroPanel.SetActive(true);

        systemIntroStep = 0;

        systemIntroText.text =
            "通常會依行程與交通成本分攤，但不一定每一趟都比其他交通方式便宜！";

        systemIntroNextTriangle.SetActive(true);
        systemIntroConfirmButton.SetActive(false);
    }

    public void NextSystemIntro()
    {
        if (systemIntroStep == 0)
        {
            systemIntroText.text =
                "接下來還需要透過平台「確認行程與時間」才算完成媒合喔！";

            systemIntroNextTriangle.SetActive(false);
            systemIntroConfirmButton.SetActive(true);

            systemIntroStep = 1;
        }
    }

    public void ShowMatchingOrderPanel()
    {
        systemIntroPanel.SetActive(false);
        matchingOrderPanel.SetActive(true);
    }

    public void SelectCard(string cardID, string cardDisplayText, GameObject card)
    {
        UnityEngine.UI.Image targetSlot = null;
        TMP_Text targetText = null;

        if (selectedCount == 0)
        {
            targetSlot = answerSlot1;
            targetText = answerSlot1Text;
        }
        else if (selectedCount == 1)
        {
            targetSlot = answerSlot2;
            targetText = answerSlot2Text;
        }
        else if (selectedCount == 2)
        {
            targetSlot = answerSlot3;
            targetText = answerSlot3Text;
        }
        else if (selectedCount == 3)
        {
            targetSlot = answerSlot4;
            targetText = answerSlot4Text;
        }

        if (targetSlot == null || targetText == null)
            return;

        // 找到卡片裡真正顯示圖片的 CardVisual
        UnityEngine.UI.Image cardImage =
            card.transform.Find("CardVisual").GetComponent<UnityEngine.UI.Image>();

        // 找到卡片裡顯示的字
        TMPro.TMP_Text cardText =
            card.transform.Find("CardVisual/CardText").GetComponent<TMP_Text>();

        // 把卡片圖片放進 AnswerSlot
        targetSlot.sprite = cardImage.sprite;
        targetSlot.color = Color.white;

        // 使用這張卡自己的文字
        targetText.text = cardDisplayText;

        // 左邊原本卡片消失
        MatchingCard matchingCard = card.GetComponent<MatchingCard>();
        if (matchingCard != null)
        {
            matchingCard.ResetCardVisual();
        }
        card.SetActive(false);

        selectedOrder.Add(cardID);
        selectedCards.Add(card);

        selectedCount++;
    }

    public void UndoLastCard()
    {
        if (selectedCount <= 0)
            return;

        selectedCount--;

        // 找到最後選的卡
        GameObject lastCard = selectedCards[selectedCards.Count - 1];

        // 讓左邊原卡片重新出現
        lastCard.SetActive(true);

        // 清掉最後一個 AnswerSlot
        if (selectedCount == 0)
        {
            answerSlot1.sprite = null;
            answerSlot1Text.text = "";
        }
        else if (selectedCount == 1)
        {
            answerSlot2.sprite = null;
            answerSlot2Text.text = "";
        }
        else if (selectedCount == 2)
        {
            answerSlot3.sprite = null;
            answerSlot3Text.text = "";
        }
        else if (selectedCount == 3)
        {
            answerSlot4.sprite = null;
            answerSlot4Text.text = "";
        }

        selectedOrder.RemoveAt(selectedOrder.Count - 1);
        selectedCards.RemoveAt(selectedCards.Count - 1);
    }

    public void CheckAnswer()
    {
        if (selectedOrder.Count < 4)
            return;

        bool isCorrect =
            selectedOrder[0] == "Supply" &&
            selectedOrder[1] == "Search" &&
            selectedOrder[2] == "Match" &&
            selectedOrder[3] == "Confirm";

        if (isCorrect)
        {
            if (correctAudio != null)
            {
                correctAudio.Play();
            }

            if (GameData.FeedbackMode == FeedbackMode.Simple)
            {
                matchingOrderPanel.SetActive(false);
                playerSeatDialogueUI.SetActive(true);
            }
            else
            {
                matchingFeedbackFlow = MatchingFeedbackFlow.Correct;

                matchingOrderPanel.SetActive(false);

                systemAnalysisText.text =
                    "車主先提供「空位、時間、路線」等供給資訊，乘客提出交通需求，平台協助搜尋與配對，最後再由雙方確認。";

                systemAnalysisPanel.SetActive(true);
            }
        }
        else
        {
            ShowWrongMatchingFeedback();
        }
    }

    public void CloseSystemMessage()
    {
        systemMessagePanel.SetActive(false);
        matchingOrderPanel.SetActive(false);
        playerSeatDialogueUI.SetActive(true);
    }

    private void ShowWrongMatchingFeedback()
    {
        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        matchingOrderPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            wrongText.text =
                "順序錯誤，請重新排列！";

            wrongPanel.SetActive(true);
        }
        else
        {
            matchingFeedbackFlow = MatchingFeedbackFlow.Wrong;

            wrongText.text =
                "順序還不正確。";

            wrongPanel.SetActive(true);
        }
    }

    public void CloseWrongPanel()
    {
        wrongPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            matchingOrderPanel.SetActive(true);
        }
        else
        {
            systemHintPanel.SetActive(true);
        }
    }

    public void CloseSystemHint()
    {
        systemHintPanel.SetActive(false);

        systemAnalysisText.text =
            "平台媒合通常從資源資訊出現開始，再由需求者搜尋，接著比對雙方條件，最後才進入確認交易。";

        systemAnalysisPanel.SetActive(true);
    }

    public void CloseSystemAnalysis()
    {
        systemAnalysisPanel.SetActive(false);

        if (matchingFeedbackFlow == MatchingFeedbackFlow.Wrong)
        {
            matchingOrderPanel.SetActive(true);
        }
        else if (matchingFeedbackFlow == MatchingFeedbackFlow.Correct)
        {
            systemMessagePanel.SetActive(true);
        }
    }

    public void ShowSupplyQuestion()
    {
        playerSeatDialogueUI.SetActive(false);
        supplyQuestionPanel.SetActive(true);
    }

    public void ChooseAnswerA()
    {
        ShowQuestionWrong();
    }

    public void ChooseAnswerB()
    {
        if (correctAudio != null)
        {
            correctAudio.Play();
        }

        supplyQuestionPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            matchingSuccessPanel.SetActive(true);
            matchingSuccessNextTriangle.SetActive(true);
            matchingSuccessConfirmButton.SetActive(false);
        }
        else
        {
            supplyAnalysisPanel.SetActive(true);
        }
    }

    public void ChooseAnswerC()
    {
        ShowQuestionWrong();
    }

    private void ShowQuestionWrong()
    {
        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            questionWrongText.text =
                "答錯了，再想想目前剩下多少空位！";
        }
        else
        {
            questionWrongText.text =
                "答錯了。\n需求人數增加，但可以提供的座位數沒有改變。";
        }

        questionWrongPanel.SetActive(true);
    }

    public void CloseQuestionWrongPanel()
    {
        questionWrongPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Deep)
        {
            supplyQuestionPanel.SetActive(false);
            supplyHintPanel.SetActive(true);
        }
    }

    public void CloseSupplyHint()
    {
        supplyHintPanel.SetActive(false);
        supplyQuestionPanel.SetActive(true);
    }

    public void CloseSupplyAnalysis()
    {
        supplyAnalysisPanel.SetActive(false);
        supplyMessagePanel.SetActive(true);
    }

    public void CloseSupplyMessage()
    {
        supplyMessagePanel.SetActive(false);
        matchingSuccessPanel.SetActive(true);
        matchingSuccessNextTriangle.SetActive(true);
        matchingSuccessConfirmButton.SetActive(false);
    }

    public void ShowSecondSuccessMessage()
    {
        matchingSuccessText.text =
            "這就是共享經濟的第一步 —「可以把閒置的座位資源，媒合給有交通需求的人。」";

        matchingSuccessNextTriangle.SetActive(false);
        matchingSuccessConfirmButton.SetActive(true);
    }

    public void ShowSustainabilityWarning()
    {
        matchingSuccessPanel.SetActive(false);
        sustainabilityWarningPanel.SetActive(true);
    }

    public void CloseSustainabilityWarning()
    {
        sustainabilityWarningPanel.SetActive(false);
        case1SummaryPanel.SetActive(true);
        if (correctAudio != null)
        {
            correctAudio.Play();
        }
    }

    public void CloseCase1Summary()
    {
        case1SummaryPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            SceneManager.LoadScene("Case1Travel");
        }
        else
        {
            case1SummaryAnalysisPanel.SetActive(true);
        }
    }

    public void CloseCase1SummaryAnalysis()
    {
        case1SummaryAnalysisPanel.SetActive(false);
        case1SummaryMessagePanel.SetActive(true);
    }

    public void CloseCase1SummaryMessage()
    {
        case1SummaryMessagePanel.SetActive(false);
        SceneManager.LoadScene("Case1Travel");
    }
}