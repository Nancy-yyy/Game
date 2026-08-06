using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 對話框與提示元件")]
    public GameObject dialoguePanel;       
    public CanvasGroup dialogueCanvasGroup;
    public GameObject hintPanel;           
    public TextMeshProUGUI dialogueText;   
    public TextMeshProUGUI speakerNameText;
    public GameObject speakerHeadImage;    

    [Header("遊戲角色與移動控制")]
    public GameObject playerObject;        
    public MonoBehaviour playerMovementScript; // 讓你在 Inspector 手動拖入移動腳本

    [Header("預設對話設定")]
    [TextArea(2, 5)]
    public string initialDialogue = "哇！原來這就是新教室...看起來好大阿...";
    public string speakerName = "主角";
    public Sprite speakerSprite;           

    [Header("漸顯動畫設定")]
    public float fadeInDuration = 0.5f;    

    private bool isDialogueActive = false;

    void Start()
    {
        // 1. 遊戲一開始：隱藏 Player 與 hintPanel
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        if (hintPanel != null) 
        {
            hintPanel.SetActive(false);
        }

        // 2. 初始化對話框透明度為 0
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
        }

        // 3. 啟動漸顯協程
        ShowDialogue(initialDialogue, speakerName);
        StartCoroutine(FadeInDialogue());
    }

    public void ShowDialogue(string content, string name)
    {
        if (dialogueText != null) dialogueText.text = content;
        if (speakerNameText != null) speakerNameText.text = name;

        if (speakerHeadImage != null)
        {
            speakerHeadImage.SetActive(true);
            Image img = speakerHeadImage.GetComponent<Image>();
            if (img != null && speakerSprite != null)
            {
                img.sprite = speakerSprite;
            }
        }
    }

    IEnumerator FadeInDialogue()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            if (dialogueCanvasGroup != null)
            {
                dialogueCanvasGroup.alpha = Mathf.Clamp01(timer / fadeInDuration);
            }
            yield return null;
        }

        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 1f;
        }

        isDialogueActive = true;
    }

    // 當點擊透明按鈕時觸發
    public void OnDialogueClicked()
    {
        if (!isDialogueActive) return; 

        CloseDialogueAndStartGame();
    }

    void CloseDialogueAndStartGame()
    {
        // 關閉對話框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        isDialogueActive = false;

        // 讓 Player 與 hintPanel 同時出現
        if (playerObject != null) 
        {
            playerObject.SetActive(true);

            // 確保剛體被喚醒，避免物理卡住
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.WakeUp();
            }
        }

        if (hintPanel != null) 
        {
            hintPanel.SetActive(true);
        }

        // 解鎖玩家移動
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true; 
            Debug.Log("玩家移動腳本已解鎖！");
        }
    }
}