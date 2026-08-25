/*
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video; // 1. 記得引入影片命名空間

public class BellSceneIntro : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    
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
    }

    private void Start()
    {
        playerNameText.text = GameData.PlayerName;

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
}
*/

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BellSceneIntro : MonoBehaviour
{
    [Header("玩家名稱")]
    [SerializeField] private TMP_Text playerNameText;

    [Header("鈴鐺")]
    public RectTransform bellImage;

    [Header("教授對話")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [TextArea(2, 5)]
    public string[] dialogueLines = new string[]
    {
        "大家好，我是《管理資訊系統》這門課的教授",
        "在上課之前，大家需要先了解...#%^$^&%^*$^",
        "...基本上就是這樣，另外，這門課有指定的原文書，大家別忘了在下次上課前準備好"
    };

    [Header("主角對話")]
    [SerializeField] private GameObject playerDialoguePanel;
    [SerializeField] private TMP_Text playerDialogueText;

    [TextArea(2, 5)]
    public string[] playerDialogueLines = new string[]
    {
        "搞什麼鬼啊！工資系第一天開學，這本原文書一本要 3,500 台幣？！",
        "我算一下……我扣掉住宿費後，口袋裡全部的生活費只剩下 500 塊欸！",
        "買完這本書，我接下來這個月要去台南路邊吃土嗎？！",
        "可是期末考一定要用這本書啊，如果不弄到一本，我這門課絕對被教授刷掉！"
    };

    [Header("鳥鳥對話")]
    [SerializeField] private GameObject birdDialoguePanel;
    [SerializeField] private TMP_Text birdDialogueText;

    [Header("系統提示")]
    [SerializeField] private GameObject systemPanel;
    [SerializeField] private TMP_Text systemText;

    [Header("下一個場景")]
    [SerializeField] private string nextSceneName;

    [Header("音效設定")]
    public AudioClip bellSoundClip;

    [Header("搖擺與淡入參數")]
    public float swingAngle = 20f;
    public float swingSpeed = 8f;
    public float fadeInDuration = 1.0f;

    private AudioSource audioSource;
    private CanvasGroup dialogueCanvasGroup;

    private int professorLineIndex = 0;
    private int playerLineIndex = 0;

    private DialogueState currentState = DialogueState.Professor;

    private bool canClickToNext = false;

    private enum DialogueState
    {
        Professor,
        Player,
        Bird,
        System,
        Finished
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;

        if (dialoguePanel != null)
        {
            dialogueCanvasGroup = dialoguePanel.GetComponent<CanvasGroup>();

            if (dialogueCanvasGroup == null)
            {
                dialogueCanvasGroup =
                    dialoguePanel.AddComponent<CanvasGroup>();
            }

            dialogueCanvasGroup.alpha = 0f;
            dialoguePanel.SetActive(false);
        }

        if (bellImage != null)
        {
            bellImage.gameObject.SetActive(true);
        }

        playerDialoguePanel.SetActive(false);
        birdDialoguePanel.SetActive(false);
        systemPanel.SetActive(false);
    }

    private void Start()
    {
        playerNameText.text = GameData.PlayerName;

        StartCoroutine(PlayStrictSequence());
    }

    public void OnProfessorClick()
    {
        Debug.Log("教授對話框被點了");
        if (currentState != DialogueState.Professor)
            return;

        professorLineIndex++;

        if (professorLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[professorLineIndex];
        }
        else
        {
            ShowPlayerDialogue();
        }
    }

    public void OnPlayerClick()
    {
        if (currentState != DialogueState.Player)
            return;

        playerLineIndex++;

        if (playerLineIndex < playerDialogueLines.Length)
        {
            playerDialogueText.text = playerDialogueLines[playerLineIndex];
        }
        else
        {
            ShowBirdDialogue();
        }
    }

    public void OnBirdClick()
    {
        if (currentState != DialogueState.Bird)
            return;

        ShowSystemPanel();
    }

    private IEnumerator PlayStrictSequence()
    {
        float soundDuration =
            bellSoundClip != null ? bellSoundClip.length : 2f;

        if (bellSoundClip != null)
        {
            audioSource.clip = bellSoundClip;
            audioSource.Play();
        }

        float elapsedTime = 0f;

        Quaternion initialRotation =
            bellImage != null
                ? bellImage.localRotation
                : Quaternion.identity;

        while (elapsedTime < soundDuration)
        {
            if (bellImage != null)
            {
                float zRotation =
                    Mathf.Sin(elapsedTime * swingSpeed) * swingAngle;

                bellImage.localRotation =
                    Quaternion.Euler(0, 0, zRotation);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (bellImage != null)
        {
            bellImage.localRotation = initialRotation;
            bellImage.gameObject.SetActive(false);
        }

        ShowProfessorDialogue();

        canClickToNext = true;
    }

    private void ShowProfessorDialogue()
    {
        currentState = DialogueState.Professor;

        professorLineIndex = 0;

        dialogueText.text =
            dialogueLines[professorLineIndex];

        dialoguePanel.SetActive(true);

        dialogueCanvasGroup.alpha = 1f;
    }

    private void ShowPlayerDialogue()
    {
        dialoguePanel.SetActive(false);

        currentState = DialogueState.Player;

        playerLineIndex = 0;

        playerDialogueText.text =
            playerDialogueLines[playerLineIndex];

        playerDialoguePanel.SetActive(true);
    }

    private void ShowBirdDialogue()
    {
        playerDialoguePanel.SetActive(false);

        currentState = DialogueState.Bird;

        birdDialogueText.text =
            "主人的錢包在劇烈發抖！";

        birdDialoguePanel.SetActive(true);
    }

    private void ShowSystemPanel()
    {
        birdDialoguePanel.SetActive(false);

        currentState = DialogueState.System;

        canClickToNext = false;

        systemText.text =
            "那就進入「書人不輸陣」平台，絕對可以在上面找到解決方法！";

        systemPanel.SetActive(true);
    }

    public void EnterNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}