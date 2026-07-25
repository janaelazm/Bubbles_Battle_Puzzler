using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text localScoreText;
    [SerializeField] private TMP_Text opponentScoreText;

    private void Start()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager instance not found.");
            return;
        }

        GameStateManager.Instance.GameEnded.OnValueChanged += OnGameEnded;

        if (GameStateManager.Instance.GameEnded.Value)
        {
            ShowResult();
        }
    }

    private void OnGameEnded(bool oldValue, bool newValue)
    {
        if (!newValue)
            return;

        ShowResult();
    }

    private void ShowResult()
    {
        if (GameStateManager.Instance == null ||
            NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "Cannot show result because the network state is unavailable."
            );

            return;
        }

        ulong localClientId =
            NetworkManager.Singleton.LocalClientId;

        int localScore =
            GameStateManager.Instance.GetPlayerScore(localClientId);

        bool opponentFound =
            GameStateManager.Instance.TryGetOpponentScore(
                localClientId,
                out int opponentScore
            );

        if (!opponentFound)
        {
            opponentScore = 0;
            Debug.LogWarning("Opponent score was not found.");
        }

        if (localScoreText != null)
        {
            localScoreText.text = $"Your score: {localScore}";
        }

        if (opponentScoreText != null)
        {
            opponentScoreText.text =
                $"Opponent score: {opponentScore}";
        }

        if (resultText != null)
        {
            if (GameStateManager.Instance.IsDraw.Value)
            {
                resultText.text = "DRAW";
            }
            else if (
                localClientId ==
                GameStateManager.Instance.WinnerClientId.Value
            )
            {
                resultText.text = "YOU WIN!";
            }
            else
            {
                resultText.text = "YOU LOSE!";
            }
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        Debug.Log(
            $"Final result displayed. " +
            $"Local: {localScore}, Opponent: {opponentScore}"
        );
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.GameEnded.OnValueChanged -= OnGameEnded;
        }
    }
}