using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ScaleNextB_Controller : MonoBehaviour
{
    [Header("【第一階段：確認書籍與兩顆按鈕】")]
    public GameObject bookInfoPanel;      // 拖入 BookInfoPanel
    public Button backToScaleBtn;         // 拖入「重新選擇」按鈕
    public Button confirmBuyBtn;          // 拖入「確認購買」按鈕
    public string previousSceneName = "Case2_GameScene02"; // 回天平場景名稱
    public SceneTransition sceneTransition; // 拖入 TransitionCanvas 腳本

    [Header("【第二階段：錢袋歸零與數字倒數】")]
    public GameObject storyDialoguePanel; // 拖入 StoryDialoguePanel
    public GameObject phase1MoneyGroup;   // 拖入 Phase1_MoneyZero
    public Transform moneyBag;            // 拖入 錢袋_0
    public TextMeshProUGUI walletBalanceText; // 拖入 WalletBalanceText
    public float moveLeftDistance = 3.0f; // 移動距離
    public float moveDuration = 1.2f;     // 滑動與倒數時間 (秒)

    [Header("【第三階段：警告提示小卡】")]
    public GameObject hintCard;           // 拖入 警告提示_0

    [Header("【第四階段：方案結果卡片 (從上方滑入)】")]
    public GameObject phase2ResultCard;   // 方案結果_0
    public float resultSlideDuration = 0.6f;

    [Header("【第五階段：痛哭與鳥鳥吐槽】")]
    public GameObject phase3CryEnding;    // Phase3_CryEnding
    public GameObject sadCharacter;       // sad_0 痛哭人物
    public GameObject cryBird;            // 哭哭小鳥_1
    public GameObject birdDialogueBox;    // Image (對話氣泡)
    public TextMeshProUGUI birdText;      // BubbleText
    public float birdDialogueInterval = 2.5f; // 兩句對話間隔 (秒)

    [Header("【第六階段：時光倒流機會按鈕】")]
    public Button timeRewindBtn;          // 拖入「時光倒流」按鈕
    public float delayBeforeRewindBtn = 3.0f; // 第二句話講完後等待 3 秒才出現按鈕

    void Start()
    {
        if (bookInfoPanel != null) bookInfoPanel.SetActive(true);
        if (storyDialoguePanel != null) storyDialoguePanel.SetActive(false);
        if (hintCard != null) hintCard.SetActive(false);
        if (phase2ResultCard != null) phase2ResultCard.SetActive(false);
        if (phase3CryEnding != null) phase3CryEnding.SetActive(false);
        if (timeRewindBtn != null) timeRewindBtn.gameObject.SetActive(false);

        if (walletBalanceText != null)
        {
            walletBalanceText.text = "【您的錢包餘額：500.00$】";
        }

        if (sceneTransition == null) sceneTransition = FindObjectOfType<SceneTransition>();

        // 綁定按鈕點擊事件
        if (backToScaleBtn != null)
        {
            backToScaleBtn.onClick.RemoveAllListeners();
            backToScaleBtn.onClick.AddListener(OnBackToScaleClicked);
        }

        if (confirmBuyBtn != null)
        {
            confirmBuyBtn.onClick.RemoveAllListeners();
            confirmBuyBtn.onClick.AddListener(OnConfirmBuyClicked);
        }

        if (timeRewindBtn != null)
        {
            timeRewindBtn.onClick.RemoveAllListeners();
            timeRewindBtn.onClick.AddListener(OnTimeRewindClicked);
        }
    }

    public void OnBackToScaleClicked()
    {
        ReturnToScaleScene();
    }

    // 點擊「時光倒流」按鈕 -> 播放 TransitionCanvas 轉場滑入並返回天平場景
    public void OnTimeRewindClicked()
    {
        ReturnToScaleScene();
    }

    private void ReturnToScaleScene()
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartTransitionAndLoadScene(previousSceneName);
        }
        else
        {
            SceneManager.LoadScene(previousSceneName);
        }
    }

    public void OnConfirmBuyClicked()
    {
        if (bookInfoPanel != null) bookInfoPanel.SetActive(false);
        if (storyDialoguePanel != null) storyDialoguePanel.SetActive(true);

        StartCoroutine(ExecuteStorySequence());
    }

    private IEnumerator ExecuteStorySequence()
    {
        if (phase1MoneyGroup != null) phase1MoneyGroup.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        // 1. 錢袋向左滑動 + 錢包餘額倒數
        if (moneyBag != null)
        {
            RectTransform bagRect = moneyBag.GetComponent<RectTransform>();
            float time = 0;
            float startMoney = 500.00f;
            float targetMoney = 0.00f;

            if (bagRect != null)
            {
                Vector2 startPos = bagRect.anchoredPosition;
                Vector2 targetPos = new Vector2(startPos.x - (moveLeftDistance * 100f), startPos.y);

                while (time < moveDuration)
                {
                    time += Time.deltaTime;
                    float progress = time / moveDuration;

                    bagRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, progress);

                    float currentMoney = Mathf.Lerp(startMoney, targetMoney, progress);
                    if (walletBalanceText != null)
                    {
                        walletBalanceText.text = $"【您的錢包餘額：{currentMoney:F2}$】";
                    }

                    yield return null;
                }
                bagRect.anchoredPosition = targetPos;
            }
            else
            {
                Vector3 startPos = moneyBag.localPosition;
                Vector3 targetPos = new Vector3(startPos.x - moveLeftDistance, startPos.y, startPos.z);

                while (time < moveDuration)
                {
                    time += Time.deltaTime;
                    float progress = time / moveDuration;

                    moneyBag.localPosition = Vector3.Lerp(startPos, targetPos, progress);

                    float currentMoney = Mathf.Lerp(startMoney, targetMoney, progress);
                    if (walletBalanceText != null)
                    {
                        walletBalanceText.text = $"您的錢包餘額：{currentMoney:F2}$";
                    }

                    yield return null;
                }
                moneyBag.localPosition = targetPos;
            }

            if (walletBalanceText != null)
            {
                walletBalanceText.text = "您的錢包餘額：0.00$";
            }
        }

        yield return new WaitForSeconds(0.4f);

        // 2. 警告提示卡閃爍 (採用你自訂的閃爍時間)
        if (hintCard != null)
        {
            yield return StartCoroutine(BlinkHintCard(hintCard, 2));
            hintCard.SetActive(false);
        }

        // 3. 清空第一階段畫面
        if (phase1MoneyGroup != null) phase1MoneyGroup.SetActive(false);

        // 4. 方案結果卡片從上方滑入
        if (phase2ResultCard != null)
        {
            phase2ResultCard.SetActive(true);
            yield return StartCoroutine(SlideInFromTop(phase2ResultCard, resultSlideDuration));
        }

        // 5. 停頓 2 秒
        yield return new WaitForSeconds(2.0f);

        // 6. 出現痛哭人物與小鳥登場
        if (phase3CryEnding != null) phase3CryEnding.SetActive(true);
        if (sadCharacter != null) sadCharacter.SetActive(true);
        if (cryBird != null) cryBird.SetActive(true);

        // 7. 小鳥依序說出兩句對話
        if (birdDialogueBox != null && birdText != null)
        {
            birdDialogueBox.SetActive(true);

            // 第一句對話
            birdText.text = "嗚嗚嗚……主人，我們的錢包空空了……";
            LayoutRebuilder.ForceRebuildLayoutImmediate(birdDialogueBox.GetComponent<RectTransform>());

            yield return new WaitForSeconds(birdDialogueInterval);

            // 第二句對話
            birdText.text = "接下來四個月要怎麼辦呀……";
            LayoutRebuilder.ForceRebuildLayoutImmediate(birdDialogueBox.GetComponent<RectTransform>());

            // 說完第二句話後，精確等待 3 秒鐘
            yield return new WaitForSeconds(delayBeforeRewindBtn);
        }

        // 8. 經過 3 秒後，顯示「時光倒流」按鈕
        if (timeRewindBtn != null)
        {
            timeRewindBtn.gameObject.SetActive(true);
        }
    }

    // 依據你的修改：亮 0.6s / 滅 0.4s，閃完亮起展示 3s
    private IEnumerator BlinkHintCard(GameObject card, int blinkCount)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            card.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            card.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }
        card.SetActive(true);
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator SlideInFromTop(GameObject targetObj, float duration)
    {
        RectTransform rect = targetObj.GetComponent<RectTransform>();

        if (rect != null)
        {
            Vector2 endPos = rect.anchoredPosition;
            Vector2 startPos = new Vector2(endPos.x, endPos.y + 1000f);
            rect.anchoredPosition = startPos;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, time / duration);
                yield return null;
            }
            rect.anchoredPosition = endPos;
        }
        else
        {
            Vector3 endPos = targetObj.transform.localPosition;
            Vector3 startPos = new Vector3(endPos.x, endPos.y + 8f, endPos.z);
            targetObj.transform.localPosition = startPos;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                targetObj.transform.localPosition = Vector3.Lerp(startPos, endPos, time / duration);
                yield return null;
            }
            targetObj.transform.localPosition = endPos;
        }
    }
}