using UnityEngine;
using TMPro;

public class SceneStartPrompt : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    [Header("UI 提示框物件")]
    public GameObject promptUI;       // 把你的提示框面板拖進來

    [Header("要顯示的提示文字")]
    public TextMeshProUGUI promptText;// 把裡面的文字元件拖進來
    [TextArea]
    public string message = "找到你的位置坐下吧";

    void Start()
    {
        // 1. 設定玩家名稱文字
        if (playerNameText != null)
        {
            playerNameText.text = GameData.PlayerName;
        }

        // 2. 設定要顯示的提示文字
        if (promptText != null)
        {
            promptText.text = message;
        }

        // 3. 自動開啟提示框
        if (promptUI != null)
        {
            promptUI.SetActive(true);
        }
    }
}