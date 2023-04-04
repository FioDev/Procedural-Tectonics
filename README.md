# Procedural Tectonic Engine

This is a demonstration of a novel implementation of physical simulation for the purposes of procedural terrain generation, simulating families of grouped tectonic plates, their motions, and the effect of their motion on the landscape.
It is designed to provide a platform for generating and viewing replicatable examples of procedurally generated terrain, as based on a basic tectonic simulation, and to be a technology demonstration for such a method of terrain generation.

Currently, the simulation is capable of generating set dimensions of terrain at a time. The exact dimensions of which can be selected from the generation menu before the simulation begins. 

## Controls

The following keybinds should be read to understanding to operate the simulation effectivley.

### [Space] - Toggle Simulation
The space key will toggle the simulation from paused, to running using the currently inputted simulation perameters. If the perameters have been changed since the last running of the simulation, space will *restart* the simulation, as opposed to *resuming* from the currently generated terrain.

### S - Toggle Smooth Only Mode
The S Key will toggle smoothing only mode, and disable the generation of any additional terrain until smooth only mode is disabled. This runs a Cross-patterned smoothing algorythm, which takes the biases of the terrain height definitions into account while applying a general smoothing onto each of the coordinates of generated terrain.

### [Tab] - Cycle UI Selection
The tab key will cycle between selected UI elements in the generation screen. For example, from the overall noise generation perameters, to the individual plate perameters, and each of the buttons and input boxes within this.

### Value Inputs
Value inputs are handled on a case by case basis, based on the UI element selected with the Tab button. If the UI element accepts a number which can be increased or decreased between values, the arrow up and down keys will be used as the input. If the UI element accepts a Vector 2, such as with the offset, position, or speed of a plate, All four arrow keys will be used as input. If a larger, or entirely unrestricted number is required, the number keys on the keyboard can be used to input this.


## Installation 

Download the latest version of the Procedural Tectonic Engine simulation from the releases page of this github. There, download the relevant zip files, and extract to the desired location on your device. From this extracted folder, locate the application executable, and run. Instructions on its operations are found in the controls section of this readme.
