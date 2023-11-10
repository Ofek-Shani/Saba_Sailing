# Unity Sail Boat

Simulate a basic sailboat with 2 sails floating on water around various obstacles, with a view from above on a 2D scene.

## Table of Contents
- [Description](#description)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
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
1. Wind power on sails compues both blowing wind and air flow affect over the sails as wings at
tight angles. 
1. Sails shape reflects the wind blow force applied to them.
1. When wind blows on loose sails, they flicker from side to side to show that. 
1. When wind changes sides from the back, sails perform a full side change (revolution) that should be done
with care. 
1. Rudder is also affecting the boat drag according to its position, so if set to 90 degrees, it will not cause turn, but slow the boat.
1. Colors of sails reflects direction of wind blow side (green when positive, red when negative). 
1. Rudder color reflect boat speed forward or backward. When boat velocity is negative, the rudder affects the boar turning in a reverse manner.

## Installation
Currently, the software is a Unity studio IDE project. Using Visual Studio coding environment and debuging, but it can also use other code editing IDEs.

## Usage
The game has UI controls as well as short-hand keyboard keys as follows:
1. rudder: move slider left and right, or use the left and right arrow keyboard keys
1. sail Tension: move slider up or down, or use the up and down arrow keyboard keys
1. keel position: up, middle or full via a 3 position vertical slider, or use the up and down Page keys on the keyboard.
1. Two button on left allow to select front sail (FS) or main sail (MS) to be affected by the sail tension slider and keys. 
If both are no selected, slider and keys control both sails together.
1. a blue left and right arrows when clicked perform a sudden kick of the rudder to the left or the right to help getting the boat out of stalling when 
facing wind for instance. Keyboard left and right Shift are short cut keys for the same purpose.
1. ANCOR is a checkbox that when on will not apply the sails force on the boat so it does not move, so the different controls over the boat can be tried out.
1. Tracking info display on top left. Four parameters:
    1. ANG D. is the Angular Drag which slows boat turning.
    1. LIN D. is the linear Drag which slows the boat velocity on all directions.
    1. FWD F. is the wind force that is applied on the boat forward direction.
    1. LAT F. is the wind force that is applied on the lateral direction.

## Contributing
The project is presently private so contributions are limited.

## License
This project is licensed under the [Uri](mailto:uri.shani@gmail.com) and [Ofek](mailto:ofeka.shani@gmail.com) Copyrights. Refer to the [LICENSE](LICENSE.txt) file for more information.