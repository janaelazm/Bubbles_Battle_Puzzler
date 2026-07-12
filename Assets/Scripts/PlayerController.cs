using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    private bool canMove;
    private bool dragging;
    private bool hasBeenPlaced;

    private BoxCollider2D boxCollider;

    [Header("Perk rules")]
    [SerializeField]
    private bool canMovePlacedPiece = true;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        canMove = false;
        dragging = false;
        hasBeenPlaced = false;
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Clic links
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            bool clickedOnThisPiece =
                Physics2D.OverlapPoint(mousePos) == boxCollider;

            if (!clickedOnThisPiece)
            {
                canMove = false;
                return;
            }


            // Wenn das Teil schon platziert wurde und der Perk es verbietet,
            // es zurückzunehmen, blockieren wir die Bewegung.
            if (hasBeenPlaced && !canMovePlacedPiece)
            {
                Debug.Log("Diese Figur kann nicht mehr bewegt werden.");
                canMove = false;
                dragging = false;
                return;
            }

            canMove = true;
            dragging = true;
        }

        // Bewegung
        if (dragging)
        {
            transform.position = mousePos;
        }

        // Loslassen des clic
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (dragging)
            {
                hasBeenPlaced = true;
            }

            canMove = false;
            dragging = false;
        }
    }

    public void SetCanMovePlacedPiece(bool allowed)
    {
        canMovePlacedPiece = allowed;
    }

    public void ResetPlacement()
    {
        hasBeenPlaced = false;
    }
}