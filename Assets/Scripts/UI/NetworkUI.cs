using TMPro;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text roomStatus;

    [SerializeField] private RelayManager relayManager;
    [SerializeField] private LobbyManager lobbyManager;


    private void OnEnable()
    {
        relayManager.OnStatusChanged += UpdateStatus;
        relayManager.OnRoomCodeGenerated += UpdateRoomCode;

        lobbyManager.OnLobbyStatusChanged += UpdateStatus;
    }

    private void OnDisable()
    {
        relayManager.OnStatusChanged -= UpdateStatus;
        relayManager.OnRoomCodeGenerated -= UpdateRoomCode;

        lobbyManager.OnLobbyStatusChanged -= UpdateStatus;
    }


    public void InitNetworkUI()
    {
        roomCode.gameObject.SetActive(false);
        roomStatus.gameObject.SetActive(false);
    }


    public async void StartHost()
    {
        roomCode.gameObject.SetActive(true);
        roomStatus.gameObject.SetActive(true);

        await relayManager.StartHost();
    }


    public async void StartClient()
    {
        string code = joinCodeInput.text;

        if (string.IsNullOrWhiteSpace(code))
            return;

        roomCode.gameObject.SetActive(true);
        roomStatus.gameObject.SetActive(true);

        roomCode.text = $"Room Code: {code}";
        await relayManager.StartClient(code);
    }


    private void UpdateStatus(string message)
    {
        roomStatus.text = message;
    }


    private void UpdateRoomCode(string code)
    {
        roomCode.text = $"Room Code: {code}";
    }

    public void QuickPlay()
    {
        roomCode.gameObject.SetActive(true);
        roomStatus.gameObject.SetActive(true);
        lobbyManager.QuickPlay();
    }
}