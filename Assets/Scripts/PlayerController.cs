using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    bool canMove;
    bool dragging;
    BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        canMove = false;
        dragging = false;
    }

    void Update()
    {
        if (!IsOwner) return;
        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Mouse button down
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Physics2D.OverlapPoint(mousePos) == boxCollider)
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }

            if (canMove)
            {
                dragging = true;
            }
        }

        // Drag movement
        if (dragging)
        {
            transform.position = mousePos;
        }

        // Mouse button up
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            canMove = false;
            dragging = false;
        }
    }
}