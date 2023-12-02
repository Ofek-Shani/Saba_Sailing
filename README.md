# Sail Boat Simulation Game

Simulate a basic sailboat with 2 sails floating on water around various obstacles, with a view from above on a 2D scene.

## Table of Contents
- [Description](#description)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Keyboard shortcuts](#keyboard-shortcuts)
- [GUI widgets summary](#gui-widgets)
- [Tutorial](#tutorial)
- [Contributing](#contributing)
- [License](#license)

## Description
There are 2 sails, a keel and a rudder. The water show via waves the direction and power of the wind.
Controls allow to change the tension by which sails are pulled, and the rudder position.
Physics simulation uses Unity to handle how wind force over the sails is induced over the boat
and affects its movement speed and direction.

Obstacles incluse fixed buoys, and rock wave breakers.

The boat can also throw a small life saver to demonstrate an excercise of pulling a "man in water" situation.

Buttons on the left allow to restrict sail tension control to operate only on one of the sails for doing
various types of maneuvers.


## Features
1. 2 sails, individually controlled, 
1. keel with 3 positions, 
1. rudder with 8 positions each side. 
1. The boat has a life saver buoy to 
create "man in water" alerts and maneuvers. 
1. Wind power on sails computes both blowing wind and air flow affect over the sails as wings at
tight angles. 
1. Sails shape reflects the wind blow force applied to them.
1. When wind blows on loose sails, they flicker from side to side to show that. 
1. When wind changes sides from the back, sails perform a full side change (revolution) that should be done
with care. 
1. Rudder is also affecting the boat drag according to its position, so if set to 90 degrees, it will not cause turn, but slow the boat.
1. Colors of sails reflects direction of wind blow side (green when positive, red when negative). 
1. Rudder color reflects boat speed forward or backward. When boat velocity is negative, the rudder affects the boat turning in a reverse manner.

## Installation
1. Get the project from GIT and use the Unity Editor to run the game.
2. Windows 64-bits: Unzip the installaiton file and run the **Sailing.exe** executable for a full screen experience.
3. Future: APK file for the Android devices.

## Usage
The game has GUI controls as well as short-hand keyboard keys to manage the different boat parts when sailing as in the following table:

| Part | GUI | Keyboard (for the PC version) |
| -------- | -------- | -------- |
|Rudder | Move horizontal slider left and right | Use **left arrow** and **right arrow** keys.|
|Sail Tension| Move left slider up or down| Use **up arrow** and **down arrow** keys.|
|Keel position: up, middle or full| Move the 3 position vertical slider on right| Use the **PgUp** and **PgDown** keys.|
|Sail selection| Two buttons <img src="./sails selection icons.JPG"  style="width:45px;height:20px"> on bottom left screen allow to select front sail (FS) or main sail (MS) to be affected by the sail tension slider and keys. If both are **not** selected, slider and keys control both sails together. | Use the **W** key to select the front sail, and **S** key for the main sail.|
|Kick rudder| A blue left <img src="./Assets/Textures/leftArrow.png"  style="width:20px;height:20px"> and right <img src="./Assets/Textures/leftArrow.png"  style="width:20px;height:20px;transform:scaleX(-1)"> arrows, perform a sudden **"kick"** of the rudder to the left or the right respectfully, when clicked. This can help getting the boat out of stalling when facing wind for instance.| Use the **Left shift** and **Right Shift** keys respectfully.|
| Anchor | clicking the toggle button <img src="./anchor icon.JPG" style="width:100px;height:25px"> will stop applying sail power on the boat so it stops | Click the **A** on the keyboard to toggle this button's effect.|
|Man in the water drill|Click the little buoy <img src="./Assets/Textures/buoy.png"  style="width:20px;height:20px"> icon to "throw" a small life saver to mark a man in the water situation. Click on the thrown buoy to grab it back to the boat.|Click the **space** bar to toggle man in the water throw/grab action.|
|Pause/Play the game| Click the <img src="./Assets/Textures/GameIcons/pause.jpg"  style="width:20px;height:20px"> icon to **pause** the game, and then the <img src="./Assets/Textures/GameIcons/play.jpg" style="width:20px;height:20px"> icon to **resume** it.| Click the **P** key to toggle pause/play modes.|
|Reset, restart the game|Click the <img src="./Assets/Textures/GameIcons/restart.jpg"  style="width:20px;height:20px"> icon to move the boat and screen back to initial state.| Click the **R** key to reset the game.|
| Help info| Click the <img src="./Assets/Textures/GameIcons/Qmark.png"  style="width:20px;height:20px"> icon, to see several help information options: First is the keyoard shortcut commands layout image (see [KEYBOARD](#keyboard)). Second is a summary of all GUI widgets on the screen (as also detailed in this table) image (see [GUI](#scene)). The third option is a series of tutorial animated lessones (see [TUTORIAL](#tutorial)). To close, click the **Close** button on the bottom, or click the icon again.| Click the **?** (usually ? and slash) key for that. Click again to close or hit the **close** button.
| Track option| Click the Follow-Boat button <img src="./Assets/Textures/GameIcons/manSteeringBoat1.jpg"  style="width:20px;height:20px;border:3px solid black"> to track the boat from onboard, which will then turn into the Follow-World button <img src="./Assets/Textures/GameIcons/compass.png"  style="width:20px;height:20px;border:3px solid black"> which will maintain a fixed view from above. | Use the **X** key to toggle between whether camera follows the boat direction, or stays fixed to the north (screen upwards).|
| Zoom in/out.| Clicking the small green arrows out icon <img src="./Assets/Textures/GameIcons/expand.png"  style="width:15px;height:15px;border:3px solid black"> button to zoom out and see more of the scene around the boat. Clicking the small red arrows in icon <img src="./Assets/Textures/GameIcons/collapse.png"  style="width:15px;height:15px;border:3px solid black"> button to zoom in (and see less of the scene). | Use the **+** keyboard key (i.e., the + and = key, without using shift!) for zoom out, and the **-** (minus and underline key) to zoom in.|
|Quit the program| Click the <img src="./Assets/Textures/GameIcons/stop.jpg" style="width:20px;height:20px"> icon, then confirm or cancel | **\<CTRL>-C** - hold the control key and click **C**. This too will invoke a confirm dialog.|
|Toggle full screen| Click the <img src="./Assets/Textures/GameIcons/f11.png"  style="width:20px;height:20px;border:3px solid black"> button on the screen to toggle full sceen option. | **F11** - click the function key F11.|


## Tracking info
Display on top left showing the vlaues of four parameters dnamically:
1. ANG D. is the Angular Drag which slows boat turning.
2. LIN D. is the linear Drag which slows the boat velocity on all directions.
3. FWD F. is the wind force that is applied on the boat forward direction.
4. LAT F. is the wind force that is applied on the lateral direction.

Like this on top left: <img src="./data display panel.JPG" style="width:200px;height:40px">

## Keyboard Shortcuts
As part of the game help informtion is the keyboard layout: <img src="./Assets/Textures/GameIcons/keyboard guide for sailing 1.png" style="width:1000px;haight:500px">

## GUI widgets
A second part of the game help information - a summary of all the GUI controls on the screen: <img src="./Assets/Textures/GameIcons/scene.png" style="width:1000px;haight:500px">

## Tutorial
The third part of the game help information is a series of lessons with animated demonstrations and hands on steps.

## Versions and changes
The current version 1.1 includes some changes in the GUI and keyboard:
- The GUI size is kept ralative to the resolution of the game screen and cannot be changed manually.
- The zoom in and out now has many levels that are applied with the keys used previously to change GUI widgets' size: the **+** and **-** keys, and the small buttons in the "Sailiung Control" box.
- The **Z** button is not interpreted anymore and the respective button on the left side has been removed.
- It is possible to remove the GUI altogether and use only keyboard with the 
## Contributing
The project is presently private so contributions are limited.

## License
This project is licensed under the [Uri](mailto:uri.shani@gmail.com) and [Ofek](mailto:ofeka.shani@gmail.com) Copyrights. Refer to the [LICENSE](LICENSE.txt) file for more information.