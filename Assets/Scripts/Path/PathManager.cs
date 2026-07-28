using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] public List<LevelNodeUI> levels;

    [Header("Level Modifiers")]
    [SerializeField]
    private List<LevelModifier> easyModifiers;

    [SerializeField]
    private List<LevelModifier> mediumModifiers;

    [SerializeField]
    private List<LevelModifier> hardModifiers;

    private bool pathInitialized;
    private bool modifierEventSubscribed;

    private void Awake()
    {
        levels = GetComponentsInChildren<LevelNodeUI>()
            .ToList();

        for (int i = 0; i < levels.Count; i++)
        {
            Debug.Log(
                $"Index {i} = {levels[i].gameObject.name}"
            );
        }
    }

    private void OnEnable()
    {
        SubscribeToGameStateManager();
    }

    private void Start()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError(
                "GameStateManager not found. " +
                "Start the game from the normal multiplayer scene."
            );

            return;
        }

        SubscribeToGameStateManager();


        if (GameStateManager.Instance.IsServer &&
            !GameStateManager.Instance.HasLevelModifiers(
                levels.Count))
        {
            List<int> modifierIndices =
                GenerateModifierIndices();

            GameStateManager.Instance
                .InitializeLevelModifiers(modifierIndices);
        }


        TryInitializePath();
    }

    private void OnDisable()
    {
        UnsubscribeFromGameStateManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromGameStateManager();
    }

    private void SubscribeToGameStateManager()
    {
        if (modifierEventSubscribed)
            return;

        if (GameStateManager.Instance == null)
            return;

        GameStateManager.Instance.OnLevelStateChanged +=
            UpdateLevelUI;

        GameStateManager.Instance.OnLevelModifiersChanged +=
            HandleLevelModifiersChanged;

        modifierEventSubscribed = true;
    }

    private void UnsubscribeFromGameStateManager()
    {
        if (!modifierEventSubscribed)
            return;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnLevelStateChanged -=
                UpdateLevelUI;

            GameStateManager.Instance.OnLevelModifiersChanged -=
                HandleLevelModifiersChanged;
        }

        modifierEventSubscribed = false;
    }

    private void HandleLevelModifiersChanged()
    {
        TryInitializePath();
    }

    private void TryInitializePath()
    {
        if (pathInitialized)
            return;

        if (GameStateManager.Instance == null)
            return;

        if (!GameStateManager.Instance.HasLevelModifiers(
                levels.Count))
        {
            Debug.Log(
                $"Waiting for synchronized modifiers: " +
                $"{GameStateManager.Instance.levelModifierIndices.Count}" +
                $"/{levels.Count}"
            );

            return;
        }

        pathInitialized = true;

        CreateNodes();
        CreateConnections();

        GameStateManager.Instance.InitializeStates(
            levels.Count
        );

        GameStateManager.Instance.RegisterPath(levels);

        UpdateNodeVisuals();

        Debug.Log(
            "Path initialized with synchronized modifiers."
        );
    }

    private List<int> GenerateModifierIndices()
    {
        List<int> modifierIndices =
            Enumerable.Repeat(-1, levels.Count).ToList();

        for (int id = 1; id < levels.Count - 1; id++)
        {
            LevelDifficulty difficulty =
                GetDifficultyForNode(id);

            modifierIndices[id] =
                GetRandomModifierIndex(difficulty);
        }

        return modifierIndices;
    }

    /// <summary>
    /// Get the difficulty for a given node ID based on its position in the path.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private LevelDifficulty GetDifficultyForNode(int id)
    {

        return id switch
        {
            1 => LevelDifficulty.Easy,
            2 => LevelDifficulty.Medium,
            3 => LevelDifficulty.Hard,

            4 => LevelDifficulty.Medium,
            5 => LevelDifficulty.Easy,
            6 => LevelDifficulty.Hard,

            7 => LevelDifficulty.Hard,
            8 => LevelDifficulty.Easy,
            9 => LevelDifficulty.Medium,

            10 => LevelDifficulty.Medium,
            11 => LevelDifficulty.Medium,
            12 => LevelDifficulty.Easy,

            _ => LevelDifficulty.Placeholder
        };
    }

    private void UpdateNodeVisuals()
    {
        LevelNode currentNode = GetCurrentNode();

        if (currentNode == null)
            return;

        foreach (LevelNodeUI nodeUI in levels)
        {
            LevelNode node = nodeUI.nodeData;

            if (node == null)
                continue;

            bool isSelectable =
                currentNode.Connections.Contains(node);

            bool isCurrentNode =
                node == currentNode;

            bool isFinished =
                node.State == LevelState.Completed;

            if (isSelectable || isCurrentNode || isFinished)
            {
                nodeUI.SetDimmed(false);
            }
            else
            {
                nodeUI.SetDimmed(true);
            }
        }
    }

    private int GetRandomModifierIndex(
        LevelDifficulty difficulty)
    {
        List<LevelModifier> possibleModifiers =
            GetModifierList(difficulty);

        if (possibleModifiers == null ||
            possibleModifiers.Count == 0)
        {
            Debug.LogWarning(
                $"No modifiers configured for {difficulty}."
            );

            return -1;
        }

        return Random.Range(
            0,
            possibleModifiers.Count
        );
    }

    private List<LevelModifier> GetModifierList(
        LevelDifficulty difficulty)
    {
        switch (difficulty)
        {
            case LevelDifficulty.Easy:
                return easyModifiers;

            case LevelDifficulty.Medium:
                return mediumModifiers;

            case LevelDifficulty.Hard:
                return hardModifiers;

            default:
                return null;
        }
    }

    private LevelModifier ResolveModifier(
        LevelDifficulty difficulty,
        int modifierIndex)
    {
        if (modifierIndex < 0)
            return null;

        List<LevelModifier> possibleModifiers =
            GetModifierList(difficulty);

        if (possibleModifiers == null)
        {
            Debug.LogError(
                $"No modifier list exists for {difficulty}."
            );

            return null;
        }

        if (modifierIndex >= possibleModifiers.Count)
        {
            Debug.LogError(
                $"Modifier index {modifierIndex} is invalid " +
                $"for {difficulty}. List size: " +
                $"{possibleModifiers.Count}."
            );

            return null;
        }

        return possibleModifiers[modifierIndex];
    }

    private void CreateNodes()
    {
        CreateNode(
            0,
            0,
            LevelDifficulty.Placeholder
        );

        for (int id = 1; id <= 12; id++)
        {
            int floor = ((id - 1) / 3) + 1;

            LevelDifficulty difficulty =
                GetDifficultyForNode(id);

            CreateNode(
                id,
                floor,
                difficulty
            );
        }

        CreateNode(
            13,
            5,
            LevelDifficulty.Placeholder,
            true
        );
    }

    private Color GetColorForDifficulty(
    LevelDifficulty difficulty)
    {
        switch (difficulty)
        {
            case LevelDifficulty.Easy:
                return new Color(
                    0f / 255f,
                    168f / 255f,
                    232f / 255f,
                    1f
                );

            case LevelDifficulty.Medium:
                return new Color(
                    245f / 255f,
                    197f / 255f,
                    24f / 255f,
                    1f
                );

            case LevelDifficulty.Hard:
                return new Color(
                    130f / 255f,
                    32f / 255f,
                    74f / 255f,
                    1f
                );

            default:
                return Color.gray;
        }
    }

    private void CreateNode(
        int id,
        int floor,
        LevelDifficulty difficulty,
        bool isEndNode = false)
    {
        int modifierIndex =
            GameStateManager.Instance
                .GetLevelModifierIndex(id);

        LevelModifier modifier =
            ResolveModifier(
                difficulty,
                modifierIndex
            );

        LevelNode node = new LevelNode(
            id,
            "PuzzleScene",
            floor,
            difficulty,
            modifier,
            isEndNode
        );

        levels[id].SetNodeData(node);
        levels[id].SetDifficultyColor(GetColorForDifficulty(difficulty));

        Debug.Log(
            modifier == null
                ? $"Node {id} | Difficulty: {difficulty} | " +
                  $"Modifier: None | Synced index: " +
                  $"{modifierIndex}"
                : $"Node {id} | Difficulty: {difficulty} | " +
                  $"Modifier: {modifier.displayName} | " +
                  $"Bonus: {modifier.scoreBonus} | " +
                  $"Synced index: {modifierIndex}"
        );
    }

    public void UpdateLevelUI(
        int index,
        LevelState state,
        Color color)
    {
        if (index >= 0 && index < levels.Count)
        {
            levels[index].SetState(state, color);
        }

        UpdateNodeVisuals();
    }

    private void CreateConnections()
    {
        // Start > Ebene 1
        Connect(0, 1);
        Connect(0, 2);
        Connect(0, 3);

        // Ebene 1 > Ebene 2
        Connect(1, 4);
        Connect(1, 5);

        Connect(2, 4);
        Connect(2, 5);

        Connect(3, 6);

        // Ebene 2 > Ebene 3
        Connect(4, 7);

        Connect(5, 8);

        Connect(6, 9);

        // Ebene 3 > Ebene 4
        Connect(7, 10);

        Connect(8, 11);
        Connect(8, 12);

        Connect(9, 11);
        Connect(9, 12);

        // Ebene 4 > Ende
        Connect(10, 13);
        Connect(11, 13);
        Connect(12, 13);
    }

    private void Connect(int a, int b)
    {
        levels[a].nodeData.Connections.Add(
            levels[b].nodeData
        );
    }

    public LevelNode GetCurrentNode()
    {
        if (LevelTransferData.SelectedLevelID < 0 ||
            LevelTransferData.SelectedLevelID >= levels.Count)
        {
            return null;
        }

        return levels[
            LevelTransferData.SelectedLevelID
        ].nodeData;
    }
}