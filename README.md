
## NOTE!
This tool is deprecated but will be kept available for the ones who like to use it. For new users, I would advise you to look at the newer [HDL Toolbox](https://github.com/m47812/hdl_toolbox) which contains very similar (and more) functionality.

# HDL Converter
![Title](/Documentation/1_Images/HDLConverterLogoSmall.png)

[![License](https://img.shields.io/badge/License-AGPL_3.0-blue.svg)](/LICENSE)
[![C#](https://img.shields.io/badge/Software-C%23-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)


A simple tool that can be used to convert the header syntax of a verilog module or VHDL Entity to an instantiation syntax and create testbench 
structures (top level and verify). 
The project is aimed at removing the need for tedious refactoring of module headers when instantiating modules or verifying individual modules with testbenches.

## Feature Overview
**Language Support**

[![Verilog](https://img.shields.io/badge/Verilog-Supported-brightgreen)]()
[![VHDL](https://img.shields.io/badge/VHDL-Supported-brightgreen)]()

**Features Included**
- Conversion of Verilog or VHDL header/entity to instantiation of the module
- Conversion of Verilog or VHDL header/entity to declare all wires in the module
- Automatic generation of testbench structure (testbench toplevel, verify module)
- Comment creation for instantiaton making port direction easily visible
- Parsing of Verilog/VHDL header syntax to objects and attributes (easy expandability to othre hdl code generations functions)
- Flexibility to support other HDL languages

**Features Planed**
- Cross HDL Generation/Translation for existing functions
- Updating of existing instantiations with new wires


## How to Use
[![Release](https://img.shields.io/badge/Latest%20Release-V1.2.0-green)](https://github.com/m47812/HDL_Converter/releases/latest)

You can finde the latest release versions executables [here](https://github.com/m47812/HDL_Converter/releases/latest).

### Installation
Unpack the .zip file of the version you would like to use (Console or GUI) to the desired location on your disk. Keep all other files contained in the zip file in the same folder. To use the app simply run the .exe file contained in the downloaded zip file. 
 
 **Note for Console App:** Note that, to run the Console app, you need the [.Net Core Runtime Environment](https://dotnet.microsoft.com/en-us/download/dotnet/3.1) (Version 3.1) installed.

For more information on how to use the Tool please refer to the user guides.
- [Console App User Guide](/Documentation/2_User_Guide/CORE_APP_USER_GUIDE.md)
- [GUI App User Guide](/Documentation/2_User_Guide/GUI_APP_USER_GUIDE.md)

## Usage Example
Input:
```verilog
module test
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
Generate Instance Output:
```verilog
test inst_test
#(
	.myParam1(myParam1),
	.myParam2(myParam2)
)(
	.clock(clock),
	.reset(reset),
	.busOut(busOut),
	.someSignal(someSignal)
);
```
Generate Wire Declaration Output:
```verilog
wire clock;
wire reset;
wire [7:0] busOut;
wire someSignal;
```

Testbench Creation:

Generates verify module with inverted IOs and connects them in a top level module. See [Feature Guide](/Documentation/2_User_Guide/TESTBENCH_FEATURE.md) for more information.



## User Interface
### GUI Based
![GUI Example](/Documentation/1_Images/GUI/GUI_Testbench.PNG)
### Console Based
![Console Example](/Documentation/1_Images/Core/generateOutput.PNG)


## Software Architecture
 ![Class Diagramm](/Documentation/1_Images/ClassDiagramm/class_diagramm.svg)
 
 ## Contributors
 - [Robin](https://github.com/m47812)
