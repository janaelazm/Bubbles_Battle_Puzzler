using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text localScoreText;
    [SerializeField] private TMP_Text opponentScoreText;
    [SerializeField] private Image backgroundImage;

    [Header("Result Backgrounds")]
    [SerializeField] private Sprite winBackground;
    [SerializeField] private Sprite loseBackground;

    [Header("Result Texts")]
    [SerializeField] private string winText = "YOU WIN!";
    [SerializeField] private string loseText = "YOU LOSE!";

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
            localScoreText.text =
                $"Your score: {localScore}";
        }

        if (opponentScoreText != null)
        {
            opponentScoreText.text =
                $"Opponent score: {opponentScore}";
        }

        bool isWinner =
            localClientId ==
            GameStateManager.Instance.WinnerClientId.Value;

        if (isWinner)
        {
            ApplyResultVisuals(
                winText,
                winBackground
            );
        }
        else
        {
            ApplyResultVisuals(
                loseText,
                loseBackground
            );
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
            endPanel.transform.SetAsLastSibling();
        }

        Debug.Log(
            $"Final result displayed. " +
            $"Local: {localScore}, Opponent: {opponentScore}"
        );
    }

    private void ApplyResultVisuals(
        string message,
        Sprite background
    )
    {
        if (resultText != null)
        {
            resultText.text = message;
        }

        if (backgroundImage != null &&
            background != null)
        {
            backgroundImage.sprite = background;
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.GameEnded.OnValueChanged -= OnGameEnded;
        }
    }
}