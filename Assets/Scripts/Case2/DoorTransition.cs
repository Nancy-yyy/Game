using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorTransition : MonoBehaviour
{
    [Header("UI 設定")]
    public GameObject storyPromptUI;  
    public GameObject transitionScreen; 

    [Header("場景設定")]
    public string nextSceneName = "classroomScene"; 

    private bool isDoorUnlocked = false; 

    // 按鈕呼叫這個來解鎖
    public void TriggerStoryEvent()
    {
        isDoorUnlocked = true;          
        if (storyPromptUI != null) storyPromptUI.SetActive(true);  
        Debug.Log("按鈕已按下！門已解鎖。");
    }

    // 當人物持續停留在門裡面時
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && isDoorUnlocked)
        {
            isDoorUnlocked = false; // 關閉開關，避免重複觸發
            Debug.Log("物理觸發成功！準備啟動過場並切換場景。");
            
            // 啟動協程開始過場與換場
            StartCoroutine(TransitionRoutine());
        }
    }

    IEnumerator TransitionRoutine()
    {
        // 1. 關閉提示框
        if (storyPromptUI != null) storyPromptUI.SetActive(false);
        
        // 2. 顯示過場黑畫面或圖片
        if (transitionScreen != null) transitionScreen.SetActive(true);
        
        // 3. 毫無保留地等待 2 秒（讓過場畫面停留一下）
        yield return new WaitForSeconds(2f);
        
        // 4. 強制切換場景！
        Debug.Log("正在強制載入新場景：" + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}