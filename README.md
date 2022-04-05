# HDL Converter
![Title](/Documentation/1_Images/HDLConverterLogoSmall.png)

[![License](https://img.shields.io/badge/License-AGPL_3.0-blue.svg)](/LICENSE)
[![C#](https://img.shields.io/badge/Software-C%23-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A simple tool that can be used to convert the header syntax of a verilog module to an instantiation syntax. The project is aimed at removing the need for tedious refactoring of module headers.

## Feature Overview
**Language Support**

[![Verilog](https://img.shields.io/badge/Verilog-Supported-brightgreen)]()
[![VHDL](https://img.shields.io/badge/VHDL-Not%20Supported-red)]()

**Features Included**
- Conversion of Verilog header to instantiation of the module
- Conversion of Verilog header to declare all wires in the module
- Comment creation for instantiaton making port direction easily visible
- Parsing of Verilog header syntax to objects and attributes (easy expandability to othre hdl code generations functions e.g. automatic testbench structure)
- Flexibility to support other HDL languages

**Features Planed**
- VHDL support
- Cross HDL Generation/Translation for existing functions
- Automatic generation of testbench structure (testbench, verify, instantiation)
- Updating of existing instantiations with new wires


## How to Use
[![Release](https://img.shields.io/badge/Latest%20Release-V1.0.0-green)](/releases/latest)

You can finde the latest release versions executables [here](/releases/latest).
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

## User Interface
### GUI Based
![GUI Example](/Documentation/1_Images/GUI/GUI_Plane.PNG)
### Console Based
![Console Example](/Documentation/1_Images/Core/generateOutput.PNG)


## Software Architecture
 ![Class Diagramm](/Documentation/1_Images/ClassDiagramm/class_diagramm.svg)
 
 ## Contributors
 - [Robin](https://github.com/m47812)
