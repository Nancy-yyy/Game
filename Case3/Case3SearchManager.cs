using System.Collections;
using UnityEngine;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Case3SearchManager : MonoBehaviour
{
    [Header("1. 鳥鳥提示對話框")]
    public GameObject birdHintDialog;
    public TextMeshProUGUI dialogText;

    [Header("2. 綠色底圖 (FrameBG)")]
    public CanvasGroup frameBGCanvasGroup;

    [Header("3. 系統提示木看板物件 (SystemSignPanel)")]
    public GameObject systemSignPanel;
    public CanvasGroup systemSignCanvasGroup;
    public TextMeshProUGUI systemSignText;

    [Header("4. 抽屜物件 (ConditionDrawer)")]
    public GameObject conditionDrawer;
    public RectTransform drawerRectTransform;
    public CanvasGroup drawerCanvasGroup;

    [Header("延遲跳出對話時間 (秒)")]
    public float delaySeconds = 0.8f;
    public float fadeSpeed = 4f;

    // 0: 鳥鳥開場 -> 1: Let's GO -> 2: 看板1 -> 3: 看板2 -> 4: 看板3 -> 5: 木板關閉+抽屜升起
    private int flowStep = 0;
    private bool canClick = false;

    private Vector2 drawerTargetPos;

    void Start()
    {
        // 初始狀態設定
        if (frameBGCanvasGroup != null)
        {
            frameBGCanvasGroup.alpha = 0f;
            frameBGCanvasGroup.blocksRaycasts = false;
        }

        if (systemSignPanel != null)
        {
            systemSignPanel.SetActive(false);
        }

        if (drawerCanvasGroup != null)
        {
            drawerCanvasGroup.alpha = 0f;
            drawerCanvasGroup.blocksRaycasts = false;
        }

        if (conditionDrawer != null)
        {
            conditionDrawer.SetActive(false);
        }

        // 記錄抽屜目標位置，並預設放在下方 500px
        if (drawerRectTransform != null)
        {
            drawerTargetPos = drawerRectTransform.anchoredPosition;
            drawerRectTransform.anchoredPosition = new Vector2(drawerTargetPos.x, drawerTargetPos.y - 500f);
        }

        if (birdHintDialog != null)
        {
            birdHintDialog.SetActive(false);
            StartCoroutine(ShowBirdHintRoutine());
        }
    }

    void Update()
    {
        if (!canClick) return;

        bool wasPressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            wasPressed = true;
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            wasPressed = true;
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            wasPressed = true;
        }
#endif

        if (wasPressed)
        {
            HandleFlowNextStep();
        }
    }

    private IEnumerator ShowBirdHintRoutine()
    {
        yield return new WaitForSeconds(delaySeconds);
        if (birdHintDialog != null)
        {
            birdHintDialog.SetActive(true);
            canClick = true;
        }
    }

    private void HandleFlowNextStep()
    {
        if (flowStep == 0)
        {
            // 第 1 點：換台詞
            if (dialogText != null)
            {
                dialogText.text = "Let's GO GO GO !";
            }
            flowStep++;
        }
        else if (flowStep == 1)
        {
            // 第 2 點：收起鳥鳥，淡入綠底圖，彈出木看板第 1 句
            if (birdHintDialog != null)
            {
                birdHintDialog.SetActive(false);
            }
            canClick = false;
            StartCoroutine(ShowBoardAndSignRoutine());
        }
        else if (flowStep == 2)
        {
            // 第 3 點：換第 2 句
            if (systemSignText != null)
            {
                systemSignText.text = "討論報告一定要畫一些流程圖，才能夠加快進展，並且期末了大家都很窮";
            }
            flowStep++;
        }
        else if (flowStep == 3)
        {
            // 第 4 點：換第 3 句
            if (systemSignText != null)
            {
                systemSignText.text = "請根據條件選篩選出適合的場地";
            }
            flowStep++;
        }
        else if (flowStep == 4)
        {
            // 第 5 點：木板直接徹底消失，抽屜由下往上滑入！
            canClick = false;
            if (systemSignPanel != null)
            {
                systemSignPanel.SetActive(false);
            }
            StartCoroutine(DrawerSlideInRoutine());
            flowStep++;
        }
    }

    private IEnumerator ShowBoardAndSignRoutine()
    {
        // 1. 底圖淡入
        if (frameBGCanvasGroup != null)
        {
            frameBGCanvasGroup.blocksRaycasts = true;
            float alpha = 0f;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                frameBGCanvasGroup.alpha = Mathf.Clamp01(alpha);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.1f);

        // 2. 顯示木看板
        if (systemSignPanel != null)
        {
            systemSignPanel.SetActive(true);
        }

        if (systemSignText != null)
        {
            systemSignText.text = "我們需要的場地必須滿足小組人數四人，並且組員很可憐，都十點後才下班，報告預計要三小時完成";
        }

        if (systemSignCanvasGroup != null)
        {
            systemSignCanvasGroup.alpha = 1f;
            systemSignCanvasGroup.blocksRaycasts = true;
        }

        flowStep = 2;
        canClick = true;
    }

    // 抽屜滑入動畫
    private IEnumerator DrawerSlideInRoutine()
    {
        if (conditionDrawer != null)
        {
            conditionDrawer.SetActive(true);
        }

        if (drawerCanvasGroup != null && drawerRectTransform != null)
        {
            drawerCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;
            float duration = 0.45f;
            Vector2 startPos = drawerRectTransform.anchoredPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f);

                drawerCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                drawerRectTransform.anchoredPosition = Vector2.Lerp(startPos, drawerTargetPos, t);
                yield return null;
            }

            drawerCanvasGroup.alpha = 1f;
            drawerRectTransform.anchoredPosition = drawerTargetPos;
        }
    }
}