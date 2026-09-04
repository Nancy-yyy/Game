using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BellSceneIntro : MonoBehaviour
{
    public enum CharacterType
    {
        Professor,  // 教授 (使用標準對話框)
        Player,     // 主角 (使用標準對話框)
        Classmate,  // 旁邊同學 (使用標準對話框，換頭像與名字)
        Bird,       // 鳥鳥 (使用獨立鳥鳥對話框)
        System      // 系統框 (最後提示與轉場按鈕)
    }

    [System.Serializable]
    public class DialogueLine
    {
        public CharacterType speakerType;
        public string speakerName;
        [TextArea(2, 4)] public string content;
    }

    [Header("玩家名稱與鈴鐺")]
    [SerializeField] private TMP_Text playerNameText;
    public RectTransform bellImage;
    public AudioClip bellSoundClip;
    public float swingAngle = 20f;
    public float swingSpeed = 8f;

    [Header("【角色通用對話面板】(主角/教授/旁邊同學)")]
    public GameObject characterDialoguePanel;    // 拖入 PlayerDialoguePanel (或 DialoguePanel)
    public Image characterHeadImage;             // 拖入裡面的 playerImage (頭像 Image)
    public TMP_Text characterNameText;           // 拖入裡面的 NameText
    public TMP_Text characterContentText;        // 拖入裡面的 DialogueText
    public Button characterNextBtn;              // 拖入 PlayerClickArea (全螢幕或對話框按鈕)

    [Header("【頭像 Sprite】")]
    public Sprite professorSprite;               // 教授頭像
    public Sprite playerSprite;                  // 主角頭像
    public Sprite classmateSprite;               // 旁邊同學頭像

    [Header("【鳥鳥專用面板】")]
    public GameObject birdDialoguePanel;         // 拖入 BirdDialoguePanel
    public TMP_Text birdDialogueText;            // 拖入裡面的 DialogueText
    public Button birdNextBtn;                   // 拖入 BirdClickArea

    [Header("【系統提示面板】")]
    public GameObject systemPanel;               // 拖入 SystemPanel
    public TMP_Text systemText;                  // 拖入裡面的 DialogueText_
    public Button continueButton;                // 拖入 ContinueButton

    [Header("【目標切換場景】")]
    [SerializeField] private string nextSceneName = "Case2_03_Stall";

    private AudioSource audioSource;
    private int currentDialogueIndex = 0;

    // 依據新劇本建立的 11 句線性對白
    private readonly DialogueLine[] storyDialogues = new DialogueLine[]
    {
        new DialogueLine { speakerType = CharacterType.Professor, speakerName = "教授", content = "這學期指定教材是《管理資訊系統》，請同學自行準備。" },
        new DialogueLine { speakerType = CharacterType.Player, speakerName = "主角", content = "蛤？！一本書三千五？" },
        new DialogueLine { speakerType = CharacterType.Classmate, speakerName = "旁邊同學", content = "就直接買一本阿，簡單又省時間，這本以後搞不好還能用。" },
        new DialogueLine { speakerType = CharacterType.Player, speakerName = "主角", content = "可是我只要用這學期啊……" },
        new DialogueLine { speakerType = CharacterType.Classmate, speakerName = "旁邊同學", content = "不然你去看看有沒有人賣二手的咧？" },
        new DialogueLine { speakerType = CharacterType.Player, speakerName = "主角", content = "我剛剛看過了...二手的也要五百塊..." },
        new DialogueLine { speakerType = CharacterType.Player, speakerName = "主角", content = "我的錢包已經只剩五百了啦！！……這下真的沒了" },
        new DialogueLine { speakerType = CharacterType.Bird, speakerName = "鳥鳥", content = "主人，你是不是遇到麻煩了？" },
        new DialogueLine { speakerType = CharacterType.Player, speakerName = "主角", content = "嗯……我需要這本書，但我根本買不起新的。" },
        new DialogueLine { speakerType = CharacterType.Bird, speakerName = "鳥鳥", content = "別緊張！我們去找找看還有沒有別的方法！" },
        new DialogueLine { speakerType = CharacterType.System, speakerName = "系統", content = "前往走廊尋找其他方法吧！" }
    };

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        // 初始隱藏所有對話面板
        if (characterDialoguePanel != null) characterDialoguePanel.SetActive(false);
        if (birdDialoguePanel != null) birdDialoguePanel.SetActive(false);
        if (systemPanel != null) systemPanel.SetActive(false);

        if (bellImage != null) bellImage.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (playerNameText != null)
        {
            playerNameText.text = GameData.PlayerName;
        }

        // 綁定點擊前進按鈕
        if (characterNextBtn != null)
        {
            characterNextBtn.onClick.RemoveAllListeners();
            characterNextBtn.onClick.AddListener(OnAdvanceDialogue);
        }

        if (birdNextBtn != null)
        {
            birdNextBtn.onClick.RemoveAllListeners();
            birdNextBtn.onClick.AddListener(OnAdvanceDialogue);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(EnterNextScene);
        }

        StartCoroutine(PlayBellSequence());
    }

    private IEnumerator PlayBellSequence()
    {
        float soundDuration = bellSoundClip != null ? bellSoundClip.length : 2f;

        if (bellSoundClip != null)
        {
            audioSource.clip = bellSoundClip;
            audioSource.Play();
        }

        float elapsedTime = 0f;
        Quaternion initialRotation = bellImage != null ? bellImage.localRotation : Quaternion.identity;

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

        // 鈴聲結束，啟動第 1 句對話
        currentDialogueIndex = 0;
        DisplayCurrentDialogue();
    }

    private void DisplayCurrentDialogue()
    {
        if (currentDialogueIndex >= storyDialogues.Length) return;

        DialogueLine line = storyDialogues[currentDialogueIndex];

        // 隱藏所有面板後再依類型開啟
        if (characterDialoguePanel != null) characterDialoguePanel.SetActive(false);
        if (birdDialoguePanel != null) birdDialoguePanel.SetActive(false);
        if (systemPanel != null) systemPanel.SetActive(false);

        switch (line.speakerType)
        {
            case CharacterType.Professor:
            case CharacterType.Player:
            case CharacterType.Classmate:
                ShowCharacterDialogue(line);
                break;

            case CharacterType.Bird:
                if (birdDialoguePanel != null)
                {
                    birdDialoguePanel.SetActive(true);
                    if (birdDialogueText != null) birdDialogueText.text = line.content;
                }
                break;

            case CharacterType.System:
                if (systemPanel != null)
                {
                    systemPanel.SetActive(true);
                    if (systemText != null) systemText.text = line.content;
                }
                break;
        }
    }

    private void ShowCharacterDialogue(DialogueLine line)
    {
        if (characterDialoguePanel == null) return;
        characterDialoguePanel.SetActive(true);

        if (characterContentText != null) characterContentText.text = line.content;

        // 設定名稱
        if (characterNameText != null)
        {
            characterNameText.text = (line.speakerType == CharacterType.Player) 
                ? (string.IsNullOrEmpty(GameData.PlayerName) ? "主角" : GameData.PlayerName) 
                : line.speakerName;
        }

        // 動態更換頭像 Sprite
        if (characterHeadImage != null)
        {
            if (line.speakerType == CharacterType.Professor && professorSprite != null)
            {
                characterHeadImage.sprite = professorSprite;
                characterHeadImage.gameObject.SetActive(true);
            }
            else if (line.speakerType == CharacterType.Player && playerSprite != null)
            {
                characterHeadImage.sprite = playerSprite;
                characterHeadImage.gameObject.SetActive(true);
            }
            else if (line.speakerType == CharacterType.Classmate && classmateSprite != null)
            {
                characterHeadImage.sprite = classmateSprite;
                characterHeadImage.gameObject.SetActive(true);
            }
        }
    }

    public void OnAdvanceDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < storyDialogues.Length)
        {
            DisplayCurrentDialogue();
        }
    }

    public void EnterNextScene()
    {
        // ⭐ 確保從教室進入攤位時，絕對是從第一階段 (Phase 0：初見攤位) 開始
        Case2State.StallPhase = 0;

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.StartTransitionAndLoadScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}