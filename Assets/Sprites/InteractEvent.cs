using UnityEngine;

public class InteractEvent : MonoBehaviour
{
    public GameObject dialogueUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player") dialogueUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player") dialogueUI.SetActive(false);
    }
}