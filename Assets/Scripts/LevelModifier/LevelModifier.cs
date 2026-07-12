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


/*using UnityEngine;

public enum LevelModifierType
{
    None,

    // pieces kommen ofter
    PieceCategoryBias,

    // nur bestimmte pieces
    OnlyPieceCategories,

    // piece ändern sich schneller
    FasterPieceSwap,

    // fixed pieces am Anfang
    FixedStartingPieces,

    // pieces nicht mehr entfernbar
    LockedPlacedPieces
}

[System.Serializable]
public class FixedPieceSetup
{
    public Piece piecePrefab;
    public Vector2Int position;
}

[CreateAssetMenu(menuName = "Battle Puzzlers/Level Modifier")]
public class LevelModifier : ScriptableObject
{
    [Header("General")]
    public string displayName;

    [TextArea(2, 4)]
    public string description;

    public LevelModifierType type;

    [Header("Piece Categories")]
    public PieceCategory[] preferredCategories;

    [Min(1)]
    public int categoryMultiplier = 4;

    [Header("Piece Swap")]
    [Min(0.1f)]
    public float swapTime = 10f;

    [Header("Fixed Starting Pieces")]
    public FixedPieceSetup[] fixedStartingPieces;

    [Header("Locked Placed Pieces")]
    public bool canMovePlacedPieces = true;
}*/