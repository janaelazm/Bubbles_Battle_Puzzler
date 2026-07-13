using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class Authenticator : MonoBehaviour
{
    public async Task Init()
    {
        try
        {
            Debug.Log("Initializing Unity Services...");
            await UnityServices.InitializeAsync();

            SetupEvents();

            Debug.Log("Signing in anonymously...");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };
    }

    public string GetPlayerId()
    {
        return AuthenticationService.Instance.PlayerId;
    }
}