using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;           //讓 Unity 可以切換 Scene

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
            "玩家姓名：" + playerName         //確認有正常抓到姓名，且不切換 Scene，測完沒問題就刪
        );

        GameData.PlayerName = playerName;    //把玩家輸入的名字存起來

        //切到序章場景
        SceneManager.LoadScene("PrologueOutdoor");    
    }

    public void CloseWarning()
    {
        warningPanel.SetActive(false);       //關閉警示

        playerNameInput.Select();            
        playerNameInput.ActivateInputField();
    }
}
