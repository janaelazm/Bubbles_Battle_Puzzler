using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("Piece Settings")]
    [SerializeField] private List<Piece> piecePrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Timing")]
    [SerializeField] private float respawnTime = 10f;

    private readonly List<Piece> currentPieces = new();
    private float timer;

    private LevelModifier activeModifier;

    public void SetModifier(LevelModifier modifier)
    {
        activeModifier = modifier;
    }

    private float GetRespawnTime()
    {
        if (activeModifier != null &&
            activeModifier.type == LevelModifierType.FasterPieceSwap)
        {
            return activeModifier.swapTime;
        }

        return respawnTime;
    }

    private void Start()
    {
        SpawnNewPieces();
        timer = GetRespawnTime();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            RemoveUnplacedPieces();
            SpawnNewPieces();
            timer = GetRespawnTime();
        }
        else if (AreAllCurrentPiecesPlaced())
        {
            SpawnNewPieces();
            timer = respawnTime;
        }
    }

    private void SpawnNewPieces()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Piece prefab = GetRandomPiecePrefab();

            Piece spawnedPiece = Instantiate(
                prefab,
                spawnPoints[i].position,
                Quaternion.identity
            );

            spawnedPiece.SetRandomRotation();
            // Debug.Log($"{spawnedPiece.name} rotation: {spawnedPiece.transform.eulerAngles.z}");
            currentPieces.Add(spawnedPiece);
        }
    }

    private Piece GetRandomPiecePrefab()
    {
        int totalWeight = 0;

        foreach (Piece piece in piecePrefabs)
        {
            totalWeight += GetEffectiveWeight(piece);
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (Piece piece in piecePrefabs)
        {
            randomValue -= GetEffectiveWeight(piece);

            if (randomValue < 0)
                return piece;
        }

        return piecePrefabs[0];
    }
    /**
    * Beispiel: 
    * 0 | 1 2 3 4 5 6 7 8 9 10 | 11 12 13 14 15
    * Z | S S S S S S S S S S  | L  L  L  L  L 
    * 
    * Zahl 11 wird ausgewählt
    *
    */

    private int GetEffectiveWeight(Piece piece)
    {
        int weight = piece.SpawnWeight;

        if (activeModifier == null ||
            activeModifier.type != LevelModifierType.PieceCategoryBias)
        {
            return weight;
        }

        foreach (PieceCategory category in activeModifier.preferredCategories)
        {
            if (piece.HasCategory(category))
            {
                weight *= activeModifier.categoryMultiplier;
                break;
            }
        }

        return weight;
    }

    private bool AreAllCurrentPiecesPlaced()
    {
        if (currentPieces.Count == 0)
            return false;

        foreach (Piece piece in currentPieces)
        {
            if (piece == null || !piece.IsPlaced)
                return false;
        }

        return true;
    }

    private void RemoveUnplacedPieces()
    {
        for (int i = currentPieces.Count - 1; i >= 0; i--)
        {
            Piece piece = currentPieces[i];

            if (piece == null)
            {
                currentPieces.RemoveAt(i);
                continue;
            }

            if (!piece.IsPlaced)
            {
                Destroy(piece.gameObject);
            }

            currentPieces.RemoveAt(i);
        }
    }
}