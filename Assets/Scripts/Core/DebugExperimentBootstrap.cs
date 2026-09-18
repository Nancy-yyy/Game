using UnityEngine;

/// <summary>
/// Unity Editor 開發測試用的實驗初始化工具。
///
/// 用途：
/// 當開發者想直接從 Case1、Case2、Case3 或 Ending 的某個 Scene 按 Play 測試時，
/// 不必每次都從 ExperimentSetup → StartScene → 前面劇情一路玩過來。
///
/// 正式實驗流程仍然由 ExperimentSetupManager 初始化 GameData。
/// 本 Script 只建議掛在需要「直接開 Scene 測試」的場景中。
///
/// 注意：
/// 1. 只會在 Unity Editor 中執行，正式 Build 不會自動初始化。
/// 2. 若 GameData 已經由正式流程初始化，預設不會覆蓋 P001 等正式受試者資料。
/// 3. Debug SubjectID 建議使用 TEST001、TEST002 等格式，避免和正式受試者混在一起。
/// </summary>
public class DebugExperimentBootstrap : MonoBehaviour
{
    // =========================================================
    // 區塊 0：Debug 開關
    // =========================================================

    [Header("Debug Bootstrap")]

    /// <summary>
    /// 是否啟用此 Debug 初始化工具。
    /// 只在 Unity Editor 中有效。
    /// </summary>
    [SerializeField] private bool enableDebugBootstrap = true;

    /// <summary>
    /// 若目前 GameData 已經有正式實驗資料，是否跳過 Debug 初始化。
    /// 建議維持 true，避免正常從 ExperimentSetup 進場時被 TEST001 覆蓋。
    /// </summary>
    [SerializeField] private bool skipIfExperimentAlreadyInitialized = true;


    // =========================================================
    // 區塊 1：Debug 受試者設定
    // =========================================================

    [Header("Debug Participant")]

    /// <summary>
    /// 開發測試專用 SubjectID。
    /// 建議使用 TEST001、TEST002 等格式，
    /// 不要使用正式研究的 P001、P002……。
    /// </summary>
    [SerializeField] private string debugSubjectID = "TEST001";

    /// <summary>
    /// 開發測試時要模擬的 2 × 2 實驗組別。
    /// 可直接在 Inspector 切換四組。
    /// </summary>
    [SerializeField] private ExperimentCondition debugCondition =
        ExperimentCondition.HighInfo_AI;


    // =========================================================
    // 區塊 2：初始化
    // =========================================================

    private void Awake()
    {
#if UNITY_EDITOR
        if (!enableDebugBootstrap)
        {
            return;
        }

        // -----------------------------------------------------
        // 若玩家是從正式 ExperimentSetup 流程進來，
        // GameData 已經會有 SubjectID 與 ExperimentStartTime。
        // 此時不要用 Debug 資料覆蓋正式資料。
        // -----------------------------------------------------
        if (
            skipIfExperimentAlreadyInitialized &&
            !string.IsNullOrWhiteSpace(GameData.SubjectID) &&
            !string.IsNullOrWhiteSpace(GameData.ExperimentStartTime)
        )
        {
            Debug.Log(
                "[DebugExperimentBootstrap] GameData 已由正式流程初始化，" +
                "跳過 Debug 初始化。\n" +
                "SubjectID: " + GameData.SubjectID
            );

            return;
        }

        // -----------------------------------------------------
        // Debug SubjectID 防呆
        // -----------------------------------------------------
        string safeDebugSubjectID =
            string.IsNullOrWhiteSpace(debugSubjectID)
            ? "TEST001"
            : debugSubjectID.Trim().ToUpperInvariant();

        // -----------------------------------------------------
        // 寫入 Debug 受試者 ID
        // -----------------------------------------------------
        GameData.SubjectID =
            safeDebugSubjectID;

        // -----------------------------------------------------
        // Debug 模式下 AssignedCondition 與 ActualCondition
        // 使用同一組條件。
        // -----------------------------------------------------
        GameData.SetExperimentConditions(
            debugCondition,
            debugCondition
        );

        // -----------------------------------------------------
        // 初始化 GameData。
        // 會建立研究 CSV Header（若檔案尚不存在），
        // 並清除上一輪測試留在 RAM 的研究資料。
        // -----------------------------------------------------
        GameData.InitializeExperiment();

        Debug.Log(
            "[DebugExperimentBootstrap 初始化完成]\n" +
            "SubjectID: " + GameData.SubjectID + "\n" +
            "AssignedCondition: " + GameData.AssignedCondition + "\n" +
            "ActualCondition: " + GameData.ActualCondition + "\n" +
            "GameVersion: " + GameData.GameVersion
        );
#endif
    }
}