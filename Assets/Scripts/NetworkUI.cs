using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using System;
using System.Threading.Tasks;
using WebSocketSharp;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text roomCode;

    [SerializeField] private RelayManager relayManager;

    void Awake()
    {
        roomCode.gameObject.SetActive(false);
    }

    public async void StartHost()
    {
        roomCode.text = "Generating Code...";
        roomCode.gameObject.SetActive(true);
        await relayManager.StartHost();
        roomCode.text = "Room Code: " + relayManager.joinCode;
    }

    public async void StartClient()
    {
        roomCode.text = "Joining Room...";
        string code = joinCodeInput.text;
        if(code.IsNullOrEmpty())
            return;
        await relayManager.StartClient(code);
        roomCode.gameObject.SetActive(true);
        roomCode.text = "Room Code: " + code;
    }
}