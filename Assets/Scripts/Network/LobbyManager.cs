using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
   /* [SerializeField] private Authenticator authenticator;
    [SerializeField] private RelayManager relayManager;

    public async void QuickPlay()
    {
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

    private async Task CreateLobby()
    {
        string playerName = PlayerProfile.Instance.PlayerName;
        string playerId = authenticator.GetPlayerId();

        // store relay join code inside lobby
        string joinCode = await relayManager.StartHost();

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
            joinCode,
            2
            TODO LATER
            playerData:
            {
                "PlayerName": playerName,
                "PlayerId": playerId
            }
        );

    }

   private async Task JoinLobby(Lobby lobby)
   {
        Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

        string joinCode = joinedLobby.Data["RelayCode"].Value;

        await relayManager.StartClient(joinCode);

        Debug.Log("Joined lobby");
    }

    private async Task<Lobby> FindAvailableLobby()
    {
        QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();

        if (response.Results.Count > 0)
        {
            return response.Results[0];
        }

        return null;
    }*/
}