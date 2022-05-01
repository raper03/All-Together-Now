# All Together, Now

![All Together Now](/Docs/ReadMeImages/splash.png)
<p> An interactive music sequencer made with Unity. 
  A typical music sequencer is capable of playing, editing, and recording audio, so what if we could have all that with an orchestra we design? </p>

## Features
* Place orchestra members with instruments into an environment to play music
* Piano roll to edit orchestra member's notes
* Real time playback with player created music
* Uses Unity's [OnAudioFilterRead](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnAudioFilterRead.html) to load in music in realtime.

## Requirements
* [Unity 2020.3.24f1](https://unity3d.com/unity/whats-new/2020.3.24)
* [Unity Hub](https://unity3d.com/get-unity/download)
* [Shapes by Freya Holmér](https://assetstore.unity.com/packages/tools/particles-effects/shapes-173167)
* Addressables (Package Manager)
* Universal Render Pipeline (Package Manager)

## Installation
The Unity project is available in the AllTogetherNow folder. For a step-by-step, check or download the Installation PDF guide in the Docs folder.

## Setup
Check the **Empty Stage** scene for the main scene, or just as a working example.

If you wish to create your own set-up please use the following components:
### Components
- **Staff View:** GameObject that parents GameObjects with persistent musical functionality; handles player input.
  - **Staff:** The music staff to write notes to orchestra members; join players to the score.
    - **Staff Editor:** Edits the staff (note placement)
    - **Staff Reader:** Reads the staff (# of bars, BPM, physical size of staff)
    - **Staff Platform:** Platform for objects in environment to grab and use musical information.
  - **Dropper:** Orchestra member suite for placement in environment.
  - **FilterManager:** Manages current position in song, allows newly instantiated orchestra members to sync up.
- **Stage View:** GameObject that parents the camera that views the environment and all members in it.
  - **Base Camera:** Base camera with Staff camera in the stack

Components for Staff View can be found in **Assets/Scripts/UI/Staff View** in the Project folder, along with the Staff View prefab itself.

Components for Stage View can be found in **Assets/Scripts/UI/Stage View** in the Project folder, along with the Base Camera prefab itself.


### Overlaying the staff view onto the main camera
Staff View acts as an overlay for the Stage View. Separate "Views" help with organization and controlled areas for different types of components.

Stage View needs to set the Base Camera Stack list manually. To do so:

**Make sure to delete any pre-existing cameras in the scene!**
1. Load both the Staff View prefab and Stage View prefab into the scene.
2. In the Staff View GameObject, expand to find and select the Camera child GameObject.
3. In the inspector, find the "Camera" component and navigate to the "Stack" list.
4. Add the Staff view to the stack. 
![](/Docs/ReadMeImages/stack.png)
## Contributions
### Adding New Components
If you have created an object that needs to communicate with the Staff View (or its children objects), but has physical presence in the scene's environment, please consider using the **StaffPlatform**. Otherwise, feel free to parent it with the Staff View.
### Adding New Instruments
If you wish to create a new instrument, please check or download the Creating an Instrument PDF guide in the Docs folder.


