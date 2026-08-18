using TMPro;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField playerNameInput;  //連接姓名輸入框

    [SerializeField]
    private GameObject warningPanel;         //連接警示視窗

    public void StartGame()
    {
        string playerName =
            playerNameInput.text.Trim();     //取得玩家輸入的姓名

        if (string.IsNullOrEmpty(playerName))
        {
            warningPanel.SetActive(true);
            return;
        }

        Debug.Log(
            "玩家姓名：" + playerName         //確認有正常抓到姓名，且不切換 Scene
        );
    }

    public void CloseWarning()
    {
        warningPanel.SetActive(false);       //關閉警示

        playerNameInput.Select();            
        playerNameInput.ActivateInputField();
    }
}
