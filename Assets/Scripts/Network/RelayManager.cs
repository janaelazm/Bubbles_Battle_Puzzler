using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    public event Action<string> OnStatusChanged;
    public event Action<string> OnRoomCodeGenerated;


    public async Task<string> StartHost()
    {
        try
        {
            OnStatusChanged?.Invoke("Generating room...");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("JOIN CODE: " + joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            OnRoomCodeGenerated?.Invoke(joinCode);
            OnStatusChanged?.Invoke("Waiting for player to join...");


            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                Debug.Log("Client connected: " + clientId);

                OnStatusChanged?.Invoke("Player joined!");

                NetworkManager.Singleton.SceneManager.LoadScene(
                    "PathSelection",
                    LoadSceneMode.Single
                );
            };


            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnStatusChanged?.Invoke("Failed to create room.");
            throw;
        }
    }


    public async Task StartClient(string joinCode)
    {
        try
        {
            OnStatusChanged?.Invoke("Joining room...");

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode.Trim());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();

            OnStatusChanged?.Invoke("Connected!");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnStatusChanged?.Invoke("Failed to join room.");
            throw;
        }
    }
}