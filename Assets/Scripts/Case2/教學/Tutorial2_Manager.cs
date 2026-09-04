using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tutorial2_Manager : MonoBehaviour
{
    [Header("【常駐標題】")]
    public GameObject titleImage;

    [Header("【第 1 階段：教學圖 1~4 依序登場】")]
    public GameObject sceneA_StudentStory;
    public GameObject teachImg_1;
    public GameObject teachImg_2;
    public GameObject teachImg_3;
    public GameObject teachImg_4;

    [Header("【System Block 系統解說框】")]
    public GameObject systemBlockPanel;
    public TextMeshProUGUI systemBlockText;
    public Button systemBlockNextBtn;

    [Header("【第 2 階段：三選一選項卡片】")]
    public GameObject quizGroup; 
    public Button optionA_Btn;   
    public Button optionB_Btn;   
    public Button optionC_Btn;   

    [Header("【錯誤警告彈窗】")]
    public GameObject warningBoxPanel;
    public TextMeshProUGUI warningText;
    public Button warningCloseBtn;

    private int teachStep = 0;
    private int conclusionStep = 0;
    private bool isConclusionPhase = false;

    private readonly string[] introDialogueLines = new string[]
    {
        "課程期間，學生正在使用這本書，這本書即為一種資產",
        "課程結束，原使用者暫時不再需要",
        "學生把課本放回書架，課本被閒置了",
        "雖然課本仍然存在，但暫時沒有人使用",
        "仔細看看這四個畫面，你覺得這本書失去價值了嗎？"
    };

    private readonly string[] conclusionLines = new string[]
    {
        "當一項資產在一段時間內沒有被原持有人使用，但仍具有使用價值時，就可能形成閒置資產。",
        "點擊右邊箭頭繼續往下看吧！"
    };

    private void Awake()
    {
        Transform bottomDialogue = transform.Find("BottomDialoguePanel");
        if (bottomDialogue != null) bottomDialogue.gameObject.SetActive(false);

        if (quizGroup != null) quizGroup.SetActive(false);
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);
        if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
    }

    void Start()
    {
        if (titleImage != null) titleImage.SetActive(true);

        if (sceneA_StudentStory != null) sceneA_StudentStory.SetActive(true);
        if (teachImg_1 != null) teachImg_1.SetActive(false);
        if (teachImg_2 != null) teachImg_2.SetActive(false);
        if (teachImg_3 != null) teachImg_3.SetActive(false);
        if (teachImg_4 != null) teachImg_4.SetActive(false);

        if (quizGroup != null) quizGroup.SetActive(false);
        if (warningBoxPanel != null) warningBoxPanel.SetActive(false);

        

        if (systemBlockNextBtn != null)
        {
            systemBlockNextBtn.onClick.RemoveAllListeners();
            systemBlockNextBtn.onClick.AddListener(OnClickSystemBlockNext);
        }

        if (warningCloseBtn != null)
        {
            warningCloseBtn.onClick.RemoveAllListeners();
            warningCloseBtn.onClick.AddListener(() => { if (warningBoxPanel != null) warningBoxPanel.SetActive(false); });
        }

        if (optionA_Btn != null)
        {
            optionA_Btn.onClick.RemoveAllListeners();
            optionA_Btn.onClick.AddListener(() => OnSelectOption(false));
            optionA_Btn.interactable = true;
        }
        if (optionB_Btn != null)
        {
            optionB_Btn.onClick.RemoveAllListeners();
            optionB_Btn.onClick.AddListener(() => OnSelectOption(false));
            optionB_Btn.interactable = true;
        }
        if (optionC_Btn != null)
        {
            optionC_Btn.onClick.RemoveAllListeners();
            optionC_Btn.onClick.AddListener(() => OnSelectOption(true));
            optionC_Btn.interactable = true;
        }

        StartCoroutine(StartFirstTeachStep());
    }

    private IEnumerator StartFirstTeachStep()
    {
        yield return new WaitForSeconds(0.2f);
        teachStep = 0;
        isConclusionPhase = false;

        if (quizGroup != null) quizGroup.SetActive(false);
        if (teachImg_1 != null) teachImg_1.SetActive(true);

        if (systemBlockPanel != null)
        {
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = introDialogueLines[0];
        }
    }

    public void OnClickSystemBlockNext()
    {
        if (isConclusionPhase)
        {
            conclusionStep++;
            if (conclusionStep == 1)
            {
                if (systemBlockText != null) systemBlockText.text = conclusionLines[1];
                if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
                
                // ⭐ 正式解鎖右箭頭，點擊只會前進一頁 (前往教學 3)
                if (TutorialCarouselManager.Instance != null)
                {
                    TutorialCarouselManager.Instance.UnlockNextPage(2); // 解鎖教學 3
                }
            }
            return;
        }

        teachStep++;
        if (teachStep == 1)
        {
            if (teachImg_2 != null) teachImg_2.SetActive(true);
            if (systemBlockText != null) systemBlockText.text = introDialogueLines[1];
        }
        else if (teachStep == 2)
        {
            if (teachImg_3 != null) teachImg_3.SetActive(true);
            if (systemBlockText != null) systemBlockText.text = introDialogueLines[2];
        }
        else if (teachStep == 3)
        {
            if (teachImg_4 != null) teachImg_4.SetActive(true);
            if (systemBlockText != null) systemBlockText.text = introDialogueLines[3];
        }
        else if (teachStep == 4)
        {
            if (systemBlockText != null) systemBlockText.text = introDialogueLines[4];
        }
        else
        {
            if (systemBlockPanel != null) systemBlockPanel.SetActive(false);
            if (sceneA_StudentStory != null) sceneA_StudentStory.SetActive(false);
            if (quizGroup != null) quizGroup.SetActive(true);
        }
    }

    public void OnSelectOption(bool isCorrect)
    {
        if (isCorrect)
        {
            if (optionA_Btn != null) optionA_Btn.interactable = false;
            if (optionB_Btn != null) optionB_Btn.interactable = false;
            if (optionC_Btn != null) optionC_Btn.interactable = false;

            StartCoroutine(ShowCorrectConclusionRoutine());
        }
        else
        {
            if (warningBoxPanel != null)
            {
                warningBoxPanel.SetActive(true);
                warningBoxPanel.transform.SetAsLastSibling();
                if (warningText != null) warningText.text = "不太對歐...再想想看吧！";
            }
        }
    }

    private IEnumerator ShowCorrectConclusionRoutine()
    {
        if (systemBlockPanel != null)
        {
            systemBlockPanel.SetActive(true);
            systemBlockPanel.transform.SetAsLastSibling();
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = false;
            if (systemBlockText != null) systemBlockText.text = "沒錯呦！";
        }

        yield return new WaitForSeconds(1.2f);

        isConclusionPhase = true;
        conclusionStep = 0;

        if (systemBlockPanel != null)
        {
            if (systemBlockNextBtn != null) systemBlockNextBtn.interactable = true;
            if (systemBlockText != null) systemBlockText.text = conclusionLines[0];
        }
    }
}