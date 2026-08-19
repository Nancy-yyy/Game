using UnityEngine;
using UnityEngine.Video;

public class PrologueOutdoorManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        vp.Pause();                                          //影片播放完後讓它停住，不把畫面關掉

        Debug.Log("序章影片播放完畢，停在最後畫面");
    }
}
