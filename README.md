# Unity-RPG-Game
Welcome to the RPG GAME project! This repository contains the complete setup for a multiplayer RPG game built using Unity, Mirror for networking, and PlayFab for backend services. Follow the instructions below to get started, configure the project, and build both the client and server components.


1.Table of Contents
2.Features
3.Setup Instructions
4.Clone the Repository
5.Install Dependencies
6.Configure PlayFab
7.Configure Mirror
8.Building the Project
9.Client Build
10.Server Build
11.Custom Features
12.Credits
13.License
14.Features

Multiplayer functionality using Mirror
Backend services powered by PlayFab
Detailed terrains with Procedural Terrain Painter
Enhanced text rendering with TextMesh Pro
Third-person character controller
Seamless integration and easy setup.

Setup Instructions

Clone the Repository:
First, clone the repository to your local machine:

git clone https://github.com/MaelikR/Unity-RPG-Game.git
Open the project in Unity.

Install Dependencies
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


Configure PlayFab:
Sign up for PlayFab and create a new title.

In Unity, navigate to Window > PlayFab > Editor Extensions > Settings.

Enter your PlayFab Title ID and Secret Key.

Follow the PlayFab setup guide to configure authentication and backend services.


Configure Mirror:
Set up the network manager in your main scene.

Configure networked objects and components as required.

Follow the Mirror documentation for detailed setup instructions.


Building the Project
Client Build:
Open the Build Settings window in Unity (File > Build Settings).

Select your target platform (e.g., PC, Mac, & Linux Standalone).

Add your main scene to the build by clicking Add Open Scenes.

Configure Player Settings as needed.

Click Build and choose a location to save the client build.


Server Build:
Open the Build Settings window in Unity (File > Build Settings).

Select your target platform.

Enable the Server Build option.

Add your main scene to the build.

Configure Player Settings, ensuring Headless Mode is enabled.

Click Build and choose a location to save the server build.


Custom Features:
Detailed terrain customization using Procedural Terrain Painter.

Enhanced UI and text rendering with TextMesh Pro.

Multiplayer networking powered by Mirror.

Backend services integration with PlayFab.


Credits:
This project uses code and concepts from:

natepac/playfabmirrorgameexample
Mirror Networking
PlayFab
TextMesh Pro
Procedural Terrain Painter
ThirdPersonController

License:
Include any licensing information here.
