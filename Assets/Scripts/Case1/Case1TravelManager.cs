using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Case1TravelManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private GameObject playerDialogueUI;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private GameObject playerNextTriangle;

    [SerializeField] private GameObject birdDialogueUI;

    private void Start()
    {
        playerDialogueUI.SetActive(false);
        birdDialogueUI.SetActive(false);

        playerNameText.text = GameData.PlayerName;

        videoPlayer.loopPointReached += OnTravelVideoFinished;

        videoPlayer.Play();
    }

    private void OnTravelVideoFinished(VideoPlayer vp)
    {
        vp.Pause();

        playerDialogueUI.SetActive(true);
    }

    public void ShowBirdDialogue()
    {
        playerNextTriangle.SetActive(false);
        birdDialogueUI.SetActive(true);
    }
    
    public void EnterCase2()
    {
        SceneManager.LoadScene("Case2_01_ClassroomMove");
    }
}