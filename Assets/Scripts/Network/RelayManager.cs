using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
public class RelayManager : MonoBehaviour
{
    public async Task<string> StartHost()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("JOIN CODE: " + joinCode);
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            Debug.Log("Host started");
            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    public async Task StartClient(string joinCode)
    {
        try{
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
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }
}
