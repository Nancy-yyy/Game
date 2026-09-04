using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExperimentSetupManager : MonoBehaviour
{
    [Header("AI Tutor")]
    [SerializeField] private Toggle aiToggle;

    [Header("Display")]
    [SerializeField] private TMP_Text currentSettingText;
    [SerializeField] private TMP_Text groupCountText;

    private FeedbackMode selectedFeedbackMode =
        FeedbackMode.Simple;

    private void Start()
    {
        selectedFeedbackMode = FeedbackMode.Simple;
        aiToggle.isOn = false;

        UpdateCurrentSettingText();
        UpdateGroupCounts();
    }

    public void SelectSimpleMode()
    {
        selectedFeedbackMode =
            FeedbackMode.Simple;

        UpdateCurrentSettingText();
    }

    public void SelectDeepMode()
    {
        selectedFeedbackMode =
            FeedbackMode.Deep;

        UpdateCurrentSettingText();
    }

    public void OnAIToggleChanged()
    {
        UpdateCurrentSettingText();
    }

    private void UpdateCurrentSettingText()
    {
        string feedbackText =
            selectedFeedbackMode == FeedbackMode.Simple
            ? "簡易系統教學引導"
            : "深層系統教學引導";

        string aiText =
            aiToggle.isOn ? "ON" : "OFF";

        currentSettingText.text =
            "【目前設定】" +
            feedbackText +
            "｜AI 助教 " +
            aiText;
    }

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

    public void ConfirmSetting()
    {
        if (selectedFeedbackMode == FeedbackMode.Simple &&
            !aiToggle.isOn)
        {
            GameData.CurrentCondition =
                ExperimentCondition.LowInfo_NoAI;
        }
        else if (selectedFeedbackMode == FeedbackMode.Deep &&
                 !aiToggle.isOn)
        {
            GameData.CurrentCondition =
                ExperimentCondition.HighInfo_NoAI;
        }
        else if (selectedFeedbackMode == FeedbackMode.Simple &&
                 aiToggle.isOn)
        {
            GameData.CurrentCondition =
                ExperimentCondition.LowInfo_AI;
        }
        else
        {
            GameData.CurrentCondition =
                ExperimentCondition.HighInfo_AI;
        }

        SceneManager.LoadScene("StartScene");
    }
}