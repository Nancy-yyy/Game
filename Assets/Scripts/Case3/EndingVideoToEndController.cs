using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingVideoToEndController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(SmoothTransitionToEnding());
    }

    private IEnumerator SmoothTransitionToEnding()
    {
        // 確保黑幕淡出效果被確實執行
        if (GlobalFader.Instance != null)
        {
            bool faded = false;
            GlobalFader.Instance.FadeTransition(() => {
                faded = true;
            });
            while (!faded) yield return null;
        }
        else
        {
            // 如果場景剛好沒有 GlobalFader，手動給它一點緩衝時間避免太死硬
            yield return new WaitForSeconds(0.5f);
        }

        // 畫面黑掉後再切換場景
        SceneManager.LoadScene("Ending");
    }
}