using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnScoreChanged;

    [SerializeField] private int currentScore = 0;

    public int CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            OnScoreChanged?.Invoke(currentScore);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"Score reset : {currentScore}");
    }

    public void AddPoints(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"+{amount} Punkte | Score = {currentScore}");
    }

    public void RemovePoints(int amount)
    {
        currentScore -= amount;

        if (currentScore < 0)
            currentScore = 0;

        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"-{amount} Punkte | Score = {currentScore}");
    }

    public int GetModifierBonus(LevelModifier modifier)
    {
        if (modifier == null)
            return 0;

        switch (modifier.type)
        {
            case LevelModifierType.FasterPieceSwap:
                return 5;

            case LevelModifierType.PieceCategoryBias:
                return 3;

            case LevelModifierType.FixedStartingPiece:
                return 5;

            default:
                return 0;
        }
    }
}