using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoToEndController : MonoBehaviour
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
            // 監聽影片播完的事件
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 啟動平滑轉場協程，避免畫面太硬切過去
        StartCoroutine(SmoothTransitionToCase3());
    }

    private IEnumerator SmoothTransitionToCase3()
    {
        // 1. 如果有 GlobalFader（黑幕淡出），先執行淡出
        if (GlobalFader.Instance != null)
        {
            bool faded = false;
            GlobalFader.Instance.FadeTransition(() => {
                faded = true;
            });
            // 等待淡出特效完成
            while (!faded) yield return null;
        }
        else
        {
            // 緩衝一小段時間讓影片自然收尾
            yield return new WaitForSeconds(0.5f);
        }

        // 2. 切換到你的群組聊天場景
        SceneManager.LoadScene("Case3_Scene");
    }
}