using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject dialoguePanel;       
    public CanvasGroup dialogueCanvasGroup;
    public GameObject hintPanel;           
    public TextMeshProUGUI dialogueText;   
    public TextMeshProUGUI speakerNameText;
    public GameObject speakerHeadImage;    

    [Header("角色與腳本")]
    public GameObject playerObject;        
    public PlayerMovement playerMovement; 

    [Header("預設對話設定")]
    [TextArea(2, 5)]
    public string initialDialogue = "哇！原來這就是新教室...看起來好大阿...";
    public string speakerName = "主角";
    public Sprite speakerSprite;           
    public float fadeInDuration = 0.5f;    

    private bool isDialogueActive = false;

    void Start()
    {
        if (playerObject != null)
        {
            playerObject.SetActive(true);

            if (playerMovement == null)
            {
                playerMovement = playerObject.GetComponent<PlayerMovement>();
            }

            if (playerMovement != null) playerMovement.enabled = false;

            // 隱藏外觀與碰撞，維持 Input System 運作
            foreach (var sr in playerObject.GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;
            foreach (var col in playerObject.GetComponentsInChildren<Collider2D>()) col.enabled = false;
        }

        if (hintPanel != null) hintPanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueCanvasGroup != null) dialogueCanvasGroup.alpha = 0f;

        ShowDialogue(initialDialogue, speakerName);
        StartCoroutine(FadeInDialogue());
    }

    public void ShowDialogue(string content, string name)
    {
        if (dialogueText != null) dialogueText.text = content;
        if (speakerNameText != null) speakerNameText.text = name;
        if (speakerHeadImage != null) speakerHeadImage.SetActive(true);
    }

    IEnumerator FadeInDialogue()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            if (dialogueCanvasGroup != null && fadeInDuration > 0f)
            {
                dialogueCanvasGroup.alpha = Mathf.Clamp01(timer / fadeInDuration);
            }
            yield return null;
        }

        if (dialogueCanvasGroup != null) dialogueCanvasGroup.alpha = 1f;
        isDialogueActive = true;
    }

    public void OnDialogueClicked()
    {
        if (!isDialogueActive) return;
        CloseDialogueAndStartGame();
    }

    void CloseDialogueAndStartGame()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isDialogueActive = false;

        if (playerObject != null) 
        {
            foreach (var sr in playerObject.GetComponentsInChildren<SpriteRenderer>()) sr.enabled = true;
            foreach (var col in playerObject.GetComponentsInChildren<Collider2D>()) col.enabled = true;
        }

        if (hintPanel != null) hintPanel.SetActive(true);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("【成功】玩家移動已解鎖！");
        }
    }
}