using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PrologueHomeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject topHintUI;

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private GameObject playerDialogueUI;

    [SerializeField]
    private AudioSource phoneNotificationAudio;

    [SerializeField]
    private GameObject birdDialogueUI;

    [SerializeField]
    private GameObject phoneIconButton;

    [SerializeField]
    private GameObject phonePanel;

    [SerializeField]
    private GameObject appHomePanel;

    [SerializeField]
    private GameObject messagePanel;

    private void Start()
    {
        playerNameText.text = GameData.PlayerName;

        topHintUI.SetActive(false);
        birdDialogueUI.SetActive(false);

        phoneIconButton.SetActive(false);
        phonePanel.SetActive(false);
        appHomePanel.SetActive(false);
        messagePanel.SetActive(false);
    }

    public void OnPlayerDialogueClicked()
    {
        playerDialogueUI.SetActive(false);

        StartCoroutine(PlayPhoneSoundThenShowBird());
    }

    private IEnumerator PlayPhoneSoundThenShowBird()
    {
        phoneNotificationAudio.Play();

        yield return new WaitForSeconds(2f);

        birdDialogueUI.SetActive(true);
    }

    public void OnBirdDialogueClicked()
    {
        birdDialogueUI.SetActive(false);

        topHintUI.SetActive(true);

        phoneIconButton.SetActive(true);
    }

    public void OpenPhonePanel()
    {
        topHintUI.SetActive(false);                       //任務提示消失
        phoneIconButton.SetActive(false);
        phonePanel.SetActive(true);                       //手機畫面出現
    }

    public void OpenAppHome()
    {
        phonePanel.SetActive(false);
        appHomePanel.SetActive(true);
    }

    public void OpenMessagePanel()
    {
        appHomePanel.SetActive(false);
        messagePanel.SetActive(true);
    }

    public void EnterCase1()
    {
        SceneManager.LoadScene("Case1");
    }
}
