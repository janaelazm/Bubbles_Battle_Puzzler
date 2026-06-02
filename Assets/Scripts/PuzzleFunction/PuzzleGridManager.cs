using UnityEngine;

public class PuzzleGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 8;
    public int height = 8;
    public float cellSize = 1f;

    [Header("Prefabs")]
    public GameObject cellPrefab;

    private Cell[,] cells;

    private void Awake()
    {
        cells = new Cell[width, height];
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = GridToWorld(new Vector2Int(x, y));

                GameObject cellObject = Instantiate(
                    cellPrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                Cell cell = cellObject.GetComponent<Cell>();
                cell.Init(new Vector2Int(x, y));

                cells[x, y] = cell;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * cellSize,
            gridPosition.y * cellSize,
            -0.1f
        );
    }

    public bool IsInsideGrid(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < width &&
               gridPosition.y >= 0 &&
               gridPosition.y < height;
    }

    public bool CanPlaceAt(Vector2Int gridPosition)
    {
        if (!IsInsideGrid(gridPosition))
            return false;

        return !cells[gridPosition.x, gridPosition.y].IsOccupied;
    }

    public bool TryPlacePiece(Piece piece, Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGrid(worldPosition);

        if (!CanPlaceAt(gridPosition))
            return false;

        Cell cell = cells[gridPosition.x, gridPosition.y];

        cell.SetOccupied(true);
        piece.Place(cell.transform.position);

        return true;
    }

    private Cell currentHoverCell;

    public void UpdateHover(Vector3 worldPosition)
    {
        ClearHover();

        Vector2Int gridPosition = WorldToGrid(worldPosition);

        if (!IsInsideGrid(gridPosition))
            return;

        Cell cell = cells[gridPosition.x, gridPosition.y];

        if (cell.IsOccupied)
            return;

        currentHoverCell = cell;
        currentHoverCell.SetHover(true);
    }

    public void ClearHover()
    {
        if (currentHoverCell == null)
            return;

        currentHoverCell.SetHover(false);
        currentHoverCell = null;
    }
}