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

    [SerializeField] private GameObject playerSustainabilityDialogueUI;
    [SerializeField] private TMP_Text playerSustainabilityNameText;

    private int sustainabilityStoryStep = 0;

    private void Start()
    {
        transferQuestionPanel.SetActive(true);
        conceptFillPanel.SetActive(false);

        wrongPanel.SetActive(false);
        
        playerTransferDialogueUI.SetActive(false);
        playerTransferNameText.text = GameData.PlayerName;

        sustainabilityStoryPanel.SetActive(false);
        sustainabilityQuestionPanel.SetActive(false);

        sustainabilityStoryConfirmButton.SetActive(false);
        sustainabilityWrongPanel.SetActive(false);
        sustainabilityResultPanel.SetActive(false);
        playerSustainabilityDialogueUI.SetActive(false);

        playerSustainabilityNameText.text = GameData.PlayerName;
    }

    public void ShowConceptFillPanel()
    {
        transferQuestionPanel.SetActive(false);
        conceptFillPanel.SetActive(true);
    }

    public void CheckConceptAnswers()
    {
        string answer1 = answerInput1.text.Trim();
        string answer2 = answerInput2.text.Trim();
        string answer3 = answerInput3.text.Trim();

        bool answer1Correct =
            answer1.Contains("閒置") ||
            answer1.Contains("社團資產") ||
            answer1.Contains("攝影機");

        bool answer2Correct =
            answer2.Contains("使用權");

        bool answer3Correct =
            answer3.Contains("平台") ||
            answer3.Contains("媒合");

        List<string> wrongConcepts = new List<string>();

        if (!answer1Correct)
        {
            wrongConcepts.Add("閒置資產");
        }

        if (!answer2Correct)
        {
            wrongConcepts.Add("暫時使用權");
        }

        if (!answer3Correct)
        {
            wrongConcepts.Add("數位平台媒合");
        }

        if (wrongConcepts.Count > 0)
        {
            if (wrongAudio != null)
            {
                wrongAudio.Play();
            }

            wrongText.text =
                string.Join("、", wrongConcepts) +
                "概念配錯囉！請重新確認。";

            wrongPanel.SetActive(true);
        }
        else
        {
            if (correctAudio != null)
            {
                correctAudio.Play();
            }

            conceptFillPanel.SetActive(false);
            playerTransferDialogueUI.SetActive(true);
        }
    }

    public void CloseWrongPanel()
    {
        wrongPanel.SetActive(false);
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
    }

    public void ChooseSustainabilityA()
    {
        ShowSustainabilityWrong();
    }

    public void ChooseSustainabilityB()
    {
        if (correctAudio != null)
        {
            correctAudio.Play();
        }

        sustainabilityQuestionPanel.SetActive(false);
        sustainabilityResultPanel.SetActive(true);
    }

    public void ChooseSustainabilityC()
    {
        ShowSustainabilityWrong();
    }

    private void ShowSustainabilityWrong()
    {
        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        sustainabilityWrongPanel.SetActive(true);
    }

    public void CloseSustainabilityWrongPanel()
    {
        sustainabilityWrongPanel.SetActive(false);
    }

    public void ShowPlayerSustainabilityDialogue()
    {
        sustainabilityResultPanel.SetActive(false);
        playerSustainabilityDialogueUI.SetActive(true);
    }

    public void EnterEndingHatching()
    {
        SceneManager.LoadScene("EndingHatching");
    }
}