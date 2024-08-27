Project Overview
This project is an RPG game developed with Unity, utilizing Mirror for network management. It includes several scenes and assets organized to facilitate the development of a multiplayer game.

Main Scene: MAINGame
The MAINGame scene is the core scene of the project. It contains all the necessary elements for the main game, including user interfaces, environment elements, and network systems.

1. Key GameObjects in MAINGame
CanvasMain:
Contains the main user interface elements, such as text fields and buttons for the login interface.
InputField (Legacy): Text input field for user input, primarily used for login or other text interactions.

ImageBG: Background image for the canvas, likely used for the login interface or other menus.

TerrainGroup:
Includes terrain and environmental design elements for the scene. Used to create the environment where players interact.

CanvasMenu:
Manages the game menus, including the login panel (LoginPanel).

LoginPanel: Panel used for player login interfaces.
NET (Network Systems):
EventSystem: Essential component for managing UI events in Unity.

ObjectiveM: Game objective,related to managing player objectives (likely where the ObjectiveManager script is attached).
Global Volume: Used for managing post-processing effects in the scene.
Ocean: Asset to simulate an ocean or large water body in the scene.

CanvasIntro:
Used to manage the introduction or startup screens.
Contains multiple camera objects (GameObjectCAM01, etc.), used to create video sequences or cinematic effects.

CanvasVIDEOINTRO:
Used to play an introductory video.
IntroVideo: GameObject linked to displaying an introductory video when the game starts.
2. Assets and Prefabs

PrefabMain:
Contains essential prefabs for the game, such as environments, interactive objects, or user interface elements.

WOMAN Folder:
Contains specific assets for female models or characters.
