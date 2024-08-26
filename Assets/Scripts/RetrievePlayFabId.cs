using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class RetrievePlayFabId : MonoBehaviour
{
    public void Start()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = "",
            CreateAccount = true
        }, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged in successfully");
        Debug.Log("Your PlayFabId: " + result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }
}
