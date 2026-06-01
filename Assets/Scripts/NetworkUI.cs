using TMPro;
using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;

    public RelayManager relayManager;

    public void StartHost()
    {
        relayManager.StartHost();
    }

    public void StartClient()
    {
        string code = joinCodeInput.text;
        Debug.Log("joinging game:" + code);
        relayManager.StartClient(code);
    }
}