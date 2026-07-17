using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public NetworkList<byte> levelStates = new NetworkList<byte>();
    public NetworkList<Color> levelColors = new NetworkList<Color>();
    // to tell other scripts of state changes
    public event System.Action<int, LevelState, Color> OnLevelStateChanged;
    public NetworkVariable<ulong> WinnerClientId = new NetworkVariable<ulong>(ulong.MaxValue);
    public NetworkVariable<bool> GameEnded = new NetworkVariable<bool>(false);
    [SerializeField] private GameObject gameEndUIPrefab;
    private GameObject gameEndUIInstance;

    private void Awake()
    {
        //set one game state manager instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        //whenever level states change call OnsStateChanged function
        levelStates.OnListChanged += OnStateChanged;
        levelColors.OnListChanged += OnColorChanged;
        GameEnded.OnValueChanged += OnGameEnded;

    }


    private void OnStateChanged(NetworkListEvent<byte> changeEvent)
    {
        Color color = Color.white;

        if (changeEvent.Index < levelColors.Count)
        {
            color = levelColors[changeEvent.Index];
        }

        OnLevelStateChanged?.Invoke(
            changeEvent.Index,
            (LevelState)changeEvent.Value,
            color
        );
    }

    private void OnColorChanged(NetworkListEvent<Color> changeEvent)
    {
        if (changeEvent.Index < levelStates.Count)
        {
            OnLevelStateChanged?.Invoke(
                changeEvent.Index,
                (LevelState)levelStates[changeEvent.Index],
                changeEvent.Value
            );
        }
    }


    public void InitializeStates(int levelCount)
    {
        if (!IsServer)
            return;

        if (levelStates.Count > 0)
            return;

        for (int i = 0; i < levelCount; i++)
        {
            levelColors.Add(Color.white);
            levelStates.Add((byte)LevelState.Available);
        }
    }


    public void OccupyLevel(int levelID, Color color)
    {
        if (!IsSpawned)
        {
            Debug.LogError("GameStateManager is not spawned yet!");
            return;
        }

        if (IsServer)
        {
            levelColors[levelID] = color;
            levelStates[levelID] = (byte)LevelState.Occupied;
        }
        else
        {
            OccupyLevelServerRpc(levelID, color);
        }
    }


    public void RegisterPath(List<LevelNodeUI> levels)
    {
        for (int i = 0; i < levelStates.Count; i++)
        {
            if (i < levels.Count)
            {
                levels[i].SetState((LevelState)levelStates[i], levelColors[i]);
            }
        }

    }

    public void CompleteLevel(int levelID, Color color)
    {
        if (!IsSpawned)
        {
            Debug.LogError("GameStateManager is not spawned yet!");
            return;
        }

        if (IsServer)
        {
            levelColors[levelID] = color;
            levelStates[levelID] = (byte)LevelState.Completed;
        }
        else
        {
            CompleteLevelServerRpc(levelID, color);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OccupyLevelServerRpc(int levelID, Color color)
    {
        if ((LevelState)levelStates[levelID] == LevelState.Occupied || (LevelState)levelStates[levelID] == LevelState.Completed)
            return;

        levelColors[levelID] = color;
        levelStates[levelID] = (byte)LevelState.Occupied;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void CompleteLevelServerRpc(int levelID, Color color)
    {
        levelColors[levelID] = color;
        levelStates[levelID] = (byte)LevelState.Completed;
    }
    public override void OnNetworkDespawn()
    {
        levelStates.OnListChanged -= OnStateChanged;
        levelColors.OnListChanged -= OnColorChanged;
        GameEnded.OnValueChanged -= OnGameEnded;
    }

    public void ReachEnd()
    {
        if (IsServer)
        {
            DeclareWinner(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            ReachEndServerRpc();
        }
    }


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ReachEndServerRpc(RpcParams rpcParams = default)
    {
        DeclareWinner(rpcParams.Receive.SenderClientId);
    }


    private void DeclareWinner(ulong clientId)
    {
        if (GameEnded.Value)
            return;

        WinnerClientId.Value = clientId;
        GameEnded.Value = true;

        Debug.Log("Winner: " + clientId);
    }

    private void OnGameEnded(bool oldValue, bool newValue)
    {
        if (!newValue)
            return;

        SpawnEndUI();

        if (NetworkManager.Singleton.LocalClientId ==
            WinnerClientId.Value)
        {
            Debug.Log("YOU WIN");
        }
        else
        {
            Debug.Log("YOU LOSE");
        }
    }

    private void SpawnEndUI()
    {
        if (gameEndUIInstance != null)
            return;

        gameEndUIInstance = Instantiate(gameEndUIPrefab);

        DontDestroyOnLoad(gameEndUIInstance);
    }
}