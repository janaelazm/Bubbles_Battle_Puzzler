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
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
        NotifyGameStateManager();

        Debug.Log($"Score reset : {currentScore}");
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        currentScore += amount;

        OnScoreChanged?.Invoke(currentScore);
        NotifyGameStateManager();

        Debug.Log($"+{amount} Punkte | Score = {currentScore}");
    }

    public void RemovePoints(int amount)
    {
        if (amount <= 0)
            return;

        currentScore -= amount;
        currentScore = Mathf.Max(0, currentScore);

        OnScoreChanged?.Invoke(currentScore);
        NotifyGameStateManager();

        Debug.Log($"-{amount} Punkte | Score = {currentScore}");
    }

    public int GetBasePoints(LevelDifficulty difficulty)
    {
        switch (difficulty)
        {
            case LevelDifficulty.Easy:
                return 10;

            case LevelDifficulty.Medium:
                return 20;

            case LevelDifficulty.Hard:
                return 35;

            default:
                Debug.LogWarning(
                    $"No score configured for difficulty: {difficulty}"
                );
                return 0;
        }
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


    private void NotifyGameStateManager()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning(
                "GameStateManager not available yet. " +
                "The score remains stored locally."
            );

            return;
        }

        GameStateManager.Instance.SetLocalScore(currentScore);
    }
}