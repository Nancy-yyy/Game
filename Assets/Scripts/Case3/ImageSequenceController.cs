using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineChatManager : MonoBehaviour
{
    [Header("依序放入 1, 2, 3 藍色對話圖片物件")]
    public List<GameObject> blueMessages;

    [Header("放入 4 白色主角對話圖片物件")]
    public GameObject whiteMessage4;

    private int currentIndex = 0;
    private bool isWaitingForWhiteMessage = false;

    void Start()
    {
        // 確保所有對話預設隱藏
        foreach (GameObject img in blueMessages)
        {
            if (img != null) img.SetActive(false);
        }

        if (whiteMessage4 != null) whiteMessage4.SetActive(false);
    }

    void Update()
    {
        // 點擊滑鼠左鍵/螢幕，依序跳出 1, 2, 3 對話
        if (Input.GetMouseButtonDown(0) && !isWaitingForWhiteMessage)
        {
            ShowNextBlueMessage();
        }
    }

    void ShowNextBlueMessage()
    {
        if (currentIndex < blueMessages.Count)
        {
            if (blueMessages[currentIndex] != null)
            {
                blueMessages[currentIndex].SetActive(true);
            }
            currentIndex++;

            // 當 1, 2, 3 都顯示完畢後，自動跳出第 4 張白色主角對話
            if (currentIndex >= blueMessages.Count)
            {
                isWaitingForWhiteMessage = true;
                if (whiteMessage4 != null) whiteMessage4.SetActive(true);
            }
        }
    }

    // 當玩家點擊第 4 張白色對話時觸發
    public void OnClickWhiteMessage()
    {
        Debug.Log("點擊第 4 張白色對話，第一幕對話完畢！");
    }
}