# Tetris

Built executable "TetrisReg.exe" is located in the TetrisBuild folder.

## Game Explanation
This version of Tetris extends features of the basic popular tetromino fitting game. Scoring resembles the original Nintendo system, where 1, 2, 3, or 4 lines cleared augments the player score by 40*(n+1), 100*(n+1), 300*(n+1), and 1200*(n+1) points respectively, for difficulty setting on level n.

### Controls
Arrow keys control the movement of the active piece, if such movement is valid. Space initiates a hard drop in which the active piece is slid down as far as possible and locks in place. Rotations are controlled by Q for counterclockwise and E for clockwise.

### Levels and Difficulty
The game has difficulty levels 1 through 10 as set by a slider on the main menu. This determines how fast the game reaches peak difficulty as determined by the time between each step, aka however long passes before the active piece is pulled down one square by gravity. This starts from a maximum of 1 second and decreases to a minimum of 0.1 seconds. The tempo of the theme music follows accordingly to reflect how fast the game is moving, and ranges from a minimum of 75% original tempo to a maximum of 200%.

The game ramps up in difficulty every time any number of lines is cleared (note: not total number of lines cleared), causing the step delay to decrease slightly. Higher levels mean that peak difficulty is reached faster with fewer clearings.

### Random Start Mode
Enabling Random Start on the menu starts the game off with a twist- n white tiles spawn randomly in the bottom half of the screen, where n is the difficulty setting. These get pulled down accordingly every time a line below them is cleared.