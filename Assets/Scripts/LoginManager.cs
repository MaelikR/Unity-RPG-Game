using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class LoginManager : MonoBehaviour
{
    public InputField customIdInput;
    public Text feedbackText;
    public Text roleText;  // Ajoutez ce champ pour afficher le rôle
    public string titleId = ""; // Remplacez par votre TitleId

    //public Configuration configuration; // Ajout de la configuration pour le client et le serveur
    public NetworkManager networkManager;
    public GameObject[] networkObjectsToActivate; // Ajoutez une liste d'objets réseau à activer

    void Start()
    {
        // Définir le TitleId
        PlayFabSettings.TitleId = titleId;

        // Ignorer la validation des certificats (pour les tests uniquement)
        PlayFab.Internal.PlayFabWebRequest.SkipCertificateValidation();

        // Vérifier les références
        if (customIdInput == null)
        {
            UnityEngine.Debug.LogError("customIdInput is not assigned in the Inspector");
        }
        if (feedbackText == null)
        {
            UnityEngine.Debug.LogError("feedbackText is not assigned in the Inspector");
        }
        if (roleText == null)
        {
            UnityEngine.Debug.LogError("roleText is not assigned in the Inspector");
        }
    }

    public void OnLoginButtonClicked()
    {
        if (customIdInput == null)
        {
            feedbackText.text = "Input field is not set.";
            UnityEngine.Debug.LogError("customIdInput is not set in the Inspector");
            return;
        }

        string customId = customIdInput.text;

        if (string.IsNullOrEmpty(customId))
        {
            feedbackText.text = "Please enter a Custom ID.";
            return;
        }

        Login(customId);
    }

    void Login(string customId)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        feedbackText.text = "Logged in successfully";
        UnityEngine.Debug.Log("Logged in successfully");
        UnityEngine.Debug.Log("Your PlayFabId: " + result.PlayFabId);

        ExecuteCloudScript("ensureAdminRoleOnLogin", result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        feedbackText.text = "Login failed: " + error.GenerateErrorReport();
        UnityEngine.Debug.LogError("Login failed: " + error.GenerateErrorReport());
    }

    void ExecuteCloudScript(string functionName, string playFabId)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = functionName,
            FunctionParameter = new { playFabId = playFabId },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudScriptSuccess, OnCloudScriptFailure);
    }

    private void OnCloudScriptSuccess(ExecuteCloudScriptResult result)
    {
        feedbackText.text = "Cloud script executed successfully";
        UnityEngine.Debug.Log("Cloud script executed successfully");

        // Vérifier et afficher le rôle
        GetUserRole();
    }

    private void OnCloudScriptFailure(PlayFabError error)
    {
        feedbackText.text = "Cloud script execution failed: " + error.GenerateErrorReport();
        UnityEngine.Debug.LogError("Cloud script execution failed: " + error.GenerateErrorReport());
    }

    void GetUserRole()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayFabSettings.staticPlayer.PlayFabId
        }, OnGetUserDataSuccess, OnGetUserDataFailure);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
       // if (result.Data != null && result.Data.ContainsKey("UserRole"))
        //{
          //  string userRole = result.Data["UserRole"].Value;
          //  roleText.text = "Role: " + userRole;
           // UnityEngine.Debug.Log("User role: " + userRole);

            // Démarrer le client ou le serveur selon le rôle
            //if (userRole == "admin" && configuration.buildType == BuildType.LOCAL_SERVER)
            //{
                //ActivateNetworkObjects();
                //networkManager.StartServer();
            //}
          //  else if (userRole == "player" && configuration.buildType == BuildType.LOCAL_CLIENT)
           // {
              //  ActivateNetworkObjects();
              //  networkManager.StartClient();
          //  }
       // }
       // else
       // {
          //  roleText.text = "Role: unknown";
          //  UnityEngine.Debug.Log("User role not found");
      //  }
    }

    private void OnGetUserDataFailure(PlayFabError error)
    {
        roleText.text = "Failed to retrieve user role";
        UnityEngine.Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
    }

    private void ActivateNetworkObjects()
    {
        foreach (var obj in networkObjectsToActivate)
        {
            obj.SetActive(true);
        }
    }
}
