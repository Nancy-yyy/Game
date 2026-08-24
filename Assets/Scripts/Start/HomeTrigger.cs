using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("碰到 HomeTrigger：" + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家到家！");
            SceneManager.LoadScene("PrologueHome");
        }
    }
}