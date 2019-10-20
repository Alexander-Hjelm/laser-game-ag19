---
layout: textSites
title: About
---
# <span style="color:#0090ff"> Goal and Motivation </span>

We were looking for an interactive game that would connect the player to the game immediately.
When we saw the projects from previous year we all found that the **PixelSense** gave us instant gratification when playing the demos. Placing physical objects on top of a screen to interact with the game was fun immediately, so we knew this was the right choice to work with.

We wanted the following properties:

#### - interactive game

#### - visually pleasing graphics

#### - instant feedback when hitting objectives through sound and VFX.

- - - -

# <span style="color:#0090ff"> Challenges & Obstacles </span>

#### - Understanding the PixelSense
One of the major problems with this hardware was the total lack of resources or documentation. The entire documentation and SDK is deprecated. Meaning we didn't even know where to start in order to complete the networking between the pixelSense and our game client.

<span style="color:#0090ff;"> **Solution:** </span> Luckily, there was a working TUIO server compatible with the PixelSense, which had enough documentation to it, which allowed us to program the game client with respect to the PixelSense.

#### -  Rotation of the phycons
One of the issues with the PixelSense is that it is really sensitive to light. This in relation with it is not entirely accurate with the rotational read on the fiducial makes the angle of the object to stutter, which proved to be really annoying and difficult for the user. The darker the room, the better it is.

<span style="color:#0090ff;"> **Solution:** </span> We are using two fiducials on a larger phycon in order to calculate a point between these fiducials. This way the low resolution fiducial read won't cause the same amount of stuttering anymore, as the final rotation and position is an interpolation of a directional vector between two fiducial positions.

- - - -
# <span style="color:#0090ff"> Lessons Learned </span>

Have fun :)
