# PipeConnect

This repository contains an algorithm that auto-generates playable and completable levels for a "Connect the Pipes" style puzzle game.

The algorithm works by generating a path of interconnected pipes, ensuring a start and end point, and guarantees that a solution always exists for each generated level. This takes the guesswork out of level design, and provides a mechanism to generate infinite, solvable puzzles for your game.

Features:

Path Generation: The algorithm efficiently generates a path from the start point to the end point, ensuring that the path is unbroken and that every puzzle is solvable.

Difficulty Scaling: The algorithm can adjust the complexity of the path based on difficulty settings. This allows the creation of simpler paths for beginners and more complex paths for advanced players.

Game Integration: The algorithm can be easily integrated into existing game code. It outputs a 2D array representing the game grid, with each cell indicating a pipe segment or an empty space.


