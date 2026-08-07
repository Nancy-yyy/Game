using System.Collections;
using UnityEngine;

public class BellSceneIntro : MonoBehaviour
{
    [Header("UI 元件")]
    [Tooltip("鈴鐺 UI (進入場景會先開啟，播完自動隱藏)")]
    public RectTransform bellImage;
    
    [Tooltip("對話框 Panel")]
    public GameObject dialoguePanel;

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

    private IEnumerator PlayStrictSequence()
    {
        // 取得音效實際長度 (如果沒放音效則預設搖擺 2 秒)
        float soundDuration = (bellSoundClip != null) ? bellSoundClip.length : 2.0f;

        // ----------------------------------------------------
        // 步驟二 & 三：鈴鐺出現，同步播放鐘聲與左右搖擺
        // ----------------------------------------------------
        if (bellSoundClip != null && audioSource != null)
        {
            audioSource.clip = bellSoundClip;
            audioSource.Play();
        }

        float elapsedTime = 0f;
        Quaternion initialRotation = (bellImage != null) ? bellImage.localRotation : Quaternion.identity;

        // 搖擺時間完全對齊鐘聲長度 (soundDuration)
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

        // ----------------------------------------------------
        // 步驟四：鐘聲結束，鈴鐺消失
        // ----------------------------------------------------
        if (bellImage != null)
        {
            bellImage.localRotation = initialRotation; // 歸位
            bellImage.gameObject.SetActive(false);     // 鈴鐺消失
        }

        // ----------------------------------------------------
        // 步驟五：對話框漸顯出現 (Fade In)
        // ----------------------------------------------------
        if (dialoguePanel != null && dialogueCanvasGroup != null)
        {
            dialoguePanel.SetActive(true); // 顯示對話框物件

            float fadeTime = 0f;
            while (fadeTime < fadeInDuration)
            {
                fadeTime += Time.deltaTime;
                dialogueCanvasGroup.alpha = Mathf.Clamp01(fadeTime / fadeInDuration);
                yield return null;
            }

            dialogueCanvasGroup.alpha = 1f; // 確保完全顯現
        }
    }
}