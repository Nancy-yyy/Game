using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class PeopleSelectionManager : MonoBehaviour
{
    public static PeopleSelectionManager Instance;

    [Header("=== 三空間完成任務進度板 ===")]
    [SerializeField] private GameObject case3SummaryPanel; // 拖入任務進度板物件

    // 提供給進度板下方【平台媒合任務完成！】木牌 Button 的 OnClick 事件
    public void OnClickSummaryPanelConfirm()
    {
        if (case3SummaryPanel != null)
        {
            case3SummaryPanel.SetActive(false); // 關閉進度板
        }

        // 開始播放縮時影片
        StartCoroutine(PlayTimelapseRoutine());
    }
    [Header("任務進度動態打勾")]
    [SerializeField] private Image summaryTaskImage; // 拖入 Case3SummaryPanel 底下的 Image
    [SerializeField] private Sprite twoChecksSprite;   // 拖入 2 個勾的圖片
    [SerializeField] private Sprite threeChecksSprite; // 拖入 3 個勾的圖片
    
    [Header("=== 結尾任務四完成面板 ===")]
    [SerializeField] private GameObject finalSummaryPanel;
    [SerializeField] private Image finalTaskImage;       // 拖入 FinalSummaryPanel 底下的 Image/PanelBackground
    [SerializeField] private Sprite mission3Sprite;       // 拖入 3 個勾的圖片 (all_mission3)
    [SerializeField] private Sprite mission4Sprite;       // 拖入 4 個勾的圖片 (all_mission4)

    [Header("=== 介面主容器 ===")]
    public GameObject frameBG;
    public GameObject conditionDrawer;
    public GameObject searchResultPanel;
    public CanvasGroup searchResultCanvasGroup;
    public CanvasGroup frameCanvasGroup;

    [Header("=== 縮時演練畫面模組 ===")]
    public GameObject timelapsePanel;
    public CanvasGroup timelapseCanvasGroup;
    public VideoPlayer timelapseVideoPlayer;

    [Header("=== 卡片輪播模組 ===")]
    public Image cardDisplayImage;
    public List<Sprite> cardSprites;
    public Button prevCardButton;
    public Button nextCardButton;
    public Button selectBookingButton;

    [Header("=== 劇情對話推進模組 ===")]
    public GameObject classroomDialogGroup;
    public Button screenClickBlocker;
    public GameObject systemSignPanel;
    public TextMeshProUGUI systemSignText;

    public GameObject playerDialogBox;
    public TextMeshProUGUI playerDialogText;
    public TextMeshProUGUI playerNameText; 
    public Image playerAvatarImage;
    
    [Header("=== 組員對話框模組 ===")]
    public GameObject memberDialogBox;          // 拖入 MemberDialogBox
    public TextMeshProUGUI memberDialogText;     // 拖入 Txt_MemberContent
    public TextMeshProUGUI memberNameText;       // 拖入 Txt_MemberName (選填)
    public Image memberAvatarImage;              // 拖入 MemberAvatar (Image)
    
    [Header("=== 角色立繪 Sprite 清單 ===")]
    public Sprite avatarPlayer;
    public Sprite avatarMemberA;
    public Sprite avatarMemberB;
    public Sprite avatarMemberC;

    public GameObject birdDialogBox;
    public TextMeshProUGUI birdDialogText;
    public Button backStepButton;

    [Header("=== 全螢幕錯誤提示面板 ===")]
    public GameObject wrongHintBackdrop;
    public Button wrongHintCloseButton;
    public Button finishButton;

    [Header("=== 系統深度解析面板 (各空間通關後觸發) ===")]
    public GameObject systemExpPanel;              
    public TextMeshProUGUI txtSystemExp;           
    public Button systemExpClickBlocker;           
    public Button btnSystemExpConfirm;             

    [Header("=== 1. 使用人數模組 ===")]
    public GameObject peopleSelectionPanel;
    public Button tabPeopleButton;
    public Button singleButton;
    public Button fourButton;
    public Button largeButton;
    public TextMeshProUGUI slot1Text;

    [Header("=== 2. 時段選擇模組 ===")]
    public GameObject timeSelectionPanel;
    public Button tabTimeButton;
    public Button timeCorrectButton;
    public Button[] timeWrongButtons;
    public TextMeshProUGUI slot2Text;

    [Header("=== 3. 預算上限模組 ===")]
    public GameObject budgetSelectionPanel;
    public Button tabBudgetButton;
    public Button budgetCorrectButton;
    public Button[] budgetWrongButtons;
    public TextMeshProUGUI slot3Text;

    [Header("=== 4. 必備設備模組 ===")]
    public GameObject equipmentSelectionPanel;
    public Button tabEquipmentButton;
    public Button equipmentCorrectButton;
    public Button[] equipmentWrongButtons;
    public TextMeshProUGUI slot4Text;

    [Header("=== 5. 隔音要求模組 ===")]
    public GameObject soundproofSelectionPanel;
    public Button tabSoundproofButton;
    public Button soundproofCorrectButton;
    public Button[] soundproofWrongButtons;
    public TextMeshProUGUI slot5Text;

    [Header("=== 6. 距離偏好模組 ===")]
    public GameObject distanceSelectionPanel;
    public Button tabDistanceButton;
    public Button distanceOpt1Button;
    public Button distanceOpt2Button;
    public Button distanceOpt3Button;
    public TextMeshProUGUI slot6Text;

    [Header("=== 7. 飲食規範模組 ===")]
    public GameObject foodSelectionPanel;
    public Button tabFoodButton;
    public Button foodOpt1Button;
    public Button foodOpt2Button;
    public Button foodOpt3Button;
    public TextMeshProUGUI slot7Text;

    [Header("=== 8. 門禁形式模組 ===")]
    public GameObject doorSelectionPanel;
    public Button tabDoorButton;
    public Button doorOpt1Button;
    public Button doorOpt2Button;
    public Button doorOpt3Button;
    public TextMeshProUGUI slot8Text;

    [Header("=== 3 大空間完成狀態紀錄 ===")]
    public bool completedSchemeA = false;
    public bool completedSchemeB = false;
    public bool completedSchemeC = false;

    private bool[] slotFilled = new bool[8];
    private int currentCardIndex = 0;
    private int storyStep = 0;
    private bool isStoryActive = false;

    // 系統解析專用變數
    private string[] currentSystemExpLines;
    private int currentSysExpIndex = 0;

    private enum StoryMode 
    { 
        SchemeA, 
        SchemeB_PreGame, 
        SchemeB_PostGame, 
        SchemeC_PreGame, 
        SchemeC_PostGame,
        TimelapseGroupStory,
        FinalSummaryStory
    }
    private StoryMode currentStoryMode = StoryMode.SchemeA;

    private enum SpeakerType { Player, MemberA, MemberB, MemberC, System, Bird }
    private struct StoryNode
    {
        public SpeakerType speaker;
        public string text;
        public StoryNode(SpeakerType s, string t) { speaker = s; text = t; }
    }

    private List<StoryNode> storyList_SchemeA = new List<StoryNode>();
    private List<StoryNode> storyList_SchemeB_Pre = new List<StoryNode>();
    private List<StoryNode> storyList_SchemeB_Post = new List<StoryNode>();
    private List<StoryNode> storyList_SchemeC_Pre = new List<StoryNode>();
    private List<StoryNode> storyList_SchemeC_Post = new List<StoryNode>();
    private List<StoryNode> storyList_Timelapse = new List<StoryNode>();
    private List<StoryNode> storyList_FinalSummary = new List<StoryNode>();

    // 在十題問答/結尾對話結束時呼叫此協程
public void TriggerFinalTaskSummary()
{
    StartCoroutine(ShowFinalSummaryRoutine());
}

private IEnumerator ShowFinalSummaryRoutine()
{
    // 1. 先顯示 3 個勾狀態並開啟面板
    if (finalTaskImage != null && mission3Sprite != null)
    {
        finalTaskImage.sprite = mission3Sprite;
    }
    if (finalSummaryPanel != null)
    {
        finalSummaryPanel.SetActive(true);
    }

    // 2. 停留 0.6 秒讓玩家看到三勾
    yield return new WaitForSeconds(0.6f);

    // 3. 瞬間打上第 4 個勾並播放通關音效
    if (finalTaskImage != null && mission4Sprite != null)
    {
        finalTaskImage.sprite = mission4Sprite;
    }

    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlayCorrect();
    }
}

// 綁定給 FinalSummaryPanel 下方木牌按鈕的 OnClick 事件
    public void OnClickFinalSummaryConfirm()
    {
        // 🌟 結算 Case 3 總耗時
        GameData.StopCase3Timer();

        if (finalSummaryPanel != null)
        {
            finalSummaryPanel.SetActive(false);
        }

        // 前往結尾影片場景
        UnityEngine.SceneManagement.SceneManager.LoadScene("Ending_VideoScene");
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameData.StartCase3Timer();// 🌟 啟動 Case 3 計時
        CloseAllPanels();

        // 任務進度板初始化（預設隱藏）
        if (case3SummaryPanel != null) case3SummaryPanel.SetActive(false);

        // 系統深度解析面板初始化
        if (systemExpPanel != null) systemExpPanel.SetActive(false);

        if (systemExpClickBlocker != null)
        {
            systemExpClickBlocker.onClick.RemoveAllListeners();
            systemExpClickBlocker.onClick.AddListener(OnSystemExpPanelClicked);
        }

        if (btnSystemExpConfirm != null)
        {
            btnSystemExpConfirm.onClick.RemoveAllListeners();
            btnSystemExpConfirm.onClick.AddListener(OnSystemExpConfirmClicked);
            btnSystemExpConfirm.gameObject.SetActive(false); 
        }

        if (playerNameText != null)
        {
            playerNameText.text = !string.IsNullOrEmpty(GameData.PlayerName) ? GameData.PlayerName : "玩家";
        }
        if (wrongHintBackdrop != null) wrongHintBackdrop.SetActive(false);
        if (searchResultPanel != null) searchResultPanel.SetActive(false);
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);
        if (backStepButton != null) backStepButton.gameObject.SetActive(false);
        if (timelapsePanel != null) timelapsePanel.SetActive(false);

        if (finishButton != null)
        {
            finishButton.interactable = false;
            finishButton.onClick.AddListener(OnClickFinishButton);
        }

        if (wrongHintCloseButton != null)
            wrongHintCloseButton.onClick.AddListener(CloseWrongHint);

        if (prevCardButton != null) prevCardButton.onClick.AddListener(ShowPrevCard);
        if (nextCardButton != null) nextCardButton.onClick.AddListener(ShowNextCard);
        if (selectBookingButton != null) selectBookingButton.onClick.AddListener(OnBookingSelected);

        if (screenClickBlocker != null) screenClickBlocker.onClick.AddListener(OnScreenClickedForStory);
        if (backStepButton != null) backStepButton.onClick.AddListener(OnBackStepClicked);

        // 綁定條件選項
        if (tabPeopleButton != null) tabPeopleButton.onClick.AddListener(() => OpenPanel(peopleSelectionPanel));
        if (singleButton != null) singleButton.onClick.AddListener(() => OnOptionClicked(false, 1, ""));
        if (largeButton != null) largeButton.onClick.AddListener(() => OnOptionClicked(false, 1, ""));
        if (fourButton != null) fourButton.onClick.AddListener(() => OnOptionClicked(true, 1, "4人小組討論"));

        if (tabTimeButton != null) tabTimeButton.onClick.AddListener(() => OpenPanel(timeSelectionPanel));
        if (timeWrongButtons != null)
            foreach (var btn in timeWrongButtons) if (btn != null) btn.onClick.AddListener(() => OnOptionClicked(false, 2, ""));
        if (timeCorrectButton != null) timeCorrectButton.onClick.AddListener(() => OnOptionClicked(true, 2, "22:00-1:00"));

        if (tabBudgetButton != null) tabBudgetButton.onClick.AddListener(() => OpenPanel(budgetSelectionPanel));
        if (budgetWrongButtons != null)
            foreach (var btn in budgetWrongButtons) if (btn != null) btn.onClick.AddListener(() => OnOptionClicked(false, 3, ""));
        if (budgetCorrectButton != null) budgetCorrectButton.onClick.AddListener(() => OnOptionClicked(true, 3, "總預算<=600"));

        if (tabEquipmentButton != null) tabEquipmentButton.onClick.AddListener(() => OpenPanel(equipmentSelectionPanel));
        if (equipmentWrongButtons != null)
            foreach (var btn in equipmentWrongButtons) if (btn != null) btn.onClick.AddListener(() => OnOptionClicked(false, 4, ""));
        if (equipmentCorrectButton != null) equipmentCorrectButton.onClick.AddListener(() => OnOptionClicked(true, 4, "白板與插座"));

        if (tabSoundproofButton != null) tabSoundproofButton.onClick.AddListener(() => OpenPanel(soundproofSelectionPanel));
        if (soundproofWrongButtons != null)
            foreach (var btn in soundproofWrongButtons) if (btn != null) btn.onClick.AddListener(() => OnOptionClicked(false, 5, ""));
        if (soundproofCorrectButton != null) soundproofCorrectButton.onClick.AddListener(() => OnOptionClicked(true, 5, "可交談"));

        if (tabDistanceButton != null) tabDistanceButton.onClick.AddListener(() => OpenPanel(distanceSelectionPanel));
        if (distanceOpt1Button != null) distanceOpt1Button.onClick.AddListener(() => OnOptionClicked(true, 6, "步行五分鐘"));
        if (distanceOpt2Button != null) distanceOpt2Button.onClick.AddListener(() => OnOptionClicked(true, 6, "機車十五分鐘"));
        if (distanceOpt3Button != null) distanceOpt3Button.onClick.AddListener(() => OnOptionClicked(true, 6, "距離不限"));

        if (tabFoodButton != null) tabFoodButton.onClick.AddListener(() => OpenPanel(foodSelectionPanel));
        if (foodOpt1Button != null) foodOpt1Button.onClick.AddListener(() => OnOptionClicked(true, 7, "僅喝水"));
        if (foodOpt2Button != null) foodOpt2Button.onClick.AddListener(() => OnOptionClicked(true, 7, "可外食"));
        if (foodOpt3Button != null) foodOpt3Button.onClick.AddListener(() => OnOptionClicked(true, 7, "禁止飲食"));

        if (tabDoorButton != null) tabDoorButton.onClick.AddListener(() => OpenPanel(doorSelectionPanel));
        if (doorOpt1Button != null) doorOpt1Button.onClick.AddListener(() => OnOptionClicked(true, 8, "櫃檯報到"));
        if (doorOpt2Button != null) doorOpt2Button.onClick.AddListener(() => OnOptionClicked(true, 8, "電子門禁卡"));
        if (doorOpt3Button != null) doorOpt3Button.onClick.AddListener(() => OnOptionClicked(true, 8, "無需驗證"));

        InitStory();
    }

    private void InitStory()
    {
        // 方案 A（共享工作室）
        storyList_SchemeA.Clear();
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Player, "共享工作室離學校走路只要 5 分鐘，環境安靜、設備又齊全，就選這間吧！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.System, "【每小時 300 元，預約 3 小時總價 900 元，超出團隊 600 元預算！】"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Bird, "嗶嗶！主人冷靜啊！你們口袋裡湊死湊活也只有 600 塊，預算爆掉了啦！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Player, "可惡……設備最好的是這間欸！難道就這樣放棄嗎？等等!如果我們不租 3 小時，改租 2 小時（22:00 至 00:00），費用不就剛好是 600 塊了嗎？！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Bird, "這樣你們討論得完嗎？必須重新分配這 2 小時的工作流程喔！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Player, "可以拉！我們那麼強！這樣兩小時綽綽有餘！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Bird, "這間最方便，但方便也是有代價的。我們得在兩小時內把事情全部做完！大家皮皮要繃緊囉！"));

        // 方案 B（補習班閒置教室）
        storyList_SchemeB_Pre.Clear();
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Player, "補習班週末晚上沒有課，教室空著也是空著，3 小時 480 元完全在預算內！"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.System, "【補習班老闆擔心設備損壞找不到人負責，也不認識我們，他有點疑慮】"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Player, "額……老闆發起疑慮連珠砲了，怎麼辦？"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Bird, "老闆聽起來不像是不願意欸，要不主人你們再想想辦法！"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.System, "【請幫助解決老闆疑慮，將正確解決方案拖移至對應方框】"));

        storyList_SchemeB_Post.Clear();
        storyList_SchemeB_Post.Add(new StoryNode(SpeakerType.Player, "太讚了！原來不是找到一間空教室就能直接進去使用。還要先讓雙方知道交易規則和責任怎麼處理。"));
        storyList_SchemeB_Post.Add(new StoryNode(SpeakerType.System, "沒錯！平台不是只負責把地址丟給你，它還要讓原本不敢交易的雙方願意完成交易！"));

        // 方案 C（社區活動中心）
        storyList_SchemeC_Pre.Clear();
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "選社區活動中心！3 小時只要 300 塊，便宜到爆，預算省下一半！"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "【活動中心管理員表示：空間今晚沒有活動，可以提供使用。但現場無工作人員，離開前須恢復桌椅並關閉電源。】"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "沒有管理員看著耶！那我們是不是進去隨便用、用完拍拍屁股走人也沒人知道？"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Bird, "主人主人....你不覺得這樣太不像法律世代了嗎...?"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "嗶嗶！大錯特錯！無人管理不等於無政府狀態！共享空間能便宜開放，靠的是使用者共同遵守『使用與歸還規則』。"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "也是……如果每個人都把這裡弄得像戰場一樣亂，下次管理員就不敢再借給學生了。"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "沒錯！唯有建立明確的自律規範，共享資源才能永續運作！快來制定今晚的使用守則，向管理員換取電子門禁卡吧！"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "【請檢視使用規範，將4項正確守則拖入規範清單中】"));

        storyList_SchemeC_Post.Clear();
        storyList_SchemeC_Post.Add(new StoryNode(SpeakerType.Player, "太好了！收到管理員發來的電子門禁密碼了！原來共享經濟要長久，除了便宜和方便，更需要『自律和歸還責任』。"));
        storyList_SchemeC_Post.Add(new StoryNode(SpeakerType.Bird, "哇哇主人你感覺長腦袋了耶！真不愧是南禾大學高材生"));
        storyList_SchemeC_Post.Add(new StoryNode(SpeakerType.System, "叮咚！答對了！共享不是免費的隨便，而是建立在大家共同維護資產的默契上！"));

        // 縮時後的組員討論演練對話
        storyList_Timelapse.Clear();
        storyList_Timelapse.Add(new StoryNode(SpeakerType.MemberA, "投影片終於全部對齊了！排版看起來超專業！"));
        storyList_Timelapse.Add(new StoryNode(SpeakerType.MemberB, "報告流程也順完了，明天應該不會站在台上互看了吧？"));
        storyList_Timelapse.Add(new StoryNode(SpeakerType.MemberC, "呼……這次真的差點要在路邊做簡報，太感人了！"));
        storyList_Timelapse.Add(new StoryNode(SpeakerType.Player, "我們找到的不只是空著的地方，而是一個在特定時間內，可以合法、安全使用空間的方法。"));
        storyList_Timelapse.Add(new StoryNode(SpeakerType.Bird, "太棒了！專題搞定了！但主人，你真的搞懂共享經濟裡面，我們、業主跟平台各自扮演什麼角色了嗎？"));
        storyList_Timelapse.Add(new StoryNode(SpeakerType.Player, "沒問題，讓我來把這個『三方關係圖』組起來！"));

        // 三方關係圖通關後的結尾總結對話
        storyList_FinalSummary.Clear();
        storyList_FinalSummary.Add(new StoryNode(SpeakerType.Player, "空間提供者始終保留『所有權』，我們只是拿到限定時間的『使用權』，而平台則是透過規則把我們兩邊連結起來！"));
        storyList_FinalSummary.Add(new StoryNode(SpeakerType.Bird, "這樣一說...平台可不是只做一個網站而已喔！"));
        storyList_FinalSummary.Add(new StoryNode(SpeakerType.System, "沒錯沒錯窩~平台的驗證、押金和規則可以降低交易風險，但平台如果規則設計不好，也可能讓提供者或使用者承擔更多成本。"));
    }

    private void OpenPanel(GameObject targetPanel)
    {
        if (conditionDrawer != null) conditionDrawer.SetActive(false);
        if (targetPanel != null) targetPanel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        if (peopleSelectionPanel != null) peopleSelectionPanel.SetActive(false);
        if (timeSelectionPanel != null) timeSelectionPanel.SetActive(false);
        if (budgetSelectionPanel != null) budgetSelectionPanel.SetActive(false);
        if (equipmentSelectionPanel != null) equipmentSelectionPanel.SetActive(false);
        if (soundproofSelectionPanel != null) soundproofSelectionPanel.SetActive(false);
        if (distanceSelectionPanel != null) distanceSelectionPanel.SetActive(false);
        if (foodSelectionPanel != null) foodSelectionPanel.SetActive(false);
        if (doorSelectionPanel != null) doorSelectionPanel.SetActive(false);
    }

    private void OnOptionClicked(bool isCorrect, int slotIndex, string fillText)
    {
        if (isCorrect)
        {
            if (slotIndex == 1 && slot1Text != null) { slot1Text.text = fillText; slotFilled[0] = true; }
            if (slotIndex == 2 && slot2Text != null) { slot2Text.text = fillText; slotFilled[1] = true; }
            if (slotIndex == 3 && slot3Text != null) { slot3Text.text = fillText; slotFilled[2] = true; }
            if (slotIndex == 4 && slot4Text != null) { slot4Text.text = fillText; slotFilled[3] = true; }
            if (slotIndex == 5 && slot5Text != null) { slot5Text.text = fillText; slotFilled[4] = true; }
            if (slotIndex == 6 && slot6Text != null) { slot6Text.text = fillText; slotFilled[5] = true; }
            if (slotIndex == 7 && slot7Text != null) { slot7Text.text = fillText; slotFilled[6] = true; }
            if (slotIndex == 8 && slot8Text != null) { slot8Text.text = fillText; slotFilled[7] = true; }

            CloseAllPanels();
            if (conditionDrawer != null) conditionDrawer.SetActive(true);

            CheckAllSlotsFinished();
        }
        else
        {
            // 🌟 記錄條件篩選錯誤次數
            GameData.Case3_FilterErrors++;
            
            if (AudioManager.Instance != null) AudioManager.Instance.PlayWrong();

            if (wrongHintBackdrop != null)
            {
                wrongHintBackdrop.transform.SetAsLastSibling();
                wrongHintBackdrop.SetActive(true);
            }
        }
    }

    private void CheckAllSlotsFinished()
    {
        bool allDone = true;
        for (int i = 0; i < slotFilled.Length; i++)
        {
            if (!slotFilled[i]) { allDone = false; break; }
        }
        if (allDone && finishButton != null) finishButton.interactable = true;
    }

    public void OnClickFinishButton()
    {
        StartCoroutine(TransitionToResultCardRoutine());
    }

    private IEnumerator TransitionToResultCardRoutine()
    {
        if (finishButton != null) finishButton.interactable = false;

        if (conditionDrawer != null)
        {
            RectTransform drawerRect = conditionDrawer.GetComponent<RectTransform>();
            CanvasGroup drawerGroup = conditionDrawer.GetComponent<CanvasGroup>();
            if (drawerGroup == null) drawerGroup = conditionDrawer.AddComponent<CanvasGroup>();

            if (drawerRect != null)
            {
                Vector2 startPos = drawerRect.anchoredPosition;
                Vector2 targetPos = new Vector2(startPos.x, startPos.y - 600f);
                float elapsed = 0f;
                float duration = 0.4f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    drawerRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                    drawerGroup.alpha = Mathf.Lerp(1f, 0f, t);
                    yield return null;
                }
            }
            conditionDrawer.SetActive(false);
        }

        yield return new WaitForSeconds(0.2f);

        currentCardIndex = 0;
        UpdateCardDisplay();

        if (searchResultPanel != null)
        {
            searchResultPanel.SetActive(true);
            if (searchResultCanvasGroup != null)
            {
                float elapsed = 0f;
                float duration = 0.4f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    searchResultCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                    yield return null;
                }
                searchResultCanvasGroup.alpha = 1f;
            }
        }
    }
    private IEnumerator ShowSummaryWithAnimationRoutine()
{
    // 1. 先換成 2 個勾的狀態並打開面板
    if (summaryTaskImage != null && twoChecksSprite != null)
    {
        summaryTaskImage.sprite = twoChecksSprite;
    }
    case3SummaryPanel.SetActive(true);

    // 2. 停留 0.6 秒讓玩家看到原本只有兩勾
    yield return new WaitForSeconds(0.6f);

    // 3. 瞬間換成 3 個勾，並播放通關音效！
    if (summaryTaskImage != null && threeChecksSprite != null)
    {
        summaryTaskImage.sprite = threeChecksSprite;
    }

    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlayCorrect(); // 播放蓋章/通關叮咚聲
    }
}

    private void ShowPrevCard()
    {
        if (cardSprites == null || cardSprites.Count == 0) return;
        currentCardIndex--;
        if (currentCardIndex < 0) currentCardIndex = cardSprites.Count - 1;
        UpdateCardDisplay();
    }

    private void ShowNextCard()
    {
        if (cardSprites == null || cardSprites.Count == 0) return;
        currentCardIndex++;
        if (currentCardIndex >= cardSprites.Count) currentCardIndex = 0;
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        if (cardDisplayImage != null && cardSprites != null && cardSprites.Count > currentCardIndex)
        {
            cardDisplayImage.sprite = cardSprites[currentCardIndex];
        }
    }

    private void OnBookingSelected()
    {
        switch (currentCardIndex)
        {
            case 0:
                currentStoryMode = StoryMode.SchemeA;
                GameData.Case3_Decision = "方案A_共享工作室"; // 🌟 記錄決策
                break;
            case 1:
                currentStoryMode = StoryMode.SchemeC_PreGame; 
                GameData.Case3_Decision = "方案C_社區活動中心"; // 🌟 記錄決策
                break;
            case 2:
                currentStoryMode = StoryMode.SchemeB_PreGame; 
                GameData.Case3_Decision = "方案B_補習班教室";   // 🌟 記錄決策
                break;
            default:
                currentStoryMode = StoryMode.SchemeA;
                GameData.Case3_Decision = "方案A_共享工作室";
                break;
        }

        if (GlobalFader.Instance != null)
        {
            GlobalFader.Instance.FadeTransition(() =>
            {
                if (searchResultPanel != null) searchResultPanel.SetActive(false);
                if (frameBG != null) frameBG.SetActive(false);
                if (conditionDrawer != null) conditionDrawer.SetActive(false);

                StartCoroutine(TransitionToStoryRoutine());
            });
        }
        else
        {
            if (searchResultPanel != null) searchResultPanel.SetActive(false);
            if (frameBG != null) frameBG.SetActive(false);
            if (conditionDrawer != null) conditionDrawer.SetActive(false);

            StartCoroutine(TransitionToStoryRoutine());
        }
    }
    public void StartPostGameStory()
    {
        currentStoryMode = StoryMode.SchemeB_PostGame;
        StartCoroutine(TransitionToStoryRoutine());
    }

    public void StartPostGameStorySchemeC()
    {
        currentStoryMode = StoryMode.SchemeC_PostGame;
        StartCoroutine(TransitionToStoryRoutine());
    }

    public void StartFinalSummaryStory()
    {
        currentStoryMode = StoryMode.FinalSummaryStory;
        StartCoroutine(TransitionToStoryRoutine());
    }

    private IEnumerator TransitionToStoryRoutine()
    {
        if (searchResultCanvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 0.3f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                searchResultCanvasGroup.alpha = alpha;
                if (frameCanvasGroup != null) frameCanvasGroup.alpha = alpha;
                yield return null;
            }
        }

        if (searchResultPanel != null) searchResultPanel.SetActive(false);
        if (frameBG != null) frameBG.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        // 🌟 關鍵修正：只要進入對話模式，一律強制將大背景設為教室底圖！
        if (backgroundImage != null && classroomBackgroundSprite != null)
        {
            backgroundImage.gameObject.SetActive(true);
            backgroundImage.sprite = classroomBackgroundSprite;
            backgroundImage.color = Color.white; // 確保顏色沒有被透明度影響
        }

        storyStep = 0;
        isStoryActive = true;
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(true);
        if (screenClickBlocker != null) screenClickBlocker.gameObject.SetActive(true);
        if (backStepButton != null) backStepButton.gameObject.SetActive(false);

        DisplayCurrentStoryNode();
    }

    private void DisplayCurrentStoryNode()
    {
        List<StoryNode> currentList = storyList_SchemeA;
        if (currentStoryMode == StoryMode.SchemeB_PreGame) currentList = storyList_SchemeB_Pre;
        else if (currentStoryMode == StoryMode.SchemeB_PostGame) currentList = storyList_SchemeB_Post;
        else if (currentStoryMode == StoryMode.SchemeC_PreGame) currentList = storyList_SchemeC_Pre;
        else if (currentStoryMode == StoryMode.SchemeC_PostGame) currentList = storyList_SchemeC_Post;
        else if (currentStoryMode == StoryMode.TimelapseGroupStory) currentList = storyList_Timelapse;
        else if (currentStoryMode == StoryMode.FinalSummaryStory) currentList = storyList_FinalSummary;

        if (storyStep >= currentList.Count)
        {
            EndStory();
            return;
        }

        StoryNode node = currentList[storyStep];

        // 先將所有對話框關閉
        if (playerDialogBox != null) playerDialogBox.SetActive(false);
        if (memberDialogBox != null) memberDialogBox.SetActive(false);
        if (systemSignPanel != null) systemSignPanel.SetActive(false);
        if (birdDialogBox != null) birdDialogBox.SetActive(false);

        switch (node.speaker)
        {
            case SpeakerType.Player:
                if (playerDialogBox != null)
                {
                    playerDialogBox.SetActive(true);
                    if (playerDialogText != null) playerDialogText.text = node.text;
                    UpdateAvatar(avatarPlayer);
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
                break;

            case SpeakerType.MemberA:
                ShowMemberDialogue("組員 A", node.text, avatarMemberA);
                break;

            case SpeakerType.MemberB:
                ShowMemberDialogue("組員 B", node.text, avatarMemberB);
                break;

            case SpeakerType.MemberC:
                ShowMemberDialogue("組員 C", node.text, avatarMemberC);
                break;

            case SpeakerType.System:
                if (systemSignPanel != null)
                {
                    systemSignPanel.SetActive(true);
                    if (systemSignText != null) systemSignText.text = node.text;
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySystemPrompt();
                break;

            case SpeakerType.Bird:
                if (birdDialogBox != null)
                {
                    birdDialogBox.SetActive(true);
                    if (birdDialogText != null) birdDialogText.text = node.text;
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
                break;
        }
    }

    // 專門顯示組員對話的輔助函式
    private void ShowMemberDialogue(string memberName, string content, Sprite avatar)
    {
        if (memberDialogBox != null)
        {
            memberDialogBox.SetActive(true);
            if (memberDialogText != null) memberDialogText.text = content;
            if (memberNameText != null) memberNameText.text = memberName;
            if (memberAvatarImage != null && avatar != null)
            {
                memberAvatarImage.sprite = avatar;
            }
        }
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
    }

    private void UpdateAvatar(Sprite targetSprite)
    {
        if (playerAvatarImage != null && targetSprite != null)
        {
            playerAvatarImage.sprite = targetSprite;
        }
    }

    private void OnScreenClickedForStory()
    {
        if (!isStoryActive) return;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayScreenClick();

        storyStep++;
        DisplayCurrentStoryNode();
    }

    private void EndStory()
    {
       isStoryActive = false;
        if (playerDialogBox != null) playerDialogBox.SetActive(false);
        if (memberDialogBox != null) memberDialogBox.SetActive(false); // 🌟 加上這行
        if (systemSignPanel != null) systemSignPanel.SetActive(false);
        if (birdDialogBox != null) birdDialogBox.SetActive(false);
        if (screenClickBlocker != null) screenClickBlocker.gameObject.SetActive(false);

        if (currentStoryMode == StoryMode.SchemeB_PreGame)
        {
            if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);
            if (TrustMiniGameManager.Instance != null)
            {
                TrustMiniGameManager.Instance.StartMiniGame();
            }
        }
        else if (currentStoryMode == StoryMode.SchemeC_PreGame)
        {
            if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);
            if (RuleMiniGameManager.Instance != null)
            {
                RuleMiniGameManager.Instance.StartMiniGame();
            }
        }
        else if (currentStoryMode == StoryMode.TimelapseGroupStory)
        {
            if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);
            if (TripartiteQAManager.Instance != null)
            {
                TripartiteQAManager.Instance.StartQAGameWithTransition();
            }
        }
        else if (currentStoryMode == StoryMode.FinalSummaryStory)
        {
            StartSystemExplanation(currentStoryMode);
        }
        else
        {
            StartSystemExplanation(currentStoryMode);
        }
    }

    private void StartSystemExplanation(StoryMode mode)
    {
        currentSysExpIndex = 0;
        currentSystemExpLines = GetSystemExpLinesForScheme(mode);

        if (systemExpPanel != null) systemExpPanel.SetActive(true);
        UpdateSystemExpText();
    }

    private void UpdateSystemExpText()
    {
        if (txtSystemExp != null && currentSystemExpLines != null && currentSysExpIndex < currentSystemExpLines.Length)
        {
            txtSystemExp.text = currentSystemExpLines[currentSysExpIndex];

            if (currentSysExpIndex == currentSystemExpLines.Length - 1)
            {
                if (systemExpClickBlocker != null) systemExpClickBlocker.gameObject.SetActive(false);
                if (btnSystemExpConfirm != null) btnSystemExpConfirm.gameObject.SetActive(true);
            }
            else
            {
                if (systemExpClickBlocker != null) systemExpClickBlocker.gameObject.SetActive(true);
                if (btnSystemExpConfirm != null) btnSystemExpConfirm.gameObject.SetActive(false);
            }
        }
    }

    private void OnSystemExpPanelClicked()
    {
        if (currentSystemExpLines == null) return;

        currentSysExpIndex++;
        if (currentSysExpIndex < currentSystemExpLines.Length)
        {
            UpdateSystemExpText();
        }
    }

  private void OnSystemExpConfirmClicked()
    {
        if (systemExpPanel != null) systemExpPanel.SetActive(false);

        // 🌟 修正這裡：十題問答後的總結解析看完，啟動任務四動態進度板
        if (currentStoryMode == StoryMode.FinalSummaryStory)
        {
            Debug.Log("【Case 3 結尾】總結解析看完，啟動任務四進度板動態打勾！");
            if (finalSummaryPanel != null)
            {
                StartCoroutine(ShowFinalSummaryRoutine());
            }
            else
            {
                // 若未指定面板則直接進入影片
                UnityEngine.SceneManagement.SceneManager.LoadScene("Ending_VideoScene");
            }
            return;
        }

        // 記錄當前完成的是哪個空間
        if (currentStoryMode == StoryMode.SchemeA) completedSchemeA = true;
        else if (currentStoryMode == StoryMode.SchemeB_PostGame) completedSchemeB = true;     
        else if (currentStoryMode == StoryMode.SchemeC_PostGame) completedSchemeC = true;     

        // 檢查三個空間是否都通關了
        if (completedSchemeA && completedSchemeB && completedSchemeC)
        {
            Debug.Log("【Case 3】三大空間全部通關，啟動任務進度動態打勾！");
            
            if (case3SummaryPanel != null)
            {
                StartCoroutine(ShowSummaryWithAnimationRoutine());
            }
            else
            {
                StartCoroutine(PlayTimelapseRoutine());
            }
        }
        else
        {
            // 還沒全部通關，顯示返回選單按鈕，讓玩家挑選下一個空間
            if (backStepButton != null) backStepButton.gameObject.SetActive(true);
        }
    }

    private string[] GetSystemExpLinesForScheme(StoryMode mode)
    {
        if (mode == StoryMode.SchemeA)
        {
            return new string[]
            {
                "【系統小百科 (1/4)】：什麼是共享？就是東西空著也是空著，拿出來給需要的人用，大家一起分攤成本，這就是最簡單的共享經濟！",
                "【系統小百科 (2/4)】：不過，時間跟金錢總是需要取捨的。像我們這次為了省錢，把 3 小時縮短成 2 小時，雖然預算過關了，但討論的時間也變得很緊湊。",
                "【系統小百科 (3/4)】：設備好、地點方便的東西通常比較貴，這提醒我們在現實生活中，預算有限時一定要算清楚「花這個錢到底划不划算」。",
                "【系統小百科 (4/4)】：所以，抓對時間、有效率地完成目標，才是用最少預算發揮最大效益的祕訣！"
            };
        }
        else if (mode == StoryMode.SchemeB_PostGame || mode == StoryMode.SchemeB_PreGame)
        {
            return new string[]
            {
                "【系統小百科 (1/4)】：當你要跟一個完全不認識的人借場地時，雙方心裡一定都會毛毛的，擔心對方把環境弄壞或是人跑掉不付錢。",
                "【系統小百科 (2/4)】：這時候，『平台』的角色就很重要了，也就是我們的【容身之地】平台！透過實名認證、押金和賠償規定，就像是在雙方之間建立了一層安全網。",
                "【系統小百科 (3/4)】：有了這些制度，本來應該由房東一個人承擔的風險，就可以透過平台規則大家一起分擔。",
                "【系統小百科 (4/4)】：所以，一個好的共享平台不只是幫忙牽線找地方，更重要的是建立『信任感』，讓大家敢安心交易！"
            };
        }
        else if (mode == StoryMode.SchemeC_PostGame || mode == StoryMode.SchemeC_PreGame)
        {
            return new string[]
            {
                "【系統小百科 (1/4)】：如果是一個『沒有管理員看著』的公共空間，大家會不會隨便亂搞？如果每個人都這樣想，最後這個地方一定會被弄到不能用。",
                "【系統小百科 (2/4)】：這就是為什麼『互相尊重跟自律』超級重要！大家一起使用的東西，如果都不愛惜，最後倒楣的還是自己。",
                "【系統小百科 (3/4)】：所以，活動中心才會要求大家遵守規定、不亂吃東西、走之前要把桌椅恢復原狀、隨手關燈。",
                "【系統小百科 (4/4)】：只要每個人都負起責任、遵守約定，便宜又方便的共享空間才能一直長久發展下去！"
            };
        }
        else if (mode == StoryMode.FinalSummaryStory)
        {
            return new string[]
            {
                "【共享經濟大總結 (1/4)】：回顧這三個關卡，我們發現『所有權』跟『使用權』是可以分開的！房東保有房子，而我們只買下需要的時段來使用。",
                "【共享經濟大總結 (2/4)】：不管是哪一種空間，『平台』在中間都扮演了超級關鍵的角色。它不只是個貼廣告的網站，更是幫大家過濾風險、建立信任的橋樑。",
                "【共享經濟大總結 (3/4)】：對提供者來說，平台保障了他們的資產安全與準時收錢；對使用者來說，平台提供了方便比較、價格透明的選擇。",
                "【共享經濟大總結 (4/4)】：所以，一個成功的共享經濟模式，就是靠著『空間主人的分享、使用者的自律、以及平臺的規則』三方互助，才能讓好資源一直循環下去！"
            };
        }
        else
        {
            return new string[]
            {
                "【系統深度解析 (1/1)】：恭喜完成此空間的共享經濟體驗！請點擊確認返回選單。"
            };
        }
    }

   private IEnumerator PlayTimelapseRoutine()
    {
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);

        if (timelapsePanel != null)
        {
            timelapsePanel.SetActive(true);

            if (AudioManager.Instance != null) AudioManager.Instance.PlayAlarm();

            if (timelapseCanvasGroup != null)
            {
                timelapseCanvasGroup.alpha = 0f;
                float t = 0f;
                while (t < 0.3f)
                {
                    t += Time.deltaTime;
                    timelapseCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / 0.3f);
                    yield return null;
                }
                timelapseCanvasGroup.alpha = 1f;
            }

            if (timelapseVideoPlayer != null)
            {
                timelapseVideoPlayer.Play();
                yield return new WaitUntil(() => timelapseVideoPlayer.isPlaying);
                yield return new WaitWhile(() => timelapseVideoPlayer.isPlaying);
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
            }

            if (timelapseCanvasGroup != null)
            {
                float t = 0f;
                while (t < 0.3f)
                {
                    t += Time.deltaTime;
                    timelapseCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
                    yield return null;
                }
                timelapseCanvasGroup.alpha = 0f;
            }

            timelapsePanel.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }

        currentStoryMode = StoryMode.TimelapseGroupStory;
        StartCoroutine(TransitionToStoryRoutine());
    }

    public void OnBackStepClicked()
    {// 1. 關閉重新選擇按鈕與教室對話群組
        if (backStepButton != null) backStepButton.gameObject.SetActive(false);
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);

        // 2. 換回預設輪播背景
        if (backgroundImage != null && defaultBackgroundSprite != null)
        {
            backgroundImage.sprite = defaultBackgroundSprite;
        }

        // 3. 喚醒外框與三張卡片輪播面板
        if (frameBG != null) frameBG.SetActive(true);
        if (searchResultPanel != null) searchResultPanel.SetActive(true);
        if (frameCanvasGroup != null) frameCanvasGroup.alpha = 1f;
        if (searchResultCanvasGroup != null) searchResultCanvasGroup.alpha = 1f;

        // 4. 刷新卡片顯示
        UpdateCardDisplay();
    }

    public void CloseWrongHint()
    {
        if (wrongHintBackdrop != null) wrongHintBackdrop.SetActive(false);
    }
   [Header("=== 背景圖替換設定 ===")]
    public Image backgroundImage;          // 拉入 Canvas 下的 background 物件
    public Sprite defaultBackgroundSprite; // 預設背景 (卡片輪播時的底圖)
    public Sprite classroomBackgroundSprite; // 教室背景 (對話時顯示的教室圖)

}