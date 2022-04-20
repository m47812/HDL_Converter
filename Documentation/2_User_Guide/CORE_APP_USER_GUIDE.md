# Core App user guide
This document should help you with using the console app.

## How to use
You can find the Console App in the latest release as "HDL_Converter_Console.exe" or build the core app in visual studio.
When starting the console app you are greated by the Window shown bellow.

### Main Menu
![Main Menu](/Documentation/1_Images/Core/MainMenu.PNG)

Here you have the Options:
- Enter `-m` to open the settings
- Enter HDL Code (Shown bellow)
  - Copy a Verlog Module into the console
  - Press Enter
  - Enter `-d` to confirm
- Enter `-c` to clear the entered module if you made an error.
 
 ![Main Menu with HDL](/Documentation/1_Images/Core/HDLInput.PNG)
 
 ### HDL Output Selection
 1) Once you have entered a Verilog module you will see the part of the image bellow marked with "1".
 2) Here again you have the following options:
 - `d` --> Will output code declaring all Wires in your module (excluding parameters)
 - `i` --> Will output the instantiation code for the module header you have entered before
 - `e` --> Will take you back to the main menu. Do this if you want to change the HDL Input.
 - `t` --> Will generate a testbench structure more information can be found in the [Testbench Feature Guide](/Documentation/2_User_Guide/TESTBENCH_FEATURE.md).
 - `-m` --> Will take you to the settings menu where you can make changes to the format you will recive as an output.
 3) The Section marked with "3" will be the output of the comand you have entered. You can now repeat step "2" or
 can exit the menu with `e`.
 
  ![HDL Output](/Documentation/1_Images/Core/generateOutput.PNG)
 
### Settings Menu
The Settings Menu can be entered and exited by entering `-m` in the console. Here you can change configurations to the HDL Language that you are using and configure the output style you want.
Configurations are:
- HDL Language --> The HDL Language your header uses and output will be in (**Please Note that only Verilog is Supported in the current Version**). 
- Emptiy IOs --> When set to `true` an instantiation will not contain any wires e.g. '.mySignal()'. If set to `false` (default) the instantiated module will be passed wires of the same name as the port e.g. '.mySignal(mySignal)'.
- User Comments --> If enabled user comments that were included in the HDL input will be added to the wire instantiation and declaration.
- Direction Comments --> If enabled every signal will contain a comment about the data direction of the wire (if it is an input or output) as well as (in case of a vector) the vector size.
If both Comment settings are enabled a combined output will be generated e.g. `//output [7:0] | my user comment`

To change a setting first enter the number that is in front of the setting in "Current Settings". Then you will be pompted to enter a configuration as in the example bellow where you can enter `verilog` or `vhdl`
in case of boolean settings you can set them by entering `t`/`f` or `true`/`false`.
**Note:** For boolean settings you also have the option to combine setting index and target value like this `2t` to set setting 2 to true.

 ![HDL Output](/Documentation/1_Images/Core/Settings.PNG)
