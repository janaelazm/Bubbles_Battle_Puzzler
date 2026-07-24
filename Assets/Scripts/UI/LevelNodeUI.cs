using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelNodeUI : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    [SerializeField] public Image image;
    [SerializeField] private TextMeshProUGUI text;
    public LevelNode nodeData;

    public void SetNodeData(LevelNode data)
    {
        nodeData = data;
        text.text = data.LevelID.ToString();
    }

    void Awake()
    {
        levelButton = GetComponent<Button>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(gameObject.name + " TEXT: " + text);
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

        if (nodeData.LevelID == 10)
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

        SceneManager.LoadScene(nodeData.LevelName);
    }

    public void SetState(LevelState state, Color color)
    {
        nodeData.State = state;
        switch (state)
        {

            case LevelState.Occupied:
                image.color = color;
                break;

            case LevelState.Completed:
                image.color = new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);
                break;
        }
    }

    public void SetDifficultyColor(Color color)
    {
        image.color = color;
    }
}