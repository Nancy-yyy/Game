using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 實驗組別條件：2 × 2 矩陣
/// 低資訊 / 高資訊 × AI 關閉 / AI 開啟
/// </summary>
public enum ExperimentCondition
{
    LowInfo_NoAI,
    HighInfo_NoAI,
    LowInfo_AI,
    HighInfo_AI
}

/// <summary>
/// 給既有遊戲流程使用的回饋模式
/// </summary>
public enum FeedbackMode
{
    Simple,
    Deep
}

public static class GameData
{
    // =========================================================
    // 區塊 0：實驗設定與條件判定
    // Experiment Settings
    // =========================================================

    /// <summary>
    /// 當前受試者的實驗組別。
    /// 由 ExperimentSetupManager 人工設定。
    /// </summary>
    public static ExperimentCondition CurrentCondition =
        ExperimentCondition.LowInfo_NoAI;

    /// <summary>
    /// 是否為 AI 助教組
    /// </summary>
    public static bool HasAI =>
        CurrentCondition == ExperimentCondition.LowInfo_AI ||
        CurrentCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 是否為高資訊量 / 深層回饋組
    /// </summary>
    public static bool IsHighInfo =>
        CurrentCondition == ExperimentCondition.HighInfo_NoAI ||
        CurrentCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 相容現有遊戲程式。
    /// LowInfo = Simple
    /// HighInfo = Deep
    /// </summary>
    public static FeedbackMode FeedbackMode =>
        IsHighInfo
            ? global::FeedbackMode.Deep
            : global::FeedbackMode.Simple;

    /// <summary>
    /// 相容現有遊戲程式的 AI 開關判斷
    /// </summary>
    public static bool AITutorEnabled => HasAI;


    // =========================================================
    // 區塊 1：遊戲基本資訊
    // Game Basic Information
    // =========================================================

    /// <summary>
    /// 玩家名稱，只供遊戲內角色對話顯示使用。
    /// 不匯出至實驗 CSV。
    /// </summary>
    public static string PlayerName = "主角";

    /// <summary>
    /// 實驗受試者 ID。
    /// 建議正式實驗使用 P001、P002 等匿名編號。
    /// </summary>
    public static string SubjectID = "";

    /// <summary>
    /// 實驗開始時間
    /// </summary>
    public static string ExperimentStartTime = "";

    /// <summary>
    /// 實驗結束時間
    /// </summary>
    public static string ExperimentEndTime = "";


    // =========================================================
    // 區塊 2：Case 1 紀錄
    // Case 1: Carpooling
    // =========================================================

    /// <summary>
    /// 教學點 1：找出閒置容量
    /// 點錯油箱、駕駛座等次數
    /// </summary>
    public static int Case1_IdleCapacityErrors = 0;

    /// <summary>
    /// 教學點 2：平台媒合流程排序錯誤次數
    /// </summary>
    public static int Case1_MatchingOrderErrors = 0;

    /// <summary>
    /// 教學點 3：供需與資源稀缺題答錯次數
    /// </summary>
    public static int Case1_ScarcityErrors = 0;

    /// <summary>
    /// Case 1 總遊玩時間（秒）
    /// </summary>
    public static float Case1_PlayTime = 0f;


    // =========================================================
    // 區塊 3：Case 2 紀錄
    // Case 2: Textbook Sharing
    // =========================================================

    /// <summary>
    /// Tutorial 1：所有權 vs 使用權錯誤次數
    /// </summary>
    public static int Case2_Tutorial1_Errors = 0;

    /// <summary>
    /// Tutorial 3：四格漫畫排序錯誤次數
    /// </summary>
    public static int Case2_Tutorial3_Errors = 0;

    /// <summary>
    /// Tutorial 4：線索判斷錯誤次數
    /// </summary>
    public static int Case2_Tutorial4_ClueErrors = 0;

    /// <summary>
    /// Tutorial 4：案例選擇錯誤次數
    /// </summary>
    public static int Case2_Tutorial4_CaseErrors = 0;

    /// <summary>
    /// Tutorial 4：自主反思輸入內容
    /// </summary>
    public static string Case2_Tutorial4_Reflection = "";

    /// <summary>
    /// 天平最終方案選擇
    /// </summary>
    public static string Case2_Scale_Choice = "";

    /// <summary>
    /// 天平理由勾選題錯誤次數
    /// </summary>
    public static int Case2_Scale_QuizErrors = 0;

    /// <summary>
    /// Case 2 總遊玩時間（秒）
    /// </summary>
    public static float Case2_PlayTime = 0f;


    // =========================================================
    // 區塊 4：Ending 紀錄
    // Concept Transfer & Sustainability
    // =========================================================

    /// <summary>
    /// 三格概念遷移題「整體答錯」次數。
    /// 每按一次檢查且至少一格錯誤，就 +1。
    /// </summary>
    public static int Ending_ConceptTransferErrors = 0;

    /// <summary>
    /// 三格填空：閒置資產概念錯誤次數
    /// </summary>
    public static int Ending_IdleAssetErrors = 0;

    /// <summary>
    /// 三格填空：使用權概念錯誤次數
    /// </summary>
    public static int Ending_UsageRightErrors = 0;

    /// <summary>
    /// 三格填空：媒合方式概念錯誤次數
    /// </summary>
    public static int Ending_MatchingMethodErrors = 0;

    /// <summary>
    /// 最終永續判斷題答錯次數
    /// </summary>
    public static int Ending_SustainabilityErrors = 0;

    /// <summary>
    /// Ending 總遊玩時間（秒）
    /// </summary>
    public static float Ending_PlayTime = 0f;


    // =========================================================
    // 區塊 5：整體時間紀錄
    // Timing
    // =========================================================

    /// <summary>
    /// 全程總遊玩時間
    /// </summary>
    public static float TotalGamePlayTime = 0f;

    private static float case1StartTime = 0f;
    private static float case2StartTime = 0f;
    private static float endingStartTime = 0f;
    private static float totalGameStartTime = 0f;


    // =========================================================
    // 區塊 6：實驗初始化
    // Experiment Initialization
    // =========================================================

    /// <summary>
    /// 正式開始實驗時呼叫一次。
    /// 不負責分組，組別已由 ExperimentSetupManager 決定。
    /// </summary>
    public static void InitializeExperiment()
    {
        ExperimentStartTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        totalGameStartTime = Time.time;

        ResetExperimentResults();

        Debug.Log(
            $"[實驗初始化] " +
            $"SubjectID: {SubjectID} | " +
            $"Condition: {CurrentCondition} | " +
            $"AI: {HasAI} | " +
            $"HighInfo: {IsHighInfo} | " +
            $"Start: {ExperimentStartTime}"
        );
    }


    /// <summary>
    /// 清除上一位受試者的實驗紀錄。
    /// 不清除 CurrentCondition、SubjectID、PlayerName。
    /// </summary>
    public static void ResetExperimentResults()
    {
        // Case 1
        Case1_IdleCapacityErrors = 0;
        Case1_MatchingOrderErrors = 0;
        Case1_ScarcityErrors = 0;
        Case1_PlayTime = 0f;

        // Case 2
        Case2_Tutorial1_Errors = 0;
        Case2_Tutorial3_Errors = 0;
        Case2_Tutorial4_ClueErrors = 0;
        Case2_Tutorial4_CaseErrors = 0;
        Case2_Tutorial4_Reflection = "";
        Case2_Scale_Choice = "";
        Case2_Scale_QuizErrors = 0;
        Case2_PlayTime = 0f;

        // Ending
        Ending_ConceptTransferErrors = 0;
        Ending_IdleAssetErrors = 0;
        Ending_UsageRightErrors = 0;
        Ending_MatchingMethodErrors = 0;
        Ending_SustainabilityErrors = 0;
        Ending_PlayTime = 0f;

        // Total
        TotalGamePlayTime = 0f;
        ExperimentEndTime = "";
    }


    // =========================================================
    // 區塊 7：各階段計時
    // Stage Timers
    // =========================================================

    public static void StartCase1Timer()
    {
        case1StartTime = Time.time;
    }

    public static void StopCase1Timer()
    {
        Case1_PlayTime = Time.time - case1StartTime;
    }

    public static void StartCase2Timer()
    {
        case2StartTime = Time.time;
    }

    public static void StopCase2Timer()
    {
        Case2_PlayTime = Time.time - case2StartTime;
    }

    public static void StartEndingTimer()
    {
        endingStartTime = Time.time;
    }

    public static void StopEndingTimer()
    {
        Ending_PlayTime = Time.time - endingStartTime;
    }


    // =========================================================
    // 區塊 8：讀取各實驗組目前完成樣本數
    // Condition Counts
    // =========================================================

    /// <summary>
    /// 從 Experiment_Results.csv 統計四組目前各有幾筆完成資料。
    /// 僅供 ExperimentSetup 顯示，不會自動分組。
    /// </summary>
    public static Dictionary<ExperimentCondition, int> GetConditionCounts()
    {
        Dictionary<ExperimentCondition, int> counts =
            new Dictionary<ExperimentCondition, int>();

        foreach (
            ExperimentCondition condition
            in Enum.GetValues(typeof(ExperimentCondition))
        )
        {
            counts[condition] = 0;
        }

        string filePath = GetCSVFilePath();

        if (!File.Exists(filePath))
        {
            return counts;
        }

        string[] lines =
            File.ReadAllLines(filePath, Encoding.UTF8);

        // 第 0 行為 Header
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            string[] columns = lines[i].Split(',');

            // CSV 第 3 欄（index 2）為組別條件
            if (columns.Length < 3)
            {
                continue;
            }

            ExperimentCondition condition;

            if (Enum.TryParse(columns[2], out condition))
            {
                counts[condition]++;
            }
        }

        return counts;
    }


    // =========================================================
    // 區塊 9：CSV 匯出
    // CSV Export
    // =========================================================

    private static string GetCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../Experiment_Results.csv"
        );
    }


    /// <summary>
    /// 遊戲完成時呼叫一次。
    /// 將本位受試者的資料追加到 Experiment_Results.csv。
    /// </summary>
    public static void ExportToCSV()
    {
        TotalGamePlayTime =
            Time.time - totalGameStartTime;

        ExperimentEndTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string filePath = GetCSVFilePath();

        // 第一次建立 CSV 時寫入 Header
        if (!File.Exists(filePath))
        {
            string header =
                "完成時間," +
                "受試者ID," +
                "組別條件," +
                "是否AI," +
                "是否高資訊," +

                "Case1閒置容量錯誤," +
                "Case1媒合流程錯誤," +
                "Case1資源稀缺錯誤," +
                "Case1耗時," +

                "Case2_Tut1錯誤," +
                "Case2_Tut3錯誤," +
                "Case2_Tut4線索錯誤," +
                "Case2_Tut4案例錯誤," +
                "Case2反思回答," +
                "Case2天平選擇," +
                "Case2理由錯誤," +
                "Case2耗時," +

                "Ending概念遷移錯誤," +
                "Ending閒置資產錯誤," +
                "Ending使用權錯誤," +
                "Ending媒合方式錯誤," +
                "Ending永續判斷錯誤," +
                "Ending耗時," +

                "總遊玩時間\n";

            File.WriteAllText(
                filePath,
                header,
                Encoding.UTF8
            );
        }

        string safeReflection =
            EscapeCSVField(
                Case2_Tutorial4_Reflection
            );

        string safeScaleChoice =
            EscapeCSVField(
                Case2_Scale_Choice
            );

        string safeSubjectID =
            EscapeCSVField(
                SubjectID
            );

        string recordLine =
            $"{ExperimentEndTime}," +
            $"{safeSubjectID}," +
            $"{CurrentCondition}," +
            $"{HasAI}," +
            $"{IsHighInfo}," +

            $"{Case1_IdleCapacityErrors}," +
            $"{Case1_MatchingOrderErrors}," +
            $"{Case1_ScarcityErrors}," +
            $"{Case1_PlayTime:F1}," +

            $"{Case2_Tutorial1_Errors}," +
            $"{Case2_Tutorial3_Errors}," +
            $"{Case2_Tutorial4_ClueErrors}," +
            $"{Case2_Tutorial4_CaseErrors}," +
            $"{safeReflection}," +
            $"{safeScaleChoice}," +
            $"{Case2_Scale_QuizErrors}," +
            $"{Case2_PlayTime:F1}," +

            $"{Ending_ConceptTransferErrors}," +
            $"{Ending_IdleAssetErrors}," +
            $"{Ending_UsageRightErrors}," +
            $"{Ending_MatchingMethodErrors}," +
            $"{Ending_SustainabilityErrors}," +
            $"{Ending_PlayTime:F1}," +

            $"{TotalGamePlayTime:F1}\n";

        File.AppendAllText(
            filePath,
            recordLine,
            Encoding.UTF8
        );

        Debug.Log(
            $"[實驗資料匯出成功] " +
            $"{Path.GetFullPath(filePath)}"
        );
    }


    /// <summary>
    /// CSV 欄位安全處理。
    /// 可處理逗號、換行與雙引號。
    /// </summary>
    private static string EscapeCSVField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        value = value.Replace(
            "\"",
            "\"\""
        );

        if (
            value.Contains(",") ||
            value.Contains("\n") ||
            value.Contains("\r") ||
            value.Contains("\"")
        )
        {
            return "\"" + value + "\"";
        }

        return value;
    }
}
