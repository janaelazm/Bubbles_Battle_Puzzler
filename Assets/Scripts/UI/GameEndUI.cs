using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        GameStateManager.Instance.GameEnded.OnValueChanged += OnGameEnded;

        // handles players joining after game already ended
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
        if (NetworkManager.Singleton.LocalClientId ==
            GameStateManager.Instance.WinnerClientId.Value)
        {
            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
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