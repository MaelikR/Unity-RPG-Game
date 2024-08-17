# Unity-RPG-Game
Welcome to the RPG GAME project! This repository contains the complete setup for a multiplayer RPG game built using Unity, Mirror for networking, and PlayFab for backend services. Follow the instructions below to get started, configure the project, and build both the client and server components.
![image](https://github.com/user-attachments/assets/1361b09d-49da-45a5-8cbe-b87d7a4f4b2f)


|1.Table of Contents

|2.Features

|3.Configure PlayFab

|4.Clone the Repository

|5.Building the Project

|6.Client Build

|7.Server Build

|8.Custom Features

|9.Setup Instructions

|10.Install Dependencies

|11.Video Tutorial (Part 1)

|12.Credits

|13.License


RPG GAME Project
Welcome to the RPG GAME project! This document will guide you through the various aspects of the project, from setup instructions to detailed feature descriptions, ensuring you have everything you need to get started and understand the project's structure.


1. Table of Contents
The Table of Contents provides a structured outline of the document.

2. Features
RPG GAME boasts several key features that make it an exciting and engaging experience:

-RPG Stats:

-Custom spawn by faction, level etc....

-Fly movement with "F" key

-Water system (simple water system)

-Sword attack with "E" key

-PvP sauvage for fun with "G" key :)

-Full customisable netwoked third person controller with camera cinemachine

-Spell attack with TAB lock (in-progress) + left click

-Cloud Script with full customisable backend playfab and 98/h per month of free Dasv8 for testing build remote server

-TriggerBox with area effet camera travelling "Lore" mode

-SkeletonAI (skeleton of AI structure full customisable) for basic enemy with Navmesh of Unity Engine

-FFAI (testing AI neutral at debug)

-Day/Night with extensible Fast Sky shader

3. Configure PlayFab
Setting up PlayFab involves several steps:

Sign up for PlayFab and create a new title.

In Unity, navigate to Window > PlayFab > Editor Extensions > Settings.

Enter your PlayFab Title ID and Secret Key.

Follow the PlayFab setup guide to configure authentication and backend services.

This integration enables powerful backend functionalities such as user authentication and data management.

4. Configure Mirror
To enable multiplayer functionality, you need to set up Mirror:

Set up the Network Manager in your main scene.

Configure networked objects and components.

Ensure proper scene management for multiplayer functionality.

Follow the Mirror documentation for detailed setup instructions.

Mirror simplifies the process of adding multiplayer features to your game.

5. Building the Project
This section explains how to build the project for different platforms. You will create executable files that can be distributed and run on various devices.

6. Client Build
To build the client-side of the project:

Open the Build Settings window in Unity (File > Build Settings).

Select your target platform (e.g., PC, Mac, & Linux Standalone).

Add your main scene to the build by clicking Add Open Scenes.

Configure Player Settings as needed.

Click Build and choose a location to save the client build.

This creates the executable file that players will use to play the game.

![image](https://github.com/user-attachments/assets/f7c6cc52-553a-4b41-aae0-6501bfbb1266)

7. Server Build
To build the server-side of the project:

Open the Build Settings window in Unity (File > Build Settings).

Select your target platform.

Enable the Server Build option.

Add your main scene to the build.

Configure Player Settings, ensuring Headless Mode is enabled.

Click Build and choose a location to save the server build.

This creates an executable optimized for running on a server to handle multiplayer sessions.

8. Custom Features
Our RPG GAME includescustom features (Unity Default Assets:

Dynamic Terrains: Detailed and customizable terrains created with Procedural Terrain Painter.

Advanced UI: Enhanced text and UI elements using TextMesh Pro.

Multiplayer Integration: Seamless multiplayer experience powered by Mirror.

Backend Services: Robust backend functionalities with PlayFab integration.

Multiplayer functionality using Mirror

Backend services powered by PlayFab

Detailed terrains with Procedural Terrain Painter

Enhanced text rendering with TextMesh Pro

Third-person character controller

Seamless integration and easy setup.

9. Setup Instructions

Clone the Repository:
First, clone the repository to your local machine:

(A) 
git clone https://github.com/MaelikR/playfabmirrorgameexample.git
or clone this: git clone https://github.com/natepac/playfabmirrorgameexample.git
(B)
Next add folder Asset from here: https://github.com/MaelikR/Unity-RPG-Game/archive/refs/heads/main.zip
(C)
Open the project in Unity.

10. Install Dependencies
Mirror:
Download and import Mirror from the Unity Asset Store.
PlayFab SDK:
Download the PlayFab SDK and import it into your Unity project.
TextMesh Pro:
Download and import TextMesh Pro from the Unity Asset Store.
Procedural Terrain Painter:
Download and import Procedural Terrain Painter from the Unity Asset Store.


ThirdPersonController:
Download and import ThirdPersonController from the Unity Asset Store.

![image](https://github.com/user-attachments/assets/585f4377-c787-411d-ab3d-d3398327d175)

12. Video Tutorial Part 1: https://youtu.be/Q90bylFXtNY

13. Credits:
This project uses code and concepts from:

|natepac/playfabmirrorgameexample: https://mirror-networking.com/](https://github.com/natepac/playfabmirrorgameexample)

|Mirror Networking: https://mirror-networking.com/

|PlayFab: https://playfab.com/

|TextMesh Pro : Unity Free Assets

|Procedural Terrain Painter: https://assetstore.unity.com/packages/tools/terrain/procedural-terrain-painter-188357 (Unity Free Assets)

![image](https://github.com/user-attachments/assets/20232a05-7ea8-4ec8-9240-81c4bbead7c1)

|ThirdPersonController: Official Unity Free Assets


13.License:
Include any licensing information here.
