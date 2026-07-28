using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("Piece Settings")]
    [SerializeField] private List<Piece> piecePrefabs;

    [Header("River Points")]
    [SerializeField] private Transform riverSpawnPoint;
    [SerializeField] private Transform riverDespawnPoint;

    [Header("River Settings")]
    [SerializeField, Min(1)]
    private int targetPieceCount = 4;

    [SerializeField, Min(0.1f)]
    private float spawnInterval = 1f;

    [SerializeField, Min(0.1f)]
    private float replacementDelay = 0.5f;

    [SerializeField, Min(0.1f)]
    private float riverSpeed = 1.5f;

    [SerializeField, Min(0f)]
    private float verticalSpawnRange = 0.7f;

    [Header("Grid")]
    [SerializeField] private PuzzleGridManager gridManager;

    private readonly List<FloatingPiece> activeRiverPieces = new();

    private float spawnTimer;
    private LevelModifier activeModifier;

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        // Das erste Piece erscheint direkt.
        spawnTimer = 0f;
    }

    private void Update()
    {
        RemoveMissingPieces();

        if (activeRiverPieces.Count >= GetTargetPieceCount())
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnRiverPiece();
            spawnTimer = GetSpawnInterval();
        }
    }

    private void OnDestroy()
    {
        foreach (FloatingPiece floatingPiece in activeRiverPieces)
        {
            UnsubscribeFromPiece(floatingPiece);
        }
    }

    public void SetModifier(LevelModifier modifier)
    {
        activeModifier = modifier;
    }

    private void SpawnRiverPiece()
    {
        Piece prefab = GetRandomPiecePrefab();

        if (prefab == null)
        {
            Debug.LogError(
                "Es konnte kein Piece-Prefab zum Spawnen gefunden werden."
            );

            return;
        }

        Vector3 spawnPosition = riverSpawnPoint.position;

        spawnPosition.y += Random.Range(
            -verticalSpawnRange,
            verticalSpawnRange
        );

        Piece spawnedPiece = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedPiece.transform.localScale =
            Vector3.one * gridManager.CellSize;

        spawnedPiece.SetRandomRotation();

        FloatingPiece floatingPiece =
            spawnedPiece.GetComponent<FloatingPiece>();

        if (floatingPiece == null)
        {
            floatingPiece =
                spawnedPiece.gameObject.AddComponent<FloatingPiece>();
        }

        floatingPiece.Init(
            GetRiverSpeed(),
            riverDespawnPoint.position.x
        );

        floatingPiece.LeftRiver += HandlePieceLeftRiver;
        floatingPiece.TakenFromRiver += HandlePieceTakenFromRiver;

        activeRiverPieces.Add(floatingPiece);
    }

    private void HandlePieceLeftRiver(FloatingPiece floatingPiece)
    {
        RemoveRiverPiece(floatingPiece);
        RequestReplacement();
    }

    private void HandlePieceTakenFromRiver(FloatingPiece floatingPiece)
    {
        RemoveRiverPiece(floatingPiece);
        RequestReplacement();
    }

    private void RemoveRiverPiece(FloatingPiece floatingPiece)
    {
        if (floatingPiece == null)
            return;

        UnsubscribeFromPiece(floatingPiece);
        activeRiverPieces.Remove(floatingPiece);
    }

    private void UnsubscribeFromPiece(FloatingPiece floatingPiece)
    {
        if (floatingPiece == null)
            return;

        floatingPiece.LeftRiver -= HandlePieceLeftRiver;
        floatingPiece.TakenFromRiver -= HandlePieceTakenFromRiver;
    }

    private void RequestReplacement()
    {
        spawnTimer = Mathf.Min(
            spawnTimer,
            GetReplacementDelay()
        );
    }

    private void RemoveMissingPieces()
    {
        for (int i = activeRiverPieces.Count - 1; i >= 0; i--)
        {
            if (activeRiverPieces[i] != null)
                continue;

            activeRiverPieces.RemoveAt(i);
        }
    }

    private Piece GetRandomPiecePrefab()
    {
        if (piecePrefabs == null || piecePrefabs.Count == 0)
            return null;

        int totalWeight = 0;

        foreach (Piece piece in piecePrefabs)
        {
            if (piece == null)
                continue;

            totalWeight += Mathf.Max(
                0,
                GetEffectiveWeight(piece)
            );
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning(
                "Alle Piece-Gewichtungen sind 0. " +
                "Das erste gültige Prefab wird verwendet."
            );

            return GetFirstValidPrefab();
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (Piece piece in piecePrefabs)
        {
            if (piece == null)
                continue;

            randomValue -= Mathf.Max(
                0,
                GetEffectiveWeight(piece)
            );

            if (randomValue < 0)
                return piece;
        }

        return GetFirstValidPrefab();
    }

    private Piece GetFirstValidPrefab()
    {
        foreach (Piece piece in piecePrefabs)
        {
            if (piece != null)
                return piece;
        }

        return null;
    }

    private int GetEffectiveWeight(Piece piece)
    {
        int weight = piece.SpawnWeight;

        if (activeModifier == null ||
            activeModifier.type !=
            LevelModifierType.PieceCategoryBias)
        {
            return weight;
        }

        foreach (
            PieceCategory category
            in activeModifier.preferredCategories
        )
        {
            if (!piece.HasCategory(category))
                continue;

            weight *= activeModifier.categoryMultiplier;
            break;
        }

        return weight;
    }

    private float GetSpawnInterval()
    {
        if (activeModifier != null &&
            activeModifier.type ==
            LevelModifierType.FasterPieceSwap)
        {
            return Mathf.Max(
                0.1f,
                activeModifier.swapTime
            );
        }

        return spawnInterval;
    }

    private float GetReplacementDelay()
    {
        if (activeModifier != null &&
            activeModifier.type ==
            LevelModifierType.FasterPieceSwap)
        {
            return Mathf.Min(
                replacementDelay,
                Mathf.Max(0.1f, activeModifier.swapTime)
            );
        }

        return replacementDelay;
    }

    private float GetRiverSpeed()
    {
        // Der FasterPieceSwap-Modifier lässt hauptsächlich
        // schneller neue Pieces erscheinen. Die Bewegung wird
        // nur leicht beschleunigt, damit Pieces anklickbar bleiben.
        if (activeModifier != null &&
            activeModifier.type ==
            LevelModifierType.FasterPieceSwap)
        {
            return riverSpeed * 1.15f;
        }

        return riverSpeed;
    }

    private int GetTargetPieceCount()
    {
        if (activeModifier != null &&
            activeModifier.type ==
            LevelModifierType.FasterPieceSwap)
        {
            return targetPieceCount + 1;
        }

        return targetPieceCount;
    }

    private bool ValidateReferences()
    {
        bool referencesValid = true;

        if (piecePrefabs == null || piecePrefabs.Count == 0)
        {
            Debug.LogError(
                "Im PieceSpawner wurden keine Piece-Prefabs eingetragen."
            );

            referencesValid = false;
        }

        if (riverSpawnPoint == null)
        {
            Debug.LogError(
                "River Spawn Point wurde nicht zugewiesen."
            );

            referencesValid = false;
        }

        if (riverDespawnPoint == null)
        {
            Debug.LogError(
                "River Despawn Point wurde nicht zugewiesen."
            );

            referencesValid = false;
        }

        if (gridManager == null)
        {
            Debug.LogError(
                "Grid Manager wurde nicht zugewiesen."
            );

            referencesValid = false;
        }

        return referencesValid;
    }
}