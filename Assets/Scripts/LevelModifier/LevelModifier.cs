using UnityEngine;

public enum LevelModifierType
{
    None,
    PieceCategoryBias,
    FasterPieceSwap,
    FixedStartingPiece
}

[CreateAssetMenu(menuName = "Battle Puzzlers/Level Modifier")]
public class LevelModifier : ScriptableObject
{
    public string displayName;
    public LevelModifierType type;

    [Header("Piece Category Bias")]
    public PieceCategory[] preferredCategories;
    public int categoryMultiplier = 4;

    [Header("Piece Swap")]
    public float swapTime = 10f;

    [Header("Fixed Starting Piece")]
    public Piece fixedPiecePrefab;
    public Vector2Int fixedPiecePosition;
}