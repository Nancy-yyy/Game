using System.Collections;
using UnityEngine;
using TMPro; // 1. 記得引入 TextMeshPro 命名空間

public class BellSceneIntro : MonoBehaviour
{
    [Header("UI 元件")]
    [Tooltip("鈴鐺 UI (進入場景會先開啟，播完自動隱藏)")]
    public RectTransform bellImage;
    
    [Tooltip("對話框 Panel")]
    public GameObject dialoguePanel;

    [Tooltip("對話文字組件")]
    public TextMeshProUGUI dialogueText; // 2. 新增文字 UI 欄位

    [Header("對話內容設定")]
    [TextArea(2, 5)]
    public string[] dialogueLines = new string[] // 3. 設定要顯示的對話
    {
        "大家好，我是《管理資訊系統》這門課的教授",
        "在上課之前，大家需要先了解...#%^$^&%^*$^",
        "...基本上就是這樣，另外，這門課有指定的原文書，大家別忘了在下次上課前準備好"
    };

    [Header("音效設定")]
    [Tooltip("鐘聲音效檔 (.mp3 / .wav)")]
    public AudioClip bellSoundClip;

    [Header("搖擺與淡入參數")]
    [Tooltip("擺動角度")]
    public float swingAngle = 20f;
    
    [Tooltip("擺動頻率/速度")]
    public float swingSpeed = 8f;
    
    [Tooltip("對話框漸顯時間 (秒)")]
    public float fadeInDuration = 1.0f;

    private AudioSource audioSource;
    private CanvasGroup dialogueCanvasGroup;
    
    // 判斷狀態用的變數
    private int currentLineIndex = 0;
    private bool canClickToNext = false; 

    private void Awake()
    {
        // 1. 設定 AudioSource 2D 全域聲音
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f; // 純 2D 聲音
        audioSource.volume = 1.0f;
        audioSource.playOnAwake = false;

        // 2. 初始化對話框 (隱藏 + 透明度 0)
        if (dialoguePanel != null)
        {
            dialogueCanvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            if (dialogueCanvasGroup == null)
            {
                dialogueCanvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
            }
            dialogueCanvasGroup.alpha = 0f;
            dialoguePanel.SetActive(false);
        }

        // 3. 步驟一：進入場景，確定鈴鐺先出現
        if (bellImage != null)
        {
            bellImage.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(PlayStrictSequence());
    }

    private void Update()
    {
        // 4. 只有當動畫結束、允許點擊，且玩家按下左鍵時才換下一句
        if (canClickToNext && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    private IEnumerator PlayStrictSequence()
    {
        float soundDuration = (bellSoundClip != null) ? bellSoundClip.length : 2.0f;

        // 步驟二 & 三：鈴鐺搖擺與鐘聲
        if (bellSoundClip != null && audioSource != null)
        {
            audioSource.clip = bellSoundClip;
            audioSource.Play();
        }

        float elapsedTime = 0f;
        Quaternion initialRotation = (bellImage != null) ? bellImage.localRotation : Quaternion.identity;

        while (elapsedTime < soundDuration)
        {
            if (bellImage != null)
            {
                float zRotation = Mathf.Sin(elapsedTime * swingSpeed) * swingAngle;
                bellImage.localRotation = Quaternion.Euler(0, 0, zRotation);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 步驟四：鈴鐺歸位並消失
        if (bellImage != null)
        {
            bellImage.localRotation = initialRotation;
            bellImage.gameObject.SetActive(false);
        }

        // 步驟五：對話框漸顯，並預先放入第一句話
        if (dialoguePanel != null && dialogueCanvasGroup != null)
        {
            // 在淡入之前先設定好第一句話
            ShowCurrentLine();
            
            dialoguePanel.SetActive(true);

            float fadeTime = 0f;
            while (fadeTime < fadeInDuration)
            {
                fadeTime += Time.deltaTime;
                dialogueCanvasGroup.alpha = Mathf.Clamp01(fadeTime / fadeInDuration);
                yield return null;
            }

            dialogueCanvasGroup.alpha = 1f;
        }

        // 動畫完全播完，開啟點擊換頁開關
        canClickToNext = true;
    }

    // 顯示目前句子的方法
    private void ShowCurrentLine()
    {
        if (dialogueText != null && dialogueLines.Length > 0 && currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
    }

    // 切換下一句的方法
    public void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            // 對話完全播放完畢後的處理（如：關閉對話框或切換場景）
            canClickToNext = false;
            Debug.Log("本幕對話結束！");
        }
    }
}