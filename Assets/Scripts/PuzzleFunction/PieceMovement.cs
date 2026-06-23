using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    private Piece piece;
    private PuzzleGridManager gridManager;
    private Camera mainCamera;

    private Vector3 offset;

    private void Awake()
    {
        piece = GetComponent<Piece>();
        mainCamera = Camera.main;
        gridManager = FindFirstObjectByType<PuzzleGridManager>();
    }

    private void Update()
    {
        if (!piece.IsDragging || piece.IsPlaced)
            return;

        Vector3 mousePosition = GetMouseWorldPosition();
        transform.position = mousePosition + offset;
        gridManager.UpdateHover(piece, transform.position);
    }

    private void OnMouseDown()
    {
        if (piece.IsPlaced)
            return;

        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        piece.StartDragging();
    }

    private void OnMouseUp()
    {
        if (piece.IsPlaced)
            return;

        piece.Release();
        gridManager.ClearHover();

        bool placedSuccessfully = gridManager.TryPlacePiece(piece, transform.position);

        if (!placedSuccessfully)
        {
            piece.ResetToStart();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = transform.position.z;

        return worldPosition;
    }
}