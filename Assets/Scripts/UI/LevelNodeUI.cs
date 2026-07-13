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
        LevelNode currentNode =
            FindAnyObjectByType<PathManager>().GetCurrentNode();

        if (!currentNode.Connections.Contains(nodeData))
            return;

        if (nodeData.LevelID == 0)
            return;

        if (GameStateManager.Instance.GameEnded.Value)
            return;

        if (nodeData.LevelID == 10)
        {
            GameStateManager.Instance.ReachEnd();
            return;
        }

        if(nodeData.State == LevelState.Completed || nodeData.State == LevelState.Occupied)
            return ;

        GameStateManager.Instance.OccupyLevel(
            nodeData.LevelID,
            PlayerProfile.Instance.PlayerColor
        );

        LevelTransferData.SelectedLevelID = nodeData.LevelID;
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