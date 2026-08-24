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

    [Header("=== 錯誤提示 (共用) ===")]
    public GameObject wrongHintBackdrop;
    public Button btnCloseWrongPrompt;

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
        if (btnProvider != null) btnProvider.onClick.AddListener(() => OnRoleClicked(RoleType.Provider, btnProvider));
        if (btnUser != null) btnUser.onClick.AddListener(() => OnRoleClicked(RoleType.User, btnUser));
        if (btnPlatform != null) btnPlatform.onClick.AddListener(() => OnRoleClicked(RoleType.Platform, btnPlatform));

        if (btnSuccessConfirm != null) btnSuccessConfirm.onClick.AddListener(OnSuccessConfirmClicked);
        if (btnCloseWrongPrompt != null) btnCloseWrongPrompt.onClick.AddListener(() => { if (wrongHintBackdrop != null) wrongHintBackdrop.SetActive(false); });

        // 🌟 開局一律自動隱藏三方面板與成功面板，等待劇情觸發才開啟
        if (tripartitePanel != null) tripartitePanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
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

    public void StartQAGame()
    {
        if (tripartitePanel != null) tripartitePanel.SetActive(true);
        if (successPanel != null) successPanel.SetActive(false);
        currentIndex = 0;
        isAnswering = false;
        DisplayCurrentQuestion();
    }

    private void DisplayCurrentQuestion()
    {
        if (currentIndex < qaList.Count)
        {
            if (txtQuestion != null) txtQuestion.text = qaList[currentIndex].questionText;
            if (txtProgress != null) txtProgress.text = $"{currentIndex + 1} / {qaList.Count}";
            if (txtHint != null) txtHint.text = "請點擊下方負責這項職責的角色！";
        }
    }

    private void OnRoleClicked(RoleType clickedRole, Button button)
    {
        if (isAnswering || currentIndex >= qaList.Count) return;

        if (clickedRole == qaList[currentIndex].correctRole)
        {
            // 🌟 播放答對叮咚聲
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCorrect();

            if (txtHint != null) txtHint.text = "答對了！沒錯，就是他！";
            StartCoroutine(CorrectRoutine(button));
        }
        else
        {
            // 🌟 播放答錯蜂鳴聲
            if (AudioManager.Instance != null) AudioManager.Instance.PlayWrong();

            if (txtHint != null) txtHint.text = "不是他喔，再想想看！";
            if (wrongHintBackdrop != null)
            {
                wrongHintBackdrop.transform.SetAsLastSibling();
                wrongHintBackdrop.SetActive(true);
            }
        }
    }

    private IEnumerator CorrectRoutine(Button btn)
    {
        isAnswering = true;
        Vector3 originalScale = btn.transform.localScale;
        btn.transform.localScale = originalScale * 1.15f;
        yield return new WaitForSeconds(0.15f);
        btn.transform.localScale = originalScale;

        yield return new WaitForSeconds(0.4f);

        currentIndex++;
        if (currentIndex >= qaList.Count)
        {
            // 🌟 10 題通關：播放歡慶音效！
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
}