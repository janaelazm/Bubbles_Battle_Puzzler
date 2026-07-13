using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 8;
    public int height = 8;
    public float cellSize = 1f;
    public Vector2Int gridOffset = new Vector2Int(-4, 0);

    [Header("Level Node Details")]
    public int levelID;


    [Header("Prefabs")]
    public GameObject cellPrefab;

    private Cell[,] cells;
    private readonly List<Cell> currentHoverCells = new();

    [Header("Score Settings")]
    [SerializeField] private int nodeBasePoints = 10;

    private bool levelCompleted = false;
    private LevelManager levelManager;


    private void Awake()
    {
        cells = new Cell[width, height];
        levelManager = FindFirstObjectByType<LevelManager>();
        GenerateGrid();
    }

    void Start()
    {
        levelID = LevelTransferData.SelectedLevelID;
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
        int x = Mathf.RoundToInt(worldPosition.x / cellSize) - gridOffset.x;
        int y = Mathf.RoundToInt(worldPosition.y / cellSize) - gridOffset.y;

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            (gridPosition.x + gridOffset.x) * cellSize,
            (gridPosition.y + gridOffset.y) * cellSize,
            -0.1f
        );
    }

    public void ApplyModifier(LevelModifier modifier)
    {
        if (modifier == null)
            return;

        if (modifier.type == LevelModifierType.FixedStartingPiece)
        {
            TryPlacePiece(
                modifier.fixedPiecePrefab,
                GridToWorld(modifier.fixedPiecePosition)
            );
        }
    }

    public bool IsInsideGrid(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < width &&
               gridPosition.y >= 0 &&
               gridPosition.y < height;
    }

    public bool CanPlaceAt(Piece piece, Vector2Int gridPosition)
    {
        foreach (Vector2Int offset in piece.GetRotatedShape())
        {
            Vector2Int cellPosition = gridPosition + offset;

            if (!IsInsideGrid(cellPosition))
                return false;

            if (cells[cellPosition.x, cellPosition.y].IsOccupied)
                return false;
        }

        return true;
    }

    public bool TryPlacePiece(Piece piece, Vector3 worldPosition)
    {
        
        Vector2Int origin = WorldToGrid(worldPosition);

        if (!CanPlaceAt(piece, origin))
            return false;

        foreach (Vector2Int offset in piece.GetRotatedShape())
        {
            Vector2Int cellPosition = origin + offset;
            cells[cellPosition.x, cellPosition.y].SetOccupied(true);
        }

        Vector3 snapPosition = GridToWorld(origin);
        snapPosition.z = piece.transform.position.z;

        piece.Place(snapPosition);

        if (!levelCompleted && IsComplete())
        {
            levelCompleted = true;

            LevelModifier activeModifier = null;

            if (levelManager != null)
            {
                activeModifier = levelManager.GetActiveModifier();
            }

            int perkBonus = ScoreManager.Instance.GetModifierBonus(activeModifier);
            int totalPoints = nodeBasePoints + perkBonus;


            ScoreManager.Instance.AddPoints(totalPoints);

            Debug.Log($"Level complete! Base: {nodeBasePoints}, Perk Bonus: {perkBonus}, Total: {totalPoints}");
            GameStateManager.Instance.CompleteLevel(levelID, PlayerProfile.Instance.PlayerColor);
            SceneManager.LoadScene("PathSelection");
        }

        return true;
    }

    public void UpdateHover(Piece piece, Vector3 worldPosition)
    {
        ClearHover();

        Vector2Int origin = WorldToGrid(worldPosition);

        if (!CanPlaceAt(piece, origin))
            return;

        foreach (Vector2Int offset in piece.GetRotatedShape())
        {
            Vector2Int cellPosition = origin + offset;
            Cell cell = cells[cellPosition.x, cellPosition.y];

            cell.SetHover(true);
            currentHoverCells.Add(cell);
        }
    }

    public void ClearHover()
    {
        foreach (Cell cell in currentHoverCells)
        {
            cell.SetHover(false);
        }

        currentHoverCells.Clear();
    }

    public bool IsComplete()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!cells[x, y].IsOccupied)
                    return false;
            }
        }

        Debug.Log("Puzzle complete!");
        // Back to Pathchoosing
        return true;
    }

    public void InstantCompleteLevel()
    {
        Debug.Log("Level complete!");
        GameStateManager.Instance.CompleteLevel(levelID, PlayerProfile.Instance.PlayerColor);
        SceneManager.LoadScene("PathSelection");
    }

}