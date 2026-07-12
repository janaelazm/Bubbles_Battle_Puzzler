using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text roomCode;
    [SerializeField] private TMP_Text roomStatus;

    [SerializeField] private RelayManager relayManager;

    public void initNetworkUI()
    {
        roomCode.gameObject.SetActive(false);
        roomStatus.gameObject.SetActive(false);
    }

    public async void StartHost()
    {
        roomCode.text = "Generating Code...";
        roomCode.gameObject.SetActive(true);
        roomStatus.gameObject.SetActive(true);
        try
        {
            string code = await relayManager.StartHost();
            roomCode.text = $"Room Code: {code}";
            roomStatus.text = "waiting for player to join...";
            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                Debug.Log("Client connected: " + clientId);
                NetworkManager.Singleton.SceneManager.LoadScene(
                "PathSelection",
                LoadSceneMode.Single
            );
            };

        }
        catch
        {   
            roomCode.text = "Failed to create room.";
        }
        
    }

    public async void StartClient()
    {
        roomCode.text = "Joining Room...";
        string code = joinCodeInput.text;
        if (string.IsNullOrWhiteSpace(code))
            return;
        try
        {
            await relayManager.StartClient(code);
            roomCode.gameObject.SetActive(true);
            roomCode.text = $"Room Code: {code}";
        }
        catch
        {
            roomCode.text = "Failed to join room.";
        }
    }

    
}