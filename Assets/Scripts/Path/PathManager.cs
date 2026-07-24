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

    private LevelDifficulty GetDifficultyForNode(int id)
    {
        int difficultyPosition = (id - 1) % 3;

        switch (difficultyPosition)
        {
            case 0:
                return LevelDifficulty.Easy;

            case 1:
                return LevelDifficulty.Medium;

            case 2:
                return LevelDifficulty.Hard;

            default:
                return LevelDifficulty.Placeholder;
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
        int id = 1;

        CreateNode(
            0,
            0,
            LevelDifficulty.Placeholder,
            Color.gray
        );

        for (int floor = 1; floor <= 3; floor++)
        {
            CreateNode(
                id++,
                floor,
                LevelDifficulty.Easy,
                new Color(
                    78f / 255f,
                    242f / 255f,
                    163f / 255f,
                    1f
                )
            );

            CreateNode(
                id++,
                floor,
                LevelDifficulty.Medium,
                new Color(
                    237f / 255f,
                    225f / 255f,
                    116f / 255f,
                    1f
                )
            );

            CreateNode(
                id++,
                floor,
                LevelDifficulty.Hard,
                new Color(
                    214f / 255f,
                    81f / 255f,
                    132f / 255f,
                    1f
                )
            );
        }

        CreateNode(
            10,
            4,
            LevelDifficulty.Placeholder,
            Color.gray
        );
    }

    private void CreateNode(
        int id,
        int floor,
        LevelDifficulty difficulty,
        Color color)
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
            modifier
        );

        levels[id].SetNodeData(node);
        levels[id].image.color = color;

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