using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] public List<LevelNodeUI> levels;

    private void Awake()
    {
        levels = GetComponentsInChildren<LevelNodeUI>().ToList();
    }


    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnLevelStateChanged += UpdateLevelUI;
        }
    }


    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnLevelStateChanged -= UpdateLevelUI;
        }
    }


    private void Start()
    {
        CreateNodes();
        CreateConnections();

        GameStateManager.Instance.InitializeStates(levels.Count);
        GameStateManager.Instance.RegisterPath(levels);
    }


    private void CreateNodes()
    {
        int id = 1;

        CreateNode(0, 0, LevelDifficulty.Placeholder, Color.gray);

        for (int floor = 1; floor <= 3; floor++)
        {
            CreateNode(id++, floor, LevelDifficulty.Easy, new Color(
                78f / 255f,
                242f / 255f,
                163f / 255f,
                1f
            ));
            CreateNode(id++, floor, LevelDifficulty.Medium, new Color(
                237f / 255f,
                225f / 255f,
                116f / 255f,
                1f
            ));
            CreateNode(id++, floor, LevelDifficulty.Hard, new Color(
                214f / 255f,
                81f / 255f,
                132f / 255f,
                1f
            ));
        
        }
        CreateNode(10, 4, LevelDifficulty.Placeholder, Color.gray);
    }

    private void CreateNode(int id, int floor, LevelDifficulty difficulty, Color color)
    {
        LevelNode node = new LevelNode(
            id,
            "PuzzleScene",
            floor,
            difficulty
        );

        levels[id].SetNodeData(node);
        levels[id].image.color = color;
    }


    public void UpdateLevelUI(int index, LevelState state, Color color)
    {
        if (index >= 0 && index < levels.Count)
        {
            levels[index].SetState(state, color);
        }
    }


    private void CreateConnections()
    {
        Connect(0, 1);
        Connect(0, 2);
        Connect(0, 3);

        // Floor 1 -> Floor 2
        Connect(1, 4);
        Connect(1, 5);
        Connect(1, 6);

        Connect(2, 4);
        Connect(2, 5);
        Connect(2, 6);

        Connect(3, 4);
        Connect(3, 5);
        Connect(3, 6);


        // Floor 2 -> Floor 3
        Connect(4, 7);
        Connect(4, 8);
        Connect(4, 9);

        Connect(5, 7);
        Connect(5, 8);
        Connect(5, 9);

        Connect(6, 7);
        Connect(6, 8);
        Connect(6, 9);

        Connect(7, 10);
        Connect(8, 10);
        Connect(9, 10);
    }


    private void Connect(int a, int b)
    {
        levels[a].nodeData.Connections.Add(levels[b].nodeData);
    }


    public LevelNode GetCurrentNode()
    {
        if (LevelTransferData.SelectedLevelID < 0 ||
            LevelTransferData.SelectedLevelID >= levels.Count)
        {
            return null;
        }

        return levels[LevelTransferData.SelectedLevelID].nodeData;
    }
}
