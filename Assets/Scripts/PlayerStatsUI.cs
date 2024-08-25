using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem.XR;
/*
 * -----------------------------------------------------------------------------
 *  Project:        RPG Game
 *  Script:         PlayerStatsUI.cs
 *  Description:    Manages the player's stats UI, allowing the player to toggle 
 *                  the visibility of the stats panel and update the displayed 
 *                  values based on the player's current attributes.
 * 
 *  Author:         [M,Ren]
 *  Date:           [25/08/2024]
 *  Version:        1.0 (Development and Debugging in Progress)
 * 
 *  Unity Version:  [2021.3.8]
 *  Mirror Version: [Mirror 2022.9.15]
 * 
 *  Status:         DEVELOPMENT AND DEBUGGING IN PROGRESS
 *                  - The script is under active development, focusing on 
 *                    connecting the UI with player stats and ensuring network 
 *                    synchronization.
 *                  - Known issues include: [List any known issues, e.g., "Stats 
 *                    may not update correctly if the player's data changes 
 *                    dynamically during gameplay."]
 * 
 *  Usage:          Attach this script to the player GameObject in the scene. 
 *                  Ensure that the PlayerStatsPanel GameObject and its 
 *                  corresponding UI Text components are correctly assigned in 
 *                  the inspector.
 * 
 *  Notes:          - The script assumes the presence of a `ThirdPersonController` 
 *                    component that provides access to the player's stats.
 *                  - Make sure that the player has the necessary UI elements and 
 *                    that the network setup is correctly configured.
 * 
 *  -----------------------------------------------------------------------------
 */
public class PlayerStatsUI : NetworkBehaviour
{
    public GameObject playerStatsPanel;
    public Text manaText;
    public Text strengthText;
    public Text agilityText;
    public Text intelligenceText;

    private ThirdPersonController controller;

    void Start()
    {
        playerStatsPanel.SetActive(false);

        if (isLocalPlayer)
        {
            controller = GetComponent<ThirdPersonController>();
        }
    }

    void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.K))
        {
            ToggleStatsPanel();
        }
    }

    void ToggleStatsPanel()
    {
        playerStatsPanel.SetActive(!playerStatsPanel.activeSelf);
        if (playerStatsPanel.activeSelf && controller != null)
        {
            UpdateStats();
        }
    }

    void UpdateStats()
    {
        var playerData = controller.GetPlayerData();  // Use the getter method
        if (playerData != null)
        {
            manaText.text = $"Mana: {playerData.Mana}";
            strengthText.text = $"Strength: {playerData.Strength}";
            agilityText.text = $"Agility: {playerData.Agility}";
            intelligenceText.text = $"Intelligence: {playerData.Intelligence}";
        }
    }
}
