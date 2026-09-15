using UnityEngine;
using TMPro;

/// <summary>
/// 掛在任何 TMP_Text 上，自動抓取 GameData.PlayerName 顯示
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class UpdatePlayerNameTMP : MonoBehaviour
{
    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    // 當物件每次被啟用（顯示）時自動更新名字
    private void OnEnable()
    {
        RefreshName();
    }

    private void Start()
    {
        RefreshName();
    }

    public void RefreshName()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        // 直接讀取 GameData 中的 PlayerName
        if (!string.IsNullOrEmpty(GameData.PlayerName))
        {
            textComponent.text = GameData.PlayerName;
        }
    }
}