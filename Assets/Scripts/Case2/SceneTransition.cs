using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 切換場景必備
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("轉場設定")]
    public RectTransform transitionPanel; // 拖入你的純色圖
    public float slideDuration = 0.5f;    // 滑動花費時間
    private float screenWidth = 1920f;    // 畫面的寬度

    void Start()
    {
        // 進入新場景時，自動執行「開場動畫」：圖從正中央往左滑開
        StartCoroutine(SlideOutRoutine());
    }

    // 這個函數是要給最後一張卡片的「確定」按鈕呼叫的
    // sceneName 請填寫你要去的名字，例如 "classroom02"
    public void StartTransitionAndLoadScene(string sceneName)
    {
        StartCoroutine(SlideInAndLoadRoutine(sceneName));
    }

    // 結束動畫：純色圖從右邊滑到中間蓋住畫面
    private IEnumerator SlideInAndLoadRoutine(string nextSceneName)
    {
        // 確保一開始圖在畫面右側外面 (Pos X = 1920)
        transitionPanel.anchoredPosition = new Vector2(screenWidth, 0);
        transitionPanel.gameObject.SetActive(true);

        Vector2 targetPos = Vector2.zero; // 目標：畫面正中央
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            transitionPanel.anchoredPosition = Vector2.Lerp(new Vector2(screenWidth, 0), targetPos, time / slideDuration);
            yield return null;
        }
        
        transitionPanel.anchoredPosition = targetPos; // 確保完全對齊蓋住

        // 畫面完全變純色後，載入下一個場景
        SceneManager.LoadScene(nextSceneName);
    }

    // 開場動畫：純色圖從中間往左邊滑走
    private IEnumerator SlideOutRoutine()
    {
        // 確保一開始圖在畫面正中央蓋住 (Pos X = 0)
        transitionPanel.anchoredPosition = Vector2.zero;
        transitionPanel.gameObject.SetActive(true);

        Vector2 targetPos = new Vector2(-screenWidth, 0); // 目標：畫面左側外面
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            transitionPanel.anchoredPosition = Vector2.Lerp(Vector2.zero, targetPos, time / slideDuration);
            yield return null;
        }

        transitionPanel.anchoredPosition = targetPos; // 確保完全滑走
        transitionPanel.gameObject.SetActive(false); // 滑走後把圖片關掉，節省效能
    }
}
