using TMPro;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField playerNameInput;

    [SerializeField]
    private GameObject warningPanel;

    public void StartGame()
    {
        string playerName =
            playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            warningPanel.SetActive(true);
            return;
        }

        Debug.Log(
            "玩家姓名：" + playerName
        );
    }
}
