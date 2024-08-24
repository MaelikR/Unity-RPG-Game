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

ThirdPersonController Script
The ThirdPersonController script is a crucial component for managing the movement, rotation, and interactions of the player character in a 3D environment. This controller is designed to work in multiplayer scenarios using Mirror and leverages Cinemachine for camera control.

Main Features
Ground and Flight Movement: The script handles both ground and flight movement, allowing the player to switch between the two modes. The movement is smooth and responsive, with mouse input controlling the rotation.
Gravity Management: A realistic gravity system is included, ensuring the character falls back to the ground when not flying.
Multiplayer Support with Mirror: The script is designed to function in a multiplayer environment, ensuring that each player only controls their own character.
Integration with Cinemachine: Utilizes Cinemachine to manage multiple cameras, including follow and orbital cameras, providing an enhanced user experience.
Synchronized Animations: Character animations are synchronized with actions such as walking, flying, swimming, and jumping.
Sound and Audio Feedback: The script also manages sounds associated with player actions, providing immersive audio feedback.
Feature Details
Ground Movement:

The player’s ground movement is controlled using keyboard inputs (WASD for movement) and mouse input for rotation. The character rotates based on the camera’s orientation, creating a fluid and natural control experience.
The walk and sprint speeds can be configured via the walkSpeed and sprintSpeed parameters.
The camera follows the character with a slight delay, offering a smoother visual experience.
Flight Movement:

Flight mode is activated by pressing a specific key (Fly). In flight, the character can move in all directions (up, down, forward, backward, left, right), with rotation controlled by the mouse.
The flight speed can be configured via the flySpeed parameter.
While in flight, gravity is temporarily disabled, allowing the character to remain airborne until the mode is deactivated.
Gravity Management:

Gravity is automatically applied when the character is not flying or swimming. If the character is flying and the mode is deactivated, they will gradually fall back to the ground.
The gravity force can be adjusted using the gravity parameter.
Camera Control:

Cinemachine is used to provide dynamic camera experiences. The script allows switching between multiple virtual cameras, such as the follow camera for ground movement and an orbital camera for a wider perspective.
Camera sensitivity can be adjusted via the mouseSensitivity parameter.
Animations and Audio Feedback:

The script uses an Animator to synchronize character animations with actions such as walking, jumping, flying, and swimming.
Specific sounds are played for each significant action, adding an extra layer of immersion.
Multiplayer Support:

Thanks to Mirror, each player controls only their own character in a multiplayer environment. Unnecessary components (such as cameras and audio sources) are disabled for remote instances.
How to Use
Installation:

Add the ThirdPersonController script to your character in Unity.
Ensure that the character has the CharacterController, Animator, AudioSource components, and that cameras are correctly configured with Cinemachine.
Check the layer settings for the ground and water masks to ensure correct detection.
Configuration:

Adjust the walk, sprint, and flight speeds as needed.
Configure the sounds for each of the character's actions.
Customize the camera settings through Cinemachine to match your gameplay vision.
Execution:

Once configured, the script automatically handles the character’s movements and interactions. It is ready to function in a multiplayer environment thanks to the integration with Mirror.
