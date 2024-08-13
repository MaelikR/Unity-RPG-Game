using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatManager : NetworkBehaviour
{
    public GameObject chatPanel; // Panel contenant les éléments UI du chat
    public InputField chatInputField;
    public Text chatDisplay;
    public ScrollRect scrollRect;

    private bool isCursorLocked = true;

    void Start()
    {
        if (isLocalPlayer)
        {
            chatInputField.onSubmit.AddListener(OnSubmit);
            chatInputField.onValueChanged.AddListener(OnInputFieldClicked);
            SetChatUIInteractive(true); // Activer les éléments interactifs du UI du chat pour le joueur local
        }
        else
        {
            SetChatUIInteractive(false); // Désactiver les éléments interactifs du UI du chat pour les joueurs distants
        }
    }

    void SetChatUIInteractive(bool isInteractive)
    {
        if (chatPanel != null)
        {
            chatInputField.enabled = isInteractive; // Activer/désactiver l'interaction avec le champ de saisie
            // Désactiver d'autres composants interactifs si nécessaire
        }
    }

    void OnSubmit(string input)
    {
        if (!isLocalPlayer) return;

        // Vérifie si la touche Enter a été pressée et si le champ de saisie a perdu le focus
        if (EventSystem.current.currentSelectedGameObject == chatInputField.gameObject && !string.IsNullOrEmpty(input))
        {
            SendMessage();
            LockCursor(true);
        }
    }

    void OnInputFieldClicked(string input)
    {
        if (!isLocalPlayer) return;

        LockCursor(false);
    }

    public void SendMessage()
    {
        if (!isLocalPlayer || string.IsNullOrEmpty(chatInputField.text))
            return;

        string message = $"[{NetworkClient.connection.identity.netId}] {chatInputField.text}";
        UnityEngine.Debug.Log($"Sending message: {message}");
        CmdSendMessage(message);
        chatInputField.text = string.Empty;
        chatInputField.ActivateInputField();
    }

    [Command]
    void CmdSendMessage(string message)
    {
        UnityEngine.Debug.Log($"CmdSendMessage called with message: {message}");
        RpcReceiveMessage(message);
    }

    [ClientRpc]
    void RpcReceiveMessage(string message)
    {
        UnityEngine.Debug.Log($"RpcReceiveMessage called with message: {message}");

        if (chatDisplay != null)
        {
            chatDisplay.text += message + "\n";
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // Assurez-vous que la position du défilement est mise à jour correctement.
        }
    }

    private void LockCursor(bool lockCursor)
    {
        isCursorLocked = lockCursor;
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }

    private void ToggleCursorLock()
    {
        LockCursor(!isCursorLocked);
    }
}
