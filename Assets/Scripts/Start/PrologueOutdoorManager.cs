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

    [SerializeField]
    private GameObject goHomeButton;

    private int birdReplyIndex = 0;

    private void Start()
    {
        birdNamingPanel.SetActive(false);
        birdWarningPanel.SetActive(false);
        birdReplyUI.SetActive(false);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
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

        GameData.BirdName = birdName;

        birdNamingPanel.SetActive(false);

        birdReplyUI.SetActive(true);
    }

    public void NextBirdReply()
    {
        birdReplyIndex++;

        if (birdReplyIndex == 1)
        {
            birdReplyText.text = "快帶我回家吧!";
        }
        else if (birdReplyIndex == 2)
        {
            goHomeButton.SetActive(true);
        }
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
