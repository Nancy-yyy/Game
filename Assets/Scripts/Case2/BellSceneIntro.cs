using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video; 
using UnityEngine.SceneManagement; // 引入場景管理命名空間

public class BellSceneIntro : MonoBehaviour
{
    [Header("UI 元件")]
    [Tooltip("鈴鐺 UI (進入場景會先開啟，播完自動隱藏)")]
    public RectTransform bellImage;
    
    [Tooltip("對話框 Panel")]
    public GameObject dialoguePanel;

    [Tooltip("對話文字組件")]
    public TextMeshProUGUI dialogueText;

    [Header("影片設定")]
    [Tooltip("負責播放影片的 VideoPlayer 組件")]
    public VideoPlayer videoPlayer;

    [Tooltip("影片播放用的 UI (例如包含 RawImage 的 GameObject)")]
    public GameObject videoUI;

    [Tooltip("按順序播放的影片清單 (放入 2 個 .mp4 Clip)")]
    public VideoClip[] videoClips;

    [Header("轉場與目標場景設定")]
    [Tooltip("拖入場景中的 TransitionCanvas (若留空會自動搜尋)")]
    public SceneTransition sceneTransition;
    [Tooltip("影片播完後要載入的目標場景名稱")]
    public string nextSceneName = "Case2_GameScene01";

    [Header("對話內容設定")]
    [TextArea(2, 5)]
    public string[] dialogueLines = new string[]
    {
        "大家好，我是《管理資訊系統》這門課的教授",
        "在上課之前，大家需要先了解...#%^$^&%^*$^",
        "...基本上就是這樣，另外，這門課有指定的原文書，大家別忘了在下次上課前準備好"
    };

    [Header("音效設定")]
    [Tooltip("鐘聲音效檔 (.mp3 / .wav)")]
    public AudioClip bellSoundClip;

    [Header("搖擺與淡入參數")]
    public float swingAngle = 20f;
    public float swingSpeed = 8f;
    public float fadeInDuration = 1.0f;

    private AudioSource audioSource;
    private CanvasGroup dialogueCanvasGroup;
    
    private int currentLineIndex = 0;
    private bool canClickToNext = false; 

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1.0f;
        audioSource.playOnAwake = false;

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

        if (bellImage != null)
        {
            bellImage.gameObject.SetActive(true);
        }

        // 初始化：先隱藏影片 UI
        if (videoUI != null)
        {
            videoUI.SetActive(false);
        }

        // 自動抓取場景中的 SceneTransition
        if (sceneTransition == null)
        {
            sceneTransition = FindObjectOfType<SceneTransition>();
        }
    }

    private void Start()
    {
        StartCoroutine(PlayStrictSequence());
    }

    private void Update()
    {
        if (canClickToNext && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    private IEnumerator PlayStrictSequence()
    {
        float soundDuration = (bellSoundClip != null) ? bellSoundClip.length : 2.0f;

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

        if (bellImage != null)
        {
            bellImage.localRotation = initialRotation;
            bellImage.gameObject.SetActive(false);
        }

        if (dialoguePanel != null && dialogueCanvasGroup != null)
        {
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

        canClickToNext = true;
    }

    private void ShowCurrentLine()
    {
        if (dialogueText != null && dialogueLines.Length > 0 && currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
    }

    public void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            // 對話結束：關閉對話框，開啟影片序列
            canClickToNext = false;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);

            StartCoroutine(PlayVideoSequence());
        }
    }

    // 連續播放影片的協程
    private IEnumerator PlayVideoSequence()
    {
        if (videoPlayer == null || videoClips == null || videoClips.Length == 0)
        {
            Debug.LogWarning("VideoPlayer 或 VideoClips 未設定，直接切換場景！");
            LoadNextGameScene();
            yield break;
        }

        if (videoUI != null) videoUI.SetActive(true);

        // 依序播放陣列中的影片
        foreach (VideoClip clip in videoClips)
        {
            if (clip == null) continue;

            videoPlayer.clip = clip;
            videoPlayer.Prepare();

            // 等待影片預載完成
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            videoPlayer.Play();

            // 等待開始播放
            while (!videoPlayer.isPlaying)
            {
                yield return null;
            }

            // 等待影片播放結束
            while (videoPlayer.isPlaying)
            {
                yield return null;
            }
        }

        // 兩部影片均播放完畢後的後續處理
        if (videoUI != null) videoUI.SetActive(false);
        Debug.Log("所有影片播放完畢！開始轉場至 " + nextSceneName);

        // 影片播完後啟動轉場進入 Case2_GameScene01
        LoadNextGameScene();
    }

    private void LoadNextGameScene()
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionAndLoadScene(nextSceneName);
        }
        else
        {
            // 防呆機制：若場景中沒有 SceneTransition，則直接切換
            SceneManager.LoadScene(nextSceneName);
        }
    }
}