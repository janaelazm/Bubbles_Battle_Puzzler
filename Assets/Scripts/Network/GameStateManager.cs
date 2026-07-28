using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerScoreEntry :
    INetworkSerializable,
    IEquatable<PlayerScoreEntry>
{
    public ulong ClientId;
    public int Score;



    public PlayerScoreEntry(ulong clientId, int score)
    {
        ClientId = clientId;
        Score = score;
    }

    public void NetworkSerialize<T>(
        BufferSerializer<T> serializer
    ) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Score);
    }

    public bool Equals(PlayerScoreEntry other)
    {
        return ClientId == other.ClientId &&
               Score == other.Score;
    }
}


public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public NetworkList<byte> levelStates = new NetworkList<byte>();
    public NetworkList<Color> levelColors = new NetworkList<Color>();
    // to tell other scripts of state changes
    public event System.Action<int, LevelState, Color> OnLevelStateChanged;
    public NetworkVariable<ulong> WinnerClientId = new NetworkVariable<ulong>(ulong.MaxValue);
    public NetworkVariable<bool> GameEnded = new NetworkVariable<bool>(false);
    public event System.Action<int> OnLocalScoreChanged;

    private int localScore;

    public int LocalScore => localScore;
    // [SerializeField] private GameObject gameEndUIPrefab;
    // private GameObject gameEndUIInstance;

    public NetworkList<PlayerScoreEntry> PlayerScores = new NetworkList<PlayerScoreEntry>();
    public NetworkList<int> levelModifierIndices = new NetworkList<int>();

    public event Action OnLevelModifiersChanged;

    public NetworkVariable<bool> IsDraw = new NetworkVariable<bool>(false);

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
        levelStates.OnListChanged += OnStateChanged;
        levelColors.OnListChanged += OnColorChanged;
        levelModifierIndices.OnListChanged += OnModifierIndexChanged;

        GameEnded.OnValueChanged += OnGameEnded;

        if (ScoreManager.Instance != null)
        {
            SetLocalScore(ScoreManager.Instance.CurrentScore);
        }

        if (levelModifierIndices.Count > 0)
        {
            OnLevelModifiersChanged?.Invoke();
        }
    }

    private void OnModifierIndexChanged(
    NetworkListEvent<int> changeEvent)
    {
        OnLevelModifiersChanged?.Invoke();
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
        levelModifierIndices.OnListChanged -= OnModifierIndexChanged;

        GameEnded.OnValueChanged -= OnGameEnded;
    }

    public void ReachEnd()
    {
        int finalScore = localScore;

        if (ScoreManager.Instance != null)
        {
            finalScore =
                ScoreManager.Instance.CurrentScore;
        }

        if (IsServer)
        {
            ulong localClientId =
                NetworkManager.Singleton.LocalClientId;

            SetPlayerScoreOnServer(
                localClientId,
                finalScore
            );

            EndGameByScore();
        }
        else
        {
            ReachEndServerRpc(finalScore);
        }
    }


    [Rpc(
    SendTo.Server,
    InvokePermission = RpcInvokePermission.Everyone
)]
    private void ReachEndServerRpc(
    int finalScore,
    RpcParams rpcParams = default
)
    {
        ulong senderClientId =
            rpcParams.Receive.SenderClientId;

        SetPlayerScoreOnServer(
            senderClientId,
            finalScore
        );

        EndGameByScore();
    }




    private void OnGameEnded(bool oldValue, bool newValue)
    {
        if (!newValue)
            return;

        // SpawnEndUI();

        if (IsDraw.Value)
        {
            Debug.Log("DRAW");
        }
        else if (
            NetworkManager.Singleton.LocalClientId ==
            WinnerClientId.Value
        )
        {
            Debug.Log("YOU WIN");
        }
        else
        {
            Debug.Log("YOU LOSE");
        }
    }





    private void EndGameByScore()
    {
        if (!IsServer || GameEnded.Value)
            return;

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(
                "NetworkManager not available."
            );

            return;
        }

        IReadOnlyList<ulong> connectedClientIds =
            NetworkManager.Singleton.ConnectedClientsIds;

        if (connectedClientIds.Count == 0)
        {
            Debug.LogError(
                "No connected players available."
            );

            return;
        }

        int highestScore = int.MinValue;
        ulong winningClientId = ulong.MaxValue;
        bool draw = false;

        foreach (ulong clientId in connectedClientIds)
        {
            int score = GetPlayerScore(clientId);

            Debug.Log(
                $"Final score: Client {clientId} = {score}"
            );

            if (score > highestScore)
            {
                highestScore = score;
                winningClientId = clientId;
                draw = false;
            }
            else if (score == highestScore)
            {
                draw = true;
            }
        }

        IsDraw.Value = draw;

        WinnerClientId.Value =
            draw ? ulong.MaxValue : winningClientId;

        GameEnded.Value = true;

        if (draw)
        {
            Debug.Log(
                $"Game ended in a draw: {highestScore}"
            );
        }
        else
        {
            Debug.Log(
                $"Winner by score: Client " +
                $"{winningClientId}, Score = {highestScore}"
            );
        }
    }




/*     private void SpawnEndUI()
    {
        if (gameEndUIInstance != null)
            return;

        gameEndUIInstance = Instantiate(gameEndUIPrefab);

        DontDestroyOnLoad(gameEndUIInstance);
    } */

    private void SetPlayerScoreOnServer(
    ulong clientId,
    int score
)
    {
        if (!IsServer)
            return;

        score = Mathf.Max(0, score);

        for (int i = 0; i < PlayerScores.Count; i++)
        {
            if (PlayerScores[i].ClientId != clientId)
                continue;

            PlayerScores[i] =
                new PlayerScoreEntry(clientId, score);

            Debug.Log(
                $"Server updated score: " +
                $"Client {clientId} = {score}"
            );

            return;
        }

        PlayerScores.Add(
            new PlayerScoreEntry(clientId, score)
        );

        Debug.Log(
            $"Server registered score: " +
            $"Client {clientId} = {score}"
        );
    }

    [Rpc(
    SendTo.Server,
    InvokePermission = RpcInvokePermission.Everyone
)]
    private void SubmitScoreServerRpc(
    int score,
    RpcParams rpcParams = default
)
    {
        ulong senderClientId =
            rpcParams.Receive.SenderClientId;

        SetPlayerScoreOnServer(senderClientId, score);
    }


    public void SetLocalScore(int newScore)
    {
        newScore = Mathf.Max(0, newScore);

        if (localScore != newScore)
        {
            localScore = newScore;
            OnLocalScoreChanged?.Invoke(localScore);
        }

        if (!IsSpawned || NetworkManager.Singleton == null)
        {
            Debug.Log(
                $"Score stored locally until network is ready: {localScore}"
            );

            return;
        }

        ulong localClientId =
            NetworkManager.Singleton.LocalClientId;

        if (IsServer)
        {
            SetPlayerScoreOnServer(localClientId, localScore);
        }
        else
        {
            SubmitScoreServerRpc(localScore);
        }

        Debug.Log(
            $"GameStateManager submitted local score: {localScore}"
        );
    }





    public int GetPlayerScore(ulong clientId)
    {
        for (int i = 0; i < PlayerScores.Count; i++)
        {
            if (PlayerScores[i].ClientId == clientId)
            {
                return PlayerScores[i].Score;
            }
        }

        return 0;
    }

    public bool TryGetOpponentScore(
    ulong localClientId,
    out int opponentScore
)
    {
        for (int i = 0; i < PlayerScores.Count; i++)
        {
            if (PlayerScores[i].ClientId == localClientId)
                continue;

            opponentScore = PlayerScores[i].Score;
            return true;
        }

        opponentScore = 0;
        return false;
    }

    public bool HasLevelModifiers(int expectedCount)
    {
        return levelModifierIndices.Count == expectedCount;
    }


    public int GetLevelModifierIndex(int levelID)
    {
        if (levelID < 0 ||
            levelID >= levelModifierIndices.Count)
        {
            return -1;
        }

        return levelModifierIndices[levelID];
    }


    public void InitializeLevelModifiers(
    IReadOnlyList<int> modifierIndices)
    {
        if (!IsServer)
        {
            Debug.LogWarning(
                "Only the server can initialize level modifiers."
            );

            return;
        }

        if (modifierIndices == null ||
            modifierIndices.Count == 0)
        {
            Debug.LogError(
                "Cannot initialize an empty modifier list."
            );

            return;
        }

       
        if (levelModifierIndices.Count ==
            modifierIndices.Count)
        {
            Debug.Log(
                "Synchronized level modifiers already exist."
            );

            return;
        }

      
        levelModifierIndices.Clear();

        for (int i = 0; i < modifierIndices.Count; i++)
        {
            levelModifierIndices.Add(modifierIndices[i]);
        }

        Debug.Log(
            $"Server initialized {levelModifierIndices.Count} " +
            "synchronized modifier indices."
        );
    }


    public void ResetLevelModifiers()
    {
        if (!IsServer)
            return;

        levelModifierIndices.Clear();

        Debug.Log(
            "Server reset synchronized level modifiers."
        );
    }

}