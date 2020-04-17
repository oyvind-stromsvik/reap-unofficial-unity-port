# Reap: Unofficial Unity port

## About the project

Reap is a game made in GameMaker for Ludum Dare 34 by Daniel Linssen.  
Read more about the original game here:  
https://managore.itch.io/reap  

This project is an unofficial Unity port made by Øyvind Strømsvik.  
Read more about the Unity port here:  
http://twiik.net/projects/reap-unofficial-unity-port  

## License/Copyright

I've spoken with Daniel and I've been given permission to make this port. Everything in the project remains the property of Daniel Linssen - all the graphics, the sounds, and the source code belongs to him. You're free to download the project, learn from it and play around with it, but don't redistribute it and please don't be a dick and try to sell it.

## Known issues

These have been moved here:  
https://github.com/oyvind-stromsvik/reap-unofficial-unity-port/issues

## Bugs that are present in the Ludum Dare version of the game

These are bugs that I haven't fixed because they also exist in the original
version of the game so I've preserved them to keep the two versions as similar as possible.

- If you're standing on the corner edge of the shore and try to build a raft
diagonally out into the water you will not be able to enter the raft. This is probably due to
an issue with the collision detection.
- If you eat a turnip while you're facing up, the turnip will pass behind your
head.
- Not really a bug, but you can travel indefinitely out into the sea without
ever looping around to the other side of the world. 
- The "Press key to begin" text on the instructions screen jerks a bit when the
counter loops around.
- The game is framerate dependent. It's currently set to max 60 frames per second. This is common for a lot of GameMaker games, but Unity it's trivial to fix. However I kept it this way to stay true to the original.
