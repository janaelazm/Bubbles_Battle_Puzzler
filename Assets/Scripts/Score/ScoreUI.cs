using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance not found.");
            return;
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreText is not assigned.");
            return;
        }

        UpdateScoreText(ScoreManager.Instance.CurrentScore);
        ScoreManager.Instance.OnScoreChanged += UpdateScoreText;
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreText;
        }
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}