using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Authenticator authenticator;
    [SerializeField] private RelayManager relayManager;

    public event Action<string> OnLobbyStatusChanged;


    public async void QuickPlay()
    {
        try
        {
            OnLobbyStatusChanged?.Invoke("Searching for players...");

            Lobby lobby = await FindAvailableLobby();

            if (lobby != null)
            {
                await JoinLobby(lobby);
            }
            else
            {
                await CreateLobby();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnLobbyStatusChanged?.Invoke("Lobby failed.");
        }
    }


    private async Task CreateLobby()
    {
        string playerName = PlayerProfile.Instance.PlayerName;
        OnLobbyStatusChanged?.Invoke("Creating lobby...");
        // Create relay first
        string relayCode = await relayManager.StartHost();
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        "PlayerName",
                        new PlayerDataObject(
                            PlayerDataObject.VisibilityOptions.Member,
                            playerName
                        )
                    }
                }
            },

            Data = new Dictionary<string, DataObject>
            {
                {
                    "RelayCode",
                    new DataObject(
                        DataObject.VisibilityOptions.Member,
                        relayCode
                    )
                }
            }
        };


        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
            "Quick Play Lobby",
            2,
            options
        );


        Debug.Log("Lobby created: " + lobby.Id);

        OnLobbyStatusChanged?.Invoke("Waiting for player...");
    }


    private async Task JoinLobby(Lobby lobby)
    {
        OnLobbyStatusChanged?.Invoke("Joining lobby...");
        Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
        string relayCode = joinedLobby.Data["RelayCode"].Value;

        Debug.Log("Relay code: " + relayCode);
        await relayManager.StartClient(relayCode);
        OnLobbyStatusChanged?.Invoke("Connected!");
    }


    private async Task<Lobby> FindAvailableLobby()
    {
        QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
        if (response.Results.Count > 0)
        {
            return response.Results[0];
        }
        return null;
    }
}