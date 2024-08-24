Unity-RPG-Game: Scripts Directory
This directory contains the essential scripts that drive various aspects of the Unity RPG game. These scripts handle everything from AI behavior to server management, player interactions, and more. Below is an overview of the key scripts included in this directory, along with their purposes, current status, and any known issues.

Core Scripts
1. Configuration.cs
Purpose: Defines the core configuration settings for the game, including build types (local client, remote client, local server, remote server), IP address, port, and PlayFab debugging options.
Details: This script is crucial for setting up different environments (e.g., server, client) in your game. It allows for flexible configuration based on where the game is being run (locally or remotely).
Note: Ensure that all configuration fields are correctly set before running the game to avoid connection issues.
2. BossAI.cs
Purpose: Controls the AI behavior of boss enemies in the game. Handles patrolling, chasing the player, attacking, and respawning. Also manages health, ground checks, and obstacle detection.
Features:
Patrol between waypoints.
Chase and attack the player within a certain range.
Strafe and dodge mechanics for more dynamic combat.
Respawn mechanism after death.
Background music and sound effects management.
Details: This script integrates multiple AI states (idle, patrolling, chasing, attacking) and transitions smoothly between them. The boss character can also interact with obstacles in the environment, adding a layer of realism to its behavior.
Note: This script is complex and includes various AI states and transitions. Proper debugging is needed to ensure smooth behavior in all scenarios. Consider stress-testing the AI in different environments to ensure robustness.
3. ArtifactVideoTrigger.cs
Purpose: Triggers a video when the player interacts with an artifact in the game. Also unlocks a spell for the player and plays a sound effect.
Features:
Plays a video when the player enters a trigger zone.
Unlocks a new spell for the player.
Plays a designated sound effect upon activation.
Details: The script ensures that the video and sound effects are synchronized with the player's actions, providing a seamless experience. It also disables the trigger after use to prevent repeated activations.
Known Issues: Ensure that the VideoPlayer and AudioSource components are correctly configured in the Unity editor to avoid null references. Additional debugging may be required if the video or sound does not play as expected.
4. ServerStartUp.cs
Purpose: Manages the startup process for both local and remote servers. Integrates with PlayFab for multiplayer server management and handles server lifecycle events like maintenance and shutdown.
Features:
Supports remote and local server configurations.
Integration with PlayFab Multiplayer Agent API.
Automated server shutdown after a certain period or based on player count.
Details: This script allows for flexible server management, supporting both local development environments and production environments using PlayFab. The script handles various server events, ensuring that the server runs smoothly and shuts down gracefully when necessary.
Note: This script is highly dependent on external services (PlayFab). Thorough testing is required to ensure compatibility with your network setup.
5. Health.cs
Purpose: Manages the health system for both players and enemies. It includes damage taking, healing, death, and respawn mechanisms.
Features:
Health management with UI updates.
Damage and healing logic.
Player and enemy respawn handling.
Details: The script is designed to be flexible, allowing it to be used for both players and enemies. It integrates with the game's UI to provide real-time feedback on health status, and it supports multiplayer scenarios by synchronizing health changes across the network.
Known Issues: Make sure to test the respawn functionality thoroughly, especially in multiplayer scenarios where synchronization can be an issue. Debugging may be necessary to handle edge cases where health updates do not propagate correctly.
6. ChatManager.cs (Currently Under Debugging)
Purpose: Manages the in-game chat functionality, allowing players to communicate with each other in a multiplayer environment. It ensures that chat messages are sent, received, and displayed correctly across all clients.
Features:
Handles player input for chat messages.
Synchronizes chat messages across the network using Mirror's Command and ClientRpc.
Manages the chat UI, including input fields and scrolling functionality.
Includes cursor lock management to ensure the chat is easy to use during gameplay.
Details:
The ChatManager script is designed to be intuitive for players, allowing them to quickly enter and send messages while playing. It also manages the chat panel's visibility and interactivity based on whether the player is local or remote.
The script ensures that chat messages are properly displayed in the UI, including handling the scrolling behavior to keep the latest messages visible.
Current Status: This script is under active debugging. Known issues include occasional synchronization problems where messages may not appear correctly on all clients or UI components not behaving as expected. Additional testing is required to ensure that all chat features function reliably in a live multiplayer environment.
Planned Improvements:
Further UI enhancements to improve the chat experience.
Better error handling to manage cases where network connectivity is unstable.
Potential integration with external chat services for more robust functionality.
Additional Utility Scripts
1. CameraCollisionHandler.cs
Purpose: Prevents the camera from clipping through objects in the game world. Adjusts the camera position dynamically to ensure visibility of the player.
Details: This script is essential for maintaining a clear view of the player character, particularly in environments with complex geometry.
2. ClientStartUp.cs
Purpose: Handles the initialization and connection process for the game client, including setting up network connections.
Details: This script ensures that the client is correctly initialized and connected to the server, handling any connection errors gracefully.
3. ExampleNetworkAuthenticator.cs
Purpose: An example script for handling network authentication, which may need customization based on your specific requirements.
Details: This script provides a basic template for authenticating players in a multiplayer environment. It can be extended to include more advanced authentication mechanisms.
4. FFAAI.cs
Purpose: Controls the AI for Friendly Forces that assist the player in combat scenarios.
Details: This script adds a layer of strategic depth to the game by allowing NPCs to assist the player during battles.
5. GUIDUtility.cs
Purpose: A utility script for generating and managing GUIDs (Globally Unique Identifiers) within the game.
Details: This script is useful for ensuring that game objects and players have unique identifiers, which is particularly important in networked games.
Quests and Game Management
1. EnableComponents.cs
Purpose: Dynamically enables or disables specific components in the game, often used to manage game states or player progress.
Details: This script can be used to toggle components on and off based on certain conditions, such as completing a quest or reaching a specific point in the game.
2. GestionImages.cs
Purpose: Manages in-game images, such as icons and textures, for UI elements.
Details: This script handles the loading and management of images used throughout the game's UI, ensuring they are displayed correctly.
3. MainQuest.cs
Purpose: Manages the main questline, tracking player progress and objectives throughout the game.
Details: This script is the backbone of the game's narrative, ensuring that the player progresses through the story in a structured manner.
4. UIController.cs
Purpose: Centralized management of the game's user interface, ensuring all UI elements are synchronized and updated based on player actions.
Details: This script is essential for maintaining a consistent and responsive user interface, particularly in a complex game with many different UI elements.
Known Issues and Debugging Notes
Script Functionality: Some scripts in this directory may not be fully functional or may require debugging. It is important to test each script in the context of the full game to identify and resolve any issues.

Network Synchronization: For scripts involving network interactions (e.g., Health.cs, ChatManager.cs), ensure that all network-related logic is thoroughly tested to prevent synchronization issues.

Dependency Management: Some scripts, like ServerStartUp.cs, are dependent on external services (e.g., PlayFab). Ensure all external dependencies are properly configured and integrated.

Customization: Scripts like ExampleNetworkAuthenticator.cs are templates and may need to be customized to fit the specific needs of your game.
