using UnityEngine;

public class PieceInputManager : MonoBehaviour
{
    [SerializeField] private LayerMask pieceLayer;

    private Camera mainCamera;
    private PieceMovement selectedPiece;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectPiece();
        }

        if (Input.GetMouseButtonUp(0) && selectedPiece != null)
        {
            selectedPiece.EndDrag();
            selectedPiece = null;
        }
    }

    private void TrySelectPiece()
    {
        Vector2 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(
            mouseWorldPosition,
            pieceLayer
        );

        foreach (Collider2D hit in hits)
        {
            PieceMovement movement =
                hit.GetComponentInParent<PieceMovement>();

            if (movement == null)
                continue;

            selectedPiece = movement;
            selectedPiece.BeginDrag();
            return;
        }
    }
}