Unity-RPG-Game: Controller Scripts
This directory contains controller scripts for the Unity RPG project. There are two distinct controller scripts available, each designed for a different gameplay style. Due to the nature of these scripts, they cannot coexist within the same project simultaneously. You must choose one based on your desired gameplay perspective.

Controller Scripts
1. ThirdPersonController.cs
This script is designed for a third-person gameplay perspective. It handles character movement, animations, and camera control for a standard third-person experience. Use this script if you want your game to have a traditional third-person perspective.

2. TopDownIsometricController.cs
This script is tailored for a top-down isometric gameplay perspective. It modifies the character movement, animations, and camera control to fit a top-down view, often used in RPGs and strategy games. Choose this script if you prefer an isometric view for your game.

Important Note
Only one of these controller scripts should be active in your project at any given time.

If you choose to use the ThirdPersonController.cs, make sure to disable or remove TopDownIsometricController.cs from your project.
Conversely, if you opt for TopDownIsometricController.cs, ensure that ThirdPersonController.cs is not included or is disabled.
Mixing these two scripts will cause conflicts and errors, as they are designed to control the character and camera in fundamentally different ways.

How to Switch Controllers
To switch to ThirdPersonController.cs:

Remove or disable TopDownIsometricController.cs in your project.
Add and configure the ThirdPersonController.cs.
To switch to TopDownIsometricController.cs:

Remove or disable ThirdPersonController.cs in your project.
Add and configure the TopDownIsometricController.cs.
Make sure to test your game after switching controllers to ensure that everything is functioning as expected.

