using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    
    [SerializeField] private Authenticator authenticator;
    [SerializeField] private NetworkUI netowrkUI;
    [SerializeField] private PlayerProfileUI playerProfileUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        try
        {
            await authenticator.Init();
            PlayerProfile.Instance.Load();
            netowrkUI.initNetworkUI();
            playerProfileUI.initPlayerUI();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
