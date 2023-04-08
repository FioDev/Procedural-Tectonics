# Procedural Tectonic Engine

This is a demonstration of a novel implementation of physical simulation for the purposes of procedural terrain generation, simulating families of grouped tectonic plates, their motions, and the effect of their motion on the landscape.
It is designed to provide a platform for generating and viewing replicatable examples of procedurally generated terrain, as based on a basic tectonic simulation, and to be a technology demonstration for such a method of terrain generation.

Currently, the simulation is capable of generating set dimensions of terrain at a time. The exact dimensions of which can be selected from the generation menu before the simulation begins. 

Users wishing to participate in a study related to this program, its outputs, and its usability with various individuals on various systems are invited to participate at their own leisure while the study remains open. An informed consent form can be distributed by @FioDev on request, after which a link to the studies' questionaire will be provided. 

## Hotkeys
### Space - Start / Stop / Pause the simulation
### S - Toggle "Smoothing Only" mode for more refined terrain once the desired rough shape has been reached
### H - Toggle UI Overlay
### O - Toggle Orbit Camera

## Pre-reading
The program comes pre-loaded with settings that, when run with no change, generate accurate, interesting, and aesthetically pleasing terrain over a period of some 400-1,000 simulated "years". By default, it involves two plates moving toward one another, and will generate a series of islands along subduction zones, much like the real-life locations seen at such oceanic subduction zones (Indonesia, Hawaii, Japan, Iceland, etc). The easiest method of generating different and unique terrain without creating strange artefacts of extreme values in the simulation is to add, alter, or remove tectonic plates from the simulation. Even adjusting the offset - world position - of a plate, or the speed at which is travels will radically alter the terrain, without changing its seed or adding additional plates to the simulation.

## Controls
From the main menu UI, two panels will be seen. 
The leftmost is the Status Display, and will output basic information about the programs running, such as the number of years - or, ticks - the simulation has completed, and the framerate at which it is currently operating. It will also display the status; whether the simulation is idle, running, or smoothing. 

The rightmost panel is the settings, and will be default hide when terrain begins to generate or smooth, or when it is manually hidden by the user using the H key.
From this settings pannel, three sub-menus will be seen. 

### Plates
From here, the settings of each individual tectonic plate can be altered. 
#### Seed
This is the seed of the currently selected tectonic plate. It will generate the plate's terrain data (its depth within the lithosphere, and the age of the crust it represents) based on this information. 
#### Offset
This is the location of the plate within the simulation. All plates occupy the entire simulation, but by changing the value of the offset, you can which parts of the genrated lithospheric plate are present at any one time. 
#### Speed
This is the speed at which the location of the plate changes. As stated above, the entire generation map is occupied by every plate simultaneously, but depending on the offset, different parts of that plate may be present and simulated. The speed changes the rate at which this offset is tracked across the simulation area. This also affects the plate's interactions with other plates, as if it is converging or diverging with another plate, deep ocean and continental plates within each plate family may cause rifting magma chambers, or subduction magma chambers to form. 

#### Plate manager
Several buttons will also be shown to the user on this screen, including navigation buttons to switch between plates, an option to remove the currently selected plate from the simulation entirely, or to add a new plate to the simulation that can be configured by the user via the plate navigation buttons, and the above input fields. 
The user should be aware that the simulation will not function if less than 2 plates are present; an empty ocean with no visible features resulting from tectonics will be the result, as no interactions with other tectonic plates ever occur.

The user should also be aware that, although no limit is built into the program, simulating many tectonic plates will not only procude physically unrealistic / innacurate terrain, but also cause performance issues as one plate will check against every other plate moving against, or toward it for tectonic interactions; this is an interaction that can potentially have an exponential complexity - an issue that i intend to resolve in future itterations of this simulation by altering the archetecture of plate storage. 

### Size
From here, the dimensions of the world can be altered.
The sliders used to control the dimensions work based on a "single touch" type of input. i.e, "sliding" the slider will not update the value. Instead, the user should press only where they intend the slider to be. The values of each slider range from 10 to 1,000. The user can then apply the changes to the map dimensions by selecting the "apply" button on the Size screen.

The user should also be aware that ***values over 200x200 are likley to cause significant framerate drops***. While the simulation has been tested up to 1000x1000, potential performance issues above this limit are indicated by turning the "size" output display yellow if either the X or Y size of the map reaches 200 units.

### Settings Configuration

Here, four sliders are presented to the user, representing variables that can be tweaked to alter the running of the simulation. 
#### Erode Factor
This is the factor by which terrain is eroded each tick. Terrain is eroded based on the "age" of the plate that generated it, and the height at which the resultant terrain sits. The erosion factor changes the rate at which this terrain will be reduced to a certain level, defined later in "Erosion Bias Limit". 
This value is very sensitive. Lower values of erosion with higher values of volcanism can cause a "runaway volcanism" effect. The Erosion factor represents a 0 to 1 value, which is then multiplied by the frame-time of the simulation to produce the maximum ammount of terrain that can be "shaved off" of the world each tick. Setting this to the lowest value means that terrain can never be eroded, and setting it to the highest factor means that if terrain is scheduled for erosion, it is instantly eroded as far as it can be. 

#### Volcanism Factor
The volcanism factor is the oposite of Erosion Factor; "How much terrain can be added to the height of the world each tick". Similarly, this is a 0 to 1 scale which describes a range of "No terrain can ever be added" to "If terrain is to be added, it will instantly achieve the maximum height it can in that tick". Unlike erosion, which occurs based off of height terrain and age of plates, volcanism occurs based on the interaction of the plates themselves. If two plates pull apart too much at a mid oceanic point, or converge or collide too violently with eachother at continental or oceanic points, a "magma chamber" will be created with a size proportional to the violence of the collision, or magnitude of the divergence. From this magma chamber's "size", the volcanism factor represents the maximum rate at which "magma" may flow out of the chamber and onto the terrain immediately above. 

#### Erosion Bias Limit
The Erosion Bias Limit is the limit at which erosion of terrain biases "creation" rather than "destruction" of terrain. For example, oceanic trenches eroding away implies an erosion upwards - towards the ocean floor - rather than an erosion downwards further into the earths core. This is another 0 to 1 value which controls a heightmap-representation of the terrain's height bias. Terrain below this erosion bias limit will erode *upward* instead of downward, Terrain above this erosion bias limit will erode downward instead of upward, and the closer terrain is to the bias limit, the slower it will erode. 
This option functions as the simulation's "entropy" setting.

#### Orbital Camera Speed
This is a setting to control the speed of the orbital camera, which can be toggled on by pressing "O" on the keyboard at any time during the program's running. 


## Installation 

Download the latest version of the Procedural Tectonic Engine simulation from the releases page of this github. There, download the relevant zip files, and extract to the desired location on your device. From this extracted folder, locate the application executable, and run. Instructions on its operations are found in the controls section of this readme.
