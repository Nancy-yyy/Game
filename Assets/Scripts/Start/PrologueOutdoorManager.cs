using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PrologueOutdoorManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private GameObject birdNamingPanel;

    [SerializeField]
    private TMP_InputField birdNameInput;

    [SerializeField]
    private GameObject birdWarningPanel;

    [SerializeField]
    private GameObject birdReplyUI;

    [SerializeField]
    private TMP_Text birdReplyText;

    [SerializeField] private GameObject villageBackground;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject homeTrigger;
    [SerializeField] private GameObject moveHintUI;

    private int birdReplyIndex = 0;

    private void Start()
    {
        // 命名介面
        birdNamingPanel.SetActive(false);
        birdWarningPanel.SetActive(false);
        
        // 鳥鳥回話
        birdReplyUI.SetActive(false);

        // 回家探索場景
        villageBackground.SetActive(false);
        player.SetActive(false);
        homeTrigger.SetActive(false);
        moveHintUI.SetActive(false);

        // 影片播放完成事件
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // 停在影片最後一格
        vp.Pause();                                        //影片播放完後讓它停住，不把畫面關掉

        birdNamingPanel.SetActive(true);
    }

    public void ConfirmBirdName()
    {
        string birdName = birdNameInput.text.Trim();

        if (string.IsNullOrEmpty(birdName))
        {
            birdWarningPanel.SetActive(true);
            return;
        }

        birdNamingPanel.SetActive(false);

        birdReplyUI.SetActive(true);
        birdReplyIndex = 0;
    }

    public void NextBirdReply()
    {
        birdReplyIndex++;

        if (birdReplyIndex == 1)
        {
            birdReplyText.text =
                "快帶我回家吧～";
        }
        else if (birdReplyIndex == 2)
        {
            StartWalkHome();
        }
    }

    public void StartWalkHome()
    {
        birdReplyUI.SetActive(false);
        
        // 關掉影片畫面
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);

        villageBackground.SetActive(true);
        player.SetActive(true);
        homeTrigger.SetActive(true);

        moveHintUI.SetActive(true);
    }

    public void GoHome()
    {
        SceneManager.LoadScene("PrologueHome");
    }

    public void CloseBirdWarning()
    {
        birdWarningPanel.SetActive(false);

        birdNameInput.Select();
        birdNameInput.ActivateInputField();
    }
}