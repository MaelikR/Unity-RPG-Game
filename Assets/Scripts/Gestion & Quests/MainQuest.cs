using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MainQuest : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnQuestStatusChange))]
    private bool questCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isServer && other.CompareTag("Player"))
        {
            RpcShowQuestMessage(other.gameObject);
        }
    }

    [ClientRpc]
    private void RpcShowQuestMessage(GameObject player)
    {
        // Display quest message to the player
        UIController.Instance.ShowQuestMessage("Hey adventurer! The kingdom is in danger. Will you accept the quest to defeat the final boss and save the realm?");

        // Simulate player response (you can implement a UI for this)
        bool playerAcceptsQuest = true;

        if (playerAcceptsQuest)
        {
            // Player accepted the quest
            questCompleted = false;
            UIController.Instance.ShowQuestMessage("Great! The fate of the kingdom rests in your hands. Go defeat the final boss!");

            // You can add additional logic here, like updating UI, setting quest markers, etc.
        }
        else
        {
            // Player declined the quest
            UIController.Instance.ShowQuestMessage("Oh well, maybe next time. Farewell!");
        }
    }

    [Command]
    public void CmdCompleteQuest()
    {
        // Called when the player completes the quest by defeating the final boss
        if (!questCompleted)
        {
            UIController.Instance.ShowQuestCompletionMessage("Congratulations! The final boss has been defeated. The kingdom is saved.");

            // Add logic to reward the player (increase currency, give items, etc.)
            RpcRewardPlayer();

            // Mark the quest as completed
            questCompleted = true;

            // End the game
            EndGame();
        }
    }

    [ClientRpc]
    private void RpcRewardPlayer()
    {
        // Add additional logic here, such as increasing the player's currency, giving items, etc.
        // You can also trigger events to update the UI for rewards.
        UIController.Instance.ShowRewardMessage("Thank you for your bravery! Here's your reward.");
    }

    private void OnQuestStatusChange(bool oldValue, bool newValue)
    {
        // This method is called when the quest status changes (completed or not)
        // You can implement additional logic here, like updating UI or triggering events.
        if (newValue)
        {
            // The quest is completed, you can add specific end quest logic here.
            UIController.Instance.ShowQuestCompletionMessage("The main quest is completed!");
        }
    }

    private void EndGame()
    {
        // Add logic to end the game (display an end screen, etc.)
        if (isServer)
        {
            UIController.Instance.ShowEndGameScreen("Game Over!");

            // Load the end screen scene (make sure to adjust the scene name based on your configuration)
            SceneManager.LoadScene("EndScreen");
        }
    }
}
