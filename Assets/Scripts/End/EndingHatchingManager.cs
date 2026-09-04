using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class EndingHatchingManager : MonoBehaviour
{
    [Header("Hatching Video")]
    [SerializeField] private VideoPlayer hatchingVideoPlayer;

    [Header("Audio")]
    [SerializeField] private AudioSource correctAudio;

    [Header("Ability Checklist")]
    [SerializeField] private GameObject abilityChecklistPanel;

    [SerializeField] private GameObject checklistImage1;
    [SerializeField] private GameObject checklistImage2;
    [SerializeField] private GameObject checklistImage3;
    [SerializeField] private GameObject checklistImage4;
    [SerializeField] private GameObject checklistImage5;

    [Header("Deep Summary")]
    [SerializeField] private GameObject deepSummaryPanel;
    [SerializeField] private TMP_Text deepSummaryText;
    [SerializeField] private GameObject deepSummaryNextTriangle;
    [SerializeField] private GameObject deepSummaryConfirmButton;

    private int deepSummaryStep = 0;

    [Header("Hatching Condition")]
    [SerializeField] private GameObject hatchingConditionPanel;

    [Header("Bird Dialogue")]
    [SerializeField] private GameObject birdDialogueUI;
    [SerializeField] private TMP_Text birdDialogueText;
    [SerializeField] private GameObject birdNextTriangle;

    private int birdDialogueStep = 0;

    [SerializeField] private GameObject mainCanvas;

    private void Start()
    {
        abilityChecklistPanel.SetActive(false);

        checklistImage1.SetActive(false);
        checklistImage2.SetActive(false);
        checklistImage3.SetActive(false);
        checklistImage4.SetActive(false);
        checklistImage5.SetActive(false);

        deepSummaryPanel.SetActive(false);
        deepSummaryConfirmButton.SetActive(false);

        hatchingConditionPanel.SetActive(false);

        birdDialogueUI.SetActive(false);

        hatchingVideoPlayer.gameObject.SetActive(false);

        StartCoroutine(ShowChecklist());
    }

    private void PlayCorrectAudio()
    {
        if (correctAudio != null)
        {
            correctAudio.Play();
        }
    }

    private IEnumerator ShowChecklist()
    {
        abilityChecklistPanel.SetActive(true);

        checklistImage1.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(1.0f);

        checklistImage1.SetActive(false);
        checklistImage2.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(1.0f);

        checklistImage2.SetActive(false);
        checklistImage3.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(1.0f);

        checklistImage3.SetActive(false);
        checklistImage4.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(1.0f);

        checklistImage4.SetActive(false);
        checklistImage5.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(1.5f);

        abilityChecklistPanel.SetActive(false);

        if (GameData.FeedbackMode == FeedbackMode.Simple)
        {
            hatchingConditionPanel.SetActive(true);
        }
        else
        {
            ShowDeepSummary();
        }
    }

    private void ShowDeepSummary()
    {
        deepSummaryStep = 0;

        deepSummaryPanel.SetActive(true);

        deepSummaryText.text =
            "你已經完成共享經濟概念建構。";

        deepSummaryNextTriangle.SetActive(true);
        deepSummaryConfirmButton.SetActive(false);
    }

    public void NextDeepSummary()
    {
        if (deepSummaryStep == 0)
        {
            deepSummaryText.text =
                "你現在知道，共享經濟不能只用「便宜」或「有 App」來判斷。";

            deepSummaryStep = 1;
        }
        else if (deepSummaryStep == 1)
        {
            deepSummaryText.text =
                "你會先觀察資源是否存在未被充分利用的容量，再判斷使用者取得的是暫時使用權還是永久所有權。";

            deepSummaryStep = 2;
        }
        else if (deepSummaryStep == 2)
        {
            deepSummaryText.text =
                "你也能辨識平台如何媒合供需、建立信任與交易規則，以及平台政策如何影響提供者與使用者。";

            deepSummaryStep = 3;
        }
        else if (deepSummaryStep == 3)
        {
            deepSummaryText.text =
                "最後，你也知道提高資源利用率並不代表一定更永續，仍然必須評估新增需求與不同角色所承擔的成本。";

            deepSummaryStep = 4;
        }
        else if (deepSummaryStep == 4)
        {
            deepSummaryText.text =
                "當你能把這套判斷方式用在從未看過的新案例上，就代表你學到的不只是三個故事，而是一套可以遷移的共享經濟概念。";

            deepSummaryNextTriangle.SetActive(false);
            deepSummaryConfirmButton.SetActive(true);

            deepSummaryStep = 5;
        }
    }

    public void CloseDeepSummary()
    {
        deepSummaryPanel.SetActive(false);
        hatchingConditionPanel.SetActive(true);
    }

    public void ShowBirdDialogue()
    {
        hatchingConditionPanel.SetActive(false);

        birdDialogueStep = 0;

        birdDialogueUI.SetActive(true);

        birdDialogueText.text =
            "主人！你已經不只是會找便宜的服務了！";

        birdNextTriangle.SetActive(true);
    }

    public void NextBirdDialogue()
    {
        if (birdDialogueStep == 0)
        {
            birdDialogueText.text =
                "你開始看得出資源為什麼閒置、使用權怎麼分配，以及平台如何讓陌生人願意合作！";

            birdDialogueStep = 1;
        }
        else if (birdDialogueStep == 1)
        {
            StartHatchingVideo();
        }
    }

    public void StartHatchingVideo()
    {
        birdDialogueUI.SetActive(false);

        mainCanvas.SetActive(false);

        hatchingVideoPlayer.gameObject.SetActive(true);
        hatchingVideoPlayer.Play();
    }
}