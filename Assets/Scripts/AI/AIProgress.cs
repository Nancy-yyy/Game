using UnityEngine;

public static class AIProgress
{
    // ======================================
    // AI 防暴雷：目前玩家的劇情進度
    // ======================================

    public static string CurrentStoryStep { get; private set; } = "unknown";

    public static void SetStoryStep(string storyStep)
    {
        CurrentStoryStep = storyStep;
        Debug.Log($"[AIProgress] StoryStep 更新：{CurrentStoryStep}");
    }

    // ======================================
    // AI 防暴雷：目前玩家的劇情進度
    // ======================================

    // ======================================
    // AI 防暴雷：分支劇情已解鎖知識
    // ======================================

    private static readonly System.Collections.Generic.HashSet<string> unlockedKnowledge
        = new System.Collections.Generic.HashSet<string>();

    public static void UnlockKnowledge(string key)
    {
        if (unlockedKnowledge.Add(key))
        {
            Debug.Log($"[AIProgress] Knowledge 解鎖：{key}");
        }
    }

    public static string[] GetUnlockedKnowledge()
    {
        string[] result = new string[unlockedKnowledge.Count];
        unlockedKnowledge.CopyTo(result);
        return result;
    }

    public static void ClearKnowledge()
    {
        unlockedKnowledge.Clear();
        Debug.Log("[AIProgress] Knowledge 已清空");
    }

    // ======================================

}