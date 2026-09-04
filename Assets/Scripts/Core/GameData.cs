using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 實驗組別條件：2 × 2
/// 資訊量（Low / High） × AI（NoAI / AI）
/// </summary>
public enum ExperimentCondition
{
    LowInfo_NoAI,
    HighInfo_NoAI,
    LowInfo_AI,
    HighInfo_AI
}

/// <summary>
/// 為了相容目前既有遊戲程式使用的回饋模式
/// LowInfo = Simple
/// HighInfo = Deep
/// </summary>
public enum FeedbackMode
{
    Simple,
    Deep
}

public static class GameData
{
    // =========================================================
    // 區塊 0：實驗設定
    // Experiment Settings
    // =========================================================

    /// <summary>
    /// 當前受試者所屬實驗組別。
    /// 由 ExperimentSetupManager 人工設定。
    /// GameData 不會自行隨機分組。
    /// </summary>
    public static ExperimentCondition CurrentCondition =
        ExperimentCondition.LowInfo_NoAI;

    /// <summary>
    /// 是否開啟 AI 助教
    /// </summary>
    public static bool HasAI =>
        CurrentCondition == ExperimentCondition.LowInfo_AI ||
        CurrentCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 是否為高資訊量／深層回饋組
    /// </summary>
    public static bool IsHighInfo =>
        CurrentCondition == ExperimentCondition.HighInfo_NoAI ||
        CurrentCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 相容目前已完成的 Simple / Deep 程式。
    ///
    /// LowInfo  → Simple
    /// HighInfo → Deep
    /// </summary>
    public static global::FeedbackMode FeedbackMode =>
        IsHighInfo
            ? global::FeedbackMode.Deep
            : global::FeedbackMode.Simple;

    /// <summary>
    /// 相容目前既有 AI 助教程式。
    /// </summary>
    public static bool AITutorEnabled => HasAI;


    // =========================================================
    // 區塊 1：遊戲基本資料
    // Game Information
    // =========================================================

    /// <summary>
    /// 玩家姓名。
    /// 只供遊戲角色對話顯示使用，
    /// 不會寫入研究 CSV。
    /// </summary>
    public static string PlayerName = "主角";

    /// <summary>
    /// 實驗受試者 ID。
    /// 正式研究建議使用 P001、P002 等匿名代碼。
    /// </summary>
    public static string SubjectID = "";

    /// <summary>
    /// 實驗開始時間
    /// </summary>
    public static string ExperimentStartTime = "";

    /// <summary>
    /// 實驗完成時間
    /// </summary>
    public static string ExperimentEndTime = "";


    // =========================================================
    // 區塊 2：Case 1
    // 共乘案例
    // =========================================================

    /// <summary>
    /// 第一個教學點：
    /// 找出閒置容量。
    ///
    /// 例如點錯：
    /// - 油箱
    /// - 駕駛座
    /// </summary>
    public static int Case1_IdleCapacityErrors = 0;

    /// <summary>
    /// 第二個教學點：
    /// 平台媒合流程排列錯誤次數。
    /// </summary>
    public static int Case1_MatchingOrderErrors = 0;

    /// <summary>
    /// 第三個教學點：
    /// 供需與資源稀缺題答錯次數。
    /// </summary>
    public static int Case1_ScarcityErrors = 0;

    /// <summary>
    /// Case 1 總耗時（秒）
    /// </summary>
    public static float Case1_PlayTime = 0f;


    // =========================================================
    // 區塊 3：Case 2
    // 原文書共享案例
    // =========================================================

    /// <summary>
    /// Tutorial 1：
    /// 所有權 vs 使用權答錯次數
    /// </summary>
    public static int Case2_Tutorial1_Errors = 0;

    /// <summary>
    /// Tutorial 3：
    /// 四格漫畫排序錯誤次數
    /// </summary>
    public static int Case2_Tutorial3_Errors = 0;

    /// <summary>
    /// Tutorial 4：
    /// 線索判斷錯誤次數
    /// </summary>
    public static int Case2_Tutorial4_ClueErrors = 0;

    /// <summary>
    /// Tutorial 4：
    /// 案例選擇錯誤次數
    /// </summary>
    public static int Case2_Tutorial4_CaseErrors = 0;

    /// <summary>
    /// Tutorial 4：
    /// 玩家自主反思輸入
    /// </summary>
    public static string Case2_Tutorial4_Reflection = "";

    /// <summary>
    /// 天平最終方案選擇
    /// 例如：買二手 B / 租借 C
    /// </summary>
    public static string Case2_Scale_Choice = "";

    /// <summary>
    /// 天平理由勾選題錯誤次數
    /// </summary>
    public static int Case2_Scale_QuizErrors = 0;

    /// <summary>
    /// Case 2 總耗時（秒）
    /// </summary>
    public static float Case2_PlayTime = 0f;


    // =========================================================
    // 區塊 4：Case 3
    // =========================================================

    /// <summary>
    /// Case 3 題目總錯誤次數
    /// </summary>
    public static int Case3_QuizErrors = 0;

    /// <summary>
    /// Case 3 主要決策結果
    /// </summary>
    public static string Case3_Decision = "";

    /// <summary>
    /// Case 3 總耗時（秒）
    /// </summary>
    public static float Case3_PlayTime = 0f;


    // =========================================================
    // 區塊 5：Ending
    // 新情境概念遷移＋永續判斷
    // =========================================================

    /// <summary>
    /// 三格概念遷移題：
    /// 每次按下檢查且至少一格錯誤，就 +1。
    ///
    /// 代表整體作答失敗次數。
    /// </summary>
    public static int Ending_ConceptTransferErrors = 0;

    /// <summary>
    /// 三格填空：
    /// 閒置資產概念錯誤次數
    /// </summary>
    public static int Ending_IdleAssetErrors = 0;

    /// <summary>
    /// 三格填空：
    /// 使用權概念錯誤次數
    /// </summary>
    public static int Ending_UsageRightErrors = 0;

    /// <summary>
    /// 三格填空：
    /// 平台媒合方式概念錯誤次數
    /// </summary>
    public static int Ending_MatchingMethodErrors = 0;

    /// <summary>
    /// 最終挑戰：
    /// 「共享一定比較永續嗎？」
    /// 答錯次數
    /// </summary>
    public static int Ending_SustainabilityErrors = 0;

    /// <summary>
    /// Ending 總耗時（秒）
    /// </summary>
    public static float Ending_PlayTime = 0f;


    // =========================================================
    // 區塊 6：整體遊戲時間
    // =========================================================

    /// <summary>
    /// 全程總遊玩時間（秒）
    /// </summary>
    public static float TotalGamePlayTime = 0f;

    private static float case1StartTime = 0f;
    private static float case2StartTime = 0f;
    private static float case3StartTime = 0f;
    private static float endingStartTime = 0f;
    private static float totalGameStartTime = 0f;


    // =========================================================
    // 區塊 7：實驗初始化
    // =========================================================

    /// <summary>
    /// 受試者正式開始遊戲時呼叫一次。
    ///
    /// 注意：
    /// 這裡不會改變 CurrentCondition。
    /// 組別由 ExperimentSetupManager 事先人工設定。
    /// </summary>
    public static void InitializeExperiment()
    {
        ResetExperimentResults();

        ExperimentStartTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        totalGameStartTime = Time.time;

        Debug.Log(
            "[實驗初始化]\n" +
            "SubjectID: " + SubjectID + "\n" +
            "Condition: " + CurrentCondition + "\n" +
            "AI: " + HasAI + "\n" +
            "HighInfo: " + IsHighInfo + "\n" +
            "StartTime: " + ExperimentStartTime
        );
    }


    /// <summary>
    /// 清除上一位受試者留下來的研究資料。
    ///
    /// 不會清除：
    /// - CurrentCondition
    /// - SubjectID
    /// - PlayerName
    /// </summary>
    public static void ResetExperimentResults()
    {
        // -------------------------
        // Case 1
        // -------------------------

        Case1_IdleCapacityErrors = 0;
        Case1_MatchingOrderErrors = 0;
        Case1_ScarcityErrors = 0;
        Case1_PlayTime = 0f;


        // -------------------------
        // Case 2
        // -------------------------

        Case2_Tutorial1_Errors = 0;
        Case2_Tutorial3_Errors = 0;
        Case2_Tutorial4_ClueErrors = 0;
        Case2_Tutorial4_CaseErrors = 0;

        Case2_Tutorial4_Reflection = "";
        Case2_Scale_Choice = "";

        Case2_Scale_QuizErrors = 0;
        Case2_PlayTime = 0f;


        // -------------------------
        // Case 3
        // -------------------------

        Case3_QuizErrors = 0;
        Case3_Decision = "";
        Case3_PlayTime = 0f;


        // -------------------------
        // Ending
        // -------------------------

        Ending_ConceptTransferErrors = 0;

        Ending_IdleAssetErrors = 0;
        Ending_UsageRightErrors = 0;
        Ending_MatchingMethodErrors = 0;

        Ending_SustainabilityErrors = 0;

        Ending_PlayTime = 0f;


        // -------------------------
        // Total
        // -------------------------

        TotalGamePlayTime = 0f;

        ExperimentEndTime = "";
    }


    // =========================================================
    // 區塊 8：Case 計時
    // =========================================================

    public static void StartCase1Timer()
    {
        case1StartTime = Time.time;
    }

    public static void StopCase1Timer()
    {
        Case1_PlayTime =
            Time.time - case1StartTime;
    }


    public static void StartCase2Timer()
    {
        case2StartTime = Time.time;
    }

    public static void StopCase2Timer()
    {
        Case2_PlayTime =
            Time.time - case2StartTime;
    }


    public static void StartCase3Timer()
    {
        case3StartTime = Time.time;
    }

    public static void StopCase3Timer()
    {
        Case3_PlayTime =
            Time.time - case3StartTime;
    }


    public static void StartEndingTimer()
    {
        endingStartTime = Time.time;
    }

    public static void StopEndingTimer()
    {
        Ending_PlayTime =
            Time.time - endingStartTime;
    }


    // =========================================================
    // 區塊 9：各實驗組目前完成樣本數
    // =========================================================

    /// <summary>
    /// 從 Experiment_Results.csv 中統計四組
    /// 目前各有多少位「已完成並匯出資料」的受試者。
    ///
    /// 這個功能只提供 ExperimentSetup 顯示。
    /// 不會自動幫玩家分組。
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
            File.ReadAllLines(
                filePath,
                Encoding.UTF8
            );

        // 第 0 行是 Header
        // 所以從第 1 行開始讀
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            List<string> columns =
                ParseCSVLine(lines[i]);

            // CSV：
            // index 0 = 完成時間
            // index 1 = 受試者ID
            // index 2 = 組別條件

            if (columns.Count < 3)
            {
                continue;
            }

            ExperimentCondition condition;

            if (
                Enum.TryParse(
                    columns[2],
                    out condition
                )
            )
            {
                counts[condition]++;
            }
        }

        return counts;
    }


    // =========================================================
    // 區塊 10：CSV 匯出
    // =========================================================

    /// <summary>
    /// CSV 檔案位置
    /// </summary>
    private static string GetCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../Experiment_Results.csv"
        );
    }


    /// <summary>
    /// 整場實驗完成時呼叫一次。
    ///
    /// 每位受試者會新增一列資料。
    /// </summary>
    public static void ExportToCSV()
    {
        TotalGamePlayTime =
            Time.time - totalGameStartTime;

        ExperimentEndTime =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss"
            );

        string filePath =
            GetCSVFilePath();


        // =====================================================
        // 第一次建立 CSV
        // =====================================================

        if (!File.Exists(filePath))
        {
            string header =

                "完成時間," +
                "受試者ID," +
                "組別條件," +
                "是否AI," +
                "是否高資訊," +

                // Case 1
                "Case1閒置容量錯誤," +
                "Case1媒合流程錯誤," +
                "Case1資源稀缺錯誤," +
                "Case1耗時," +

                // Case 2
                "Case2_Tut1錯誤," +
                "Case2_Tut3錯誤," +
                "Case2_Tut4線索錯誤," +
                "Case2_Tut4案例錯誤," +
                "Case2反思回答," +
                "Case2天平選擇," +
                "Case2理由錯誤," +
                "Case2耗時," +

                // Case 3
                "Case3錯誤數," +
                "Case3決策," +
                "Case3耗時," +

                // Ending
                "Ending概念遷移錯誤," +
                "Ending閒置資產錯誤," +
                "Ending使用權錯誤," +
                "Ending媒合方式錯誤," +
                "Ending永續判斷錯誤," +
                "Ending耗時," +

                // Total
                "總遊玩時間\n";


            File.WriteAllText(
                filePath,
                header,
                new UTF8Encoding(true)
            );
        }


        // =====================================================
        // CSV 安全字串
        // =====================================================

        string safeSubjectID =
            EscapeCSVField(
                SubjectID
            );

        string safeReflection =
            EscapeCSVField(
                Case2_Tutorial4_Reflection
            );

        string safeScaleChoice =
            EscapeCSVField(
                Case2_Scale_Choice
            );

        string safeCase3Decision =
            EscapeCSVField(
                Case3_Decision
            );


        // =====================================================
        // 建立本位受試者資料
        // =====================================================

        string recordLine =

            $"{ExperimentEndTime}," +
            $"{safeSubjectID}," +
            $"{CurrentCondition}," +
            $"{HasAI}," +
            $"{IsHighInfo}," +

            // Case 1
            $"{Case1_IdleCapacityErrors}," +
            $"{Case1_MatchingOrderErrors}," +
            $"{Case1_ScarcityErrors}," +
            $"{Case1_PlayTime:F1}," +

            // Case 2
            $"{Case2_Tutorial1_Errors}," +
            $"{Case2_Tutorial3_Errors}," +
            $"{Case2_Tutorial4_ClueErrors}," +
            $"{Case2_Tutorial4_CaseErrors}," +
            $"{safeReflection}," +
            $"{safeScaleChoice}," +
            $"{Case2_Scale_QuizErrors}," +
            $"{Case2_PlayTime:F1}," +

            // Case 3
            $"{Case3_QuizErrors}," +
            $"{safeCase3Decision}," +
            $"{Case3_PlayTime:F1}," +

            // Ending
            $"{Ending_ConceptTransferErrors}," +
            $"{Ending_IdleAssetErrors}," +
            $"{Ending_UsageRightErrors}," +
            $"{Ending_MatchingMethodErrors}," +
            $"{Ending_SustainabilityErrors}," +
            $"{Ending_PlayTime:F1}," +

            // Total
            $"{TotalGamePlayTime:F1}\n";


        File.AppendAllText(
            filePath,
            recordLine,
            new UTF8Encoding(true)
        );


        Debug.Log(
            "[實驗資料匯出成功]\n" +
            Path.GetFullPath(filePath)
        );
    }


    // =========================================================
    // 區塊 11：CSV 字串安全處理
    // =========================================================

    /// <summary>
    /// 將逗號、換行、雙引號安全存入 CSV。
    /// </summary>
    private static string EscapeCSVField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        value =
            value.Replace(
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


    /// <summary>
    /// 解析 CSV 一整列。
    /// 用於 GetConditionCounts()。
    ///
    /// 支援被雙引號包住、內含逗號的欄位。
    /// </summary>
    private static List<string> ParseCSVLine(string line)
    {
        List<string> result =
            new List<string>();

        StringBuilder current =
            new StringBuilder();

        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // CSV 中 "" 代表一個真正的 "
                if (
                    insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"'
                )
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (
                c == ',' &&
                !insideQuotes
            )
            {
                result.Add(
                    current.ToString()
                );

                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(
            current.ToString()
        );

        return result;
    }
}
