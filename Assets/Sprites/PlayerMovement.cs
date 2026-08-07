using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        movement = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) movement.y = 1;
            if (Keyboard.current.sKey.isPressed) movement.y = -1;
            if (Keyboard.current.aKey.isPressed) movement.x = -1;
            if (Keyboard.current.dKey.isPressed) movement.x = 1;
        }

        if (anim != null)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
            anim.SetFloat("Speed", movement.sqrMagnitude); 
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}