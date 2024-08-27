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
 *  Author:         M.Ren
 *  Date:           [25/08/2024]
 *  Version:        1.0 (Debugging in Progress)
 * 
 *  Unity Version:  [2021.3.8]
 *  Mirror Version: [Mirror 2022.9.15]
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
    public InputField chatInputField;
    public Text chatDisplay;
    public ScrollRect scrollRect;

    void Start()
    {
        if (isLocalPlayer)
        {
            chatInputField.onEndEdit.AddListener(HandleChatSubmit);
        }
    }

    void HandleChatSubmit(string input)
    {
        if (!isLocalPlayer || string.IsNullOrWhiteSpace(input)) return;

        SendMessageToServer(input);
        chatInputField.text = string.Empty;
        chatInputField.ActivateInputField(); // Keep the input field active after sending the message
    }

    [Command]
    void SendMessageToServer(string message)
    {
        if (NetworkClient.isConnected)
        {
            string formattedMessage = $"[{NetworkClient.connection.identity.netId}] {message}";
            RpcReceiveMessage(formattedMessage);
        }
        else
        {
            Debug.LogWarning("Tried to send a message but the client is not connected.");
        }
    }


    [ClientRpc]
    void RpcReceiveMessage(string message)
    {
        if (chatDisplay != null)
        {
            chatDisplay.text += message + "\n";
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f; // Scroll to the bottom
            }
        }
        else
        {
            Debug.LogError("Chat display or Scroll Rect is not assigned.");
        }
    }
   
}
