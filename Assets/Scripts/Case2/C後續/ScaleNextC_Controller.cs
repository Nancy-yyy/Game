using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ScaleNextC_Controller : MonoBehaviour
{
    [Header("【第一階段：書籍資訊面板】")]
    public GameObject bookInfoPanel;      
    public Button backToScaleBtn;         
    public Button confirmBuyBtn;          
    public string previousSceneName = "Case2_05_ScaleGame"; 
    public SceneTransition sceneTransition; 

    [Header("【第二階段：合約與手寫簽名】")]
    public GameObject contractPanel;        
    public Button signOrStampBtn;           // 合約上的簽名按鈕
    public RawImage contractSignDisplay;    // 合約上的簽名圖格
    public SignaturePad signaturePad;       // SignaturePopupPanel
    public GameObject stampImage;           // StampImage (印章)
    public GameObject backpackObj;          // Backpack 物件 (預設關閉)
    public RectTransform bookRect;          // Book 物件 (預設關閉)
    public RectTransform backpackRect;      // Backpack 的 RectTransform
    public AudioSource audioSource;         
    public AudioClip stampSFX;              // 蓋章聲 (咚！)

    [Header("【第三階段：理由勾選題】")]
    public GameObject reasonQuizPanel;      
    public Toggle opt1_FourMonths;        // ✓ 我只需要使用四個月
    public Toggle opt2_NoNeedOwn;         // ✓ 我不需要永久擁有這本書
    public Toggle opt3_Free;              // ✗ 租借完全不用花錢
    public Toggle opt4_Newer;             // ✗ 租借的書一定比二手書更新
    public Toggle opt5_LowestPrice;       // ✗ 只要價格最低就是最適合的方案
    public Button submitReasonsBtn;       

    [Header("【第四階段：錯誤提示系統 (System_msg)】")]
    public RectTransform systemMsgPanel;    // 拖入 System_msg 面板
    public float sysMsgSlideDuration = 0.5f; // 滑動速度
    public float sysMsgDisplayDuration = 5.0f; // 停留秒數 (5秒)
    private Coroutine activeSysMsgRoutine;
    private Vector2 sysMsgTargetPos;
    private Vector2 sysMsgHidePos;

    [Header("【第五階段：鳥鳥回饋與返回攤位】")]
    public GameObject birdFeedbackBubble; 
    public TextMeshProUGUI birdFeedbackText; 
    public Button birdFeedbackClickBtn;     // 點擊後進入攤位結尾（可綁在氣泡本身或透明按鈕）
    public string stallSceneName = "Case2_03_Stall"; // 攤位場景名稱

    void Start()
    {
        // 初始狀態配置
        if (bookInfoPanel != null) bookInfoPanel.SetActive(true);
        if (contractPanel != null) contractPanel.SetActive(false);
        if (signaturePad != null) signaturePad.gameObject.SetActive(false);
        if (stampImage != null) stampImage.SetActive(false);
        if (backpackObj != null) backpackObj.SetActive(false);
        if (bookRect != null) bookRect.gameObject.SetActive(false);
        if (reasonQuizPanel != null) reasonQuizPanel.SetActive(false);
        if (birdFeedbackBubble != null) birdFeedbackBubble.SetActive(false);

        // 初始化 System_msg 隱藏位置
        if (systemMsgPanel != null)
        {
            sysMsgTargetPos = systemMsgPanel.anchoredPosition;
            sysMsgHidePos = new Vector2(sysMsgTargetPos.x, sysMsgTargetPos.y + 800f);
            systemMsgPanel.anchoredPosition = sysMsgHidePos;
            systemMsgPanel.gameObject.SetActive(false);
        }

        ResetAllToggles();

        if (sceneTransition == null) sceneTransition = FindObjectOfType<SceneTransition>();

        // 綁定按鈕事件
        if (backToScaleBtn != null)
        {
            backToScaleBtn.onClick.RemoveAllListeners();
            backToScaleBtn.onClick.AddListener(OnBackToScaleClicked);
        }

        if (confirmBuyBtn != null)
        {
            confirmBuyBtn.onClick.RemoveAllListeners();
            confirmBuyBtn.onClick.AddListener(OnConfirmBuyClicked);
        }

        if (signOrStampBtn != null)
        {
            signOrStampBtn.onClick.RemoveAllListeners();
            signOrStampBtn.onClick.AddListener(OnOpenSignaturePadClicked);
        }

        if (signaturePad != null)
        {
            if (signaturePad.clearBtn != null)
            {
                signaturePad.clearBtn.onClick.RemoveAllListeners();
                signaturePad.clearBtn.onClick.AddListener(signaturePad.ClearPad);
            }

            if (signaturePad.confirmBtn != null)
            {
                signaturePad.confirmBtn.onClick.RemoveAllListeners();
                signaturePad.confirmBtn.onClick.AddListener(OnFinishSignatureClicked);
            }
        }

        if (submitReasonsBtn != null)
        {
            submitReasonsBtn.onClick.RemoveAllListeners();
            submitReasonsBtn.onClick.AddListener(OnSubmitReasonsClicked);
        }

        // 鳥鳥回饋點擊監聽：手動點擊後才觸發返回攤位
        if (birdFeedbackClickBtn != null)
        {
            birdFeedbackClickBtn.onClick.RemoveAllListeners();
            birdFeedbackClickBtn.onClick.AddListener(OnBirdFeedbackClicked);
        }
    }

    private void ResetAllToggles()
    {
        if (opt1_FourMonths != null) opt1_FourMonths.isOn = false;
        if (opt2_NoNeedOwn != null) opt2_NoNeedOwn.isOn = false;
        if (opt3_Free != null) opt3_Free.isOn = false;
        if (opt4_Newer != null) opt4_Newer.isOn = false;
        if (opt5_LowestPrice != null) opt5_LowestPrice.isOn = false;
    }

    public void OnBackToScaleClicked()
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionAndLoadScene(previousSceneName);
        }
        else
        {
            SceneManager.LoadScene(previousSceneName);
        }
    }

    public void OnConfirmBuyClicked()
    {
        if (bookInfoPanel != null) bookInfoPanel.SetActive(false);
        if (contractPanel != null) contractPanel.SetActive(true);
    }

    public void OnOpenSignaturePadClicked()
    {
        if (signaturePad != null)
        {
            signaturePad.gameObject.SetActive(true);
            signaturePad.ClearPad();
        }
    }

    public void OnFinishSignatureClicked()
    {
        if (signaturePad == null || !signaturePad.hasSigned) return;

        if (contractSignDisplay != null)
        {
            contractSignDisplay.texture = signaturePad.GetSignatureTexture();
            contractSignDisplay.color = Color.white;
        }

        signaturePad.gameObject.SetActive(false);
        if (signOrStampBtn != null) signOrStampBtn.gameObject.SetActive(false);

        StartCoroutine(ExecuteContractAndBookSequence());
    }

    private IEnumerator ExecuteContractAndBookSequence()
    {
        yield return new WaitForSeconds(0.2f);

        if (audioSource != null && stampSFX != null)
        {
            audioSource.PlayOneShot(stampSFX);
        }

        // 1. 印章砸下
        if (stampImage != null)
        {
            stampImage.SetActive(true);
            RectTransform stampRect = stampImage.GetComponent<RectTransform>();
            Vector3 originalScale = stampRect.localScale;
            Vector3 giantScale = originalScale * 3.5f;

            float stampDuration = 0.45f;
            float time = 0;
            while (time < stampDuration)
            {
                time += Time.deltaTime;
                stampRect.localScale = Vector3.Lerp(giantScale, originalScale, time / stampDuration);
                yield return null;
            }
            stampRect.localScale = originalScale;
        }

        yield return new WaitForSeconds(1.5f);

        // 2. 背包與書本出現，書本滑入背包
        if (backpackObj != null) backpackObj.SetActive(true);

        if (bookRect != null)
        {
            bookRect.gameObject.SetActive(true);
            Vector2 targetPos = backpackRect != null ? backpackRect.anchoredPosition : bookRect.anchoredPosition;
            Vector2 startPos = new Vector2(targetPos.x, targetPos.y + 600f);
            Vector3 originalScale = bookRect.localScale;

            bookRect.anchoredPosition = startPos;

            float slideDuration = 0.8f;
            float time = 0;
            while (time < slideDuration)
            {
                time += Time.deltaTime;
                float progress = time / slideDuration;

                bookRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, progress);

                if (progress > 0.5f)
                {
                    float scaleProgress = (progress - 0.5f) / 0.5f;
                    bookRect.localScale = Vector3.Lerp(originalScale, originalScale * 0.7f, scaleProgress);
                }

                yield return null;
            }

            bookRect.anchoredPosition = targetPos;
        }

        yield return new WaitForSeconds(1.5f);

        // 3. 開啟理由選擇題
        if (contractPanel != null) contractPanel.SetActive(false);
        if (reasonQuizPanel != null)
        {
            ResetAllToggles();
            reasonQuizPanel.SetActive(true);
        }
    }

    public void OnSubmitReasonsClicked()
    {
        bool isCorrect = (opt1_FourMonths != null && opt1_FourMonths.isOn) &&
                         (opt2_NoNeedOwn != null && opt2_NoNeedOwn.isOn) &&
                         (opt3_Free != null && !opt3_Free.isOn) &&
                         (opt4_Newer != null && !opt4_Newer.isOn) &&
                         (opt5_LowestPrice != null && !opt5_LowestPrice.isOn);

        if (isCorrect)
        {
            if (systemMsgPanel != null) systemMsgPanel.gameObject.SetActive(false);

            // 彈出鳥鳥回饋氣泡，不再以時間倒數自動跳轉
            if (birdFeedbackBubble != null && birdFeedbackText != null)
            {
                birdFeedbackBubble.SetActive(true);
                birdFeedbackText.text = "哇！主人好像找到一個不會讓錢包哭哭的方法了！";
                LayoutRebuilder.ForceRebuildLayoutImmediate(birdFeedbackBubble.GetComponent<RectTransform>());
            }

            if (submitReasonsBtn != null) submitReasonsBtn.interactable = false;
        }
        else
        {
            if (activeSysMsgRoutine != null) StopCoroutine(activeSysMsgRoutine);
            activeSysMsgRoutine = StartCoroutine(ShowSystemErrorSlideRoutine());
        }
    }

    // 玩家點擊鳥鳥氣泡或按鈕後觸發
    // 玩家點擊鳥鳥氣泡後觸發
    public void OnBirdFeedbackClicked()
    {
        // ⭐ 標記攤位為第七幕「最終感悟結尾」對話狀態
        Case2State.StallPhase = 3;

        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionAndLoadScene(stallSceneName);
        }
        else
        {
            SceneManager.LoadScene(stallSceneName);
        }
    }

    private IEnumerator ShowSystemErrorSlideRoutine()
    {
        if (systemMsgPanel == null) yield break;

        systemMsgPanel.gameObject.SetActive(true);

        float time = 0;
        while (time < sysMsgSlideDuration)
        {
            time += Time.deltaTime;
            systemMsgPanel.anchoredPosition = Vector2.Lerp(sysMsgHidePos, sysMsgTargetPos, time / sysMsgSlideDuration);
            yield return null;
        }
        systemMsgPanel.anchoredPosition = sysMsgTargetPos;

        yield return new WaitForSeconds(sysMsgDisplayDuration);

        time = 0;
        while (time < sysMsgSlideDuration)
        {
            time += Time.deltaTime;
            systemMsgPanel.anchoredPosition = Vector2.Lerp(sysMsgTargetPos, sysMsgHidePos, time / sysMsgSlideDuration);
            yield return null;
        }
        systemMsgPanel.anchoredPosition = sysMsgHidePos;
        systemMsgPanel.gameObject.SetActive(false);
    }
}