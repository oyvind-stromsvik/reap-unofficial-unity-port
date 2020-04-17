ABOUT THE GAME
==============

A game by Daniel Linssen (https://managore.itch.io/reap)
Made for Ludum Dare 34
Ported to Unity by Øyvind Strømsvik (http://twiik.net)

LICENSE/COPYRIGHT
=================

I have no idea how to write one of these, but this game belongs to
Daniel Linssen. I just made a port of it to Unity for fun. All the art, sounds,
code, concept, music, whatnot is made by him and belongs to him.

KNOWN ISSUES
============

- The stroke surrounding the view mask is not exactly 1 pixel thick all around
like it is in the original. This is surely caused by my crappy shader, but
I don't know how to fix it.
- The lines for the energy and food meter sometimes because 2 pixel thick. No
idea how to fix this one as they come straight from GL.LINES
- There's a pixelsnap bug with the line for the food meter when it's pointing
straight up. No idea how to fix. This bug suddenly appeared towards the end.
- "Rendertexture error: Material doesn't have a texture property '_MainTex' 
UnityEngine.Canvas:SendWillRenderCanvases()"
Something is wonky with shaders. Probably my view mask shader.

BUGS THAT ARE PRESENT IN VERSION 1.0 OF THE ORIGINAL GAME
=========================================================

These are bugs that I haven't fixed because they also exist in the original
game so I've preserved them to keep the two versions as similar as possible.

- If you're standing on the corner edge of the shore and try to build a raft
diagonally out into the water you will not be able to enter the raft. Probably
an issue with the collision detection.
- If you eat a turnip while you're facing up, the turnip will pass behind your
head.
- Not really a bug, but you can travel indefinitely out into the sea without
ever looping around to the other side of the world. 
- The "Press key to begin" text on the instructions screen jerks a bit when the
counter loops around.
