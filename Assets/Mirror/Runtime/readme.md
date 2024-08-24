Unity-RPG-Game: Mirror Runtime Scripts
This directory contains custom runtime scripts for the Unity RPG project that are designed to replace specific default scripts provided by the Mirror networking library. These custom scripts are tailored to fit the needs of this project and must be used instead of the default Mirror scripts.

Important Instructions
Before installing Mirror, you must take the following steps:

Backup Existing Scripts:

Before installing Mirror, move the existing scripts (NetworkStartPosition.cs, NetworkStartPositionManager.cs, Player.cs, PlayerData.cs) from this directory to a safe location outside the Unity project. This will prevent them from being overwritten during the installation of Mirror.
Install Mirror:

Install the Mirror networking library into your Unity project as you normally would.
Replace Default Mirror Scripts:

After Mirror is installed, copy the scripts from this directory (Runtime) back into the Assets/Mirror/Runtime directory of your project.
This action will overwrite the default Mirror scripts with the custom scripts specific to this project.
Custom Scripts Overview
NetworkStartPosition.cs: Custom logic for network start positions tailored for this RPG.
NetworkStartPositionManager.cs: Manages the custom network start positions within the game environment.
Player.cs: Handles player-related networking logic, including custom data syncing and interactions.
PlayerData.cs: Manages the player data structure with custom attributes relevant to this RPG.
Note: These scripts are essential for the proper functioning of the RPG project with Mirror. Ensure that the replacement process is done carefully to avoid issues during runtime.
