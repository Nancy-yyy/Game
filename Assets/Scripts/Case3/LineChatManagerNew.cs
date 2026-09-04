using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LineChatManagerNew : MonoBehaviour
{
    [Header("依序放入 1, 2, 3 藍色對話圖片物件")]
    public List<GameObject> blueMessages;

    [Header("放入 4 白色主角對話圖片物件")]
    public GameObject whiteMessage4;

    [Header("提示文字物件 (點擊繼續)")]
    public GameObject hintText;

    [Header("決策思考彈窗 (DecisionDialog)")]
    public GameObject decisionDialog;

    [Header("群組緩和提示彈窗 (CalmDownDialog)")]
    public GameObject calmDownDialog;

    [Header("聊天室完整畫面 (PhoneBackground)")]
    public GameObject chatRoomPanel;

    [Header("圖書館畫面 (LibraryPanel)")]
    public GameObject libraryPanel;

    [Header("圖書館專用背景圖 (LibraryBackground)")]
    public GameObject libraryBackground;

    [Header("圖書館提示文字 (LibraryTapHint)")]
    public GameObject libraryTapHint;

    [Header("圖書館已滿的圖片 (LibraryFullPhone Sprite)")]
    public Sprite libraryFullSprite;

    [Header("回主頁按鈕 (Btn_Home)")]
    public GameObject btnHome;

    [Header("提示延遲時間 (秒)")]
    public float hintDelaySeconds = 3f;

    [Header("木牌滑入設定")]
    public float slideDuration = 0.5f;
    public float startPosY = 800f;
    public float targetPosY = 0f;

    [Header("切換轉場動畫設定")]
    public float transitionDuration = 0.4f;

    private int currentIndex = 0;
    private bool isWaitingForDecision = false;
    private bool isWhiteMessageShowing = false;
    private bool isSignShowing = false;
    private bool isTransitioning = false;

    private RectTransform calmDownRect;
    private RectTransform chatRect;
    private RectTransform libraryRect;
    private CanvasGroup chatGroup;
    private CanvasGroup libraryGroup;
    private CanvasGroup libraryBgGroup;
    private Image libraryImage;

    void Start()
    {
        foreach (GameObject img in blueMessages)
        {
            if (img != null) img.SetActive(false);
        }

        if (whiteMessage4 != null) whiteMessage4.SetActive(false);
        if (decisionDialog != null) decisionDialog.SetActive(false);
        
        if (calmDownDialog != null)
        {
            calmDownRect = calmDownDialog.GetComponent<RectTransform>();
            calmDownDialog.SetActive(false);
        }

        if (chatRoomPanel != null)
        {
            chatRect = chatRoomPanel.GetComponent<RectTransform>();
            chatGroup = chatRoomPanel.GetComponent<CanvasGroup>();
            if (chatGroup == null) chatGroup = chatRoomPanel.AddComponent<CanvasGroup>();
        }

        if (libraryPanel != null)
        {
            libraryRect = libraryPanel.GetComponent<RectTransform>();
            libraryImage = libraryPanel.GetComponent<Image>();
            libraryGroup = libraryPanel.GetComponent<CanvasGroup>();
            if (libraryGroup == null) libraryGroup = libraryPanel.AddComponent<CanvasGroup>();
            libraryPanel.SetActive(false);
        }

        if (libraryBackground != null)
        {
            libraryBgGroup = libraryBackground.GetComponent<CanvasGroup>();
            if (libraryBgGroup == null) libraryBgGroup = libraryBackground.AddComponent<CanvasGroup>();
            libraryBackground.SetActive(false);
        }

        if (libraryTapHint != null) libraryTapHint.SetActive(false);
        if (btnHome != null) btnHome.SetActive(false);
        if (hintText != null) hintText.SetActive(true);
    }

    public void OnScreenClicked()
    {
        if (isTransitioning) return;

        if (hintText != null && hintText.activeSelf)
        {
            hintText.SetActive(false);
        }

        if (currentIndex < blueMessages.Count)
        {
            if (blueMessages[currentIndex] != null)
            {
                blueMessages[currentIndex].SetActive(true);
            }
            currentIndex++;

            if (currentIndex >= blueMessages.Count)
            {
                isWaitingForDecision = true;
            }
            return;
        }

        if (isWaitingForDecision)
        {
            if (decisionDialog != null)
            {
                decisionDialog.SetActive(true);
            }
            isWaitingForDecision = false;
            return;
        }

        if (isWhiteMessageShowing)
        {
            if (calmDownDialog != null)
            {
                calmDownDialog.SetActive(true);
                StartCoroutine(SlideInCalmDownDialog());
            }
            isWhiteMessageShowing = false;
            isSignShowing = true;
            return;
        }

        if (isSignShowing)
        {
            StartCoroutine(AppSwitchTransition());
        }
    }

    private IEnumerator SlideInCalmDownDialog()
    {
        if (calmDownRect == null) calmDownRect = calmDownDialog.GetComponent<RectTransform>();

        Vector2 startPos = new Vector2(calmDownRect.anchoredPosition.x, startPosY);
        Vector2 endPos = new Vector2(calmDownRect.anchoredPosition.x, targetPosY);
        calmDownRect.anchoredPosition = startPos;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / slideDuration) * Mathf.PI * 0.5f);
            calmDownRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        calmDownRect.anchoredPosition = endPos;
    }

    private IEnumerator AppSwitchTransition()
    {
        isTransitioning = true;
        isSignShowing = false;

        if (calmDownDialog != null && calmDownRect != null)
        {
            Vector2 curSignPos = calmDownRect.anchoredPosition;
            Vector2 upSignPos = new Vector2(curSignPos.x, startPosY);
            float signElapsed = 0f;
            while (signElapsed < 0.2f)
            {
                signElapsed += Time.deltaTime;
                calmDownRect.anchoredPosition = Vector2.Lerp(curSignPos, upSignPos, signElapsed / 0.2f);
                yield return null;
            }
            calmDownDialog.SetActive(false);
        }

        if (libraryPanel != null && libraryRect != null)
        {
            libraryPanel.SetActive(true);
            libraryRect.anchoredPosition = new Vector2(800f, 0f);
            if (libraryGroup != null) libraryGroup.alpha = 0f;
        }

        if (libraryBackground != null)
        {
            libraryBackground.SetActive(true);
            if (libraryBgGroup != null) libraryBgGroup.alpha = 0f;
        }

        Vector2 chatStartPos = chatRect != null ? chatRect.anchoredPosition : Vector2.zero;
        Vector2 chatEndPos = new Vector2(-800f, chatStartPos.y);

        Vector2 libStartPos = new Vector2(800f, 0f);
        Vector2 libEndPos = Vector2.zero;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            if (chatRect != null) chatRect.anchoredPosition = Vector2.Lerp(chatStartPos, chatEndPos, t);
            if (chatGroup != null) chatGroup.alpha = Mathf.Lerp(1f, 0f, t);

            if (libraryRect != null) libraryRect.anchoredPosition = Vector2.Lerp(libStartPos, libEndPos, t);
            if (libraryGroup != null) libraryGroup.alpha = Mathf.Lerp(0f, 1f, t);
            if (libraryBgGroup != null) libraryBgGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (chatRoomPanel != null) chatRoomPanel.SetActive(false);
        if (libraryRect != null) libraryRect.anchoredPosition = libEndPos;
        if (libraryGroup != null) libraryGroup.alpha = 1f;
        if (libraryBgGroup != null) libraryBgGroup.alpha = 1f;

        isTransitioning = false;

        if (libraryTapHint != null)
        {
            StartCoroutine(ShowHintAfterDelay(hintDelaySeconds));
        }
    }

    private IEnumerator ShowHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (libraryTapHint != null)
        {
            libraryTapHint.SetActive(true);
        }
    }

    public void OnClickSpaceBooking()
    {
        if (libraryImage != null && libraryFullSprite != null)
        {
            libraryImage.sprite = libraryFullSprite;
        }

        if (libraryTapHint != null) libraryTapHint.SetActive(false);
        if (btnHome != null) btnHome.SetActive(true);
    }

    public void OnClickGoHome()
    {
        SceneManager.LoadScene("Case3_SearchScene");
    }

    public void OnClickReplyButton()
    {
        if (decisionDialog != null) decisionDialog.SetActive(false);
        if (whiteMessage4 != null)
        {
            whiteMessage4.SetActive(true);
            isWhiteMessageShowing = true;
        }
    }

    public void OnClickCalmDownDialog()
    {
        if (!isTransitioning && isSignShowing)
        {
            StartCoroutine(AppSwitchTransition());
        }
    }
}