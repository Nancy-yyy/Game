using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class EndingHatchingManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer introVideoPlayer;
    [SerializeField] private VideoPlayer hatchingVideoPlayer;

    [SerializeField] private AudioSource correctAudio;

    [SerializeField] private GameObject abilityChecklistPanel;

    [SerializeField] private GameObject checklistImage1;
    [SerializeField] private GameObject checklistImage2;
    [SerializeField] private GameObject checklistImage3;
    [SerializeField] private GameObject checklistImage4;
    [SerializeField] private GameObject checklistImage5;

    [SerializeField] private GameObject hatchingConditionPanel;

    private void Start()
    {
        abilityChecklistPanel.SetActive(false);

        checklistImage1.SetActive(false);
        checklistImage2.SetActive(false);
        checklistImage3.SetActive(false);
        checklistImage4.SetActive(false);
        checklistImage5.SetActive(false);

        hatchingConditionPanel.SetActive(false);

        hatchingVideoPlayer.gameObject.SetActive(false);

        introVideoPlayer.loopPointReached += OnIntroVideoFinished;

        introVideoPlayer.Play();
    }

    private void OnIntroVideoFinished(VideoPlayer vp)
    {
        vp.Pause();

        abilityChecklistPanel.SetActive(true);

        StartCoroutine(ShowChecklist());
    }

    private void PlayCorrectAudio()
    {
        if (correctAudio != null)
        {
            correctAudio.Play();
        }
    }

    private IEnumerator ShowChecklist()
    {
        checklistImage1.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(0.4f);

        checklistImage2.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(0.4f);

        checklistImage3.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(0.4f);

        checklistImage4.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(0.4f);

        checklistImage5.SetActive(true);
        PlayCorrectAudio();
        yield return new WaitForSeconds(0.4f);

        hatchingConditionPanel.SetActive(true);
    }

    public void StartHatchingVideo()
    {
        abilityChecklistPanel.SetActive(false);
        hatchingConditionPanel.SetActive(false);

        introVideoPlayer.gameObject.SetActive(false);

        hatchingVideoPlayer.gameObject.SetActive(true);
        hatchingVideoPlayer.Play();
    }
}