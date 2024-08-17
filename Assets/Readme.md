My Custom Game Project "RPG GAME - Unity Multiplayer Adventure"
This project is based on the example from natepac/playfabmirrorgameexample and includes additional custom features that enhance the gameplay and visual experience.

Setup Instructions

Clone the Repository: First, clone the repository to your local machine:

(A): git clone https://github.com/MaelikR/playfabmirrorgameexample.git Or clone this depot: git clone https://github.com/natepac/playfabmirrorgameexample.git (B): Next add folder Asset from here: https://github.com/MaelikR/Unity-RPG-Game/archive/refs/heads/main.zip (C): Open the project in Unity 2021.3.8f1 or Unity 2021.3.39f1 (Note: Other higher versions of unity do not work properly with the original depot backend you need to customize the code and update the Playfab plus Mirror API and custom configuration files). (D): Follow the steps in the repository for the Playfab, Mirror configuration. (E) Video Tutorial (Part 1): https://youtu.be/Q90bylFXtNY 10. Install Dependencies Mirror: Download and import Mirror from the Unity Asset Store. PlayFab SDK: Download the PlayFab SDK and import it into your Unity project. TextMesh Pro: Download and import TextMesh Pro from the Unity Asset Store. Procedural Terrain Painter: Download and import Procedural Terrain Painter from the Unity Asset Store.

Install Dependencies (Secondary method)
Mirror:

Mirror is the networking library that powers the multiplayer functionality of the game. It simplifies the creation of networked games by providing a robust framework for handling networked objects and communication.
Download and import Mirror from the Unity Asset Store.

PlayFab SDK:

PlayFab is used for backend services, including user authentication, data storage, and analytics. It provides scalable cloud services essential for managing game data and player accounts.
Download the PlayFab SDK and import it into your Unity project.

TextMesh Pro:

TextMesh Pro is used for high-quality text rendering and UI elements. It offers advanced text formatting features, making your UI more visually appealing.
Download and import TextMesh Pro from the Unity Asset Store.

Procedural Terrain Painter:

Procedural Terrain Painter allows you to create detailed and customizable terrains. It simplifies the process of painting textures like grass, soil, and rocks onto your terrain, making the environment more dynamic and visually appealing.
Download and import Procedural Terrain Painter from the Unity Asset Store.

ThirdPersonController:

This asset provides a ready-to-use third-person character controller, which can be customized to suit your game's needs. It handles movement, camera control, and basic interactions.
Download and import ThirdPersonController from the Unity Asset Store.

Cinemachine:

Cinemachine is a powerful camera system that allows for dynamic and cinematic camera control. For this project, it is used to enhance the third-person camera experience, offering smooth transitions, tracking, and framing.
Download and import Cinemachine from the Unity Asset Store.

Fast Sky:

Fast Sky provides a realistic and performance-optimized skybox for your game. It simulates realistic sky conditions, including sun, clouds, and atmospheric scattering, creating an immersive environment.
Download and import Fast Sky from the Unity Asset Store.

Simple Water Material:

This asset provides a basic water material with realistic shading effects, such as reflections and refractions. It is ideal for creating bodies of water that interact naturally with light, enhancing the visual quality of your environments.
Download and import Simple Water Material from the Unity Asset Store.
Import Assets
After downloading the assets, import Mirror, PlayFab SDK, TextMesh Pro, Procedural Terrain Painter, ThirdPersonController, Cinemachine, Fast Sky, and Simple Water Material into your Unity project.

Configure PlayFab
PlayFab Setup:
Sign up for PlayFab and create a new title for your game.
In Unity, navigate to Window > PlayFab > Editor Extensions > Settings.
Enter your PlayFab Title ID and Secret Key.
Follow the PlayFab setup guide here to configure user authentication, data storage, and other backend services.
Configure Mirror
Mirror Setup:
Set up the Network Manager in your main scene to handle networked objects and player connections.
Configure your scenes for networked play by adding the necessary components and ensuring that objects are correctly set up as networked prefabs.
Follow the Mirror setup guide here for detailed instructions.
Run the Project
Running the Scene:
Open the Unity project and load the main scene.
Ensure that all configurations for PlayFab and Mirror are completed.
Press the Play button in Unity to run the game and test the multiplayer functionality, camera controls, and environmental effects.
Custom Features
This project includes several custom features that enhance the original example:

Cinemachine-Enhanced Camera: The third-person camera system has been upgraded using Cinemachine, offering dynamic and smooth camera movements that adapt to the player’s actions and the environment.

Realistic Sky with Fast Sky: The environment features a realistic sky created with Fast Sky, simulating natural light conditions and enhancing the overall immersion of the game world.

Simple Water Material: Bodies of water within the game use the Simple Water Material, providing realistic reflections and light interactions, making the environment more visually appealing.

Procedural Terrain Customization: The terrain is dynamically painted using Procedural Terrain Painter, allowing for detailed and varied landscapes, including custom textures for grass, soil, and rocks.

Credits
This project uses code and concepts from:

Mirror Networking:
https://assetstore.unity.com/packages/tools/network/mirror-129321

PlayFab SDK:
https://docs.microsoft.com/en-us/gaming/playfab/

TextMesh Pro:
https://assetstore.unity.com/packages/essentials/beta-projects/textmesh-pro-84126

Procedural Terrain Painter:
(https://assetstore.unity.com/packages/tools/terrain/procedural-terrain-painter-188357)


Fast Sky:
https://assetstore.unity.com/packages/tools/visual-scripting/fast-sky-144052

Simple Water Shader Material:
(https://assetstore.unity.com/packages/2d/textures-materials/water/simple-water-shader-urp-191449)

natepac/playfabmirrorgameexample (GitHub Repository):
https://github.com/natepac/playfabmirrorgameexample

Mirror Networking Documentation:
https://mirror-networking.com/docs/

PlayFab Setup Guide:
https://docs.microsoft.com/en-us/gaming/playfab/

License
Include any licensing information here.
M.RenDev
