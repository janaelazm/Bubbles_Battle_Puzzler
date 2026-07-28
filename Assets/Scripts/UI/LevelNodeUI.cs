using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button levelButton;
    [SerializeField] public Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    public LevelNode nodeData;

    public void SetNodeData(LevelNode data)
    {
        nodeData = data;
        text.text = data.LevelID.ToString();
    }

    void Awake()
    {
        tooltipPanel.SetActive(false);
        Debug.Log(gameObject.name + " TEXT: " + text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nodeData == null || nodeData.Modifier == null)
        {
            tooltipPanel.SetActive(false);
            return;
        }

        if (string.IsNullOrEmpty(nodeData.Modifier.displayName))
        {
            tooltipPanel.SetActive(false);
            return;
        }

        tooltipText.text = nodeData.Modifier.displayName;
        tooltipPanel.SetActive(true);
    }

    private string GetModifierDescription(LevelModifier modifier)
    {
        string modifierName = modifier.ToString();

        string difficulty = "";
        string type = "";

        if (modifierName.Contains("Green"))
            difficulty = "Easy";
        else if (modifierName.Contains("Yellow"))
            difficulty = "Medium";
        else if (modifierName.Contains("Red"))
            difficulty = "Hard";

        if (modifierName.Contains("Small"))
            type = "Small";
        else if (modifierName.Contains("Diagonal"))
            type = "Diagonal";

        return $"{difficulty} {type}";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }

    public void EnterLevel()
    {
        PathManager pathManager = FindAnyObjectByType<PathManager>();

        if (pathManager == null)
        {
            Debug.LogError("PathManager not found.");
            return;
        }

        if (nodeData == null)
        {
            Debug.LogError($"No nodeData assigned to {gameObject.name}.");
            return;
        }

        LevelNode currentNode = pathManager.GetCurrentNode();

        if (currentNode == null)
        {
            Debug.LogError("Current node is null.");
            return;
        }

        if (currentNode.Connections == null)
        {
            Debug.LogError(
                $"Connections are null for current node {currentNode.LevelID}."
            );
            return;
        }

        if (!currentNode.Connections.Contains(nodeData))
            return;

        if (nodeData.LevelID == 0)
            return;

        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager instance not found.");
            return;
        }

        if (GameStateManager.Instance.GameEnded.Value)
            return;

        if (nodeData.IsEndNode)
        {
            GameStateManager.Instance.ReachEnd();
            return;
        }

        if (nodeData.State == LevelState.Completed ||
            nodeData.State == LevelState.Occupied)
        {
            return;
        }

        if (PlayerProfile.Instance == null)
        {
            Debug.LogError("PlayerProfile instance not found.");
            return;
        }

        GameStateManager.Instance.OccupyLevel(
            nodeData.LevelID,
            PlayerProfile.Instance.PlayerColor
        );

        LevelTransferData.SelectedLevelID = nodeData.LevelID;
        LevelTransferData.SelectedDifficulty = nodeData.Difficulty;
        LevelTransferData.SelectedModifier = nodeData.Modifier;

        SceneManager.LoadScene(nodeData.LevelName);
    }

    private Color difficultyColor = Color.white;
    private Color currentColor = Color.white;
    private bool isDimmed;

    public void SetDifficultyColor(Color color)
    {
        difficultyColor = color;
        currentColor = color;

        RefreshColor();
    }

    public void SetState(LevelState state, Color playerColor)
    {
        if (nodeData != null)
            nodeData.State = state;

        switch (state)
        {
            case LevelState.Available:
                currentColor = difficultyColor;
                break;

            case LevelState.Occupied:
                currentColor = playerColor;
                break;

            case LevelState.Completed:
                currentColor = new Color(
                    playerColor.r * 0.5f,
                    playerColor.g * 0.5f,
                    playerColor.b * 0.5f,
                    playerColor.a
                );
                break;
        }

        RefreshColor();
    }

    public void SetDimmed(bool dimmed)
    {
        isDimmed = dimmed;
        levelButton.interactable = !dimmed;

        RefreshColor();
    }

    private void RefreshColor()
    {
        Color visibleColor = currentColor;

        visibleColor.a = isDimmed
            ? 0.97f
            : 1f;

        image.color = visibleColor;
    }
}