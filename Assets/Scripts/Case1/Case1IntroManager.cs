using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Case1IntroManager : MonoBehaviour
{
    [SerializeField]
    private GameObject birdDialogueUI;

    [SerializeField]
    private GameObject playerDialogueUI;

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private GameObject systemPlatformPanel;

    [SerializeField]
    private GameObject BirdDialoguePanel;

    [SerializeField]
    private GameObject systemTrustPanel;

    [SerializeField]
    private GameObject birdMechanismDialogueUI;

    private void Start()
    {
        birdDialogueUI.SetActive(true);
        playerDialogueUI.SetActive(false);

        playerNameText.text = GameData.PlayerName;

        systemPlatformPanel.SetActive(false);
        BirdDialoguePanel.SetActive(false);
        systemTrustPanel.SetActive(false);
        birdMechanismDialogueUI.SetActive(false);
    }

    public void ShowPlayerDialogue()
    {
        birdDialogueUI.SetActive(false);
        playerDialogueUI.SetActive(true);
    }

    public void ShowSystemDialogue()
    {
        playerDialogueUI.SetActive(false);
        systemPlatformPanel.SetActive(true);
    }

    public void ShowBirdDialoguePanel()
    {
        systemPlatformPanel.SetActive(false);
        BirdDialoguePanel.SetActive(true);
    }

    public void ShowTrustSystemPanel()
    {
        BirdDialoguePanel.SetActive(false);
        systemTrustPanel.SetActive(true);
    }

    public void ShowBirdMechanismDialogue()
    {
        systemTrustPanel.SetActive(false);
        birdMechanismDialogueUI.SetActive(true);
    }

    public void EnterCase1Car()
    {
        SceneManager.LoadScene("Case1Car");
    }
}