using TMPro;
using UnityEngine;

public class EndingDialogueManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private GameObject playerDialogueUI;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerDialogueText;

    [Header("Leader UI")]
    [SerializeField] private GameObject leaderDialogueUI;
    [SerializeField] private TMP_Text leaderDialogueText;

    [Header("Student A UI")]
    [SerializeField] private GameObject studentDialogueUI;
    [SerializeField] private TMP_Text studentDialogueText;

    [Header("Ending Flow")]
    [SerializeField] private EndingManager endingManager;

    private int dialogueStep = 0;

    private void Start()
    {
        playerNameText.text = GameData.PlayerName;

        playerDialogueUI.SetActive(false);
        leaderDialogueUI.SetActive(false);
        studentDialogueUI.SetActive(false);

        ShowDialogue();
    }

    public void NextDialogue()
    {
        dialogueStep++;
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        playerDialogueUI.SetActive(false);
        leaderDialogueUI.SetActive(false);
        studentDialogueUI.SetActive(false);

        switch (dialogueStep)
        {
            case 0:
                playerDialogueUI.SetActive(true);
                playerDialogueText.text =
                    "社長好~ 終於加入夢寐以求的社團，超級開心~";
                break;

            case 1:
                leaderDialogueUI.SetActive(true);
                leaderDialogueText.text =
                    "歡迎加入攝影社！我們社團目前有三台攝影機，不過平常只有社團活動的時候才會使用，所以其他時間大多都放在社辦裡。";
                break;

            case 2:
                studentDialogueUI.SetActive(true);
                studentDialogueText.text =
                    "社長好，我想透過校園平台跟攝影社租一台攝影機，可以借兩天嗎？";
                break;

            case 3:
                playerDialogueUI.SetActive(true);
                playerDialogueText.text =
                    "原來平常沒用到的器材，也可以透過校園平台借給其他同學！";
                break;

            case 4:
                leaderDialogueUI.SetActive(true);
                leaderDialogueText.text =
                    "對啊！校園平台可以透過身分驗證、器材狀況紀錄、押金與歸還提醒機制，讓其他學生安全、方便地租借閒置器材。";
                break;

            case 5:
                playerDialogueUI.SetActive(true);
                playerDialogueText.text =
                    "這樣器材不會一直閒置，大家也能更方便地使用校園資源耶~";
                break;

            case 6:
                playerDialogueUI.SetActive(false);
                leaderDialogueUI.SetActive(false);
                studentDialogueUI.SetActive(false);

                endingManager.StartTransferQuestion();
                break;
        }
    }
}