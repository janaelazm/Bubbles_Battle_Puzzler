using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private BoxCollider2D puzzleArea;

    [Header("Level Node Details")]
    [SerializeField] private int levelID;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;

    private float cellSize;
    private Vector2 gridOrigin;

    private Cell[,] cells;
    private readonly List<Cell> currentHoverCells = new();


    private bool levelCompleted = false;
    private LevelManager levelManager;


    public float CellSize => cellSize;

    private void Awake()
    {
        CalculateGridLayout();

        cells = new Cell[width, height];
        levelManager = FindFirstObjectByType<LevelManager>();
        GenerateGrid();
    }

    private void Start()
    {
        levelID = LevelTransferData.SelectedLevelID;
    }

    private void CalculateGridLayout()
    {
        if (puzzleArea == null)
        {
            Debug.LogError("PuzzleArea wurde im PuzzleGridManager nicht zugewiesen.");
            return;
        }

        Bounds bounds = puzzleArea.bounds;

        float cellWidth = bounds.size.x / width;
        float cellHeight = bounds.size.y / height;

        // Verhindert, dass die Zellen gestreckt werden.
        cellSize = Mathf.Min(cellWidth, cellHeight);

        float gridWidth = width * cellSize;
        float gridHeight = height * cellSize;

        // Position des Mittelpunkts der linken unteren Zelle.
        gridOrigin = new Vector2(
            bounds.center.x - gridWidth / 2f + cellSize / 2f,
            bounds.center.y - gridHeight / 2f + cellSize / 2f
        );
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = GridToWorld(gridPosition);

                GameObject cellObject = Instantiate(
                    cellPrefab,
                    worldPosition,
                    Quaternion.identity,
                    transform
                );

                // Voraussetzung: Das Cell-Prefab ist ursprünglich 1x1 Units groß.
                cellObject.transform.localScale =
                    new Vector3(cellSize, cellSize, 1f);

                Cell cell = cellObject.GetComponent<Cell>();

                if (cell == null)
                {
                    Debug.LogError("Das Cell-Prefab besitzt kein Cell-Script.");
                    return;
                }

                cell.Init(gridPosition);
                cells[x, y] = cell;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(
            (worldPosition.x - gridOrigin.x) / cellSize
        );

        int y = Mathf.RoundToInt(
            (worldPosition.y - gridOrigin.y) / cellSize
        );

        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridOrigin.x + gridPosition.x * cellSize,
            gridOrigin.y + gridPosition.y * cellSize,
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

    public bool CanPlaceAt(Piece piece, Vector2Int gridPosition)
    {
        if (piece == null)
            return false;

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
           

            LevelModifier activeModifier = null;

            if (levelManager != null)
            {
                activeModifier = levelManager.GetActiveModifier();
            }

            if (ScoreManager.Instance == null)
            {
                Debug.LogError("ScoreManager instance not found.");
            }
            else
            {
                LevelDifficulty difficulty =
                    LevelTransferData.SelectedDifficulty;

                int basePoints =
                    ScoreManager.Instance.GetBasePoints(difficulty);

                int perkBonus =
                    ScoreManager.Instance.GetModifierBonus(activeModifier);

                int totalPoints = basePoints + perkBonus;

                ScoreManager.Instance.AddPoints(totalPoints);

                Debug.Log(
                    $"Level complete! " +
                    $"Difficulty: {difficulty}, " +
                    $"Base: {basePoints}, " +
                    $"Perk Bonus: {perkBonus}, " +
                    $"Total: {totalPoints}"
                );
            }


            if (IsComplete())
            {
                FinishLevel();
            }
        }

        return true;
    }

    public void RemovePiece(Piece piece, Vector3 worldPosition)
    {
        Vector2Int origin = WorldToGrid(worldPosition);

        foreach (Vector2Int offset in piece.GetRotatedShape())
        {
            Vector2Int cellPosition = origin + offset;

            if (IsInsideGrid(cellPosition))
            {
                cells[cellPosition.x, cellPosition.y].SetOccupied(false);
            }
        }

        ClearHover();
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
            if (cell != null)
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

        return true;
    }

    public void ApplyModifier(LevelModifier modifier)
    {
        if (modifier == null ||
            modifier.type != LevelModifierType.FixedStartingPiece ||
            modifier.fixedPiecePrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = GridToWorld(modifier.fixedPiecePosition);

        Piece fixedPiece = Instantiate(
            modifier.fixedPiecePrefab,
            spawnPosition,
            Quaternion.identity
        );

        fixedPiece.transform.localScale = Vector3.one * cellSize;

        if (!TryPlacePiece(fixedPiece, spawnPosition))
        {
            Debug.LogWarning("Das feste Startteil konnte nicht platziert werden.");
            Destroy(fixedPiece.gameObject);
        }
    }

    private void AwardLevelPoints()
    {

        LevelModifier activeModifier = null;

        if (levelManager != null)
        {
            activeModifier = levelManager.GetActiveModifier();
        }

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance not found.");
            return;
        }

        LevelDifficulty difficulty = LevelTransferData.SelectedDifficulty;

        int basePoints =
            ScoreManager.Instance.GetBasePoints(difficulty);

        int perkBonus =
            ScoreManager.Instance.GetModifierBonus(activeModifier);

        int totalPoints = basePoints + perkBonus;

        ScoreManager.Instance.AddPoints(totalPoints);

        Debug.Log(
            $"Level complete! Difficulty: {difficulty}, " +
            $"Base: {basePoints}, Perk Bonus: {perkBonus}, " +
            $"Total: {totalPoints}"
        );
    }

    private void CompleteLevel()
    {
        Debug.Log("Level complete!");

        GameStateManager.Instance.CompleteLevel(
            levelID,
            PlayerProfile.Instance.PlayerColor
        );

        SceneManager.LoadScene("PathSelection");
    }

    public void InstantCompleteLevel()
    {
        FinishLevel();
    }

    private void FinishLevel()
    {
        if (levelCompleted)
            return;

       
        levelCompleted = true;

        AwardLevelPoints();
        CompleteLevel();
    }
}