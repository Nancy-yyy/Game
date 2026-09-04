using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Case1CarManager : MonoBehaviour
{
    // 版本判斷
    [Header("Feedback Version")]
    [SerializeField] private TMP_Text driverWrongText;
    [SerializeField] private TMP_Text oilTankWrongText;

    [SerializeField] private GameObject systemAnalysisPanel;
    [SerializeField] private TMP_Text systemAnalysisText;
        
    [SerializeField]
    private GameObject systemAssetPanel;

    [SerializeField]
    private GameObject systemCarExplainPanel;

    [SerializeField]
    private GameObject fullCarImage;

    [SerializeField]
    private TMP_Text carExplainText;

    [SerializeField]
    private Image carExplainImage;

    [SerializeField]
    private Sprite transparentCarSprite;

    [SerializeField]
    private Sprite idleSeatSprite;

    [SerializeField]
    private GameObject playerDialogueUI;

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private GameObject birdDialogueUI;

    [SerializeField]
    private GameObject carInteractionPanel;

    [SerializeField]
    private GameObject interactionHintUI;

    [SerializeField]
    private GameObject interactiveCarImage;

    [SerializeField]
    private GameObject driverSeatPreviewImage;

    [SerializeField]
    private GameObject oilTankPreviewImage;

    [SerializeField]
    private GameObject trunkPreviewImage;

    [SerializeField]
    private GameObject driverSeatWrongPanel;

    [SerializeField]
    private GameObject oilTankWrongPanel;

    [SerializeField]
    private AudioSource correctAudio;

    [SerializeField]
    private AudioSource wrongAudio;

    [SerializeField]
    private GameObject playerEndDialogueUI;

    [SerializeField]
    private TMP_Text playerEndNameText;

    [SerializeField]
    private GameObject playerEndNextTriangle;

    [SerializeField]
    private GameObject birdEndDialogueUI;
    
    private int carExplainStep = 0;

    // 變數：用來記住看完解析後要做什麼
    private enum AnalysisNextAction
    {
        ReturnToQuestion,
        ContinueAfterTrunk
    }

    private AnalysisNextAction analysisNextAction;

    private void Start()
    {
        systemAssetPanel.SetActive(false);
        StartCoroutine(ShowSystemAssetPanelAfterDelay());
        
        systemCarExplainPanel.SetActive(false);
        playerDialogueUI.SetActive(false);

        fullCarImage.SetActive(true);
        
        playerNameText.text = GameData.PlayerName;
        playerEndNameText.text = GameData.PlayerName;

        birdDialogueUI.SetActive(false);

        carInteractionPanel.SetActive(false);
        interactionHintUI.SetActive(false);

        driverSeatPreviewImage.SetActive(false);
        oilTankPreviewImage.SetActive(false);
        trunkPreviewImage.SetActive(false);

        driverSeatWrongPanel.SetActive(false);
        oilTankWrongPanel.SetActive(false);
        systemAnalysisPanel.SetActive(false);

        playerEndDialogueUI.SetActive(false);
        birdEndDialogueUI.SetActive(false);
    }

    private IEnumerator ShowSystemAssetPanelAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        systemAssetPanel.SetActive(true);
    }

    public void ShowCarExplainPanel()
    {
        systemAssetPanel.SetActive(false);

        fullCarImage.SetActive(false);

        systemCarExplainPanel.SetActive(true);

        carExplainStep = 0;

        carExplainText.text =
            "接著看看車內空間。";

        carExplainImage.sprite =
            transparentCarSprite;
    }

    public void NextCarExplain()
    {
        carExplainStep++;

        if (carExplainStep == 1)
        {
            carExplainText.text =
                "「駕駛座以外的空位」就是沒有乘客使用的「閒置空位」！";

            carExplainImage.sprite =
                idleSeatSprite;
        }
        else if (carExplainStep == 2)
        {
            systemCarExplainPanel.SetActive(false);

            fullCarImage.SetActive(true);

            playerDialogueUI.SetActive(true);
        }
    }

    public void ShowBirdDialogue()
    {
        playerDialogueUI.SetActive(false);
        birdDialogueUI.SetActive(true);
    }

    public void StartCarInteraction()
    {
        birdDialogueUI.SetActive(false);

        fullCarImage.SetActive(false);

        carInteractionPanel.SetActive(true);
        interactionHintUI.SetActive(true);
    }

    public void ShowDriverSeatPreview()
    {
        interactiveCarImage.SetActive(false);
        driverSeatPreviewImage.SetActive(true);
    }

    public void HideDriverSeatPreview()
    {
        driverSeatPreviewImage.SetActive(false);
        interactiveCarImage.SetActive(true);
    }

    public void ShowOilTankPreview()
    {
        interactiveCarImage.SetActive(false);
        oilTankPreviewImage.SetActive(true);
    }

    public void HideOilTankPreview()
    {
        oilTankPreviewImage.SetActive(false);
        interactiveCarImage.SetActive(true);
    }

    public void ShowTrunkPreview()
    {
        interactiveCarImage.SetActive(false);
        trunkPreviewImage.SetActive(true);
    }

    public void HideTrunkPreview()
    {
        trunkPreviewImage.SetActive(false);
        interactiveCarImage.SetActive(true);
    }

    public void ClickDriverSeat()
    {
        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        carInteractionPanel.SetActive(false);
        interactionHintUI.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            driverWrongText.text =
                "選錯了！駕駛座已經坐著學姐了啦！";
        }
        else
        {
            driverWrongText.text =
                "選錯了！駕駛座目前已經由學姐使用，因此它不是閒置容量。";
        }
        
        driverSeatWrongPanel.SetActive(true);
    }

    public void CloseDriverSeatWrongPanel()
    {
        driverSeatWrongPanel.SetActive(false);
        driverSeatPreviewImage.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Deep)
        {
            analysisNextAction = AnalysisNextAction.ReturnToQuestion;

            systemAnalysisText.text =
                "「存在」不代表「閒置」。只有目前未被使用、又可以提供其他用途的部分，才具有重新配置的可能。";

            systemAnalysisPanel.SetActive(true);
        }
        else
        {
            interactiveCarImage.SetActive(true);
            carInteractionPanel.SetActive(true);
            interactionHintUI.SetActive(true);
        }
    }

    public void ClickOilTank()
    {
        if (wrongAudio != null)
        {
            wrongAudio.Play();
        }

        carInteractionPanel.SetActive(false);
        interactionHintUI.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            oilTankWrongText.text =
                "選錯了！這裡不是本題要找的閒置空間，再試一次吧！";
        }
        else
        {
            oilTankWrongText.text =
                "選錯了！汽油會隨著行程被消耗，它屬於這趟旅程需要使用的能源，而不是可以另外提供他人使用的「剩餘容量」。";
        }

        oilTankWrongPanel.SetActive(true);
    }

    public void CloseOilTankWrongPanel()
    {
        oilTankWrongPanel.SetActive(false);
        oilTankPreviewImage.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Deep)
        {
            analysisNextAction = AnalysisNextAction.ReturnToQuestion;

            systemAnalysisText.text =
                "判斷閒置資源時，要找的是「原本已經存在，但目前沒有被充分利用的資源或容量」。";

            systemAnalysisPanel.SetActive(true);
        }
        else
        {
            interactiveCarImage.SetActive(true);
            carInteractionPanel.SetActive(true);
            interactionHintUI.SetActive(true);
        }
    }

    public void ClickTrunk()
    {
        interactionHintUI.SetActive(false);

        carInteractionPanel.SetActive(false);

        fullCarImage.SetActive(true);

        if (correctAudio != null)
        {
            correctAudio.Play();
        }

        if (GameData.FeedbackMode == FeedbackMode.Deep)
        {
            analysisNextAction = AnalysisNextAction.ContinueAfterTrunk;

            systemAnalysisText.text =
                "共享經濟不一定要創造新的資源。\n像空座位、剩餘行李空間，都可以是原本存在卻沒有被充分利用的容量。若透過媒合讓其他有需求的人使用，就能提高既有資產的利用程度。";

            systemAnalysisPanel.SetActive(true);
        }
        else
        {
            playerEndDialogueUI.SetActive(true);
        }
    }

    public void CloseSystemAnalysis()
    {
        systemAnalysisPanel.SetActive(false);

        if (analysisNextAction == AnalysisNextAction.ReturnToQuestion)
        {
            interactiveCarImage.SetActive(true);

            driverSeatPreviewImage.SetActive(false);
            oilTankPreviewImage.SetActive(false);
            trunkPreviewImage.SetActive(false);

            carInteractionPanel.SetActive(true);
            interactionHintUI.SetActive(true);
        }
        else if (analysisNextAction == AnalysisNextAction.ContinueAfterTrunk)
        {
            playerEndDialogueUI.SetActive(true);
        }
    }

    public void ShowBirdEndDialogue()
    {
        birdEndDialogueUI.SetActive(true);
        playerEndNextTriangle.SetActive(false);
    }

    public void EnterCase1Matching()
    {
        SceneManager.LoadScene("Case1Matching");
    }
}