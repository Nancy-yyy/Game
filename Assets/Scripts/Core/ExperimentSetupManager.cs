using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 實驗開始前的設定管理器。
///
/// 功能：
/// 1. 輸入受試者 ID 數字部分，例如 001，程式自動組成 P001。
/// 2. 選擇回饋模式（Simple / Deep）。
/// 3. 選擇是否開啟 AI 助教。
/// 4. 顯示本機目前四個實驗組的完成樣本數。
/// 5. 將 SubjectID、AssignedCondition、ActualCondition 寫入 GameData。
/// 6. 初始化本位受試者的實驗資料後，進入 StartScene。
/// </summary>
public class ExperimentSetupManager : MonoBehaviour
{
    // =========================================================
    // 區塊 0：受試者 ID
    // =========================================================

    [Header("Participant ID")]

    /// <summary>
    /// 只輸入三位數字，例如：
    /// 001、002、125。
    ///
    /// 程式會自動在前方加上 P，
    /// 最後存入 GameData.SubjectID 的格式為：
    /// P001、P002、P125。
    /// </summary>
    [SerializeField] private TMP_InputField subjectIdInput;

    /// <summary>
    /// 受試者 ID 輸入錯誤時顯示的提示文字。
    /// 平常保持空白。
    /// </summary>
    [SerializeField] private TMP_Text subjectIdErrorText;


    // =========================================================
    // 區塊 1：AI Tutor
    // =========================================================

    [Header("AI Tutor")]

    /// <summary>
    /// 是否開啟 AI 助教。
    /// OFF = NoAI
    /// ON  = AI
    /// </summary>
    [SerializeField] private Toggle aiToggle;


    // =========================================================
    // 區塊 2：畫面顯示
    // =========================================================

    [Header("Display")]

    /// <summary>
    /// 顯示目前選擇的回饋模式與 AI 狀態。
    /// </summary>
    [SerializeField] private TMP_Text currentSettingText;

    /// <summary>
    /// 顯示本機 Experiment_Results.csv 中
    /// 四個實驗組目前已完成的樣本數。
    /// </summary>
    [SerializeField] private TMP_Text groupCountText;


    // =========================================================
    // 區塊 3：目前選擇的回饋模式
    // =========================================================

    /// <summary>
    /// 預設為 Simple。
    /// Simple = LowInfo
    /// Deep   = HighInfo
    /// </summary>
    private FeedbackMode selectedFeedbackMode =
        FeedbackMode.Simple;


    // =========================================================
    // 區塊 4：初始化
    // =========================================================

    private void Start()
    {
        // -------------------------
        // 預設實驗設定
        // -------------------------

        selectedFeedbackMode =
            FeedbackMode.Simple;

        aiToggle.isOn = false;


        // -------------------------
        // 受試者 ID 輸入框設定
        // -------------------------

        if (subjectIdInput != null)
        {
            // 只允許輸入整數。
            subjectIdInput.contentType =
                TMP_InputField.ContentType.IntegerNumber;

            // 最多輸入三碼。
            subjectIdInput.characterLimit = 3;

            // 每次重新進入設定畫面時清空。
            subjectIdInput.text = "";

            // 玩家重新輸入時，自動清除上一個錯誤提示。
            subjectIdInput.onValueChanged.AddListener(
                OnSubjectIdChanged
            );
        }


        // -------------------------
        // 錯誤提示預設清空
        // -------------------------

        ClearSubjectIdError();


        // -------------------------
        // 更新畫面
        // -------------------------

        UpdateCurrentSettingText();
        UpdateGroupCounts();
    }


    private void OnDestroy()
    {
        // 移除程式在 Start() 中加入的 Listener。
        if (subjectIdInput != null)
        {
            subjectIdInput.onValueChanged.RemoveListener(
                OnSubjectIdChanged
            );
        }
    }


    // =========================================================
    // 區塊 5：回饋模式按鈕
    // =========================================================

    /// <summary>
    /// 選擇簡易系統教學引導。
    /// 對應 LowInfo。
    /// </summary>
    public void SelectSimpleMode()
    {
        selectedFeedbackMode =
            FeedbackMode.Simple;

        UpdateCurrentSettingText();
    }


    /// <summary>
    /// 選擇深層系統教學引導。
    /// 對應 HighInfo。
    /// </summary>
    public void SelectDeepMode()
    {
        selectedFeedbackMode =
            FeedbackMode.Deep;

        UpdateCurrentSettingText();
    }


    // =========================================================
    // 區塊 6：AI Toggle
    // =========================================================

    /// <summary>
    /// AI Toggle 狀態改變時呼叫。
    /// </summary>
    public void OnAIToggleChanged()
    {
        UpdateCurrentSettingText();
    }


    // =========================================================
    // 區塊 7：目前設定顯示
    // =========================================================

    /// <summary>
    /// 更新畫面上的目前實驗設定文字。
    /// </summary>
    private void UpdateCurrentSettingText()
    {
        string feedbackText =
            selectedFeedbackMode == FeedbackMode.Simple
            ? "簡易系統教學引導"
            : "深層系統教學引導";

        string aiText =
            aiToggle.isOn
            ? "ON"
            : "OFF";

        currentSettingText.text =
            "【目前設定】" +
            feedbackText +
            "｜AI 助教 " +
            aiText;
    }


    // =========================================================
    // 區塊 8：各實驗組完成樣本數
    // =========================================================

    /// <summary>
    /// 讀取本機 Experiment_Results.csv，
    /// 顯示四個實驗組目前已完成的樣本數。
    ///
    /// 注意：
    /// 多台電腦進行實驗時，
    /// 這裡顯示的是「本台電腦」自己的完成樣本數，
    /// 不是所有實驗電腦的總樣本數。
    /// </summary>
    private void UpdateGroupCounts()
    {
        Dictionary<ExperimentCondition, int> counts =
            GameData.GetConditionCounts();

        groupCountText.text =
            "【目前完成樣本數】\n" +
            "  簡易 / 無 AI：" +
            counts[ExperimentCondition.LowInfo_NoAI] + "\n" +

            "  深層 / 無 AI：" +
            counts[ExperimentCondition.HighInfo_NoAI] + "\n" +

            "  簡易 / AI：" +
            counts[ExperimentCondition.LowInfo_AI] + "\n" +

            "  深層 / AI：" +
            counts[ExperimentCondition.HighInfo_AI];
    }


    // =========================================================
    // 區塊 9：SubjectID 驗證
    // =========================================================

    /// <summary>
    /// 玩家修改 SubjectID 時，
    /// 清除上一個錯誤提示。
    /// </summary>
    private void OnSubjectIdChanged(string value)
    {
        ClearSubjectIdError();
    }


    /// <summary>
    /// 驗證受試者編號。
    ///
    /// 玩家只需輸入三位數字：
    /// 001～999。
    ///
    /// 不接受：
    /// 空白、1、01、000、1000、ABC。
    /// </summary>
    private bool ValidateSubjectNumber(
        string subjectNumber
    )
    {
        if (string.IsNullOrWhiteSpace(subjectNumber))
        {
            ShowSubjectIdError(
                "格式有誤"
            );

            return false;
        }

        // 必須剛好為三位數字。
        if (!Regex.IsMatch(subjectNumber, @"^\d{3}$"))
        {
            ShowSubjectIdError(
                "需為三位數字"
            );

            return false;
        }

        // P000 不作為正式受試者編號。
        if (subjectNumber == "000")
        {
            ShowSubjectIdError(
                "請從 001 開始"
            );

            return false;
        }

        return true;
    }


    /// <summary>
    /// 顯示 SubjectID 錯誤訊息。
    /// </summary>
    private void ShowSubjectIdError(string message)
    {
        if (subjectIdErrorText != null)
        {
            subjectIdErrorText.text = message;
        }
    }


    /// <summary>
    /// 清除 SubjectID 錯誤訊息。
    /// </summary>
    private void ClearSubjectIdError()
    {
        if (subjectIdErrorText != null)
        {
            subjectIdErrorText.text = "";
        }
    }


    // =========================================================
    // 區塊 10：取得目前選擇的 2 × 2 實驗條件
    // =========================================================

    /// <summary>
    /// 根據：
    /// 1. Simple / Deep
    /// 2. AI OFF / ON
    ///
    /// 回傳四個 ExperimentCondition 其中之一。
    /// </summary>
    private ExperimentCondition GetSelectedCondition()
    {
        if (
            selectedFeedbackMode == FeedbackMode.Simple &&
            !aiToggle.isOn
        )
        {
            return ExperimentCondition.LowInfo_NoAI;
        }

        if (
            selectedFeedbackMode == FeedbackMode.Deep &&
            !aiToggle.isOn
        )
        {
            return ExperimentCondition.HighInfo_NoAI;
        }

        if (
            selectedFeedbackMode == FeedbackMode.Simple &&
            aiToggle.isOn
        )
        {
            return ExperimentCondition.LowInfo_AI;
        }

        return ExperimentCondition.HighInfo_AI;
    }


    // =========================================================
    // 區塊 11：確認設定並開始實驗
    // =========================================================

    /// <summary>
    /// ConfirmButton 的 OnClick() 呼叫此方法。
    ///
    /// 執行順序：
    /// 1. 驗證受試者編號。
    /// 2. 自動組成完整 SubjectID，例如 P001。
    /// 3. 取得目前選擇的實驗條件。
    /// 4. 設定 AssignedCondition / ActualCondition。
    /// 5. 初始化 GameData。
    /// 6. 載入 StartScene。
    /// </summary>
    public void ConfirmSetting()
    {
        // -------------------------
        // 1. 取得玩家輸入的三位數編號
        // -------------------------

        string subjectNumber =
            subjectIdInput.text.Trim();


        // -------------------------
        // 2. 驗證 SubjectID
        // -------------------------

        if (!ValidateSubjectNumber(subjectNumber))
        {
            return;
        }


        // -------------------------
        // 3. 組成完整 SubjectID
        //
        // 例如：
        // 輸入 001
        // → P001
        // -------------------------

        string fullSubjectID =
            "P" + subjectNumber;


        // -------------------------
        // 4. 取得目前選擇的實驗條件
        // -------------------------

        ExperimentCondition selectedCondition =
            GetSelectedCondition();


        // -------------------------
        // 5. 寫入 GameData
        // -------------------------

        GameData.SubjectID =
            fullSubjectID;

        // 目前此畫面由實驗人員直接選擇
        // 該受試者應進入的實驗組，
        // 因此 AssignedCondition 與 ActualCondition
        // 在正常情況下會相同。
        GameData.SetExperimentConditions(
            selectedCondition,
            selectedCondition
        );


        // -------------------------
        // 6. 初始化本位受試者的研究資料
        // -------------------------

        GameData.InitializeExperiment();


        // -------------------------
        // 7. Debug 確認
        // -------------------------

        Debug.Log(
            "[ExperimentSetup 完成]\n" +
            "SubjectID: " +
            GameData.SubjectID + "\n" +

            "AssignedCondition: " +
            GameData.AssignedCondition + "\n" +

            "ActualCondition: " +
            GameData.ActualCondition
        );


        // -------------------------
        // 8. 進入遊戲開始場景
        // -------------------------

        SceneManager.LoadScene(
            "StartScene"
        );
    }
}