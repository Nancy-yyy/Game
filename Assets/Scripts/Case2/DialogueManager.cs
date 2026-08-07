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
    public MonoBehaviour playerMovementScript; // 手動把你的移動腳本拖進來！

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
        if (playerObject != null)
        {
            // 讓 Player 保持 Active，但一開始關閉圖片與碰撞
            playerObject.SetActive(true); 
            
            SpriteRenderer sr = playerObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Collider2D col = playerObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 一開始強制關閉移動腳本
            if (playerMovementScript != null) 
            {
                playerMovementScript.enabled = false;
            }
        }

        // 隱藏 hintPanel
        if (hintPanel != null) hintPanel.SetActive(false);

        // 初始化對話框透明度為 0
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueCanvasGroup != null) dialogueCanvasGroup.alpha = 0f;

        // 啟動漸顯協程
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
        // 1. 關閉對話框
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        isDialogueActive = false;

        // 2. 讓 Player 的圖片與碰撞恢復顯示
        if (playerObject != null) 
        {
            SpriteRenderer sr = playerObject.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = true;

            Collider2D col = playerObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }

        // 3. 讓 hintPanel 出現
        if (hintPanel != null) 
        {
            hintPanel.SetActive(true);
        }

        // 4. 解鎖玩家移動腳本
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true; 
            Debug.Log("【成功】玩家移動腳本已手動解鎖！");
        }
        else
        {
            Debug.LogError("【錯誤】你忘記在 Inspector 把移動腳本拖進 Player Movement Script 格子裡了！");
        }
    }
}