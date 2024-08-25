using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/*
 * -----------------------------------------------------------------------------
 *  Project:        RPG Game
 *  Script:         ChatManager.cs
 *  Description:    Handles the chat functionality in the game, including message
 *                  input, display, and network synchronization. The chat UI is 
 *                  updated in real-time and includes features like cursor lock 
 *                  management.
 * 
 *  Author:         [Your Name]
 *  Date:           [Date]
 *  Version:        1.0 (Debugging in Progress)
 * 
 *  Unity Version:  [Unity Version]
 *  Mirror Version: [Mirror Version]
 * 
 *  Status:         DEBUGGING IN PROGRESS
 *                  - Actively working on resolving issues related to network 
 *                    synchronization and UI behavior.
 *                  - Known issues include: [List any known issues, e.g., "Cursor
 *                    may not lock correctly after sending a message."]
 * 
 *  Usage:          Attach this script to a GameObject in your scene that 
 *                  contains the UI elements for the chat. Ensure that the 
 *                  necessary components (InputField, Text, ScrollRect) are 
 *                  properly assigned in the inspector.
 * 
 *  Notes:          - The script is in active development and debugging. Expect 
 *                    changes and possible issues as debugging progresses.
 *                  - This script relies on Mirror for networking and requires 
 *                    proper setup of networked objects in the scene.
 * 
 *  -----------------------------------------------------------------------------
 *  License:        This script is provided as-is during the debugging phase. 
 *                  Modify it freely for your project needs, but be aware that 
 *                  some features may not be fully functional.
 * -----------------------------------------------------------------------------
 */

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

    void OnInputFieldClicked(string input)
    {
        if (!isLocalPlayer) return;

        LockCursor(false);
        chatInputField.ActivateInputField(); // Activer le champ de saisie
    }

    void OnSubmit(string input)
    {
        if (!isLocalPlayer) return;

        if (EventSystem.current.currentSelectedGameObject == chatInputField.gameObject && !string.IsNullOrEmpty(input))
        {
            if (!string.IsNullOrWhiteSpace(input)) // Éviter l'envoi de messages vides ou contenant seulement des espaces
            {
                SendMessage();
            }
            LockCursor(true);
        }
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
