using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class TutorialCarouselManager : MonoBehaviour
{
    public static TutorialCarouselManager Instance;

    [Header("【輪播容器與按鈕】")]
    public RectTransform sliderContainer; // 拖入 TutorialSliderContainer
    public Button arrowLeftBtn;           // 拖入 Arrow_LeftBtn
    public Button arrowRightBtn;          // 拖入 Arrow_RightBtn

    [Header("【4 個教學 Panel】")]
    public RectTransform panelTutorial1;  
    public RectTransform panelTutorial2;  
    public RectTransform panelTutorial3;  
    public RectTransform panelTutorial4;  

    [Header("【參數設定】")]
    public float slideDuration = 0.5f;

    [Header("【當前進度狀態】")]
    public int currentPageIndex = 0;
    public int maxUnlockedPage = 0; // 記錄玩家已通關解鎖的最大頁數 (0~3)

    private bool isSliding = false;
    private readonly float[] targetPositions = new float[] { 0f, -1920f, -3840f, -5760f };

    private void Awake()
    {
        Instance = this;
    }

    [Header("【開發除錯設定】")]
    public bool debugStartAtPage4 = false; // 勾選後直接從教學 4 開始測試

    void Start()
    {
        if (sliderContainer == null) sliderContainer = GetComponent<RectTransform>();

        RebindArrowButtons();

        // ⭐ 若是由學長贈書後進入 (TeachStartPhase == 1)，直達教學 2 (Index: 1, Pos: -1920)
        if (Case2State.TeachStartPhase == 1)
        {
            currentPageIndex = 1;
            maxUnlockedPage = 1;
            if (sliderContainer != null)
            {
                sliderContainer.anchoredPosition = new Vector2(-1920f, sliderContainer.anchoredPosition.y);
            }
        }

        SyncCurrentPage();
    }

    void Update()
    {
        if (!isSliding && sliderContainer != null)
        {
            SyncCurrentPage();
        }
    }

    public void RebindArrowButtons()
    {
        if (arrowLeftBtn != null)
        {
            arrowLeftBtn.onClick.RemoveAllListeners();
            arrowLeftBtn.onClick.AddListener(PrevPage);
        }

        if (arrowRightBtn != null)
        {
            arrowRightBtn.onClick.RemoveAllListeners();
            arrowRightBtn.onClick.AddListener(NextPage);
        }

        UpdateArrowState();
    }

    private void SyncCurrentPage()
    {
        float currentX = sliderContainer.anchoredPosition.x;
        currentPageIndex = Mathf.Clamp(Mathf.RoundToInt(-currentX / 1920f), 0, 3);
        UpdateArrowState();
    }

    public void NextPage()
    {
        if (isSliding) return;
        SyncCurrentPage();

        if (currentPageIndex < maxUnlockedPage && currentPageIndex < 3)
        {
            GoToPage(currentPageIndex + 1);
        }
    }

    public void PrevPage()
    {
        if (isSliding) return;
        SyncCurrentPage();

        if (currentPageIndex > 0)
        {
            GoToPage(currentPageIndex - 1);
        }
    }

    public void GoToPage(int targetIndex)
    {
        if (isSliding) return;
        StartCoroutine(SlideToPageRoutine(targetIndex));
    }

    private IEnumerator SlideToPageRoutine(int targetIndex)
    {
        isSliding = true;
        currentPageIndex = targetIndex;
        float targetX = targetPositions[currentPageIndex];

        if (sliderContainer != null)
        {
            Vector2 startPos = sliderContainer.anchoredPosition;
            Vector2 targetPos = new Vector2(targetX, startPos.y);
            float time = 0;

            while (time < slideDuration)
            {
                time += Time.deltaTime;
                sliderContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / slideDuration);
                yield return null;
            }
            sliderContainer.anchoredPosition = targetPos;
        }

        isSliding = false;
        SyncCurrentPage();

        // 抵達教學 3 (-3840) 時自動觸發漫畫第一階段
        if (currentPageIndex == 2 && panelTutorial3 != null)
        {
            Tutorial3_Manager t3 = panelTutorial3.GetComponent<Tutorial3_Manager>();
            if (t3 != null)
            {
                t3.StartTutorial3Phase();
            }
        }
    }

    // ⭐ 解鎖下一頁權限（通關時呼叫）
    public void UnlockNextPage(int nextPageIndex)
    {
        if (nextPageIndex > maxUnlockedPage)
        {
            maxUnlockedPage = nextPageIndex;
        }
        UpdateArrowState();
    }

    public void UpdateArrowState()
    {
        // 左箭頭：只要不是在第一頁 (教學 1) 就可以往回按
        if (arrowLeftBtn != null)
        {
            arrowLeftBtn.gameObject.SetActive(currentPageIndex > 0);
        }

        // 右箭頭：只要當前頁數小於已解鎖的最大關卡，就永遠亮起供玩家自由前進！
        if (arrowRightBtn != null)
        {
            bool canGoRight = currentPageIndex < maxUnlockedPage && currentPageIndex < 3;
            arrowRightBtn.gameObject.SetActive(canGoRight);
        }
    }
}