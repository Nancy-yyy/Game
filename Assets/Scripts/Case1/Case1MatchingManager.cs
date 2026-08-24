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
    [SerializeField] private TMP_Text playerDialogueText;
    [SerializeField] private GameObject playerNextTriangle;
    [SerializeField] private Image playerImage;

    [Header("Player Sprites")]
    [SerializeField] private Sprite playerFirstSprite;
    [SerializeField] private Sprite playerSecondSprite;


    [Header("Bird UI")]
    [SerializeField] private GameObject birdDialogueUI;
    [SerializeField] private TMP_Text birdDialogueText;
    [SerializeField] private GameObject birdNextTriangle;
    [SerializeField] private Image birdImage;
    [SerializeField] private Image birdDialogueBoxImage;

    [Header("Bird Sprites")]
    [SerializeField] private Sprite birdFirstSprite;
    [SerializeField] private Sprite birdSecondSprite;

    [Header("Bird Dialogue Box Sprites")]
    [SerializeField] private Sprite birdFirstDialogueBoxSprite;
    [SerializeField] private Sprite birdSecondDialogueBoxSprite;

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
    [SerializeField] private AudioSource wrongAudio;
    [SerializeField] private AudioSource correctAudio;

    [SerializeField] private GameObject playerSeatDialogueUI;
    [SerializeField] private TMP_Text playerSeatNameText;

    [SerializeField] private GameObject supplyQuestionPanel;
    [SerializeField] private GameObject questionWrongPanel;

    [SerializeField] private GameObject matchingSuccessPanel;
    [SerializeField] private TMP_Text matchingSuccessText;
    [SerializeField] private GameObject matchingSuccessNextTriangle;
    [SerializeField] private GameObject matchingSuccessConfirmButton;
    [SerializeField] private GameObject sustainabilityWarningPanel;

    private int dialogueStep = 0;

    private int selectedCount = 0;

    private List<string> selectedOrder = new List<string>();
    private List<GameObject> selectedCards = new List<GameObject>();

    private void Start()
    {
        playerDialogueUI.SetActive(true);
        birdDialogueUI.SetActive(false);

        playerNameText.text = GameData.PlayerName;
        playerSeatNameText.text = GameData.PlayerName;

        playerDialogueText.text =
            "那我還有一個問題... 共乘會比客運便宜嗎？";

        playerImage.sprite = playerFirstSprite;

        playerNextTriangle.SetActive(true);
        matchingOrderPanel.SetActive(false);

        wrongPanel.SetActive(false);
        playerSeatDialogueUI.SetActive(false);
        supplyQuestionPanel.SetActive(false);

        questionWrongPanel.SetActive(false);
        matchingSuccessPanel.SetActive(false);
        matchingSuccessConfirmButton.SetActive(false);
        sustainabilityWarningPanel.SetActive(false);
    }


    public void OnPlayerDialogueClicked()
    {
        if (dialogueStep == 0)
        {
            playerNextTriangle.SetActive(false);

            birdDialogueUI.SetActive(true);

            birdDialogueText.text =
                "通常會依行程與交通成本分攤，但不一定每一趟都比其他交通方式便宜！";

            birdImage.sprite = birdFirstSprite;
            birdDialogueBoxImage.sprite = birdFirstDialogueBoxSprite;

            birdNextTriangle.SetActive(true);

            dialogueStep = 1;
        }

        else if (dialogueStep == 2)
        {
            playerNextTriangle.SetActive(false);

            birdDialogueText.text =
                "等等！你這樣會被當成變態打出來好嗎！還需要『媒合行程與時間』才算完成！";

            birdImage.sprite = birdSecondSprite;
            birdDialogueBoxImage.sprite = birdSecondDialogueBoxSprite;

            birdNextTriangle.SetActive(true);

            dialogueStep = 3;
        }
    }

    public void OnBirdDialogueClicked()
    {
        if (dialogueStep == 1)
        {
            birdNextTriangle.SetActive(false);

            playerDialogueText.text =
                "好的，那我現在直接去學姐家找她嗎？";

            playerImage.sprite = playerSecondSprite;

            playerNextTriangle.SetActive(true);

            dialogueStep = 2;
        }

        else if (dialogueStep == 3)
        {
            birdNextTriangle.SetActive(false);

            playerDialogueUI.SetActive(false);
            birdDialogueUI.SetActive(false);

            matchingOrderPanel.SetActive(true);

            dialogueStep = 4;
        }
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

            matchingOrderPanel.SetActive(false);
            playerSeatDialogueUI.SetActive(true);
        }
        else
        {
            if (wrongAudio != null)
            {
                wrongAudio.Play();
            }

            wrongPanel.SetActive(true);
        }
    }

    public void CloseWrongPanel()
    {
        wrongPanel.SetActive(false);
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
        matchingSuccessPanel.SetActive(true);
        matchingSuccessNextTriangle.SetActive(true);
        matchingSuccessConfirmButton.SetActive(false);
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

        questionWrongPanel.SetActive(true);
    }

    public void CloseQuestionWrongPanel()
    {
        questionWrongPanel.SetActive(false);
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

    public void EnterCase1Travel()
    {
        SceneManager.LoadScene("Case1Travel");
    }
}