using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    private Piece piece;
    private PuzzleGridManager gridManager;
    private DeleteZone deleteZone;
    private Camera mainCamera;

    private Vector3 offset;
    private Vector3 previousPlacedPosition;

    private bool wasPlacedWhenPickedUp;

    private void Awake()
    {
        piece = GetComponent<Piece>();
        mainCamera = Camera.main;

        gridManager = FindFirstObjectByType<PuzzleGridManager>();
        deleteZone = FindFirstObjectByType<DeleteZone>();
    }

    private void Update()
    {
        if (!piece.IsDragging || piece.IsPlaced)
            return;

        Vector3 mousePosition = GetMouseWorldPosition();

        transform.position = mousePosition + offset;
        gridManager.UpdateHover(piece, transform.position);
    }

    public void BeginDrag()
    {
        if (piece.IsDragging)
            return;

        Debug.Log($"BeginDrag: {name}, IsPlaced: {piece.IsPlaced}");

        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        wasPlacedWhenPickedUp = piece.IsPlaced;

        if (wasPlacedWhenPickedUp)
        {
            previousPlacedPosition = transform.position;

            gridManager.RemovePiece(
                piece,
                previousPlacedPosition
            );

            piece.PickUpFromGrid();
        }
        else
        {
            piece.StartDragging();
        }
    }

    public void EndDrag()
    {
        if (!piece.IsDragging)
            return;

        piece.Release();
        gridManager.ClearHover();

        if (wasPlacedWhenPickedUp)
        {
            HandlePreviouslyPlacedPiece();
        }
        else
        {
            HandleNewPiece();
        }

        wasPlacedWhenPickedUp = false;
    }

    /* private void OnMouseDown()
    {

        Debug.Log($"Angeklickt: {name}, IsPlaced: {piece.IsPlaced}");

        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        wasPlacedWhenPickedUp = piece.IsPlaced;

        if (wasPlacedWhenPickedUp)
        {
            previousPlacedPosition = transform.position;

            gridManager.RemovePiece(
                piece,
                previousPlacedPosition
            );

            piece.PickUpFromGrid();
        }
        else
        {
            piece.StartDragging();
        }
    }

    private void OnMouseUp()
    {
        if (!piece.IsDragging)
            return;

        piece.Release();
        gridManager.ClearHover();

        if (wasPlacedWhenPickedUp)
        {
            HandlePreviouslyPlacedPiece();
        }
        else
        {
            HandleNewPiece();
        }

        wasPlacedWhenPickedUp = false;
    } */

    private void HandlePreviouslyPlacedPiece()
    {
        if (IsPointerOverDeleteZone())
        {
            Destroy(gameObject);
            return;
        }

        // Das Piece darf nicht an eine andere Stelle verschoben werden.
        bool returnedSuccessfully = gridManager.TryPlacePiece(
            piece,
            previousPlacedPosition
        );

        if (!returnedSuccessfully)
        {
            Debug.LogWarning(
                $"{piece.name} konnte nicht an seine alte Position zurückgesetzt werden."
            );

            transform.position = previousPlacedPosition;
        }
    }

    private void HandleNewPiece()
    {
        bool placedSuccessfully = gridManager.TryPlacePiece(
            piece,
            transform.position
        );

        if (!placedSuccessfully)
        {
            piece.ResetToStart();
        }
    }

    private bool IsPointerOverDeleteZone()
    {
        if (deleteZone == null)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            deleteZone.RectTransform,
            Input.mousePosition,
            mainCamera
        );
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        worldPosition.z = transform.position.z;

        return worldPosition;
    }
}