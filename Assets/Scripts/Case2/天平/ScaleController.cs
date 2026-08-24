using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ScaleController : MonoBehaviour
{
    [Header("天平各部位")]
    public Transform scaleBeam;   // 拖入「天秤上_0」
    public Transform leftPan;     // 拖入「天秤左_0」
    public Transform rightPan;    // 拖入「天秤右_0」

    [Header("主角頭部 Sprite 與表情")]
    public SpriteRenderer playerFaceRenderer;
    public Sprite faceNormal;     
    public Sprite faceCry;        
    public Sprite faceHesitate;   
    public Sprite faceThink;      
    public Sprite faceCollapse;   

    [Header("小鳥助教氣泡對話框 (無AI情緒對話)")]
    public GameObject birdBubbleObj;
    public TextMeshProUGUI birdText;

    [Header("系統資訊回饋對話框 (低資訊回饋)")]
    public GameObject systemDialogueObj;
    public TextMeshProUGUI systemText;

    [Header("按鈕管理")]
    public Button resetBtn;         
    public Button confirmBtn;       

    [Header("轉場控制")]
    public SceneTransition sceneTransition; // 拖入 TransitionCanvas 上的腳本

    [Header("雙按鈕排版微調")]
    public float buttonSpacing = 160f;
    public float buttonScaleFactor = 0.8f;

    [Header("卡片管理")]
    public DraggableCard2D[] allCards;

    // 記錄當前放入的卡片類型 (用來決定切換到哪一個支線場景)
    private string currentSelectedCard = "";

    private Coroutine activeScaleRoutine;
    private const float MAX_TILT_ANGLE = -12f;
    private const float WOBBLE_AMPLITUDE = 6f;
    private const string DEFAULT_SYSTEM_HINT = "拖曳右手邊的卡牌到天平上吧！";

    private Vector2 resetBtnOriginalPos;
    private Vector3 resetBtnOriginalScale;
    private RectTransform resetBtnRect;
    private RectTransform confirmBtnRect;

    void Start()
    {
        if (scaleBeam == null) scaleBeam = this.transform;
        
        if (allCards == null || allCards.Length == 0)
        {
            allCards = FindObjectsOfType<DraggableCard2D>();
        }

        if (sceneTransition == null)
        {
            sceneTransition = FindObjectOfType<SceneTransition>();
        }

        if (resetBtn != null)
        {
            resetBtnRect = resetBtn.GetComponent<RectTransform>();
            resetBtnOriginalPos = resetBtnRect.anchoredPosition;
            resetBtnOriginalScale = resetBtnRect.localScale;

            resetBtn.onClick.RemoveAllListeners();
            resetBtn.onClick.AddListener(ResetScaleAndCards);
        }

        if (confirmBtn != null)
        {
            confirmBtnRect = confirmBtn.GetComponent<RectTransform>();
            confirmBtn.onClick.RemoveAllListeners();
            confirmBtn.onClick.AddListener(OnConfirmClicked);
            confirmBtn.gameObject.SetActive(false);
        }

        ResetToInitialState();
    }

    private void ResetToInitialState()
    {
        currentSelectedCard = "";
        SetPlayerFace(faceNormal != null ? faceNormal : faceThink);
        
        if (birdBubbleObj != null) birdBubbleObj.SetActive(false);

        if (systemDialogueObj != null && systemText != null)
        {
            systemDialogueObj.SetActive(true);
            systemText.text = DEFAULT_SYSTEM_HINT;
        }

        RestoreResetButton();
    }

    public void TriggerReaction(string type)
    {
        currentSelectedCard = type; // 記錄當前放置的卡片 (A/B/C/D)

        if (activeScaleRoutine != null) StopCoroutine(activeScaleRoutine);

        switch (type)
        {
            case "A":
                SetPlayerFace(faceCry);
                RestoreResetButton();
                activeScaleRoutine = StartCoroutine(RotateSmooth(MAX_TILT_ANGLE, 0.4f));
                ShowDialogues("主人你快要沒錢啦！", "目前檢測到資金不足，無法選擇該選項。");
                break;

            case "B": // 二手書
                SetPlayerFace(faceHesitate);
                AdjustButtonsForChoice();
                activeScaleRoutine = StartCoroutine(WobbleAndBalance());
                ShowDialogues("二手書啊...不知道會不會有很多塗鴉呢？", "目前選項：購買二手書，請問確定支出500元現金嗎？");
                break;

            case "C": // 租借
                SetPlayerFace(faceThink);
                AdjustButtonsForChoice();
                activeScaleRoutine = StartCoroutine(WobbleAndBalance());
                ShowDialogues("租嗎？這樣還要還回去耶...", "目前選項：教科書租借，使用期間約一學期，租借費用 150 元。");
                break;

            case "D":
                SetPlayerFace(faceCollapse);
                RestoreResetButton();
                activeScaleRoutine = StartCoroutine(RotateSmooth(MAX_TILT_ANGLE, 0.4f));
                ShowDialogues("這樣會被當掉的主人！", "目前檢測到教授站在你身後，看起來很生氣。");
                break;
        }
    }

    private void AdjustButtonsForChoice()
    {
        if (resetBtnRect != null)
        {
            resetBtnRect.localScale = resetBtnOriginalScale * buttonScaleFactor;
            resetBtnRect.anchoredPosition = new Vector2(resetBtnOriginalPos.x + buttonSpacing, resetBtnOriginalPos.y);
        }

        if (confirmBtn != null && confirmBtnRect != null)
        {
            confirmBtn.gameObject.SetActive(true);
            confirmBtnRect.localScale = resetBtnOriginalScale * buttonScaleFactor;
            confirmBtnRect.anchoredPosition = new Vector2(resetBtnOriginalPos.x - buttonSpacing, resetBtnOriginalPos.y);
        }
    }

    private void RestoreResetButton()
    {
        if (resetBtnRect != null)
        {
            resetBtnRect.localScale = resetBtnOriginalScale;
            resetBtnRect.anchoredPosition = resetBtnOriginalPos;
        }

        if (confirmBtn != null)
        {
            confirmBtn.gameObject.SetActive(false);
        }
    }

    private void ShowDialogues(string birdMsg, string sysMsg)
    {
        if (birdBubbleObj != null && birdText != null)
        {
            birdBubbleObj.SetActive(true);
            birdText.text = birdMsg;
            LayoutRebuilder.ForceRebuildLayoutImmediate(birdBubbleObj.GetComponent<RectTransform>());
        }

        if (systemDialogueObj != null && systemText != null)
        {
            systemDialogueObj.SetActive(true);
            systemText.text = sysMsg;
        }
    }

    private IEnumerator RotateSmooth(float targetZ, float duration)
    {
        float time = 0;
        float startZ = GetCurrentAngle();

        while (time < duration)
        {
            time += Time.deltaTime;
            float currentZ = Mathf.Lerp(startZ, targetZ, time / duration);
            ApplyScaleRotation(currentZ);
            yield return null;
        }
        ApplyScaleRotation(targetZ);
    }

    private IEnumerator WobbleAndBalance()
    {
        float duration = 2.2f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float damping = 1f - (time / duration);
            float currentZ = Mathf.Sin(time * 10f) * (WOBBLE_AMPLITUDE * damping);
            ApplyScaleRotation(currentZ);
            yield return null;
        }
        ApplyScaleRotation(0f);
    }

    private void ApplyScaleRotation(float zAngle)
    {
        scaleBeam.localEulerAngles = new Vector3(0, 0, zAngle);
        if (leftPan != null) leftPan.localEulerAngles = new Vector3(0, 0, -zAngle);
        if (rightPan != null) rightPan.localEulerAngles = new Vector3(0, 0, -zAngle);
    }

    private float GetCurrentAngle()
    {
        float z = scaleBeam.localEulerAngles.z;
        return (z > 180) ? z - 360 : z;
    }

    private void SetPlayerFace(Sprite face)
    {
        if (playerFaceRenderer != null && face != null)
        {
            playerFaceRenderer.sprite = face;
        }
    }

    // 確定按鈕：根據當前選中卡片動態決定跳轉場景 (B -> ScaleNext_B, C -> ScaleNext_C)
    public void OnConfirmClicked()
    {
        string targetScene = "";

        if (currentSelectedCard == "B")
        {
            targetScene = "ScaleNext_B";
        }
        else if (currentSelectedCard == "C")
        {
            targetScene = "ScaleNext_C";
        }

        if (string.IsNullOrEmpty(targetScene)) return;

        Debug.Log("前往支線場景: " + targetScene);

        if (sceneTransition == null)
        {
            sceneTransition = FindObjectOfType<SceneTransition>();
        }

        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionAndLoadScene(targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    public void ResetScaleAndCards()
    {
        if (activeScaleRoutine != null) StopCoroutine(activeScaleRoutine);
        activeScaleRoutine = StartCoroutine(RotateSmooth(0f, 0.3f));
        
        ResetToInitialState();

        DraggableCard2D[] cards = (allCards != null && allCards.Length > 0) ? allCards : FindObjectsOfType<DraggableCard2D>();
        foreach (var card in cards)
        {
            if (card != null) card.ResetToOriginalPos();
        }
    }
}