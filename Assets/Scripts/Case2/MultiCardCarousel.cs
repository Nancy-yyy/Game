using UnityEngine;
using System.Collections;

public class MultiCardCarousel : MonoBehaviour
{
    [Header("滑動設定")]
    public RectTransform container; // 拖入你的 SliderContainer
    public float spacing = 1500f;   // 卡片之間的距離 (你的設定是 1500)
    public float slideDuration = 0.5f; // 滑動花費的時間 (秒)
    public int totalCards = 4;      // 總共有幾張卡片

    private int currentIndex = 0;   // 目前顯示第幾張 (從 0 開始算)
    private bool isSliding = false; // 防呆機制：防止玩家瘋狂連點按鈕

    // 給按鈕呼叫的函數
    public void SlideToNextCard()
    {
        // 如果還沒滑到最後一張，且目前沒有在滑動中
        if (currentIndex < totalCards - 1 && !isSliding)
        {
            currentIndex++;
            StartCoroutine(SlideRoutine());
        }
        else if (currentIndex >= totalCards - 1)
        {
            Debug.Log("已經是最後一張卡片了，可以切換場景或結束對話！");
            // 你可以把之後「完成任務」或「切換場景」的程式碼寫在這裡
        }
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true; // 鎖住按鈕
        
        Vector2 startPos = container.anchoredPosition;
        // 計算目標位置：目前進度 * (-1500)
        Vector2 targetPos = new Vector2(-currentIndex * spacing, startPos.y);
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            // Lerp 做出平滑的滑動過場
            container.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / slideDuration);
            yield return null;
        }
        
        container.anchoredPosition = targetPos; // 確保精準對位
        isSliding = false; // 解鎖按鈕
    }
}