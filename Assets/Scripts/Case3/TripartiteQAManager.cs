using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TripartiteQAManager : MonoBehaviour
{
    public static TripartiteQAManager Instance;

    public enum RoleType { Provider, User, Platform }

    [System.Serializable]
    public struct QAItem
    {
        [TextArea] public string questionText;
        public RoleType correctRole;
    }

    [Header("=== 介面面板 ===")]
    public GameObject tripartitePanel;         // 白光空間底圖面板
    public TextMeshProUGUI txtQuestion;        // 上方螢幕顯示的題目文字
    public TextMeshProUGUI txtProgress;        // 進度提示 (例如: 1/10)
    public GameObject successPanel;            // 三方關係圖.jpg 面板
    public Button btnSuccessConfirm;           // 【太棒了！】按鈕

    [Header("=== 題目下方提示對話框 ===")]
    public TextMeshProUGUI txtHint;            // 題目下方的提示文字

    [Header("=== 3 個角色大按鈕 ===")]
    public Button btnProvider;                 // 空間提供者
    public Button btnUser;                     // 空間使用者
    public Button btnPlatform;                 // 共享平台

    [Header("=== 錯誤提示 (低資訊量共用) ===")]
    public GameObject wrongHintBackdrop;
    public Button btnCloseWrongPrompt;

    [Header("=== 錯誤提示 (高資訊量深層解析) ===")]
    public GameObject highInfoPanel;             // 高資訊專用解析夾板面板
    public TextMeshProUGUI txtHighInfoExplanation; // 解析夾板內的文字
    public Button btnHighInfoConfirm;            // 解析看完了之後的【確認】按鈕
    public Button highInfoClickBlocker;          // 全螢幕點擊感應器（點擊推進解析）

    [Header("引導台詞與分層控制")]
    public GameObject introGroup;               // 整個提示圖層 (包含標題底圖與夾板)
    public CanvasGroup introBoardCanvasGroup;   // 橘色夾板上的 CanvasGroup
    public TextMeshProUGUI introText;           // 夾板內的文字 (Txt_Intro)
    public Button introClickBlocker;            // 全螢幕透明點擊感應器
    public Button btnStartChallenge;            // 「開始挑戰」按鈕
    public GameObject gameContentGroup;         // 題目與三方按鈕群組

    [Header("動畫參數")]
    public float exitAnimDuration = 0.4f;

    private readonly string[] introLines = new string[]
    {
        "在共享經濟與媒合模式中，\n平台、空間提供者與使用者各自承擔不同的權利與義務。",
        "接下來將進行情境測驗，\n請根據剛才的討論，選出最合理的三方應對策略！"
    };
    private int currentIntroIndex = 0;
    private bool canClickIntro = false;

    // 高資訊深層解析專用變數
    private string[] currentExplanationLines;
    private int currentExpIndex = 0;

    [Header("=== 10 道題庫清單 ===")]
    public List<QAItem> qaList = new List<QAItem>();

    private int currentIndex = 0;
    private bool isAnswering = false;

    void Awake()
    {
        Instance = this;
        InitDefaultQuestions();
    }

    void Start()
    {
        // 綁定三方角色與關卡按鈕
        if (btnProvider != null) btnProvider.onClick.AddListener(() => OnRoleClicked(RoleType.Provider, btnProvider));
        if (btnUser != null) btnUser.onClick.AddListener(() => OnRoleClicked(RoleType.User, btnUser));
        if (btnPlatform != null) btnPlatform.onClick.AddListener(() => OnRoleClicked(RoleType.Platform, btnPlatform));
        if (btnSuccessConfirm != null) btnSuccessConfirm.onClick.AddListener(OnSuccessConfirmClicked);
        
        // 低資訊關閉按鈕
        if (btnCloseWrongPrompt != null) btnCloseWrongPrompt.onClick.AddListener(() => { if (wrongHintBackdrop != null) wrongHintBackdrop.SetActive(false); });

        // 高資訊：點擊螢幕推進解析
        if (highInfoClickBlocker != null)
        {
            highInfoClickBlocker.onClick.RemoveAllListeners();
            highInfoClickBlocker.onClick.AddListener(OnHighInfoPanelClicked);
        }

        // 高資訊：點擊【確認】按鈕返回重新作答
        if (btnHighInfoConfirm != null)
        {
            btnHighInfoConfirm.onClick.RemoveAllListeners();
            btnHighInfoConfirm.onClick.AddListener(OnHighInfoConfirmClicked);
        }

        // 綁定引導點擊互動
        if (introClickBlocker != null)
        {
            introClickBlocker.onClick.RemoveAllListeners();
            introClickBlocker.onClick.AddListener(OnIntroClicked);
        }

        if (btnStartChallenge != null)
        {
            btnStartChallenge.onClick.RemoveAllListeners();
            btnStartChallenge.onClick.AddListener(OnStartChallengeClicked);
        }

        // 開局隱藏
        if (tripartitePanel != null) tripartitePanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
        if (highInfoPanel != null) highInfoPanel.SetActive(false);
    }

    public void StartQAGameWithTransition()
    {
        if (GlobalFader.Instance != null)
        {
            GlobalFader.Instance.FadeTransition(() => ShowIntro());
        }
        else
        {
            ShowIntro();
        }
    }

    public void ShowIntro()
    {
        currentIntroIndex = 0;
        canClickIntro = false;

        if (tripartitePanel != null) tripartitePanel.SetActive(true);
        if (introGroup != null) introGroup.SetActive(true);
        if (gameContentGroup != null) gameContentGroup.SetActive(false);
        if (btnStartChallenge != null) btnStartChallenge.gameObject.SetActive(false);
        if (introClickBlocker != null) introClickBlocker.gameObject.SetActive(true);

        if (introBoardCanvasGroup != null)
        {
            introBoardCanvasGroup.alpha = 1f;
            introBoardCanvasGroup.transform.localScale = Vector3.one;
        }

        UpdateIntroText();
        StartCoroutine(EnableIntroClickRoutine());
    }

    private void UpdateIntroText()
    {
        if (introText != null && currentIntroIndex < introLines.Length)
        {
            introText.text = introLines[currentIntroIndex];
        }
    }

    private IEnumerator EnableIntroClickRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        canClickIntro = true;
    }

    private void OnIntroClicked()
    {
        if (!canClickIntro) return;

        currentIntroIndex++;

        if (currentIntroIndex < introLines.Length)
        {
            UpdateIntroText();

            if (currentIntroIndex == introLines.Length - 1)
            {
                if (introClickBlocker != null) introClickBlocker.gameObject.SetActive(false);
                if (btnStartChallenge != null) btnStartChallenge.gameObject.SetActive(true);
            }
        }
    }

    private void OnStartChallengeClicked()
    {
        StartCoroutine(DismissIntroAndStartGameRoutine());
    }

    private IEnumerator DismissIntroAndStartGameRoutine()
    {
        if (introBoardCanvasGroup != null)
        {
            float elapsed = 0f;
            Vector3 startScale = introBoardCanvasGroup.transform.localScale;
            Vector3 targetScale = Vector3.one * 0.7f;

            while (elapsed < exitAnimDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / exitAnimDuration;
                float t = Mathf.SmoothStep(0f, 1f, progress);

                introBoardCanvasGroup.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                introBoardCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
        }

        if (introGroup != null) introGroup.SetActive(false);
        if (gameContentGroup != null) gameContentGroup.SetActive(true);

        StartQAGame();
    }

    public void StartQAGame()
    {
        if (tripartitePanel != null) tripartitePanel.SetActive(true);
        if (successPanel != null) successPanel.SetActive(false);
        if (highInfoPanel != null) highInfoPanel.SetActive(false);
        currentIndex = 0;
        isAnswering = false;
        DisplayCurrentQuestion();
    }

    private void DisplayCurrentQuestion()
    {
        if (currentIndex < qaList.Count)
        {
            // 研究紀錄：每一題都是 C3_THREEPARTY 題組中的獨立子 Task
            // 這樣每題都有自己的 FirstAttemptCorrect、Attempts、TimeToCorrect。
            GameData.StartTask(GetResearchTaskId(currentIndex));

            if (txtQuestion != null) txtQuestion.text = qaList[currentIndex].questionText;
            if (txtProgress != null) txtProgress.text = $"{currentIndex + 1} / {qaList.Count}";
            if (txtHint != null) txtHint.text = "請點擊下方負責這項職責的角色！";
        }
    }

    private void OnRoleClicked(RoleType clickedRole, Button button)
    {
        if (isAnswering || currentIndex >= qaList.Count) return;

        RoleType correctRole = qaList[currentIndex].correctRole;
        bool isCorrect = clickedRole == correctRole;

        // 研究紀錄：保留玩家實際選擇 Provider / User / Platform
        GameData.RecordAnswer(clickedRole.ToString(), isCorrect);

        if (isCorrect)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCorrect();
            if (txtHint != null) txtHint.text = "答對了！沒錯，就是他！";
            StartCoroutine(CorrectRoutine(button));
        }
        else
        {
            // 🌟 記錄作答錯誤次數
            GameData.Case3_QuizErrors++;

            if (AudioManager.Instance != null) AudioManager.Instance.PlayWrong();

            if (GameData.IsHighInfo)
            {
                // 🌟 高資訊版本：開啟獨立解析夾板，載入多句解析
                if (highInfoPanel != null) highInfoPanel.SetActive(true);
                currentExpIndex = 0;
                currentExplanationLines = GetDetailedExplanationLines(currentIndex, clickedRole, correctRole);
                UpdateHighInfoText();
            }
            else
            {
                // 🌟 低資訊版本：維持簡單提示
                if (txtHint != null) txtHint.text = "不是他喔，再想想看！";
                if (wrongHintBackdrop != null)
                {
                    wrongHintBackdrop.transform.SetAsLastSibling();
                    wrongHintBackdrop.SetActive(true);
                }
            }

            // 研究紀錄：錯誤後實際看到一次 Low / High 回饋
            GameData.RecordFeedbackShown(
                "C3_THREEPARTY_Q" + (currentIndex + 1) + "_WRONG"
            );
        }
    }

    // 點擊畫面推進解析
    private void OnHighInfoPanelClicked()
    {
        if (currentExplanationLines == null) return;

        currentExpIndex++;
        if (currentExpIndex < currentExplanationLines.Length)
        {
            UpdateHighInfoText();
        }
        else
        {
            // 解析看完了：隱藏點擊感應器，顯示【確認】按鈕
            if (highInfoClickBlocker != null) highInfoClickBlocker.gameObject.SetActive(false);
            if (btnHighInfoConfirm != null) btnHighInfoConfirm.gameObject.SetActive(true);
        }
    }

    private void UpdateHighInfoText()
    {
        if (txtHighInfoExplanation != null && currentExplanationLines != null && currentExpIndex < currentExplanationLines.Length)
        {
            txtHighInfoExplanation.text = $"【觀念深層解析 ({currentExpIndex + 1}/{currentExplanationLines.Length})】\n\n" + 
                                          currentExplanationLines[currentExpIndex];

            // 確保尚未看完前，確認按鈕是關閉的，點擊感應器是開啟的
            if (btnHighInfoConfirm != null) btnHighInfoConfirm.gameObject.SetActive(false);
            if (highInfoClickBlocker != null) highInfoClickBlocker.gameObject.SetActive(true);
        }
    }

    // 點擊【確認】按鈕後：關閉解析面板，讓玩家留在同一題重新選擇
    private void OnHighInfoConfirmClicked()
    {
        if (highInfoPanel != null) highInfoPanel.SetActive(false);
        // currentIndex 不增加，代表留在同一題讓玩家重新作答
        isAnswering = false; 
    }

    private IEnumerator CorrectRoutine(Button btn)
    {
        isAnswering = true;
        Vector3 originalScale = btn.transform.localScale;
        btn.transform.localScale = originalScale * 1.15f;
        yield return new WaitForSeconds(0.15f);
        btn.transform.localScale = originalScale;

        yield return new WaitForSeconds(0.4f);

        // 目前這一題正式完成
        GameData.CompleteTask();

        currentIndex++;
        if (currentIndex >= qaList.Count)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySuccess();
            if (tripartitePanel != null) tripartitePanel.SetActive(false);
            if (successPanel != null) successPanel.SetActive(true);
        }
        else
        {
            DisplayCurrentQuestion();
            isAnswering = false;
        }
    }

    private void OnSuccessConfirmClicked()
    {
        if (successPanel != null) successPanel.SetActive(false);
        if (PeopleSelectionManager.Instance != null)
        {
            PeopleSelectionManager.Instance.StartFinalSummaryStory();
        }
    }
  
    // 10 道題目的多句深層解析對照
    private string[] GetDetailedExplanationLines(int qIndex, RoleType clicked, RoleType correct)
    {
        string correctName = GetRoleName(correct);
        string clickedName = GetRoleName(clicked);

        switch (qIndex)
        {
            case 0: // 提供特定時段未使用的閒置空間 (Provider)
                return new string[]
                {
                    $"你剛剛選擇了「{clickedName}」，這是不對的喔！",
                    "在共享經濟中，空間提供者擁有實體資產的產權，並在閒置時段將其釋出以活化資產。",
                    "而平台只負責居中媒合與提供資訊，並不擁有或提供自有空間。請重新思考產權與釋出者的關係！"
                };
            case 1: // 支付租借費用或押金 (User)
                return new string[]
                {
                    $"選錯囉！這項職責不屬於「{clickedName}」。",
                    "空間使用者是享受服務、使用資產，並必須支付對價成本與租借費用的一方。",
                    "相對地，空間提供者才是收取收益的人。請再次確認金流與使用者的對應關係！"
                };
            case 2: // 蒐集供需資訊並比較時間、設備與價格 (Platform)
                return new string[]
                {
                    $"答錯囉！你選到了「{clickedName}」。",
                    "共享平台在模式中扮演的是「資訊中介」的角色。",
                    "平台負責統整市場上的供需資訊，讓使用者能夠便利地比較時間、設備與價格。"
                };
            case 3: // 保留空間的「所有權」 (Provider)
                return new string[]
                {
                    $"注意囉！「{clickedName}」並不具備這項權利。",
                    "共享經濟的核心精神就是「所有權與使用權的分離」。",
                    "空間提供者雖然出讓空間給別人使用，但依然完整保留該資產的「所有權」。"
                };
            case 4: // 僅取得限定期間的「使用權」 (User)
                return new string[]
                {
                    $"這樣想是不對的喔！你選了「{clickedName}」。",
                    "使用者付費後，所獲得的僅僅是「特定期間內的合法使用權」。",
                    "這並不代表使用者取得了該空間的產權或永久所有權。"
                };
            case 5: // 提供實名身分驗證與損壞賠償保障機制 (Platform)
                return new string[]
                {
                    $"不對喔！這題的正確負責方是「{correctName}」。",
                    "為了降低陌生人間的交易風險與安全疑慮，中介平台必須建立完善的信任機制。",
                    "這包含了實名身分驗證以及損壞賠償保障等制度設計。"
                };
            case 6: // 設定可使用範圍與進出基本規則 (Provider)
                return new string[]
                {
                    $"再想想看！選「{clickedName}」是不對的。",
                    "空間擁有者（提供者）為了保護自己的實體資產與隱私，",
                    "有權利也有義務設定場地的使用邊界、進出時間與基本行為守則。"
                };
            case 7: // 遵守空間使用規範並於結束時拍照回報復原 (User)
                return new string[]
                {
                    $"這題不是由「{clickedName}」來執行喔！",
                    "使用者在享受共享便利的同時，必須履行維護環境的責任。",
                    "活動結束後按規定拍照回報並將場地復原，是使用者基本的信用義務。"
                };
            case 8: // 保存交易紀錄並在發生爭議時釐清責任歸屬 (Platform)
                return new string[]
                {
                    $"方向有點偏差囉！你選到了「{clickedName}」。",
                    "平台作為第三方媒合者，必須妥善保存所有的交易與通訊紀錄。",
                    "當提供者與使用者發生消費爭議時，平台必須客觀地依據紀錄來釐清責任歸屬。"
                };
            case 9: // 制定取消預約規範以保障業主不被臨時放鴿子 (Platform)
                return new string[]
                {
                    $"答錯囉！這項機制是由「{correctName}」來制定的。",
                    "為了維護市場秩序並保障空間業主的權益，避免業主被臨時取消而蒙受損失，",
                    "平台會建立標準化的取消預約與退款規範。"
                };
            default:
                return new string[]
                {
                    $"選錯囉！正確答案應該由「{correctName}」來負責。",
                    "請重新回想三方在共享經濟中的權利與義務分工界線！"
                };
        }
    }

    private string GetResearchTaskId(int qIndex)
    {
        switch (qIndex)
        {
            case 0: return GameData.TaskIds.C3_THREEPARTY_Q1;
            case 1: return GameData.TaskIds.C3_THREEPARTY_Q2;
            case 2: return GameData.TaskIds.C3_THREEPARTY_Q3;
            case 3: return GameData.TaskIds.C3_THREEPARTY_Q4;
            case 4: return GameData.TaskIds.C3_THREEPARTY_Q5;
            case 5: return GameData.TaskIds.C3_THREEPARTY_Q6;
            case 6: return GameData.TaskIds.C3_THREEPARTY_Q7;
            case 7: return GameData.TaskIds.C3_THREEPARTY_Q8;
            case 8: return GameData.TaskIds.C3_THREEPARTY_Q9;
            case 9: return GameData.TaskIds.C3_THREEPARTY_Q10;
            default: return GameData.TaskIds.C3_THREEPARTY;
        }
    }

    private string GetRoleName(RoleType role)
    {
        switch (role)
        {
            case RoleType.Provider: return "空間提供者";
            case RoleType.User: return "空間使用者";
            case RoleType.Platform: return "共享平台";
            default: return "未知角色";
        }
    }
    public void InitDefaultQuestions()
    {
        qaList.Clear();
        qaList.Add(new QAItem { questionText = "提供特定時段未使用的閒置空間", correctRole = RoleType.Provider });
        qaList.Add(new QAItem { questionText = "支付租借費用或押金", correctRole = RoleType.User });
        qaList.Add(new QAItem { questionText = "蒐集供需資訊並比較時間、設備與價格", correctRole = RoleType.Platform });
        qaList.Add(new QAItem { questionText = "保留空間的「所有權」", correctRole = RoleType.Provider });
        qaList.Add(new QAItem { questionText = "僅取得限定期間的「使用權」", correctRole = RoleType.User });
        qaList.Add(new QAItem { questionText = "提供實名身分驗證與損壞賠償保障機制", correctRole = RoleType.Platform });
        qaList.Add(new QAItem { questionText = "設定可使用範圍與進出基本規則", correctRole = RoleType.Provider });
        qaList.Add(new QAItem { questionText = "遵守空間使用規範並於結束時拍照回報復原", correctRole = RoleType.User });
        qaList.Add(new QAItem { questionText = "保存交易紀錄並在發生爭議時釐清責任歸屬", correctRole = RoleType.Platform });
        qaList.Add(new QAItem { questionText = "制定取消預約規範以保障業主不被臨時放鴿子", correctRole = RoleType.Platform });
    }
}