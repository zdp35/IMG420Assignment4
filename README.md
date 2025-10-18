# IMG420Assignment4

For Assignment 4, I went back and took the assets from my Assignment 1 to complete this game. The basis is simple, for level 1, collect all 3 gems and reach the door at the end to continue. Level 2 is a little bit harder with a different theme, bridges, and 2 patrolling enemies you have to dodge. Collect the gems, avoid the patrolling enemies, and relax in the cave at the end.

This game uses A (left), D(right), and space (jump). You can double jump if you press space twice. Player does have gravity.

The sprite's animations come in the form of jumping, when jumping it will smile and raise an arm up to mimic the jumping movement. 
The enemies navigation comes in the form of patrolling. They patrol to the edge of the nearest wall, and then will go back to the other wall and repeat.
Particle effects both require the player. When running, little particles appear behind him like small dust storms. When dying, a small explosion of particles appears. The Gems also have a small glitter particle effect on them to make them more enticing.
Collision interactions, if the player touches a spike, or an enemy, they respawn at the beginning. The player can walk/run/jump on all platforms and such, and cannot "phase" or run through any unintended places.
For the UI, there is a counter to show you how many gems you have collected.
All code has been completed in C#
TileMap and TileSet were implemented as assets for this game.
