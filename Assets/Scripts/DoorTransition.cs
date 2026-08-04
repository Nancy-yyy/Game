using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI; // 為了讓你換圖片加的

public class DoorTransition : MonoBehaviour
{
    [Header("UI 設定")]
    public GameObject storyPromptUI;  
    public GameObject transitionScreen; 
    public Image transitionImage; // 新增：讓你放過場圖片的元件

    [Header("場景設定")]
    public string nextSceneName = "classroom"; 

    private bool isDoorUnlocked = false; 

    public void TriggerStoryEvent()
    {
        isDoorUnlocked = true;          
        storyPromptUI.SetActive(true);  
        Debug.Log("按鈕已按下！門已解鎖。");
    }

    

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 只要有「任何東西」碰到這扇門的感應區，控制台就會瘋狂印出這行字！
        Debug.Log("【物理測試】有東西在門裡面！它的名字是：[" + collision.gameObject.name + "]");
        
        CheckAndTransition(collision.gameObject);
    }

    private void CheckAndTransition(GameObject target)
    {
        if (target.name == "Player" && isDoorUnlocked)
        {
            isDoorUnlocked = false; // 鎖住，避免重複執行
            Debug.Log("開始執行過場動畫並準備換場景！");
            
            // ⚠️ 關鍵：把這扇門標記為「不要在換場景時刪除」，這樣腳本就能平安執行到底
            DontDestroyOnLoad(this.gameObject); 
            
            StartCoroutine(PlayTransitionAndLoadScene(target));
        }
    }

    IEnumerator PlayTransitionAndLoadScene(GameObject player)
    {
        // 1. 關閉提示
        if (storyPromptUI != null) storyPromptUI.SetActive(false);
        
        // 2. 顯示過場圖片 (UI 打開)
        if (transitionScreen != null) transitionScreen.SetActive(true);
        
        // 3. 凍結玩家
        if (player != null) player.GetComponent<PlayerMovement>().enabled = false;
        
        // 4. 確實等待 2 秒鐘讓你看看圖片
        yield return new WaitForSeconds(2f);
        
        // 5. 確保要載入的場景不是空白，才開始載入
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("正在載入新場景：" + nextSceneName);
            // ⚠️ 關鍵：使用穩定的非同步載入
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            
            // 等待直到新場景完全讀取完畢
            while (!asyncLoad.isDone)
            {
                yield return null; 
            }
            Debug.Log("場景載入完成！");
        }
        else
        {
            Debug.LogError("請在 Inspector 面板中填寫下一個場景的名稱！");
        }

        // 6. 換完場景後，這個門的任務就結束了，可以把自己刪掉
        Destroy(this.gameObject);
    }
}