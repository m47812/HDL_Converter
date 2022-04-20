# Testbench Feature Guide

This document provides an introduction to the testbench creation feature implemented with version v1.1.0. 

## Overview
With the Testbench feature, simple testbench structures can be created automatically. From a given input module header
the HDL Converter will generate the following structure.

```
    Top Level
    -----------------------------------------
    |   Verify              User Module     |
    |   -------------       -------------   |
    |   |           |       |           |   |
    |   |           |------>|           |   |
    |   |           |       |           |   |
    |   |           |<------|           |   |
    |   |           |       |           |   |
    |   -------------       -------------   |
    |                                       |
    -----------------------------------------
```

The structure consists of these components
### Top Level

A toplevel module that does not have any IOs or Parameters. It serves as the top level file for simulation. In it
the user given module is instantiated as well as a verify module for the user module. The verify and user module are
connected in the top level file. Also the top level contains all needed declarations of constants (from user modules parameters) and wires
(from user modules IO port). This file should normaly not need any further modifications but can be enhanced with more other modules if needed by the user.

### Verify

The verify module is a user template where the simulations stimuli and assertions can be inserted by the user. The verfiy module is therefore
empty in the RTL section. The header of the module will contain the same wires and parameters as the user given module but with inverted dircetions (input becomes output and
output becomes input) such that the verify module can easily be connected to the user module via the toplevel file.

### User Module

A HDL header of a module given by the user, for which a testbench shall be created.


## Example
Given user input module:
```verilog
module example
#(
parameter myParam1 = 12,
parameter myParam2 = 2
)
(
input wire clock,
input wire reset, //Comment
output wire [7:0] busOut,
inout wire someSignal //Comment
);
```

The created Top Level:
```verilog
////////////////////////////////////////////////////////////////////////////
//
// Name: tb_example
// Description:
//
// Version: 1.0.0
// Date: 15.04.2022 21:40:21
//
// Company: 
// Engineer:
// 
////////////////////////////////////////////////////////////////////////////

module tb_example();
//========================================================
// Constants
//========================================================

localparam myParam1 = 12;
localparam myParam2 = 2;

//========================================================
// Wire Declarations
//========================================================

wire clock;
wire reset;
wire [7:0] busOut;
wire someSignal;

//========================================================
// Module Instantiations
//========================================================

example
#(
	.myParam1(myParam1),
	.myParam2(myParam2)
)
 DUT
(
	.clock(clock),
	.reset(reset),
	.busOut(busOut),
	.someSignal(someSignal)
);

verify_example
#(
	.myParam1(myParam1),
	.myParam2(myParam2)
)
 Verify
(
	.clock(clock),
	.reset(reset),
	.busOut(busOut),
	.someSignal(someSignal)
);

endmodule
```

The created Verify:
```verilog
////////////////////////////////////////////////////////////////////////////
//
// Name: verify_example
// Description:
//
// Version: 1.0.0
// Date: 15.04.2022 21:41:29
//
// Company: 
// Engineer:
// 
////////////////////////////////////////////////////////////////////////////

module verify_example
#(
parameter myParam1 = 12, 
parameter myParam2 = 2 
)(
output clock, 
output reset, 
input [7:0] busOut, 
inout someSignal 
);
//========================================================
// Wire Declarations
//========================================================



//========================================================
// Verification Code
//========================================================



endmodule
```

## How to use in GUI

![GUI Example](/Documentation/1_Images/GUI/GUI_Testbench.PNG)

In the GUI version of the programm two options are supported:
- Creation of files for verify and top level
- Copy Verify/Top Level to Clipboard

If the testbench shall be stored to files a target folder has to be provided using the "Target Folder Loacation" button.
Only when a folder path is provided can the "Generate Testbench" button be clicked in this mode. If the "Target Folder Loacation" button is not clickable,
select "Both to File" with the radio buttons on the left.  After clicking "Generate Testbench"
the two generated files "verify_XY.v" and "tb_XY.v" (XY will be the inputed modules name) be found in the selected folder path. If the generated file names are not available
a incrementing number will be added to the file name. The module will not allow to write more than 5 modules of the same name in the same directory to avoid accidentally filling
a folder with module testbenches when clicking "Generate Testbench" often.

If the testbench shall be stored to the clipboard instead, use the radio buttons on the left to select if the verify or top level shall be generated.
The after that click "Generate Testbench". The generated testbench file content can the be pasted into any file from the clipboard.

## How to use in Console

For the console version input your HDL code first until you get to the "HDL Output Selection" menu instructions for this can be found [here](/Documentation/2_User_Guide/CORE_APP_USER_GUIDE.md)
After that the "t" command can be used to generate the verify or toplevel file. If only "t" is entered as a instruction, both the top level and the verify
will be printed to the console. If you only want one of the to at a time use "t -t" (for testbench top level) or "t -v" (for testbench verify). You can
also use "help t" to see instructions on available modifiers for this command.
