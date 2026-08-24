using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScaleNextB_StorySequence : MonoBehaviour
{
    [Header("第一階段：錢包歸零")]
    public GameObject phase1Group;        // Phase1_MoneyZero 群組
    public Transform moneyBag;            // 拖入「錢袋_0」
    public Transform lineTrack;           // 拖入「直線_0」
    public float moveDuration = 1.2f;     // 滑動時間 (秒)
    public float moveLeftDistance = 250f; // 向左移動的像素/單位距離

    [Header("中間提示卡片")]
    public GameObject hintCard;           // 提示卡片 (警告提示_0)
    public float hintCardDuration = 1.5f; // 提示卡片停留秒數

    [Header("第二階段：方案結果")]
    public GameObject phase2ResultCard;   // 方案結果_0

    [Header("第三階段：痛哭與小鳥登場")]
    public GameObject phase3CryEnding;    // Phase3_CryEnding 群組
    public GameObject sadCharacter;       // sad_0 痛哭人物
    public GameObject cryBird;            // 哭哭小鳥_1
    public GameObject birdDialogueBox;    // 小鳥對話框 (Image)
    public TextMeshProUGUI birdText;      // 對話文字 (BubbleText)

    void Start()
    {
        // 初始狀態確保
        if (phase1Group != null) phase1Group.SetActive(true);
        if (hintCard != null) hintCard.SetActive(false);
        if (phase2ResultCard != null) phase2ResultCard.SetActive(false);
        if (phase3CryEnding != null) phase3CryEnding.SetActive(false);

        // 如果父物件有 Layout Group，暫時關閉避免它鎖死錢袋座標
        if (phase1Group != null)
        {
            LayoutGroup layout = phase1Group.GetComponent<LayoutGroup>();
            if (layout != null) layout.enabled = false;
        }

        StartCoroutine(PlayStorySequence());
    }

    private IEnumerator PlayStorySequence()
    {
        Debug.Log("【劇情開始】等待 0.6 秒進場...");
        yield return new WaitForSeconds(0.6f);

        // 錢袋向左移動
        if (moneyBag != null)
        {
            Debug.Log("【錢袋移動中...】");
            RectTransform bagRect = moneyBag.GetComponent<RectTransform>();

            if (bagRect != null) // Canvas UI
            {
                Vector2 startPos = bagRect.anchoredPosition;
                Vector2 targetPos = new Vector2(startPos.x - moveLeftDistance, startPos.y);
                float time = 0;

                while (time < moveDuration)
                {
                    time += Time.deltaTime;
                    bagRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / moveDuration);
                    yield return null;
                }
                bagRect.anchoredPosition = targetPos;
            }
            else // 一般 2D 物件
            {
                Vector3 startPos = moneyBag.localPosition;
                Vector3 targetPos = new Vector3(startPos.x - (moveLeftDistance * 0.01f), startPos.y, startPos.z);
                float time = 0;

                while (time < moveDuration)
                {
                    time += Time.deltaTime;
                    moneyBag.localPosition = Vector3.Lerp(startPos, targetPos, time / moveDuration);
                    yield return null;
                }
                moneyBag.localPosition = targetPos;
            }
        }
        else
        {
            Debug.LogWarning("【警告】未指定 Money Bag 物件！");
        }

        Debug.Log("【錢袋已歸零】等待 0.5 秒...");
        yield return new WaitForSeconds(0.5f);

        // 3. 出現警告提示小卡
        if (hintCard != null)
        {
            Debug.Log("【顯示警告提示卡】");
            hintCard.SetActive(true);
            yield return new WaitForSeconds(hintCardDuration);
            hintCard.SetActive(false);
        }

        // 4. 清空第一階段畫面
        if (phase1Group != null) phase1Group.SetActive(false);

        // 5. 出現方案結果卡片
        if (phase2ResultCard != null)
        {
            Debug.Log("【顯示方案結果卡片】");
            phase2ResultCard.SetActive(true);
        }

        // 6. 停頓 2 秒
        yield return new WaitForSeconds(2.0f);

        // 7. 痛哭人物與小鳥登場
        Debug.Log("【主角痛哭與小鳥登場】");
        if (phase3CryEnding != null) phase3CryEnding.SetActive(true);
        if (sadCharacter != null) sadCharacter.SetActive(true);
        if (cryBird != null) cryBird.SetActive(true);

        if (birdDialogueBox != null)
        {
            birdDialogueBox.SetActive(true);
            if (birdText != null)
            {
                birdText.text = "買完生活費直接歸零，接下來四個月你要啃樹皮了喔！";
            }
        }
    }
}