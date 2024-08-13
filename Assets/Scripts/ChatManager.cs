using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatManager : NetworkBehaviour
{
    public GameObject chatPanel; // Panel contenant les �l�ments UI du chat
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
            SetChatUIInteractive(true); // Activer les �l�ments interactifs du UI du chat pour le joueur local
        }
        else
        {
            SetChatUIInteractive(false); // D�sactiver les �l�ments interactifs du UI du chat pour les joueurs distants
        }
    }

    void SetChatUIInteractive(bool isInteractive)
    {
        if (chatPanel != null)
        {
            chatInputField.enabled = isInteractive; // Activer/d�sactiver l'interaction avec le champ de saisie
            // D�sactiver d'autres composants interactifs si n�cessaire
        }
    }

    void OnSubmit(string input)
    {
        if (!isLocalPlayer) return;

        // V�rifie si la touche Enter a �t� press�e et si le champ de saisie a perdu le focus
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
            scrollRect.verticalNormalizedPosition = 0f; // Assurez-vous que la position du d�filement est mise � jour correctement.
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
