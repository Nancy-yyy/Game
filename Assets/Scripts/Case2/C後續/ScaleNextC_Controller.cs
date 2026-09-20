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
    public Button signOrStampBtn;           
    public RawImage contractSignDisplay;    
    public SignaturePad signaturePad;       
    public GameObject stampImage;           
    public GameObject backpackObj;          
    public RectTransform bookRect;          
    public RectTransform backpackRect;      
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
    public RectTransform systemMsgPanel;    
    public float sysMsgSlideDuration = 0.5f; 
    public float sysMsgDisplayDuration = 1.5f; 
    private Coroutine activeSysMsgRoutine;
    private Vector2 sysMsgTargetPos;
    private Vector2 sysMsgHidePos;

    [Header("【第五階段：鳥鳥回饋】")]
    public GameObject birdFeedbackBubble; 
    public TextMeshProUGUI birdFeedbackText; 
    public Button birdFeedbackClickBtn;     
    public string stallSceneName = "Case2_03_Stall"; 

    [Header("【第六階段：任務打勾面板 (Checklist)】")]
    public GameObject abilityChecklistPanel; // 拖入 AbilityChecklistPanel
    public GameObject checklistImage1;       // 拖入 ChecklistImage1 (1 個勾)
    public GameObject checklistImage2;       // 拖入 ChecklistImage2 (2 個勾)
    public AudioClip checkmarkDingSFX;       // 拖入 打勾音效 (噹！/ 叮咚聲)
    public float waitBeforeCheck = 0.6f;     // 跳出面板後多久打勾
    public float displayAfterCheck = 1.8f;   // 打勾後停留幾秒進入攤位

    void Start()
    {
        if (bookInfoPanel != null) bookInfoPanel.SetActive(true);
        if (contractPanel != null) contractPanel.SetActive(false);
        if (signaturePad != null) signaturePad.gameObject.SetActive(false);
        if (stampImage != null) stampImage.SetActive(false);
        if (backpackObj != null) backpackObj.SetActive(false);
        if (bookRect != null) bookRect.gameObject.SetActive(false);
        if (reasonQuizPanel != null) reasonQuizPanel.SetActive(false);
        if (birdFeedbackBubble != null) birdFeedbackBubble.SetActive(false);

        // 初始化任務清單面板隱藏
        if (abilityChecklistPanel != null) abilityChecklistPanel.SetActive(false);
        if (checklistImage1 != null) checklistImage1.SetActive(false);
        if (checklistImage2 != null) checklistImage2.SetActive(false);

        if (systemMsgPanel != null)
        {
            sysMsgTargetPos = systemMsgPanel.anchoredPosition;
            sysMsgHidePos = new Vector2(sysMsgTargetPos.x, sysMsgTargetPos.y + 800f);
            systemMsgPanel.anchoredPosition = sysMsgHidePos;
            systemMsgPanel.gameObject.SetActive(false);
        }

        ResetAllToggles();

        if (sceneTransition == null) sceneTransition = FindObjectOfType<SceneTransition>();

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

        // 鳥鳥氣泡點擊監聽
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
        if (sceneTransition != null) sceneTransition.StartTransitionAndLoadScene(previousSceneName);
        else SceneManager.LoadScene(previousSceneName);
    }

    public void OnConfirmBuyClicked()
    {
        // 研究紀錄：玩家正式確認 C（租借）方案。
        // C2_SCALE 在方案 C 確認後即完成；理由題另列為 C2_SCALE_REASON。
        GameData.Case2_Scale_Choice = "C";
        GameData.RecordAnswer("PLAN_C", true);
        GameData.CompleteTask();

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

        if (contractPanel != null) contractPanel.SetActive(false);
        if (reasonQuizPanel != null)
        {
            ResetAllToggles();

            // 研究紀錄：理由題從真正顯示、可作答時才開始計時
            GameData.StartTask(GameData.TaskIds.C2_SCALE_REASON);

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

        // 研究紀錄：保留本次理由勾選內容。
        System.Collections.Generic.List<string> selectedReasons =
            new System.Collections.Generic.List<string>();

        if (opt1_FourMonths != null && opt1_FourMonths.isOn) selectedReasons.Add("1");
        if (opt2_NoNeedOwn != null && opt2_NoNeedOwn.isOn) selectedReasons.Add("2");
        if (opt3_Free != null && opt3_Free.isOn) selectedReasons.Add("3");
        if (opt4_Newer != null && opt4_Newer.isOn) selectedReasons.Add("4");
        if (opt5_LowestPrice != null && opt5_LowestPrice.isOn) selectedReasons.Add("5");

        string reasonAnswer =
            "REASONS:" + string.Join("|", selectedReasons);

        GameData.RecordAnswer(reasonAnswer, isCorrect);

        if (isCorrect)
        {
            if (systemMsgPanel != null) systemMsgPanel.gameObject.SetActive(false);

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
            // 舊版摘要欄位保留：理由勾選題答錯次數
            GameData.Case2_Scale_QuizErrors++;

            if (activeSysMsgRoutine != null) StopCoroutine(activeSysMsgRoutine);
            activeSysMsgRoutine = StartCoroutine(ShowSystemErrorSlideRoutine());
        }
    }

    // ⭐ 點擊鳥鳥後：開啟任務清單 ➔ 切圖打勾 ➔ 播放噹一聲 ➔ 返回攤位
    public void OnBirdFeedbackClicked()
    {
        StartCoroutine(ShowChecklistAndProceedRoutine());
    }

    private IEnumerator ShowChecklistAndProceedRoutine()
    {
        // 1. 隱藏題目與鳥鳥氣泡
        if (reasonQuizPanel != null) reasonQuizPanel.SetActive(false);
        if (birdFeedbackBubble != null) birdFeedbackBubble.SetActive(false);

        // 2. 彈出任務面板 (先顯示只有 1 個勾的圖片)
        if (abilityChecklistPanel != null)
        {
            abilityChecklistPanel.SetActive(true);
            if (checklistImage1 != null) checklistImage1.SetActive(true);
            if (checklistImage2 != null) checklistImage2.SetActive(false);
        }

        // 3. 等待片刻 (0.6 秒)
        yield return new WaitForSeconds(waitBeforeCheck);

        // 4. 切換為 2 個勾的圖片，並播放「噹！」的一聲
        if (checklistImage1 != null) checklistImage1.SetActive(false);
        if (checklistImage2 != null) checklistImage2.SetActive(true);

        if (audioSource != null && checkmarkDingSFX != null)
        {
            audioSource.PlayOneShot(checkmarkDingSFX);
        }

        // 5. 停留 1.8 秒讓玩家看到獲得第二個打勾
        yield return new WaitForSeconds(displayAfterCheck);

        // 6. 理由題到此正式完成，再切換回攤位
        GameData.CompleteTask();

        // 確保摘要欄位保留最終方案
        GameData.Case2_Scale_Choice = "C";

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