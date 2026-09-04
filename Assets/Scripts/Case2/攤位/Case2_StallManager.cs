using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Case2_StallManager : MonoBehaviour
{
    public enum SpeakerType
    {
        Character, // 主角 / 學長
        Bird,      // 鳥鳥
        System     // 系統
    }

    public enum ActorEmotion
    {
        Normal,   // 正常
        Happy,    // 開心 (actor_happy)
        Thinking, // 疑惑/思考 (actor_thinking)
        Senior    // 學長
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public SpeakerType speakerType;
        public string speakerName;
        [TextArea(2, 4)] public string content;
        public ActorEmotion emotion = ActorEmotion.Normal;
    }

    [Header("【主角/角色對話框面板】")]
    public GameObject characterPanel;          
    public GameObject characterBoxAndTextGroup; 
    public TMP_Text characterNameText;         
    public TMP_Text characterContentText;      
    public Button characterClickArea;          

    [Header("【獨立表情物件】")]
    public GameObject headNormalObj;           
    public GameObject headHappyObj;            
    public GameObject headThinkingObj;         
    public GameObject headSeniorObj;           

    [Header("【鳥鳥專用面板】")]
    public GameObject birdPanel;               
    public TMP_Text birdContentText;           
    public Button birdClickArea;               

    [Header("【系統專用面板】")]
    public GameObject systemPanel;             
    public TMP_Text systemContentText;         
    public Button continueButton;              

    [Header("【場景立繪】")]
    public GameObject seniorStandee;           

    private int currentDialogueIndex = 0;
    private DialogueEntry[] activeDialogues;

    // 【第三幕：初見攤位】
    private readonly DialogueEntry[] phase0_Dialogues = new DialogueEntry[]
    {
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "主人主人！你看那裏！" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "怎麼那麼多人啊？二手書拍賣...ㄟㄟㄟ你等等我！", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "再等你就要買不到書了啦！快跟上！" },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "欸？這兩個攤位都是跟書有關的...但好像不太一樣ㄟ？" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "咦？明明都是一本書，為什麼一個是買，一個是租？", emotion = ActorEmotion.Thinking },
        new DialogueEntry { speakerType = SpeakerType.System, speakerName = "系統", content = "先來想想看，你真正取得的是什麼呢？" }
    };

    // 【第四幕：學長登場】
    private readonly DialogueEntry[] phase1_Dialogues = new DialogueEntry[]
    {
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "學長", content = "欸學弟！你還記得我嗎？", emotion = ActorEmotion.Senior },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "你是...？啊！是早上幫我指路的學長！謝謝你早上幫我！不然我就要遲到了！", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "學長", content = "小事啦小事！啊你怎麼在這？要買書喔？", emotion = ActorEmotion.Senior },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "對啊，我在找有沒有《管理資訊系統》，大學的書都好貴...", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "學長", content = "喔...我好像沒有這本欸，拍謝幫不了你。但我經濟學的書好像還沒丟，你需要嗎？", emotion = ActorEmotion.Senior },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "太好了我也需要！但是我能夠下個月再給你錢嗎？", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "學長", content = "不用啦，反正我現在也用不到了，放著也是放著，你直接拿吧！", emotion = ActorEmotion.Senior },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "真的嗎？那我就不客氣啦！學長再見！", emotion = ActorEmotion.Happy },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "學長人真好！" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "是啊！可是...學長明明已經用不到這本書了，但這本書還好好的，如果我沒有剛好需要，它是不是就會一直放在那裡？", emotion = ActorEmotion.Thinking },
        new DialogueEntry { speakerType = SpeakerType.System, speakerName = "系統", content = "你剛剛注意到了一個很重要的地方歐！" }
    };

    // 【第六幕：前往天平前】
    private readonly DialogueEntry[] phase2_Dialogues = new DialogueEntry[]
    {
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "原來是這樣...這就是共享經濟呢！", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "呼...幸好還有這麼多方式可以拿到需要的書！" },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "那主人你最後打算怎麼獲得《管理資訊系統》這本書咧？" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "呃...完蛋，結果最重要的書這兩個攤位都沒賣！！怎麼辦！！", emotion = ActorEmotion.Thinking },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "我想到了！我之前聽過一個平台叫做『書人不輸陣』！" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "蛤？阿你怎麼現在才講？", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "聰明的人是不會在意這些小細節的！好啦我們快點來看看吧！" }
    };

    // ⭐【第七幕：天平完成後的最終感悟結尾】
    private readonly DialogueEntry[] phase3_Dialogues = new DialogueEntry[]
    {
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "我好像終於知道剛才那本書為什麼值得被重新利用了。", emotion = ActorEmotion.Thinking },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "我不是缺一個『屬於我的東西』。", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "我是缺一個『這學期可以用的東西』。", emotion = ActorEmotion.Normal },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "而且……租借只是取得使用權的一種方式。", emotion = ActorEmotion.Thinking },
        new DialogueEntry { speakerType = SpeakerType.System, speakerName = "系統", content = "你已經從『我要買一本書』，進一步理解了『一項已存在的資產如何被重新利用』。" },
        new DialogueEntry { speakerType = SpeakerType.Bird, speakerName = "鳥鳥", content = "哇...主人你好像有點變聰明了ㄟ" },
        new DialogueEntry { speakerType = SpeakerType.Character, speakerName = "主角", content = "什麼叫作有點？？而且我本來就很聰明好嘛！！", emotion = ActorEmotion.Happy }
    };

    void Start()
    {
        // ⭐ 加上嚴格判定，若數值異常或小於等於 0，一律強制走 phase0_Dialogues
        if (Case2State.StallPhase == 1)
        {
            activeDialogues = phase1_Dialogues;
        }
        else if (Case2State.StallPhase == 2)
        {
            activeDialogues = phase2_Dialogues;
        }
        else if (Case2State.StallPhase == 3)
        {
            activeDialogues = phase3_Dialogues;
        }
        else
        {
            Case2State.StallPhase = 0;
            activeDialogues = phase0_Dialogues;
        }

        if (seniorStandee != null)
        {
            seniorStandee.SetActive(Case2State.StallPhase == 1);
        }

        // 後續原本的按鈕綁定邏輯維持不變 ...


        if (characterClickArea != null)
        {
            characterClickArea.onClick.RemoveAllListeners();
            characterClickArea.onClick.AddListener(OnNextDialogue);
        }

        if (birdClickArea != null)
        {
            birdClickArea.onClick.RemoveAllListeners();
            birdClickArea.onClick.AddListener(OnNextDialogue);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnSystemContinueClicked);
        }

        currentDialogueIndex = 0;
        DisplayCurrentDialogue();
    }

    private void DisplayCurrentDialogue()
    {
        if (activeDialogues == null || currentDialogueIndex >= activeDialogues.Length) return;

        DialogueEntry entry = activeDialogues[currentDialogueIndex];

        if (birdPanel != null) birdPanel.SetActive(false);
        if (systemPanel != null) systemPanel.SetActive(false);

        switch (entry.speakerType)
        {
            case SpeakerType.Character:
                if (characterPanel != null) characterPanel.SetActive(true);
                if (characterBoxAndTextGroup != null) characterBoxAndTextGroup.SetActive(true);
                if (characterNameText != null) characterNameText.gameObject.SetActive(true);
                if (characterContentText != null) characterContentText.gameObject.SetActive(true);
                
                if (characterContentText != null) characterContentText.text = entry.content;
                if (characterNameText != null)
                {
                    characterNameText.text = (entry.speakerName == "主角" && !string.IsNullOrEmpty(GameData.PlayerName)) 
                        ? GameData.PlayerName 
                        : entry.speakerName;
                }

                SwitchEmotionObject(entry.emotion);
                break;

            case SpeakerType.Bird:
                if (characterPanel != null) characterPanel.SetActive(true);
                if (characterBoxAndTextGroup != null) characterBoxAndTextGroup.SetActive(false);
                if (characterNameText != null) characterNameText.gameObject.SetActive(false);
                if (characterContentText != null) characterContentText.gameObject.SetActive(false);

                SwitchEmotionObject(ActorEmotion.Normal);

                if (birdPanel != null)
                {
                    birdPanel.SetActive(true);
                    if (birdContentText != null) birdContentText.text = entry.content;
                }
                break;

            case SpeakerType.System:
                if (characterPanel != null) characterPanel.SetActive(false);
                if (birdPanel != null) birdPanel.SetActive(false);

                if (systemPanel != null)
                {
                    systemPanel.SetActive(true);
                    if (systemContentText != null) systemContentText.text = entry.content;
                }
                break;
        }
    }

    private void SwitchEmotionObject(ActorEmotion emotion)
    {
        if (headNormalObj != null) headNormalObj.SetActive(emotion == ActorEmotion.Normal);
        if (headHappyObj != null) headHappyObj.SetActive(emotion == ActorEmotion.Happy);
        if (headThinkingObj != null) headThinkingObj.SetActive(emotion == ActorEmotion.Thinking);
        if (headSeniorObj != null) headSeniorObj.SetActive(emotion == ActorEmotion.Senior);
    }

    public void OnNextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < activeDialogues.Length)
        {
            DisplayCurrentDialogue();
        }
        else
        {
            TransitionToNextTarget();
        }
    }

    public void OnSystemContinueClicked()
    {
        // 系統框按下後，若還有下一句繼續顯示，否則換場
        OnNextDialogue();
    }

    private void TransitionToNextTarget()
    {
        if (Case2State.StallPhase == 0)
        {
            Case2State.TeachStartPhase = 0;
            LoadTargetScene("Case2_04_SysTeach");
        }
        else if (Case2State.StallPhase == 1)
        {
            Case2State.TeachStartPhase = 1;
            LoadTargetScene("Case2_04_SysTeach");
        }
        else if (Case2State.StallPhase == 2)
        {
            LoadTargetScene("Case2_05_ScaleGame");
        }
        else if (Case2State.StallPhase == 3)
        {
            // ⭐ Case 2 圓滿完結！可在此載入大地圖或結算畫面
            Debug.Log("🎉 Case 2 劇情全數完成！");
            // LoadTargetScene("MapScene"); // 若有後續場景可填寫於此
        }
    }

    private void LoadTargetScene(string sceneName)
    {
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.StartTransitionAndLoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}