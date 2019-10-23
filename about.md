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

One of the first and most obvious lessons we learned was that regularly testing the game was crucial in development. Often small changes could lead to huge consequences in the game. Aspects like the rotation readability of the PixelSense did not have as high fidelity as rotating the object in the engine. Because of this both levels and mechanics had to be iterated on to make up for the limitations of the PixelSense.

For a closer look at the technological lessons, we had to learn during the course of this project take a look at “Challenges & Obstacles”. We delved into shaders and particle systems to make our game look more aesthetically pleasing. We also explored different game mechanics that could improve and make our game more fun, which was not always trivial. Things like getting the laser to behave realistically and bounce off of mirrors, as well as creating a black hole to bend the laser taught us some interesting aspects of raycasting.

Organization was paramount in creating this game. Early on we noted that even small changes could render a level implayable, therefore we created strict rules for github and held regular meetings so that we would all be aware of what the other team members were doing. Communication was not always easy as many group members did not frequently check the communication channels, but because of this, we have learned to communicate better and more efficiently.
