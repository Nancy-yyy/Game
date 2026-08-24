using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PeopleSelectionManager : MonoBehaviour
{
    public static PeopleSelectionManager Instance;

    [Header("=== 介面主容器 ===")]
    public GameObject frameBG;
    public GameObject conditionDrawer;
    public GameObject searchResultPanel;
    public CanvasGroup searchResultCanvasGroup;
    public CanvasGroup frameCanvasGroup;

    [Header("=== 縮時演練畫面模組 ===")]
    public GameObject timelapsePanel;
    public CanvasGroup timelapseCanvasGroup;

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
    public Image playerAvatarImage;

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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CloseAllPanels();
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
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Player, "可惡……設備最好的是這間欸！難道就這樣放棄嗎？等等！如果我們不租 3 小時，改租 2 小時（22:00 至 00:00），費用不就剛好是 600 塊了嗎？！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Bird, "紙張吧?這樣你們討論得完嗎？必須重新分配這 2 小時的工作流程喔！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Player, "可以拉！我們那麼強！這樣兩小時綽綽有餘！"));
        storyList_SchemeA.Add(new StoryNode(SpeakerType.Bird, "這間最方便，但方便也是有代價的。我們得在兩小時內把事情全部做完！大家皮皮要繃緊囉！"));

        // 方案 B（補習班閒置教室）
        storyList_SchemeB_Pre.Clear();
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Player, "補習班週末晚上沒有課，教室空著也是空著，3 小時 480 元完全在預算內！"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.System, "【補習班老闆擔心設備損壞找不到人負責，也不認識我們，他有點疑慮】"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Player, "額……老闆發起疑慮連珠砲了，怎麼辦？"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.Bird, "老闆不是完全不願意，他只是還不知道風險要由誰負責。平台上有一些保障功能，我們先看看能不能幫老闆把疑慮解開！"));
        storyList_SchemeB_Pre.Add(new StoryNode(SpeakerType.System, "【請幫助解決老闆疑慮，將正確解決方案拖移至對應方框】"));

        storyList_SchemeB_Post.Clear();
        storyList_SchemeB_Post.Add(new StoryNode(SpeakerType.Player, "太讚了！原來不是找到一間空教室就能直接進去使用。還要先讓雙方知道交易規則和責任怎麼處理。"));
        storyList_SchemeB_Post.Add(new StoryNode(SpeakerType.Bird, "沒錯！平台不是只負責把地址丟給你，它還要讓原本不敢交易的雙方願意完成交易！"));

        // 方案 C（社區活動中心）
        storyList_SchemeC_Pre.Clear();
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "選社區活動中心！3 小時只要 300 塊，便宜到爆，預算省下一半！"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "【活動中心管理員表示：空間今晚沒有活動，可以提供使用。但現場無工作人員，離開前須恢復桌椅並關閉電源。】"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "沒有管理員看著耶！那我們是不是進去隨便用、用完拍拍屁股走人也沒人知道？"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Bird, "嗶嗶！大錯特錯！無人管理不等於無政府狀態！共享空間能便宜開放，靠的是使用者共同遵守『使用與歸還規則』。"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Player, "也是……如果每個人都把這裡弄得像戰場一樣亂，下次管理員就不敢再借給學生了。"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.Bird, "沒錯！唯有建立明確的自律規範，共享資源才能永續運作！快來制定今晚的使用守則，向管理員換取電子門禁卡吧！"));
        storyList_SchemeC_Pre.Add(new StoryNode(SpeakerType.System, "【請檢視使用規範，將4項正確守則拖入規範清單中】"));

        storyList_SchemeC_Post.Clear();
        storyList_SchemeC_Post.Add(new StoryNode(SpeakerType.Player, "太好了！收到管理員發來的電子門禁密碼了！原來共享經濟要長久，除了便宜和方便，更需要『自律和歸還責任』。"));
        storyList_SchemeC_Post.Add(new StoryNode(SpeakerType.Bird, "叮咚！答對了！共享不是免費的隨便，而是建立在大家共同維護資產的默契上！"));

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
        storyList_FinalSummary.Add(new StoryNode(SpeakerType.Bird, "答對了！平台可不是只做一個網站而已喔！"));
        storyList_FinalSummary.Add(new StoryNode(SpeakerType.Bird, "平台的驗證、押金和規則可以降低交易風險，但平台如果規則設計不好，也可能讓提供者或使用者承擔更多成本。"));
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
            // 🌟 選錯條件時播放答錯音效
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
        if (currentCardIndex == 0)
        {
            currentStoryMode = StoryMode.SchemeA;
            StartCoroutine(TransitionToStoryRoutine());
        }
        else if (currentCardIndex == 1)
        {
            currentStoryMode = StoryMode.SchemeC_PreGame;
            StartCoroutine(TransitionToStoryRoutine());
        }
        else if (currentCardIndex == 2)
        {
            currentStoryMode = StoryMode.SchemeB_PreGame;
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

        if (playerDialogBox != null) playerDialogBox.SetActive(false);
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
                if (playerDialogBox != null)
                {
                    playerDialogBox.SetActive(true);
                    if (playerDialogText != null) playerDialogText.text = node.text;
                    UpdateAvatar(avatarMemberA);
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
                break;

            case SpeakerType.MemberB:
                if (playerDialogBox != null)
                {
                    playerDialogBox.SetActive(true);
                    if (playerDialogText != null) playerDialogText.text = node.text;
                    UpdateAvatar(avatarMemberB);
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
                break;

            case SpeakerType.MemberC:
                if (playerDialogBox != null)
                {
                    playerDialogBox.SetActive(true);
                    if (playerDialogText != null) playerDialogText.text = node.text;
                    UpdateAvatar(avatarMemberC);
                }
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDialogue();
                break;

            case SpeakerType.System:
                if (systemSignPanel != null)
                {
                    systemSignPanel.SetActive(true);
                    if (systemSignText != null) systemSignText.text = node.text;
                }
                // 🌟 系統告示顯示音效
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

        // 🌟 點擊畫面推進對話音效
        if (AudioManager.Instance != null) AudioManager.Instance.PlayScreenClick();

        storyStep++;
        DisplayCurrentStoryNode();
    }

    private void EndStory()
    {
        isStoryActive = false;
        if (playerDialogBox != null) playerDialogBox.SetActive(false);
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
                TripartiteQAManager.Instance.StartQAGame();
            }
        }
        else if (currentStoryMode == StoryMode.FinalSummaryStory)
        {
            Debug.Log("【Case 3 結尾】共享經濟空間案例全部通關！");
        }
        else
        {
            if (currentStoryMode == StoryMode.SchemeA) completedSchemeA = true;
            else if (currentStoryMode == StoryMode.SchemeB_PostGame) completedSchemeB = true;
            else if (currentStoryMode == StoryMode.SchemeC_PostGame) completedSchemeC = true;

            if (completedSchemeA && completedSchemeB && completedSchemeC)
            {
                StartCoroutine(PlayTimelapseRoutine());
            }
            else
            {
                if (backStepButton != null) backStepButton.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator PlayTimelapseRoutine()
    {
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);

        if (timelapsePanel != null)
        {
            timelapsePanel.SetActive(true);

            // 🌟 縮時畫面播放鬧鐘音效！
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

            yield return new WaitForSeconds(2.0f);

            if (timelapseCanvasGroup != null)
            {
                float t = 0f;
                while (t < 0.3f)
                {
                    t += Time.deltaTime;
                    timelapseCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / 0.3f);
                    yield return null;
                }
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

    private void OnBackStepClicked()
    {
        if (backStepButton != null) backStepButton.gameObject.SetActive(false);
        if (classroomDialogGroup != null) classroomDialogGroup.SetActive(false);

        if (frameBG != null) frameBG.SetActive(true);
        if (searchResultPanel != null) searchResultPanel.SetActive(true);
        if (frameCanvasGroup != null) frameCanvasGroup.alpha = 1f;
        if (searchResultCanvasGroup != null) searchResultCanvasGroup.alpha = 1f;
    }

    public void CloseWrongHint()
    {
        if (wrongHintBackdrop != null) wrongHintBackdrop.SetActive(false);
    }
}