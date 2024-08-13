using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

using System.Diagnostics;

public class NPCDialogue : NetworkBehaviour
{
    public Text dialogueText; // Reference to the UI Text object for displaying dialogue
    public string[] dialogueMessages; // Dialogue messages to display
    public float idleTime = 2.0f; // Time for which the NPC stays idle after dialogue

    private int currentMessageIndex = 0;
    private bool isDialogueActive = false;
    private bool isPlayerNearby = false; // Indicates if the player is nearby

    private void Start()
    {
        // Hide dialogue text if this is not the local player
        if (!isLocalPlayer)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for input and proximity to trigger dialogue
        if (isLocalPlayer && isPlayerNearby && Input.GetKeyDown(KeyCode.G) && !isDialogueActive)
        {
            CmdStartDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLocalPlayer)
            {
                isPlayerNearby = true; // Player is nearby
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLocalPlayer)
            {
                isPlayerNearby = false; // Player has left the area
            }
        }
    }

    [Command]
    private void CmdStartDialogue()
    {
        RpcShowDialogue();
    }

    [ClientRpc]
    private void RpcShowDialogue()
    {
        // Ensure that only the local player sees the dialogue
        if (isLocalPlayer)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    private IEnumerator ShowDialogue()
    {
        isDialogueActive = true;
        dialogueText.gameObject.SetActive(true);

        // Display each message one by one
        while (currentMessageIndex < dialogueMessages.Length)
        {
            dialogueText.text = dialogueMessages[currentMessageIndex];
            currentMessageIndex++;
            yield return new WaitForSeconds(4.0f); // Adjusted wait time to ensure readability
        }

        // Wait a bit before returning to idle state
        yield return new WaitForSeconds(idleTime);
        dialogueText.gameObject.SetActive(false);
        currentMessageIndex = 0; // Reset index for the next interaction
        isDialogueActive = false;
    }

    private void OnValidate()
    {
        // Ensure that dialogueText and dialogueMessages are not null
        if (dialogueText == null)
        {
            UnityEngine.Debug.LogWarning("dialogueText is not assigned.");
        }

        if (dialogueMessages == null || dialogueMessages.Length == 0)
        {
            UnityEngine.Debug.LogWarning("dialogueMessages is not assigned or empty.");
        }
    }
}
