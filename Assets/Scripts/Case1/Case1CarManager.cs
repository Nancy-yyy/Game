using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Case1CarManager : MonoBehaviour
{
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
    private GameObject playerEndDialogueUI;

    [SerializeField]
    private TMP_Text playerEndNameText;

    [SerializeField]
    private GameObject playerEndNextTriangle;

    [SerializeField]
    private GameObject birdEndDialogueUI;
    
    private int carExplainStep = 0;

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
        carInteractionPanel.SetActive(false);
        interactionHintUI.SetActive(false);
        driverSeatWrongPanel.SetActive(true);
    }

    public void CloseDriverSeatWrongPanel()
    {
        driverSeatWrongPanel.SetActive(false);
        driverSeatPreviewImage.SetActive(false);
        interactiveCarImage.SetActive(true);
        carInteractionPanel.SetActive(true);
        interactionHintUI.SetActive(true);
    }

    public void ClickOilTank()
    {
        carInteractionPanel.SetActive(false);
        interactionHintUI.SetActive(false);
        oilTankWrongPanel.SetActive(true);
    }

    public void CloseOilTankWrongPanel()
    {
        oilTankWrongPanel.SetActive(false);
        oilTankPreviewImage.SetActive(false);
        interactiveCarImage.SetActive(true);
        carInteractionPanel.SetActive(true);
        interactionHintUI.SetActive(true);
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

        playerEndDialogueUI.SetActive(true);
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