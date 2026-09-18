using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private GameObject transferQuestionPanel;
    [SerializeField] private GameObject conceptFillPanel;

    [SerializeField] private TMP_InputField answerInput1;
    [SerializeField] private TMP_InputField answerInput2;
    [SerializeField] private TMP_InputField answerInput3;

    [SerializeField] private GameObject wrongPanel;
    [SerializeField] private TMP_Text wrongText;
    [SerializeField] private AudioSource wrongAudio;

    [Header("Deep Concept Wrong Feedback")]
    [SerializeField] private GameObject conceptWarningPanel;
    [SerializeField] private TMP_Text conceptWarningText;

    [SerializeField] private GameObject conceptHintPanel;
    [SerializeField] private TMP_Text conceptHintText;

    private bool wrongIdleAsset = false;
    private bool wrongUsageRight = false;
    private bool wrongMatchingMethod = false;

    [Header("Deep Concept Correct Feedback")]
    [SerializeField] private GameObject conceptCorrectFeedbackPanel;
    [SerializeField] private TMP_Text conceptCorrectFeedbackText;
    [SerializeField] private GameObject conceptCorrectNextTriangle;
    [SerializeField] private GameObject conceptCorrectConfirmButton;

    private int conceptCorrectFeedbackStep = 0;

    [SerializeField] private GameObject playerTransferDialogueUI;
    [SerializeField] private TMP_Text playerTransferNameText;
    [SerializeField] private AudioSource correctAudio;

    [SerializeField] private GameObject sustainabilityStoryPanel;
    [SerializeField] private TMP_Text sustainabilityStoryText;
    [SerializeField] private GameObject sustainabilityStoryNextTriangle;
    [SerializeField] private GameObject sustainabilityStoryConfirmButton;

    [SerializeField] private GameObject sustainabilityQuestionPanel;
    [SerializeField] private GameObject sustainabilityWrongPanel;
    [SerializeField] private GameObject sustainabilityResultPanel;

    [Header("Sustainability Feedback Version")]
    [SerializeField] private GameObject sustainabilityDeepWrongPanel;

    [SerializeField] private GameObject sustainabilityDeepCorrectPanel;
    [SerializeField] private TMP_Text sustainabilityDeepCorrectText;
    [SerializeField] private GameObject sustainabilityDeepCorrectNextTriangle;
    [SerializeField] private GameObject sustainabilityDeepCorrectConfirmButton;

    private int sustainabilityDeepCorrectStep = 0;

    [SerializeField] private GameObject playerSustainabilityDialogueUI;
    [SerializeField] private TMP_Text playerSustainabilityNameText;

    private int sustainabilityStoryStep = 0;

    // 防止結尾按鈕重複觸發時，Experiment_Results.csv 被寫入兩次
    private bool experimentResultExported = false;

    private void Start()
    {
        // 研究紀錄：Ending 從本場景開始計時
        GameData.StartEndingTimer();

        transferQuestionPanel.SetActive(false);
        conceptFillPanel.SetActive(false);

        wrongPanel.SetActive(false);
        conceptWarningPanel.SetActive(false);
        conceptHintPanel.SetActive(false);

        conceptCorrectFeedbackPanel.SetActive(false);
        conceptCorrectConfirmButton.SetActive(false);
        
        playerTransferDialogueUI.SetActive(false);
        playerTransferNameText.text = GameData.PlayerName;

        sustainabilityStoryPanel.SetActive(false);
        sustainabilityQuestionPanel.SetActive(false);

        sustainabilityStoryConfirmButton.SetActive(false);
        sustainabilityWrongPanel.SetActive(false);
        sustainabilityResultPanel.SetActive(false);

        sustainabilityDeepWrongPanel.SetActive(false);
        sustainabilityDeepCorrectPanel.SetActive(false);
        sustainabilityDeepCorrectConfirmButton.SetActive(false);

        playerSustainabilityDialogueUI.SetActive(false);

        playerSustainabilityNameText.text = GameData.PlayerName;
    }

    public void StartTransferQuestion()
    {
        transferQuestionPanel.SetActive(true);
    }

    public void ShowConceptFillPanel()
    {
        transferQuestionPanel.SetActive(false);
        conceptFillPanel.SetActive(true);

        // 研究紀錄：
        // END_TRANSFER_TEXT = 三欄建構反應的整體提交
        // END_IDLE / END_RIGHT / END_PLATFORM = 同一次提交中的三個概念維度
        GameData.StartTask(GameData.TaskIds.END_TRANSFER_TEXT);
        GameData.StartParallelTask(GameData.TaskIds.END_IDLE);
        GameData.StartParallelTask(GameData.TaskIds.END_RIGHT);
        GameData.StartParallelTask(GameData.TaskIds.END_PLATFORM);
    }

    public void CheckConceptAnswers()
    {
        string answer1 = answerInput1.text.Trim();
        string answer2 = answerInput2.text.Trim();
        string answer3 = answerInput3.text.Trim();

        bool answer1Correct =
            answer1.Contains("閒置") ||
            answer1.Contains("社團資產") ||
            answer1.Contains("社團物品") ||
            answer1.Contains("社團財產") ||
            answer1.Contains("社團設備") ||
            answer1.Contains("公用資產") ||
            answer1.Contains("社團器材") ||
            answer1.Contains("公有資產") ||
            answer1.Contains("公用財產") ||
            answer1.Contains("公有財產") ||
            answer1.Contains("攝影機") ||
            answer1.Contains("相機") ||
            answer1.Contains("器材") ||
            answer1.Contains("設備") ;

        bool answer2Correct =
            !answer2.Contains("所有權") &&
            (answer2.Contains("暫時") ||
            answer2.Contains("短期") ||
            answer2.Contains("短暫") ||
            answer2.Contains("兩天") ||
            answer2.Contains("使用權"));

        bool answer3Correct =
            answer3.Contains("平台") ||
            answer3.Contains("媒合");

        bool allTransferCorrect =
            answer1Correct &&
            answer2Correct &&
            answer3Correct;

        // 研究紀錄：整體建構反應保留三欄原文
        string combinedAnswer =
            "IDLE=" + answer1 +
            "|RIGHT=" + answer2 +
            "|PLATFORM=" + answer3;

        GameData.RecordAnswer(
            combinedAnswer,
            allTransferCorrect
        );

        // 三個概念維度獨立追蹤。
        // 某一概念第一次答對後即完成，不再因後續重送而增加 Attempt。
        if (GameData.RecordParallelAnswer(
            GameData.TaskIds.END_IDLE,
            answer1,
            answer1Correct
        ) && answer1Correct)
        {
            GameData.CompleteParallelTask(GameData.TaskIds.END_IDLE);
        }

        if (GameData.RecordParallelAnswer(
            GameData.TaskIds.END_RIGHT,
            answer2,
            answer2Correct
        ) && answer2Correct)
        {
            GameData.CompleteParallelTask(GameData.TaskIds.END_RIGHT);
        }

        if (GameData.RecordParallelAnswer(
            GameData.TaskIds.END_PLATFORM,
            answer3,
            answer3Correct
        ) && answer3Correct)
        {
            GameData.CompleteParallelTask(GameData.TaskIds.END_PLATFORM);
        }

        if (!allTransferCorrect)
        {
            // 舊版摘要欄位保留
            GameData.Ending_ConceptTransferErrors++;
            if (!answer1Correct) GameData.Ending_IdleAssetErrors++;
            if (!answer2Correct) GameData.Ending_UsageRightErrors++;
            if (!answer3Correct) GameData.Ending_MatchingMethodErrors++;

            if (wrongAudio != null)
            {
                wrongAudio.Play();
            }

            if (GameData.FeedbackMode == FeedbackMode.Simple)
            {
                ShowSimpleConceptWrong(
                    answer1Correct,
                    answer2Correct,
                    answer3Correct
                );
            }
            else
            {
                ShowDeepConceptWrong(
                    answer1Correct,
                    answer2Correct,
                    answer3Correct
                );
            }

            // 一次錯誤提交視為一次 Low / High 回饋套件
            GameData.RecordFeedbackShown("END_TRANSFER_TEXT_WRONG");
        }
        else
        {
            if (correctAudio != null)
            {
                correctAudio.Play();
            }

            conceptFillPanel.SetActive(false);

            if (GameData.FeedbackMode == FeedbackMode.Simple)
            {
                playerTransferDialogueUI.SetActive(true);
            }
            else
            {
                ShowDeepConceptCorrectFeedback();
            }

            GameData.RecordFeedbackShown("END_TRANSFER_TEXT_CORRECT");
            GameData.CompleteTask();
        }
    }

    private void ShowSimpleConceptWrong(bool answer1Correct, bool answer2Correct, bool answer3Correct)
    {
        List<string> wrongConcepts = new List<string>();

        if (!answer1Correct)
        {
            wrongConcepts.Add("閒置資產");
        }

        if (!answer2Correct)
        {
            wrongConcepts.Add("權利類型");
        }

        if (!answer3Correct)
        {
            wrongConcepts.Add("媒合方式");
        }

        wrongText.text =
            string.Join("、", wrongConcepts) +
            "概念有誤！請重新確認。";

        wrongPanel.SetActive(true);
    }

    private void ShowDeepConceptWrong(bool answer1Correct, bool answer2Correct, bool answer3Correct)
    {
        wrongIdleAsset = !answer1Correct;
        wrongUsageRight = !answer2Correct;
        wrongMatchingMethod = !answer3Correct;

        List<string> wrongConcepts = new List<string>();

        if (wrongIdleAsset)
        {
            wrongConcepts.Add("閒置資產");
        }

        if (wrongUsageRight)
        {
            wrongConcepts.Add("權利類型");
        }

        if (wrongMatchingMethod)
        {
            wrongConcepts.Add("媒合方式");
        }

        conceptWarningText.text =
            "【" + string.Join("、", wrongConcepts) +
            "有誤】";

        conceptFillPanel.SetActive(false);
        conceptWarningPanel.SetActive(true);
    }

    public void CloseConceptWarning()
    {
        conceptWarningPanel.SetActive(false);

        List<string> hints = new List<string>();

        if (wrongIdleAsset)
        {
            hints.Add(
                "【閒置資產】哪項資源『原本就存在』，且在沒有社團活動時暫時不會使用？"
            );
        }

        if (wrongUsageRight)
        {
            hints.Add(
                "【權利類型】借用兩天後需歸還攝影機，所以學生取得的是攝影機本身，還是特定期間內使用的權利？"
            );
        }

        if (wrongMatchingMethod)
        {
            hints.Add(
                "【媒合方式】甚麼東西在學生和攝影社之間，幫忙完成了搜尋、驗證、付款與交易管理工作？" 
            );
        }

        conceptHintText.text =
            string.Join("\n", hints);

        conceptHintPanel.SetActive(true);
    }

    public void CloseConceptHint()
    {
        conceptHintPanel.SetActive(false);
        conceptFillPanel.SetActive(true);
    }

    public void CloseWrongPanel()
    {
        wrongPanel.SetActive(false);
    }

    private void ShowDeepConceptCorrectFeedback()
    {
        conceptCorrectFeedbackStep = 0;

        conceptCorrectFeedbackPanel.SetActive(true);

        conceptCorrectFeedbackText.text =
            "雖然資源從汽車、書籍、空間變成攝影機，但判斷結構沒有改變。";

        conceptCorrectNextTriangle.SetActive(true);
        conceptCorrectConfirmButton.SetActive(false);
    }

    public void NextConceptCorrectFeedback()
    {
        if (conceptCorrectFeedbackStep == 0)
        {
            conceptCorrectFeedbackText.text =
                "攝影機在沒有社團活動時，是尚未被充分使用的資源。\n學生借用是取得兩天的暫時使用權，而不是攝影機的所有權。\n校園平台則負責連結供需，並透過驗證、付款、押金與歸還紀錄協助交易完成。";

            conceptCorrectFeedbackStep = 1;
        }
        else if (conceptCorrectFeedbackStep == 1)
        {
            conceptCorrectFeedbackText.text =
                "真正的概念遷移，就是不依靠熟悉的共享經濟經典代表，例如早期的Uber，也能用相同原則判斷新的共享情境。";

            conceptCorrectNextTriangle.SetActive(false);
            conceptCorrectConfirmButton.SetActive(true);

            conceptCorrectFeedbackStep = 2;
        }
    }

    public void CloseConceptCorrectFeedback()
    {
        conceptCorrectFeedbackPanel.SetActive(false);
        playerTransferDialogueUI.SetActive(true);
    }
    
    public void ShowSustainabilityStory()
    {
        playerTransferDialogueUI.SetActive(false);

        sustainabilityStoryPanel.SetActive(true);

        sustainabilityStoryStep = 0;

        sustainabilityStoryNextTriangle.SetActive(true);
        sustainabilityStoryConfirmButton.SetActive(false);
    }

    public void NextSustainabilityStory()
    {
        if (sustainabilityStoryStep == 0)
        {
            sustainabilityStoryText.text =
                "平台交易量大增，但器材運送次數、包材使用與維修頻率也跟著增加。";

            sustainabilityStoryNextTriangle.SetActive(false);
            sustainabilityStoryConfirmButton.SetActive(true);

            sustainabilityStoryStep = 1;
        }
    }

    public void ShowSustainabilityQuestion()
    {
        sustainabilityStoryPanel.SetActive(false);
        sustainabilityQuestionPanel.SetActive(true);

        // 研究紀錄：永續判斷題正式開始
        GameData.StartTask(GameData.TaskIds.END_SUSTAIN);
    }

    public void ChooseSustainabilityA()
    {
        ShowSustainabilityWrong("A");
    }

    public void ChooseSustainabilityB()
    {
        // 研究紀錄：B 為正確答案
        GameData.RecordAnswer("B", true);

        if (correctAudio != null)
        {
            correctAudio.Play();
        }

        sustainabilityQuestionPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            sustainabilityResultPanel.SetActive(true);
        }
        else
        {
            ShowDeepSustainabilityCorrect();
        }

        GameData.RecordFeedbackShown("END_SUSTAIN_CORRECT");
        GameData.CompleteTask();
    }

    public void ChooseSustainabilityC()
    {
        ShowSustainabilityWrong("C");
    }

    private void ShowSustainabilityWrong(string selectedAnswer)
    {
        // 研究紀錄：A / C 為錯誤答案
        GameData.RecordAnswer(selectedAnswer, false);

        // 舊版摘要欄位保留
        GameData.Ending_SustainabilityErrors++;

        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        sustainabilityQuestionPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            sustainabilityWrongPanel.SetActive(true);
        }
        else
        {
            sustainabilityDeepWrongPanel.SetActive(true);
        }

        GameData.RecordFeedbackShown("END_SUSTAIN_WRONG");
    }

    public void CloseSustainabilityWrongPanel()
    {
        sustainabilityWrongPanel.SetActive(false);
        sustainabilityQuestionPanel.SetActive(true);
    }

    public void CloseSustainabilityDeepWrongPanel()
    {
        sustainabilityDeepWrongPanel.SetActive(false);
        sustainabilityQuestionPanel.SetActive(true);
    }

    private void ShowDeepSustainabilityCorrect()
    {
        sustainabilityDeepCorrectStep = 0;

        sustainabilityDeepCorrectPanel.SetActive(true);
        sustainabilityDeepCorrectNextTriangle.SetActive(true);
        sustainabilityDeepCorrectConfirmButton.SetActive(false);

        sustainabilityDeepCorrectText.text =
            "共享確實可能提高既有資源的利用率，並減少部分重複購買，但如果低價格與便利性刺激了大量原本不存在的新需求，運送、使用、維修與耗材也可能同步增加。";
    }

    public void NextDeepSustainabilityCorrect()
    {
        if (sustainabilityDeepCorrectStep == 0)
        {
            sustainabilityDeepCorrectText.text =
                "這種新增需求可能抵銷原本節省下來的資源效益。";

            sustainabilityDeepCorrectStep = 1;
        }
        else if (sustainabilityDeepCorrectStep == 1)
        {
            sustainabilityDeepCorrectText.text =
                "所以評估共享服務是否更永續，不能只問「有沒有共享」，而要進一步問兩件事：\n" +
                "1、它取代了什麼？\n" +
                "2、它又額外增加了什麼？";

            sustainabilityDeepCorrectNextTriangle.SetActive(false);
            sustainabilityDeepCorrectConfirmButton.SetActive(true);

            sustainabilityDeepCorrectStep = 2;
        }
    }

    public void CloseDeepSustainabilityCorrect()
    {
        sustainabilityDeepCorrectPanel.SetActive(false);
        playerSustainabilityDialogueUI.SetActive(true);
    }

    public void ShowPlayerSustainabilityDialogue()
    {
        sustainabilityResultPanel.SetActive(false);
        playerSustainabilityDialogueUI.SetActive(true);
    }

    public void EnterEndingHatching()
    {
        // Ending 的正式互動題到此全部結束
        GameData.StopEndingTimer();

        // 研究紀錄：每位受試者整場遊戲只匯出一次摘要資料
        if (!experimentResultExported)
        {
            GameData.ExportToCSV();
            experimentResultExported = true;
        }

        SceneManager.LoadScene("EndingHatching");
    }
}