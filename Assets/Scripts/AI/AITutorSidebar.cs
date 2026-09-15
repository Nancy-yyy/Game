using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class AIQueryRequest
{
    public string query;
    public string case_id;
    // ======================================
    // AI 防暴雷：傳送目前劇情進度
    public string story_step;
    // ======================================
}

[Serializable]
public class AIQueryResponse
{
    public string question;
    public string case_id;
    public string short_answer;
}

[Serializable]
public class ChatHistoryRecord
{
    public string role;
    public string message;
}

public class AITutorSidebar : MonoBehaviour
{
    [Header("【關卡設定】")]
    public string currentCaseId = "case2";

    [Header("【主容器與開關】")]
    [SerializeField] private GameObject sidebarRoot;
    [SerializeField] private GameObject chatWindow;
    [SerializeField] private GameObject dimmerOverlay; // 抽出來的霧白全螢幕底色
    [SerializeField] private Button birdOpenBtn;
    [SerializeField] private Button closeBtn;          // ⭐ 僅保留這一個 closeBtn

    [Header("【歷史對話開關】")]
    [SerializeField] private Button historyToggleBtn;
    [SerializeField] private GameObject historyPopupPanel;
    [SerializeField] private Transform historyContent;
    [SerializeField] private GameObject historyItemPrefab;

    [Header("【聊天訊息流 (Scroll View)】")]
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private Transform chatMessageContent;
    [SerializeField] private GameObject playerMessagePrefab;
    [SerializeField] private GameObject birdMessagePrefab;

    [Header("【自適應高度輸入框】")]
    [SerializeField] private Image inputFrameImage;
    [SerializeField] private RectTransform inputAreaRect;
    [SerializeField] private TMP_InputField questionInput;
    [SerializeField] private Button sendBtn;

    [Header("【三種高度 Sprite 素材】")]
    [SerializeField] private Sprite bgHeight1_Sprite;
    [SerializeField] private Sprite bgHeight2_Sprite;
    [SerializeField] private Sprite bgHeight3_Sprite;
    [SerializeField] private float height1 = 80f;
    [SerializeField] private float height2 = 130f;
    [SerializeField] private float height3 = 180f;

    [Header("【後端連線】")]
    [SerializeField] private string askUrl = "http://127.0.0.1:8000/ask";

    private bool isRequesting = false;
    private List<ChatHistoryRecord> chatHistoryList = new List<ChatHistoryRecord>();

    private void Awake()
{
    // 1. 自動取得當前 Scene 名稱作為 case_id 傳給後端
    currentCaseId = SceneManager.GetActiveScene().name;

    // 2. ⭐ 同時判定 SubjectID 與 PlayerName：若已有玩家名字，絕不覆蓋
    if (string.IsNullOrEmpty(GameData.SubjectID) && string.IsNullOrEmpty(GameData.PlayerName))
    {
        GameData.SubjectID = "Test_Dev";
        GameData.PlayerName = "測試主角";
        GameData.CurrentCondition = ExperimentCondition.HighInfo_AI;
        Debug.Log($"<color=#00FF00>【場景 {currentCaseId} 獨立測試】已自動配置預設測試身分與 AI！</color>");
    }
    else if (string.IsNullOrEmpty(GameData.SubjectID) && !string.IsNullOrEmpty(GameData.PlayerName))
    {
        // 若前面已有玩家名字，補上 SubjectID 同步即可，不覆蓋姓名
        GameData.SubjectID = GameData.PlayerName;
    }
}
    private void Start()
    {
        // 3. 實驗組別判定：無 AI 則隱藏整個側欄物件
        if (!GameData.HasAI)
        {
            if (sidebarRoot != null) sidebarRoot.SetActive(false);
            else gameObject.SetActive(false); // 防呆：若沒綁定 sidebarRoot 則關閉自身
            return;
        }

        if (sidebarRoot != null) sidebarRoot.SetActive(true);
        if (historyPopupPanel != null) historyPopupPanel.SetActive(false);

        // 初始狀態：隱藏對話主視窗與背後遮罩
        SetChatVisible(false);

        // 4. 安全綁定按鈕點擊事件
        if (birdOpenBtn != null)
        {
            birdOpenBtn.onClick.RemoveAllListeners();
            birdOpenBtn.onClick.AddListener(() => SetChatVisible(true));
        }

        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(() => SetChatVisible(false));
        }

        if (historyToggleBtn != null)
        {
            historyToggleBtn.onClick.RemoveAllListeners();
            historyToggleBtn.onClick.AddListener(ToggleHistoryPanel);
        }

        if (sendBtn != null)
        {
            sendBtn.onClick.RemoveAllListeners();
            sendBtn.onClick.AddListener(OnSendQuestion);
        }

        if (questionInput != null)
        {
            questionInput.onValueChanged.RemoveAllListeners();
            questionInput.onValueChanged.AddListener(OnInputValueChanged);
        }

        SetInputHeightLevel(1);
    }

    // 統一切換面板與霧白底色
    public void SetChatVisible(bool isShow)
    {
        if (chatWindow != null) chatWindow.SetActive(isShow);
        if (dimmerOverlay != null) dimmerOverlay.SetActive(isShow);
        if (!isShow && historyPopupPanel != null) historyPopupPanel.SetActive(false);
    }

    private void OnInputValueChanged(string text)
    {
        int lineCount = text.Split('\n').Length;
        int textLength = text.Length;

        if (lineCount >= 3 || textLength > 45)
            SetInputHeightLevel(3);
        else if (lineCount == 2 || textLength > 20)
            SetInputHeightLevel(2);
        else
            SetInputHeightLevel(1);
    }

    private void SetInputHeightLevel(int level)
    {
        if (inputFrameImage == null || inputAreaRect == null) return;

        float targetHeight = height1;
        Sprite targetSprite = bgHeight1_Sprite;

        if (level == 2)
        {
            targetHeight = height2;
            targetSprite = bgHeight2_Sprite;
        }
        else if (level == 3)
        {
            targetHeight = height3;
            targetSprite = bgHeight3_Sprite;
        }

        inputFrameImage.sprite = targetSprite;
        inputAreaRect.sizeDelta = new Vector2(inputAreaRect.sizeDelta.x, targetHeight);
    }

    private void ToggleHistoryPanel()
    {
        if (historyPopupPanel == null) return;
        bool newState = !historyPopupPanel.activeSelf;
        historyPopupPanel.SetActive(newState);
        if (newState) RefreshHistoryView();
    }

    public void OnSendQuestion()
    {
        if (isRequesting) return;

        string question = questionInput != null ? questionInput.text.Trim() : "";
        if (string.IsNullOrEmpty(question)) return;

        // 1. 生成主角問題氣泡
        SpawnMessageBubble(playerMessagePrefab, question);
        RecordHistory("主角", question);

        questionInput.text = "";
        SetInputHeightLevel(1);

        // 2. 生成鳥鳥思考氣泡 (起跑點 0.0s)
        GameObject birdBubble = SpawnMessageBubble(birdMessagePrefab, "鳥鳥思考中... (0.0s) 啾！");
        TMP_Text birdReplyText = birdBubble.GetComponentInChildren<TMP_Text>();

        StartCoroutine(PostQuestionRoutineWithTimer(question, birdReplyText, birdBubble));
    }

    private IEnumerator PostQuestionRoutineWithTimer(string userQuery, TMP_Text targetText, GameObject bubbleObj)
    {
        isRequesting = true;
        if (sendBtn != null) sendBtn.interactable = false;

        AIQueryRequest reqPayload = new AIQueryRequest
        {
            query = userQuery,
            case_id = currentCaseId,

            // ======================================
            // AI 防暴雷：附帶目前劇情進度
            story_step = AIProgress.CurrentStoryStep
            // ======================================
        };

        string jsonString = JsonUtility.ToJson(reqPayload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest request = new UnityWebRequest(askUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 40; // 放寬至 40 秒避免誤報 timeout

            var asyncOp = request.SendWebRequest();

            float timer = 0f;
            // ⭐ 只要網路請求還在跑，每一幀持續累加秒數並刷新文字
            while (!asyncOp.isDone)
            {
                timer += Time.deltaTime;
                if (targetText != null)
                {
                    targetText.text = $"鳥鳥思考中... ({timer:F1}s) ";
                }
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string resJson = request.downloadHandler.text;
                try
                {
                    AIQueryResponse response = JsonUtility.FromJson<AIQueryResponse>(resJson);
                    if (targetText != null) targetText.text = response.short_answer;
                    RecordHistory("鳥鳥", response.short_answer);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[JSON Error]: {e.Message}");
                    if (targetText != null) targetText.text = "鳥鳥收到了回覆，但腦袋解讀打結了...啾！";
                }
            }
            else
            {
                if (targetText != null)
                {
                    targetText.text = $"連線失敗 ({request.error})，請再試一次喔！啾～";
                }
            }
        }

        // 強制刷新佈局高度
        if (bubbleObj != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(bubbleObj.GetComponent<RectTransform>());
        if (chatMessageContent != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(chatMessageContent.GetComponent<RectTransform>());

        ScrollToBottom();
        isRequesting = false;
        if (sendBtn != null) sendBtn.interactable = true;
    }

    private GameObject SpawnMessageBubble(GameObject prefab, string message)
    {
        if (prefab == null)
        {
            Debug.LogError("【AI Tutor 錯誤】Message Prefab 尚未拖入 Inspector 欄位！");
            return null;
        }

        if (chatMessageContent == null)
        {
            Debug.LogError("【AI Tutor 錯誤】Chat Message Content 尚未拖入 Inspector 欄位！");
            return null;
        }

        GameObject bubbleObj = Instantiate(prefab, chatMessageContent);
        
        // 強制重設尺寸與縮放，防止變形或隱形
        RectTransform rt = bubbleObj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.one;
            rt.localPosition = Vector3.zero;
        }

        TMP_Text textComp = bubbleObj.GetComponentInChildren<TMP_Text>();
        if (textComp != null)
        {
            textComp.text = message;
            Debug.Log($"【AI Tutor 成功】成功生成訊息氣泡，文字：{message}");
        }
        else
        {
            Debug.LogError("【AI Tutor 警告】Prefab 內部找不到任何 TMP_Text 元件，文字無法顯示！");
        }

        ScrollToBottom();
        return bubbleObj;
    }

    private void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomRoutine());
    }

    private IEnumerator ScrollToBottomRoutine()
    {
        yield return new WaitForEndOfFrame();
        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void RecordHistory(string role, string message)
    {
        chatHistoryList.Add(new ChatHistoryRecord { role = role, message = message });
    }

    private void RefreshHistoryView()
    {
        if (historyContent == null) return;

        foreach (Transform child in historyContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var record in chatHistoryList)
        {
            if (historyItemPrefab != null)
            {
                GameObject item = Instantiate(historyItemPrefab, historyContent);
                TMP_Text t = item.GetComponentInChildren<TMP_Text>();
                if (t != null) t.text = $"<b>{record.role}：</b>{record.message}";
            }
        }
    }
}