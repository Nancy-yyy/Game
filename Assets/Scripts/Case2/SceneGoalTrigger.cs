using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGoalTrigger : MonoBehaviour
{
    [Header("切換到的目標場景名稱")]
    public string targetSceneName = "classroom02";

    // 當 Player 碰到這個觸發區塊時觸發
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 檢查碰撞到的是不是 Player (你可以為 Player 設定 Tag 叫 Player)
        if (collision.gameObject.CompareTag("Player"))
        {
            // 隱藏 hintPanel 或直接切換場景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
