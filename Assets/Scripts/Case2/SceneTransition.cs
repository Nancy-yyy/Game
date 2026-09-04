using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("轉場面板設定")]
    public RectTransform transitionPanel; // 拖入純色轉場圖
    public float slideDuration = 0.6f;    // 滑動花費時間
    private float screenWidth = 1920f;    // 畫面寬度

    [Header("小鳥飛行動畫設定")]
    public Image birdImage;               // 拖入 TransitionBird 的 Image 元件
    public Sprite birdFlyFrame1;          // 拍翅圖 1 (翅膀抬起)
    public Sprite birdFlyFrame2;          // 拍翅圖 2 (翅膀壓下)
    public float flapInterval = 0.15f;    // 兩張圖切換的時間間隔 (秒)

    private Coroutine birdFlapCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 進入新場景時自動執行開場動畫：轉場面板滑開
        StartCoroutine(SlideOutRoutine());
    }

    public void StartTransitionAndLoadScene(string sceneName)
    {
        StartCoroutine(SlideInAndLoadRoutine(sceneName));
    }

    // 換場滑入：面板從小鳥在前帶頭滑入畫面
    // 換場滑入：面板滑入畫面
    private IEnumerator SlideInAndLoadRoutine(string nextSceneName)
    {
        transitionPanel.anchoredPosition = new Vector2(screenWidth, 0);
        transitionPanel.gameObject.SetActive(true);

        // 確保小鳥一開始就顯示並拍翅
        StartBirdFlapping();

        Vector2 targetPos = Vector2.zero;
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            transitionPanel.anchoredPosition = Vector2.Lerp(new Vector2(screenWidth, 0), targetPos, time / slideDuration);
            yield return null;
        }

        transitionPanel.anchoredPosition = targetPos;

        // ⭐ 這裡不要呼叫 StopBirdFlapping() 或隱藏 birdImage，讓小鳥停在畫面上直到新場景接管
        SceneManager.LoadScene(nextSceneName);
    }

    // 開場滑出：新場景載入後自動往外滑
    private IEnumerator SlideOutRoutine()
    {
        transitionPanel.anchoredPosition = Vector2.zero;
        transitionPanel.gameObject.SetActive(true);

        StartBirdFlapping();

        Vector2 targetPos = new Vector2(-screenWidth, 0);
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            transitionPanel.anchoredPosition = Vector2.Lerp(Vector2.zero, targetPos, time / slideDuration);
            yield return null;
        }

        transitionPanel.anchoredPosition = targetPos;
        transitionPanel.gameObject.SetActive(false);

        // ⭐ 滑出螢幕完全看不到之後，才停止動畫
        StopBirdFlapping();
    }

    // 啟動兩幀拍翅輪播
    private void StartBirdFlapping()
    {
        if (birdImage == null || birdFlyFrame1 == null || birdFlyFrame2 == null) return;

        birdImage.gameObject.SetActive(true);
        if (birdFlapCoroutine != null) StopCoroutine(birdFlapCoroutine);
        birdFlapCoroutine = StartCoroutine(BirdFlapRoutine());
    }

    // 停止拍翅輪播
    private void StopBirdFlapping()
    {
        if (birdFlapCoroutine != null)
        {
            StopCoroutine(birdFlapCoroutine);
            birdFlapCoroutine = null;
        }
        if (birdImage != null)
        {
            birdImage.gameObject.SetActive(false);
        }
    }

    // 每隔一段時間輪流切換 Frame 1 與 Frame 2
    private IEnumerator BirdFlapRoutine()
    {
        bool isFirstFrame = true;
        while (true)
        {
            birdImage.sprite = isFirstFrame ? birdFlyFrame1 : birdFlyFrame2;
            isFirstFrame = !isFirstFrame;
            yield return new WaitForSeconds(flapInterval);
        }
    }
}