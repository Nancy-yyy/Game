using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
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
    /// 當前遊戲實際執行的實驗組別。
    /// 為相容既有程式，保留 CurrentCondition 名稱。
    /// </summary>
    public static ExperimentCondition CurrentCondition =
        ExperimentCondition.LowInfo_NoAI;

    /// <summary>
    /// 事前分派給受試者的實驗組別。
    /// 正式實驗時，ExperimentSetupManager 應設定此欄位。
    /// </summary>
    public static ExperimentCondition AssignedCondition =
        ExperimentCondition.LowInfo_NoAI;

    /// <summary>
    /// 實際執行的實驗組別。
    /// 直接對應 CurrentCondition，避免兩份實際組別資料不同步。
    /// </summary>
    public static ExperimentCondition ActualCondition =>
        CurrentCondition;

    /// <summary>
    /// 建議 ExperimentSetupManager 使用此方法一次設定
    /// 「事前分派組別」與「實際執行組別」。
    /// </summary>
    public static void SetExperimentConditions(
        ExperimentCondition assignedCondition,
        ExperimentCondition actualCondition
    )
    {
        AssignedCondition = assignedCondition;
        CurrentCondition = actualCondition;
    }

    /// <summary>
    /// 是否開啟 AI 助教。
    /// 以實際執行組別為準。
    /// </summary>
    public static bool HasAI =>
        ActualCondition == ExperimentCondition.LowInfo_AI ||
        ActualCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 是否為高資訊量／深層回饋組。
    /// 以實際執行組別為準。
    /// </summary>
    public static bool IsHighInfo =>
        ActualCondition == ExperimentCondition.HighInfo_NoAI ||
        ActualCondition == ExperimentCondition.HighInfo_AI;

    /// <summary>
    /// 相容目前已完成的 Simple / Deep 程式。
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
    /// 由報名階段事先配發，並於 Unity 實驗設定畫面輸入。
    /// 格式統一為 P001、P002、P003……。
    /// 前測、遊戲與後測皆使用同一組 SubjectID，以利後續資料合併。
    /// </summary>
    public static string SubjectID = "";

    /// <summary>
    /// 遊戲版本。
    /// 正式收案前請確認所有實驗電腦皆使用相同版本。
    /// 若正式實驗期間有更新 Build，應同步更新此版本號。
    /// </summary>
    public const string GameVersion = "1.0";

    /// <summary>
    /// 實驗開始時間。
    /// </summary>
    public static string ExperimentStartTime = "";

    /// <summary>
    /// 實驗完成時間。
    /// </summary>
    public static string ExperimentEndTime = "";


    // =========================================================
    // 區塊 2：研究 Task ID / Concept Tag
    // =========================================================

    public static class CaseIds
    {
        public const string C1 = "C1";
        public const string C2 = "C2";
        public const string C3 = "C3";
        public const string END = "END";
    }

    /// <summary>
    /// 蛋內正式研究任務 ID。
    /// 後續各關卡程式建議統一使用這些常數，不要自行打字。
    /// </summary>
    public static class TaskIds
    {
        // Case 1
        public const string C1_IDLE = "C1_IDLE";
        public const string C1_MATCH = "C1_MATCH";
        public const string C1_SCARCITY = "C1_SCARCITY";

        // Case 2
        public const string C2_RIGHT_1 = "C2_RIGHT_1";
        public const string C2_RIGHT_2 = "C2_RIGHT_2";
        public const string C2_RIGHT_3 = "C2_RIGHT_3";
        public const string C2_RIGHT_4 = "C2_RIGHT_4";
        public const string C2_IDLE_VALUE = "C2_IDLE_VALUE";
        public const string C2_REUSE_ORDER = "C2_REUSE_ORDER";
        public const string C2_REUSE_CASE = "C2_REUSE_CASE";
        public const string C2_DETECTOR = "C2_DETECTOR";
        public const string C2_REFLECTION = "C2_REFLECTION";
        public const string C2_SCALE = "C2_SCALE";
        public const string C2_SCALE_REASON = "C2_SCALE_REASON";

        // Case 3
        public const string C3_FILTER = "C3_FILTER";
        public const string C3_SCHEDULE = "C3_SCHEDULE";
        public const string C3_TRUST = "C3_TRUST";
        public const string C3_RULES = "C3_RULES";

        // 三方關係題組：保留原 C3_THREEPARTY 作為題組名稱，
        // 正式紀錄拆成 Q1~Q10，避免十題共用同一組 Attempt / TimeToCorrect。
        public const string C3_THREEPARTY = "C3_THREEPARTY";
        public const string C3_THREEPARTY_Q1 = "C3_THREEPARTY_Q1";
        public const string C3_THREEPARTY_Q2 = "C3_THREEPARTY_Q2";
        public const string C3_THREEPARTY_Q3 = "C3_THREEPARTY_Q3";
        public const string C3_THREEPARTY_Q4 = "C3_THREEPARTY_Q4";
        public const string C3_THREEPARTY_Q5 = "C3_THREEPARTY_Q5";
        public const string C3_THREEPARTY_Q6 = "C3_THREEPARTY_Q6";
        public const string C3_THREEPARTY_Q7 = "C3_THREEPARTY_Q7";
        public const string C3_THREEPARTY_Q8 = "C3_THREEPARTY_Q8";
        public const string C3_THREEPARTY_Q9 = "C3_THREEPARTY_Q9";
        public const string C3_THREEPARTY_Q10 = "C3_THREEPARTY_Q10";

        // Ending：遊戲內遷移，不等同正式 Posttest
        // END_TRANSFER_TEXT 為有關鍵詞／概念判定規則的文字建構反應題，
        // 不屬於開放式 Reflection。
        public const string END_IDLE = "END_IDLE";
        public const string END_RIGHT = "END_RIGHT";
        public const string END_PLATFORM = "END_PLATFORM";
        public const string END_SUSTAIN = "END_SUSTAIN";
        public const string END_TRANSFER_TEXT = "END_TRANSFER_TEXT";
    }

    public static class ConceptTags
    {
        public const string IdleAsset = "IdleAsset";
        public const string PlatformMatching = "PlatformMatching";
        public const string SupplyDemand = "SupplyDemand";
        public const string UsageRightOwnership = "UsageRightOwnership";
        public const string ResourceReuse = "ResourceReuse";
        public const string SharingBoundary = "SharingBoundary";
        public const string GameplayDecision = "GameplayDecision";
        public const string ResourceConstraint = "ResourceConstraint";
        public const string PlatformTrust = "PlatformTrust";
        public const string UserResponsibility = "UserResponsibility";
        public const string MultiStakeholder = "MultiStakeholder";
        public const string Reflection = "Reflection";
        public const string TransferIdleAsset = "Transfer_IdleAsset";
        public const string TransferUsageRight = "Transfer_UsageRight";
        public const string TransferPlatform = "Transfer_Platform";
        public const string TransferSustainability = "Transfer_Sustainability";
        public const string TransferConceptResponse = "Transfer_ConceptResponse";
    }


    // =========================================================
    // 區塊 3：舊版欄位（第一階段保留相容）
    // Legacy Fields
    // =========================================================

    // -------------------------
    // Case 1
    // -------------------------

    public static int Case1_IdleCapacityErrors = 0;
    public static int Case1_MatchingOrderErrors = 0;
    public static int Case1_ScarcityErrors = 0;
    public static float Case1_PlayTime = 0f;


    // -------------------------
    // Case 2
    // -------------------------

    public static int Case2_Tutorial1_Errors = 0;
    public static int Case2_Tutorial3_Errors = 0;
    public static int Case2_Tutorial4_ClueErrors = 0;
    public static int Case2_Tutorial4_CaseErrors = 0;
    public static string Case2_Tutorial4_Reflection = "";
    public static string Case2_Scale_Choice = "";
    public static int Case2_Scale_QuizErrors = 0;
    public static float Case2_PlayTime = 0f;


    // -------------------------
    // Case 3
    // -------------------------

    /// <summary>
    /// Case 3 舊版總 Quiz 錯誤次數。
    /// 第一階段保留，供既有程式相容使用。
    /// </summary>
    public static int Case3_QuizErrors = 0;

    /// <summary>
    /// Case 3 條件篩選互動錯誤次數。
    /// PeopleSelectionManager 目前仍會使用此欄位。
    /// 第一階段保留相容。
    /// </summary>
    public static int Case3_FilterErrors = 0;

    /// <summary>
    /// Case 3 小遊戲錯誤次數。
    /// TrustMiniGameManager、RuleMiniGameManager
    /// 目前仍會使用此欄位。
    /// 第一階段保留相容。
    /// </summary>
    public static int Case3_MiniGameErrors = 0;

    /// <summary>
    /// Case 3 玩家選擇紀錄。
    /// 第一階段保留相容。
    /// </summary>
    public static string Case3_Decision = "";

    /// <summary>
    /// Case 3 總耗時（秒）。
    /// </summary>
    public static float Case3_PlayTime = 0f;


    // -------------------------
    // Ending
    // -------------------------

    public static int Ending_ConceptTransferErrors = 0;
    public static int Ending_IdleAssetErrors = 0;
    public static int Ending_UsageRightErrors = 0;
    public static int Ending_MatchingMethodErrors = 0;
    public static int Ending_SustainabilityErrors = 0;

    public static float Ending_PlayTime = 0f;


    // =========================================================
    // 區塊 4：整體遊戲時間
    // =========================================================

    public static float TotalGamePlayTime = 0f;

    private static float case1StartTime = 0f;
    private static float case2StartTime = 0f;
    private static float case3StartTime = 0f;
    private static float endingStartTime = 0f;
    private static float totalGameStartTime = 0f;


    // =========================================================
    // 區塊 5：當前 Task 狀態與 AI 使用統計
    // 只保留目前受試者、目前任務的狀態，不累積所有受試者在 RAM
    // =========================================================

    private class TaskState
    {
        public string CaseID = "";
        public string TaskID = "";
        public string ConceptTag = "";

        public string TaskStartTime = "";
        public float StartRealtime = 0f;
        public float LastAttemptRealtime = 0f;

        public int AttemptCount = 0;
        public string FirstAnswer = "";
        public bool FirstAttemptCorrect = false;
        public float FirstResponseTime = 0f;

        public string FinalAnswer = "";
        public bool FinalCorrect = false;

        public bool CorrectAchieved = false;
        public float TimeToCorrect = 0f;

        public int AIUseCount = 0;
        public int FeedbackShownCount = 0;
        public string LastFeedbackLevelShown = "";

        public bool Completed = false;
    }

    private static TaskState currentTask = null;

    // Ending 的三欄建構反應（閒置資產／使用權／平台）
    // 是同一次送出、但需要各自保留作答歷程，因此允許少量平行 Task。
    // 其他 Case 仍維持原本單一 currentTask 架構。
    private static readonly Dictionary<string, TaskState> parallelTasks =
        new Dictionary<string, TaskState>();

    /// <summary>
    /// AI 組每位受試者整場遊戲至少需完成的有效 AI 問答次數。
    /// 「有效使用一次」定義為：玩家送出問題，且 AI 成功回傳非空白回答。
    /// </summary>
    public const int MinimumAIUseCount = 3;

    /// <summary>
    /// 目前受試者整場遊戲已完成的有效 AI 問答總次數。
    /// NoAI 組正常情況應維持 0。
    /// </summary>
    public static int TotalAIUseCount = 0;

    /// <summary>
    /// AI 組是否已達到最低 3 次有效 AI 問答要求。
    /// NoAI 組不適用此要求，因此直接視為已達成。
    /// </summary>
    public static bool HasMetMinimumAIUsage =>
        !HasAI || TotalAIUseCount >= MinimumAIUseCount;

    /// <summary>
    /// AI 組距離最低使用次數尚差幾次。
    /// NoAI 組固定為 0。
    ///
    /// Ending／離開遊戲前的流程控制程式可讀取此值，
    /// 若大於 0，顯示提醒並暫不讓 AI 組完成實驗。
    /// GameData 本身只負責統計，不直接控制 UI 或場景切換。
    /// </summary>
    public static int RemainingAIUsesRequired =>
        HasAI
            ? Math.Max(0, MinimumAIUseCount - TotalAIUseCount)
            : 0;

    /// <summary>
    /// 用於反思紀錄：統計同一 Case 在反思送出前已使用多少次 AI。
    /// </summary>
    private static readonly Dictionary<string, int> caseAIUseCounts =
        new Dictionary<string, int>();


    // =========================================================
    // 區塊 6：實驗初始化
    // =========================================================

    /// <summary>
    /// 受試者正式開始遊戲時呼叫一次。
    ///
    /// 注意：
    /// - 不會改變 CurrentCondition / AssignedCondition。
    /// - 不會改變 SubjectID。
    /// - SubjectID 應在呼叫本方法前，由 ExperimentSetupManager 寫入。
    /// - 組別由 ExperimentSetupManager 事先設定。
    /// - 研究用 CSV 若不存在，會建立 Header。
    /// </summary>
    public static void InitializeExperiment()
    {
        ResetExperimentResults();

        ExperimentStartTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        totalGameStartTime = Time.realtimeSinceStartup;

        EnsureResearchCSVFiles();

        Debug.Log(
            "[實驗初始化]\n" +
            "SubjectID: " + SubjectID + "\n" +
            "AssignedCondition: " + AssignedCondition + "\n" +
            "ActualCondition: " + ActualCondition + "\n" +
            "GameVersion: " + GameVersion + "\n" +
            "AI: " + HasAI + "\n" +
            "HighInfo: " + IsHighInfo + "\n" +
            "StartTime: " + ExperimentStartTime
        );
    }


    /// <summary>
    /// 清除上一位受試者留在 RAM 的研究資料。
    ///
    /// 不會清除：
    /// - CurrentCondition
    /// - AssignedCondition
    /// - SubjectID
    /// - PlayerName
    /// - GameVersion（固定常數）
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

        // Case 3
        Case3_QuizErrors = 0;
        Case3_FilterErrors = 0;
        Case3_MiniGameErrors = 0;
        Case3_Decision = "";
        Case3_PlayTime = 0f;

        // Ending
        Ending_ConceptTransferErrors = 0;
        Ending_IdleAssetErrors = 0;
        Ending_UsageRightErrors = 0;
        Ending_MatchingMethodErrors = 0;
        Ending_SustainabilityErrors = 0;
        Ending_PlayTime = 0f;

        // Total
        TotalGamePlayTime = 0f;
        TotalAIUseCount = 0;
        ExperimentEndTime = "";

        // Current task / case support counts
        currentTask = null;
        parallelTasks.Clear();
        caseAIUseCounts.Clear();
    }


    // =========================================================
    // 區塊 7：Case 計時
    // 使用 realtimeSinceStartup，避免 Time.timeScale 影響研究計時
    // =========================================================

    public static void StartCase1Timer()
    {
        case1StartTime = Time.realtimeSinceStartup;
    }

    public static void StopCase1Timer()
    {
        Case1_PlayTime =
            Time.realtimeSinceStartup - case1StartTime;
    }

    public static void StartCase2Timer()
    {
        case2StartTime = Time.realtimeSinceStartup;
    }

    public static void StopCase2Timer()
    {
        Case2_PlayTime =
            Time.realtimeSinceStartup - case2StartTime;
    }

    public static void StartCase3Timer()
    {
        case3StartTime = Time.realtimeSinceStartup;
    }

    public static void StopCase3Timer()
    {
        Case3_PlayTime =
            Time.realtimeSinceStartup - case3StartTime;
    }

    public static void StartEndingTimer()
    {
        endingStartTime = Time.realtimeSinceStartup;
    }

    public static void StopEndingTimer()
    {
        Ending_PlayTime =
            Time.realtimeSinceStartup - endingStartTime;
    }


    // =========================================================
    // 區塊 8：Gameplay Task API
    // =========================================================

    /// <summary>
    /// 開始一個研究任務。
    /// 會依 TaskID 自動推定 CaseID 與 ConceptTag。
    /// </summary>
    public static void StartTask(string taskID)
    {
        StartTask(
            InferCaseIdFromTaskId(taskID),
            taskID,
            GetDefaultConceptTag(taskID)
        );
    }

    /// <summary>
    /// 開始一個研究任務。
    /// </summary>
    public static void StartTask(
        string caseID,
        string taskID,
        string conceptTag
    )
    {
        if (currentTask != null && !currentTask.Completed)
        {
            Debug.LogWarning(
                "[GameData] 前一個 Task 尚未 CompleteTask()，" +
                "系統將先結束前一個 Task：" + currentTask.TaskID
            );

            CompleteTask();
        }

        currentTask = new TaskState();
        currentTask.CaseID = caseID ?? "";
        currentTask.TaskID = taskID ?? "";
        currentTask.ConceptTag = conceptTag ?? "";
        currentTask.TaskStartTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        currentTask.StartRealtime = Time.realtimeSinceStartup;
        currentTask.LastAttemptRealtime = currentTask.StartRealtime;

        AppendGameplayEvent(
            "TASK_START",
            "",
            "",
            0f,
            0f
        );
    }

    /// <summary>
    /// 記錄一次正式作答。
    /// 每一次作答都會立即 append 到 Gameplay_Events.csv。
    ///
    /// GameData 只負責紀錄 Attempt，不負責限制 AI 何時可以使用。
    /// 因此若玩家在某次作答前曾使用 AI，資料會由 AIUseCount 留下紀錄。
    /// </summary>
    public static bool RecordAnswer(
        string answer,
        bool isCorrect
    )
    {
        if (!EnsureCurrentTask("RecordAnswer"))
        {
            return false;
        }

        float now = Time.realtimeSinceStartup;

        currentTask.AttemptCount++;

        float responseTime =
            now - currentTask.LastAttemptRealtime;

        float taskElapsedTime =
            now - currentTask.StartRealtime;

        if (currentTask.AttemptCount == 1)
        {
            currentTask.FirstAnswer = answer ?? "";
            currentTask.FirstAttemptCorrect = isCorrect;
            currentTask.FirstResponseTime = taskElapsedTime;
        }

        currentTask.FinalAnswer = answer ?? "";
        currentTask.FinalCorrect = isCorrect;

        if (isCorrect && !currentTask.CorrectAchieved)
        {
            currentTask.CorrectAchieved = true;
            currentTask.TimeToCorrect = taskElapsedTime;
        }

        currentTask.LastAttemptRealtime = now;

        AppendGameplayEvent(
            "ANSWER",
            answer ?? "",
            BoolToCsv(isCorrect),
            taskElapsedTime,
            responseTime
        );

        return true;
    }

    /// <summary>
    /// 記錄系統回饋實際顯示一次。
    /// 回饋層次由實際組別自動判定為 Low / High。
    /// feedbackID 可留空。
    ///
    /// Feedback 為本研究的正式實驗操弄之一，因此保留實際顯示紀錄。
    /// </summary>
    public static bool RecordFeedbackShown(string feedbackID = "")
    {
        if (!EnsureCurrentTask("RecordFeedbackShown"))
        {
            return false;
        }

        currentTask.FeedbackShownCount++;
        currentTask.LastFeedbackLevelShown =
            IsHighInfo ? "High" : "Low";

        float taskElapsedTime =
            Time.realtimeSinceStartup - currentTask.StartRealtime;

        AppendGameplayEvent(
            "FEEDBACK",
            feedbackID ?? "",
            "",
            taskElapsedTime,
            0f
        );

        return true;
    }

    /// <summary>
    /// 每次 AI 成功回答完成後呼叫。
    ///
    /// 記錄：
    /// - 玩家問了什麼
    /// - AI 回了什麼
    /// - AI 回應時間
    /// - 此次是整場遊戲第幾次有效 AI 使用
    ///
    /// 有效 AI 使用一次的定義：
    /// 玩家問題非空白，且 AI 成功回傳非空白回答。
    /// 若 AI 呼叫失敗或沒有取得回答，不計入最低 3 次要求。
    ///
    /// GameData 不限制 AI 必須在第幾次作答後才可使用；
    /// AI 的開放時機若有其他介面規則，由 AI / UI 程式自行控制。
    ///
    /// 不記錄 RetrievedSourceID / KnowledgeBaseVersion / AIPromptVersion。
    /// </summary>
    public static bool RecordAIInteraction(
        string playerQuestion,
        string aiResponse,
        float aiResponseTime = 0f
    )
    {
        if (!EnsureCurrentTask("RecordAIInteraction"))
        {
            return false;
        }

        if (!HasAI)
        {
            Debug.LogWarning(
                "[GameData] 非 AI 組嘗試記錄 AI 使用，已忽略。"
            );
            return false;
        }

        if (string.IsNullOrWhiteSpace(playerQuestion))
        {
            Debug.LogWarning(
                "[GameData] AI 問題為空白，本次不計入有效 AI 使用。"
            );
            return false;
        }

        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            Debug.LogWarning(
                "[GameData] AI 回答為空白或呼叫失敗，本次不計入有效 AI 使用。"
            );
            return false;
        }

        currentTask.AIUseCount++;
        TotalAIUseCount++;

        IncrementDictionaryCount(
            caseAIUseCounts,
            currentTask.CaseID
        );

        AppendAILog(
            currentTask.CaseID,
            currentTask.TaskID,
            TotalAIUseCount,
            playerQuestion ?? "",
            aiResponse ?? "",
            aiResponseTime
        );

        float taskElapsedTime =
            Time.realtimeSinceStartup - currentTask.StartRealtime;

        AppendGameplayEvent(
            "AI_USED",
            "AIUseNo=" + TotalAIUseCount,
            "",
            taskElapsedTime,
            0f
        );

        return true;
    }

    /// <summary>
    /// 結束目前 Task，並在 Gameplay_Events.csv 寫入 TASK_COMPLETE 摘要列。
    ///
    /// FirstAnswer / FirstAttemptCorrect / TotalAttempts /
    /// FinalAnswer / FinalCorrect / FirstResponseTime / TimeToCorrect /
    /// AIUseCount / FeedbackShownCount
    /// 都會在此列留下最終狀態。
    /// </summary>
    public static bool CompleteTask()
    {
        if (currentTask == null)
        {
            return false;
        }

        if (currentTask.Completed)
        {
            return true;
        }

        float taskElapsedTime =
            Time.realtimeSinceStartup - currentTask.StartRealtime;

        currentTask.Completed = true;

        AppendGameplayEvent(
            "TASK_COMPLETE",
            "",
            "",
            taskElapsedTime,
            0f
        );

        currentTask = null;
        return true;
    }


    // ---------------------------------------------------------
    // Ending 專用：平行子 Task API
    // 用於同一次三欄文字提交，同時追蹤：
    // END_IDLE / END_RIGHT / END_PLATFORM
    // ---------------------------------------------------------

    public static bool StartParallelTask(string taskID)
    {
        if (string.IsNullOrEmpty(taskID))
        {
            return false;
        }

        if (parallelTasks.ContainsKey(taskID))
        {
            return true;
        }

        TaskState task = new TaskState();
        task.CaseID = InferCaseIdFromTaskId(taskID);
        task.TaskID = taskID;
        task.ConceptTag = GetDefaultConceptTag(taskID);
        task.TaskStartTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        task.StartRealtime = Time.realtimeSinceStartup;
        task.LastAttemptRealtime = task.StartRealtime;

        parallelTasks[taskID] = task;

        AppendGameplayEventForTask(
            task,
            "TASK_START",
            "",
            "",
            0f,
            0f
        );

        return true;
    }

    public static bool RecordParallelAnswer(
        string taskID,
        string answer,
        bool isCorrect
    )
    {
        TaskState task;

        if (!parallelTasks.TryGetValue(taskID, out task) ||
            task == null ||
            task.Completed)
        {
            return false;
        }

        float now = Time.realtimeSinceStartup;

        // 若 Ending 主 Task 期間使用過 AI，
        // 子概念題同步保留「作答前已使用 AI」的暴露狀態。
        if (currentTask != null)
        {
            task.AIUseCount = currentTask.AIUseCount;
        }

        task.AttemptCount++;

        float responseTime =
            now - task.LastAttemptRealtime;

        float taskElapsedTime =
            now - task.StartRealtime;

        if (task.AttemptCount == 1)
        {
            task.FirstAnswer = answer ?? "";
            task.FirstAttemptCorrect = isCorrect;
            task.FirstResponseTime = taskElapsedTime;
        }

        task.FinalAnswer = answer ?? "";
        task.FinalCorrect = isCorrect;

        if (isCorrect && !task.CorrectAchieved)
        {
            task.CorrectAchieved = true;
            task.TimeToCorrect = taskElapsedTime;
        }

        task.LastAttemptRealtime = now;

        AppendGameplayEventForTask(
            task,
            "ANSWER",
            answer ?? "",
            BoolToCsv(isCorrect),
            taskElapsedTime,
            responseTime
        );

        return true;
    }

    public static bool CompleteParallelTask(string taskID)
    {
        TaskState task;

        if (!parallelTasks.TryGetValue(taskID, out task) ||
            task == null)
        {
            return false;
        }

        if (task.Completed)
        {
            return true;
        }

        float taskElapsedTime =
            Time.realtimeSinceStartup - task.StartRealtime;

        task.Completed = true;

        AppendGameplayEventForTask(
            task,
            "TASK_COMPLETE",
            "",
            "",
            taskElapsedTime,
            0f
        );

        return true;
    }


    // =========================================================
    // 區塊 9：Reflection API
    // =========================================================

    /// <summary>
    /// 記錄 Case 2 的開放式反思原始文字。
    ///
    /// 目前只有 C2_REFLECTION 屬於「開放式反思」：
    /// 玩家可自由說明想法，沒有以關鍵詞決定通過／不通過。
    ///
    /// Ending 的文字輸入題不屬於 Reflection。
    /// Ending 文字題有指定關鍵詞／概念判定是否通過，
    /// 因此視為「建構反應式的遊戲內遷移題」，
    /// 請使用 TaskIds.END_TRANSFER_TEXT，並透過 RecordAnswer(rawText, isCorrect)
    /// 記錄到 Gameplay_Events.csv。
    ///
    /// 建議流程：
    /// StartTask(TaskIds.C2_REFLECTION)
    /// → 玩家輸入
    /// → RecordReflection(TaskIds.C2_REFLECTION, rawText)
    ///
    /// RecordReflection 送出後會自動 CompleteTask()。
    /// </summary>
    public static bool RecordReflection(
        string reflectionID,
        string rawText
    )
    {
        if (reflectionID != TaskIds.C2_REFLECTION)
        {
            Debug.LogWarning(
                "[GameData] ReflectionID 不在目前正式反思題清單中：" +
                reflectionID +
                "。目前 Reflection_Results.csv 僅記錄 C2_REFLECTION。"
            );
            return false;
        }

        string caseID =
            currentTask != null
                ? currentTask.CaseID
                : InferCaseIdFromTaskId(reflectionID);

        float responseTime = 0f;

        if (currentTask != null)
        {
            responseTime =
                Time.realtimeSinceStartup - currentTask.StartRealtime;
        }

        int caseAIUseCount =
            GetDictionaryCount(caseAIUseCounts, caseID);

        Case2_Tutorial4_Reflection = rawText ?? "";

        AppendReflectionLog(
            caseID,
            reflectionID ?? "",
            rawText ?? "",
            responseTime,
            caseAIUseCount
        );

        if (currentTask != null)
        {
            float taskElapsedTime =
                Time.realtimeSinceStartup - currentTask.StartRealtime;

            AppendGameplayEvent(
                "REFLECTION_SUBMIT",
                reflectionID ?? "",
                "",
                taskElapsedTime,
                0f
            );

            CompleteTask();
        }

        return true;
    }


    // =========================================================
    // 區塊 10：各實驗組目前完成樣本數
    // =========================================================

    /// <summary>
    /// 從 Experiment_Results.csv 中統計四組
    /// 目前各有多少位「已完成並匯出資料」的受試者。
    ///
    /// 多台實驗電腦各自儲存 CSV 時，
    /// 此方法統計的是「目前這台電腦」已完成的四組人數，
    /// 不代表所有實驗電腦合計人數。
    ///
    /// 使用 index 2 的「組別條件（CurrentCondition / ActualCondition）」
    /// 因此保留舊有統計邏輯。
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

        string filePath = GetExperimentResultsCSVFilePath();

        if (!File.Exists(filePath))
        {
            return counts;
        }

        string[] lines =
            File.ReadAllLines(
                filePath,
                Encoding.UTF8
            );

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }

            List<string> columns =
                ParseCSVLine(lines[i]);

            // index 0 = 完成時間
            // index 1 = 受試者ID
            // index 2 = 組別條件（ActualCondition）
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
    // 區塊 11：Experiment_Results.csv 匯出
    // 原檔名沿用；舊欄位第一階段保留
    // =========================================================

    /// <summary>
    /// 整場蛋內實驗完成時呼叫一次。
    /// 每位受試者新增一列 Experiment_Results.csv。
    ///
    /// AI 組正式完成遊戲前，建議外部流程先檢查：
    /// GameData.HasMetMinimumAIUsage
    /// 或 GameData.RemainingAIUsesRequired。
    ///
    /// 為避免因流程程式錯誤而完全遺失受試者資料，
    /// 本方法即使偵測到 AI 組未達最低 3 次仍會匯出，
    /// 但會寫入警告，且 CSV 中會保留「AI最低使用次數達成」供後續檢查。
    /// </summary>
    public static void ExportToCSV()
    {
        // 若遊戲在某 Task 完成後忘記 CompleteTask，
        // 匯出前仍先保留最後 Task 摘要。
        if (currentTask != null && !currentTask.Completed)
        {
            CompleteTask();
        }

        TotalGamePlayTime =
            Time.realtimeSinceStartup - totalGameStartTime;

        ExperimentEndTime =
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (HasAI && !HasMetMinimumAIUsage)
        {
            Debug.LogWarning(
                "[GameData] AI 組受試者尚未達最低 AI 使用次數。" +
                " SubjectID=" + SubjectID +
                ", TotalAIUseCount=" + TotalAIUseCount +
                ", Required=" + MinimumAIUseCount
            );
        }

        string filePath =
            GetExperimentResultsCSVFilePath();

        EnsureExperimentResultsCSV();

        string recordLine = CreateCSVLine(
            ExperimentEndTime,
            SubjectID,
            ActualCondition.ToString(),              // 舊「組別條件」欄位
            AssignedCondition.ToString(),
            ActualCondition.ToString(),
            GameVersion,
            HasAI.ToString(),
            IsHighInfo.ToString(),
            TotalAIUseCount.ToString(),
            HasAI ? BoolToCsv(HasMetMinimumAIUsage) : "",

            // Case 1 legacy summary
            Case1_IdleCapacityErrors.ToString(),
            Case1_MatchingOrderErrors.ToString(),
            Case1_ScarcityErrors.ToString(),
            FormatFloat(Case1_PlayTime),

            // Case 2 legacy summary
            Case2_Tutorial1_Errors.ToString(),
            Case2_Tutorial3_Errors.ToString(),
            Case2_Tutorial4_ClueErrors.ToString(),
            Case2_Tutorial4_CaseErrors.ToString(),
            Case2_Tutorial4_Reflection,
            Case2_Scale_Choice,
            Case2_Scale_QuizErrors.ToString(),
            FormatFloat(Case2_PlayTime),

            // Case 3 legacy summary
            Case3_QuizErrors.ToString(),
            Case3_Decision,
            FormatFloat(Case3_PlayTime),

            // Ending legacy summary
            Ending_ConceptTransferErrors.ToString(),
            Ending_IdleAssetErrors.ToString(),
            Ending_UsageRightErrors.ToString(),
            Ending_MatchingMethodErrors.ToString(),
            Ending_SustainabilityErrors.ToString(),
            FormatFloat(Ending_PlayTime),

            // Total
            FormatFloat(TotalGamePlayTime)
        );

        File.AppendAllText(
            filePath,
            recordLine + Environment.NewLine,
            new UTF8Encoding(false)
        );

        Debug.Log(
            "[實驗資料匯出成功]\n" +
            Path.GetFullPath(filePath)
        );
    }


    // =========================================================
    // 區塊 12：Gameplay_Events.csv
    // =========================================================

    private static void AppendGameplayEvent(
        string eventType,
        string eventValue,
        string isCorrectValue,
        float taskElapsedTime,
        float responseTime
    )
    {
        AppendGameplayEventForTask(
            currentTask,
            eventType,
            eventValue,
            isCorrectValue,
            taskElapsedTime,
            responseTime
        );
    }

    private static void AppendGameplayEventForTask(
        TaskState task,
        string eventType,
        string eventValue,
        string isCorrectValue,
        float taskElapsedTime,
        float responseTime
    )
    {
        if (task == null)
        {
            return;
        }

        EnsureGameplayEventsCSV();

        bool hasAttempt = task.AttemptCount > 0;
        bool aiUsed = task.AIUseCount > 0;
        bool feedbackShown = task.FeedbackShownCount > 0;

        string line = CreateCSVLine(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            SubjectID,
            AssignedCondition.ToString(),
            ActualCondition.ToString(),
            GameVersion,

            task.CaseID,
            task.TaskID,
            task.ConceptTag,

            eventType,
            task.AttemptCount.ToString(),
            eventValue,
            isCorrectValue,

            task.TaskStartTime,
            FormatFloat(taskElapsedTime),
            FormatFloat(responseTime),

            // 支援暴露：在目前事件發生前／當下已累積多少次
            BoolToCsv(aiUsed),
            task.AIUseCount.ToString(),
            BoolToCsv(feedbackShown),
            task.LastFeedbackLevelShown,
            task.FeedbackShownCount.ToString(),

            // Task summary（TASK_COMPLETE 列為最終值；其他列為當下累積值）
            task.FirstAnswer,
            hasAttempt ? BoolToCsv(task.FirstAttemptCorrect) : "",
            task.AttemptCount.ToString(),
            task.FinalAnswer,
            hasAttempt ? BoolToCsv(task.FinalCorrect) : "",
            hasAttempt ? FormatFloat(task.FirstResponseTime) : "",
            task.CorrectAchieved
                ? FormatFloat(task.TimeToCorrect)
                : "",
            BoolToCsv(aiUsed),
            task.AIUseCount.ToString(),
            BoolToCsv(feedbackShown),
            task.FeedbackShownCount.ToString()
        );

        File.AppendAllText(
            GetGameplayEventsCSVFilePath(),
            line + Environment.NewLine,
            new UTF8Encoding(false)
        );
    }


    // =========================================================
    // 區塊 13：Reflection_Results.csv（目前僅 Case 2 開放式反思）
    // =========================================================

    private static void AppendReflectionLog(
        string caseID,
        string reflectionID,
        string rawText,
        float responseTime,
        int caseAIUseCount
    )
    {
        EnsureReflectionCSV();

        string line = CreateCSVLine(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            SubjectID,
            AssignedCondition.ToString(),
            ActualCondition.ToString(),
            GameVersion,
            caseID,
            reflectionID,
            rawText,
            FormatFloat(responseTime),
            BoolToCsv(caseAIUseCount > 0),
            caseAIUseCount.ToString(),
            (rawText ?? "").Length.ToString()
        );

        File.AppendAllText(
            GetReflectionCSVFilePath(),
            line + Environment.NewLine,
            new UTF8Encoding(false)
        );
    }


    // =========================================================
    // 區塊 14：AI_Log.csv
    // =========================================================

    private static void AppendAILog(
        string caseID,
        string taskID,
        int aiUseNo,
        string playerQuestion,
        string aiResponse,
        float aiResponseTime
    )
    {
        EnsureAILogCSV();

        string line = CreateCSVLine(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            SubjectID,
            AssignedCondition.ToString(),
            ActualCondition.ToString(),
            GameVersion,
            caseID,
            taskID,
            aiUseNo.ToString(),
            playerQuestion,
            aiResponse,
            FormatFloat(Mathf.Max(0f, aiResponseTime))
        );

        File.AppendAllText(
            GetAILogCSVFilePath(),
            line + Environment.NewLine,
            new UTF8Encoding(false)
        );
    }


    // =========================================================
    // 區塊 15：CSV 檔案與 Header
    // =========================================================

    private static string GetExperimentResultsCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../Experiment_Results.csv"
        );
    }

    private static string GetGameplayEventsCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../Gameplay_Events.csv"
        );
    }

    private static string GetReflectionCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../Reflection_Results.csv"
        );
    }

    private static string GetAILogCSVFilePath()
    {
        return Path.Combine(
            Application.dataPath,
            "../AI_Log.csv"
        );
    }

    private static void EnsureResearchCSVFiles()
    {
        EnsureExperimentResultsCSV();
        EnsureGameplayEventsCSV();
        EnsureReflectionCSV();
        EnsureAILogCSV();
    }

    private static void EnsureExperimentResultsCSV()
    {
        string header = CreateCSVLine(
            "完成時間",
            "受試者ID",
            "組別條件",
            "AssignedCondition",
            "ActualCondition",
            "GameVersion",
            "是否AI",
            "是否高資訊",
            "總AI使用次數",
            "AI最低使用次數達成",

            "Case1閒置容量錯誤",
            "Case1媒合流程錯誤",
            "Case1資源稀缺錯誤",
            "Case1耗時",

            "Case2_Tut1錯誤",
            "Case2_Tut3錯誤",
            "Case2_Tut4線索錯誤",
            "Case2_Tut4案例錯誤",
            "Case2反思回答",
            "Case2天平選擇",
            "Case2理由錯誤",
            "Case2耗時",

            "Case3錯誤數",
            "Case3決策",
            "Case3耗時",

            "Ending概念遷移錯誤",
            "Ending閒置資產錯誤",
            "Ending使用權錯誤",
            "Ending媒合方式錯誤",
            "Ending永續判斷錯誤",
            "Ending耗時",

            "總遊玩時間"
        );

        EnsureCSVFile(
            GetExperimentResultsCSVFilePath(),
            header
        );
    }

    private static void EnsureGameplayEventsCSV()
    {
        string header = CreateCSVLine(
            "記錄時間",
            "受試者ID",
            "AssignedCondition",
            "ActualCondition",
            "GameVersion",
            "CaseID",
            "TaskID",
            "ConceptTag",
            "EventType",
            "AttemptNo",
            "EventValue",
            "IsCorrect",
            "TaskStartTime",
            "TaskElapsedTime",
            "ResponseTime",
            "AIUsedBeforeCurrentPoint",
            "AIUseCountBeforeCurrentPoint",
            "FeedbackShownBeforeCurrentPoint",
            "FeedbackLevelShown",
            "FeedbackShownCount",
            "FirstAnswer",
            "FirstAttemptCorrect",
            "TotalAttempts",
            "FinalAnswer",
            "FinalCorrect",
            "FirstResponseTime",
            "TimeToCorrect",
            "AIUsed",
            "AIUseCount",
            "FeedbackShown",
            "TotalFeedbackShownCount"
        );

        EnsureCSVFile(
            GetGameplayEventsCSVFilePath(),
            header
        );
    }

    private static void EnsureReflectionCSV()
    {
        // Reflection_Results.csv 目前只存 Case 2 的開放式反思。
        // Ending 關鍵詞文字題改存 Gameplay_Events.csv，不寫入本檔。
        string header = CreateCSVLine(
            "記錄時間",
            "受試者ID",
            "AssignedCondition",
            "ActualCondition",
            "GameVersion",
            "CaseID",
            "ReflectionID",
            "RawText",
            "ResponseTime",
            "AIUsedBeforeReflection",
            "AIUseCountBeforeReflection",
            "CharacterCount"
        );

        EnsureCSVFile(
            GetReflectionCSVFilePath(),
            header
        );
    }

    private static void EnsureAILogCSV()
    {
        string header = CreateCSVLine(
            "記錄時間",
            "受試者ID",
            "AssignedCondition",
            "ActualCondition",
            "GameVersion",
            "CaseID",
            "TaskID",
            "AIUseNo",
            "PlayerQuestion",
            "AIResponse",
            "AIResponseTime"
        );

        EnsureCSVFile(
            GetAILogCSVFilePath(),
            header
        );
    }

    /// <summary>
    /// 若檔案不存在或為空，建立 Header。
    /// 若檔案已存在但 Header 與目前程式版本不同，
    /// 不自動覆寫資料，只發出警告。
    ///
    /// 正式收案前建議刪除或另外備份測試期舊 CSV，
    /// 讓程式依目前最新欄位重新建立新檔。
    /// </summary>
    private static void EnsureCSVFile(
        string filePath,
        string header
    )
    {
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            File.WriteAllText(
                filePath,
                header + Environment.NewLine,
                new UTF8Encoding(true)
            );
            return;
        }

        string firstLine = "";

        using (
            StreamReader reader =
                new StreamReader(filePath, Encoding.UTF8, true)
        )
        {
            firstLine = reader.ReadLine() ?? "";
        }

        // 去除可能存在的 UTF-8 BOM 後再比較 Header。
        firstLine = firstLine.TrimStart('\uFEFF');

        if (!string.Equals(firstLine, header, StringComparison.Ordinal))
        {
            Debug.LogWarning(
                "[GameData] 偵測到既有 CSV Header 與目前程式版本不同，" +
                "程式不會自動覆寫：\n" +
                Path.GetFullPath(filePath) + "\n" +
                "正式實驗前請先備份並清除測試期舊檔，" +
                "避免新舊欄位混在同一份 CSV。"
            );
        }
    }


    // =========================================================
    // 區塊 16：Task / Concept 對照
    // =========================================================

    private static string InferCaseIdFromTaskId(string taskID)
    {
        if (string.IsNullOrEmpty(taskID))
        {
            return "";
        }

        if (taskID.StartsWith("C1_"))
        {
            return CaseIds.C1;
        }

        if (taskID.StartsWith("C2_"))
        {
            return CaseIds.C2;
        }

        if (taskID.StartsWith("C3_"))
        {
            return CaseIds.C3;
        }

        if (taskID.StartsWith("END_"))
        {
            return CaseIds.END;
        }

        return "";
    }

    private static string GetDefaultConceptTag(string taskID)
    {
        switch (taskID)
        {
            case TaskIds.C1_IDLE:
                return ConceptTags.IdleAsset;

            case TaskIds.C1_MATCH:
                return ConceptTags.PlatformMatching;

            case TaskIds.C1_SCARCITY:
                return ConceptTags.SupplyDemand;

            case TaskIds.C2_RIGHT_1:
            case TaskIds.C2_RIGHT_2:
            case TaskIds.C2_RIGHT_3:
            case TaskIds.C2_RIGHT_4:
                return ConceptTags.UsageRightOwnership;

            case TaskIds.C2_IDLE_VALUE:
                return ConceptTags.IdleAsset;

            case TaskIds.C2_REUSE_ORDER:
            case TaskIds.C2_REUSE_CASE:
                return ConceptTags.ResourceReuse;

            case TaskIds.C2_DETECTOR:
                return ConceptTags.SharingBoundary;

            case TaskIds.C2_REFLECTION:
                return ConceptTags.Reflection;

            case TaskIds.C2_SCALE:
            case TaskIds.C2_SCALE_REASON:
                return ConceptTags.GameplayDecision;

            case TaskIds.C3_FILTER:
                return ConceptTags.PlatformMatching;

            case TaskIds.C3_SCHEDULE:
                return ConceptTags.ResourceConstraint;

            case TaskIds.C3_TRUST:
                return ConceptTags.PlatformTrust;

            case TaskIds.C3_RULES:
                return ConceptTags.UserResponsibility;

            case TaskIds.C3_THREEPARTY:
            case TaskIds.C3_THREEPARTY_Q1:
            case TaskIds.C3_THREEPARTY_Q2:
            case TaskIds.C3_THREEPARTY_Q3:
            case TaskIds.C3_THREEPARTY_Q4:
            case TaskIds.C3_THREEPARTY_Q5:
            case TaskIds.C3_THREEPARTY_Q6:
            case TaskIds.C3_THREEPARTY_Q7:
            case TaskIds.C3_THREEPARTY_Q8:
            case TaskIds.C3_THREEPARTY_Q9:
            case TaskIds.C3_THREEPARTY_Q10:
                return ConceptTags.MultiStakeholder;

            case TaskIds.END_IDLE:
                return ConceptTags.TransferIdleAsset;

            case TaskIds.END_RIGHT:
                return ConceptTags.TransferUsageRight;

            case TaskIds.END_PLATFORM:
                return ConceptTags.TransferPlatform;

            case TaskIds.END_SUSTAIN:
                return ConceptTags.TransferSustainability;

            case TaskIds.END_TRANSFER_TEXT:
                return ConceptTags.TransferConceptResponse;

            default:
                return "";
        }
    }


    // =========================================================
    // 區塊 17：工具函式
    // =========================================================

    private static bool EnsureCurrentTask(string caller)
    {
        if (currentTask != null)
        {
            return true;
        }

        Debug.LogWarning(
            "[GameData] " + caller +
            " 呼叫時沒有正在進行的 Task。" +
            "請先呼叫 GameData.StartTask(...)."
        );

        return false;
    }

    private static void IncrementDictionaryCount(
        Dictionary<string, int> dictionary,
        string key
    )
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = 0;
        }

        dictionary[key]++;
    }

    private static int GetDictionaryCount(
        Dictionary<string, int> dictionary,
        string key
    )
    {
        if (
            string.IsNullOrEmpty(key) ||
            !dictionary.ContainsKey(key)
        )
        {
            return 0;
        }

        return dictionary[key];
    }

    private static string BoolToCsv(bool value)
    {
        return value ? "1" : "0";
    }

    private static string FormatFloat(float value)
    {
        return value.ToString(
            "F3",
            CultureInfo.InvariantCulture
        );
    }

    /// <summary>
    /// 建立一整列 CSV，所有欄位都套用安全 escaping。
    /// </summary>
    private static string CreateCSVLine(params string[] fields)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            builder.Append(
                EscapeCSVField(fields[i])
            );
        }

        return builder.ToString();
    }

    /// <summary>
    /// 將逗號、換行、雙引號安全存入 CSV。
    /// AI 回答與反思原文可能包含上述字元，因此所有文字欄位都必須經過此方法。
    /// </summary>
    private static string EscapeCSVField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        value = value.Replace("\"", "\"\"");

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

            if (c == '\"')
            {
                // CSV 中 "" 代表一個真正的 "
                if (
                    insideQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '\"'
                )
                {
                    current.Append('\"');
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
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result;
    }
}
