﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Classes.HDL_Structures
{
    /// <summary>
    /// A enum for defining the data direction of a Wire
    /// </summary>
    public enum PortDirection{
        UNKNOWN = 0,
        Input = 1,
        Output = 2,
        InOut = 3
    }

    /// <summary>
    /// A Class resembling a Module in HDL it contains lists of all wires
    /// and parameters that make up the interface for the module.
    /// </summary>
    public abstract class HDLModule
    {
        public string name;
        public Settings settings;
        public List<Parameter> parameters = new List<Parameter>();
        public List<Wire> wires = new List<Wire>();

        public HDLModule() { }

        /// <summary>
        /// Initializes a Module from HDL code in string format.
        /// </summary>
        /// <param name="hdlModule"> hdl (header) code in string fromat</param>
        /// <param name="settings">settings object that will be used for the output</param>
        public HDLModule(string hdlModule, Settings settings)
        {
            this.settings = settings;
            this.initializeFormHDLCode(hdlModule);
        }

        protected abstract void initializeFormHDLCode(string hdlCode);
        protected abstract void initializeParameters(string hdlCode);
        protected abstract void initializeWires(string hdlCode);
        public abstract string generateWireDeclaration();
        public abstract string generateParameterDeclaration();
        public abstract string generateModuleInstantiation();
        public abstract string generateModuleHeader();
        public abstract string generateHeaderPort();
        public abstract string generateHeaderParameters();
        public abstract string generateTestbenchTopLevel();
        public abstract string generateTestbenchVerify();

        /// <summary>
        /// Calculates the open and close index of the top parentheses. 
        /// In case there is no top parentheses wraping everything it returns the first opened
        /// paranthesis start and stop Index.
        /// </summary>
        /// <exception cref="Uneven Parenteses">
        /// This Exception gets thrown when the number of opened and closed parentheses did
        /// not match.
        /// </exception>
        /// <param name="open">The character that opens a parentheses (like '(' or '{') can be any character</param>
        /// <param name="close">The character that closes a parentheses (like ')' or '}') can be any character</param>
        /// <param name="data">The string data in which the parentheses index is searched</param>
        /// <returns>index of opening and close parentheses as a Tuple</returns>
        protected Tuple<int, int> getTopParenteses(char open, char close, string data)
        {
            int cnt_opened = 0;
            bool hadParenteses = false;
            bool hasClosed = false;
            int startIndex = 0, endIndex = 0;
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i] == open) 
                {
                    cnt_opened++;
                    if (!hadParenteses)
                    {
                        hadParenteses = true;
                        startIndex = i;
                    }
                }
                if (data[i] == close)
                {
                    cnt_opened--;
                    if(cnt_opened == 0 && !hasClosed)
                    {
                        endIndex = i;
                        hasClosed = true;
                    }
                }
            }
            if (cnt_opened != 0) throw new FormatException("Uneven Parenteses");
            return new Tuple<int, int>(startIndex, endIndex);
        }

        /// <summary>
        /// Inverts the all wire's direction
        /// Input --> Output
        /// Output --> Input
        /// InOut --> InOut
        /// UNKNOWN --> Throws InvalidOperationException
        /// </summary>
        public void invertAllWires()
        {
            foreach (Wire wire in this.wires)
               wire.invertDirection();
        }
    }

    /// <summary>
    /// A abstract class that represents a input/output component of a module
    /// </summary>
    public abstract class ModuleComponent
    {
        /// <summary>
        /// The name of the parameter or IO
        /// </summary>
        public string name;
        /// <summary>
        /// The comment (if available) At the end of the line
        /// </summary>
        public string comment;

        /// <summary>
        /// Settings for the output generated
        /// </summary>
        public Settings settings;

        public ModuleComponent()
        {
            this.name = "";
        }
        public ModuleComponent(string name)
        {
            this.name = name;
        }

        public ModuleComponent(ModuleComponent otherModuleComponent)
        {
            this.name = otherModuleComponent.name;
            this.comment = otherModuleComponent.comment;
            this.settings = otherModuleComponent.settings;
        }

        /// <summary>
        /// This Function Initializes the obejcts attributes from a line of HDL Code (Excluding Comments)
        /// </summary>
        /// <param name="codeLine">HDL Code for one Wire or Parameter without comment (output format of separate elements function)</param>
        public abstract void initializeFromCodeLine(string codeLine);

        /// <summary>
        /// Generates a Line for this component in an module instantiation
        /// </summary>
        /// <returns>Module instantion line as a string</returns>
        public abstract string generateInstantiationLine();
        /// <summary>
        /// Generates a Line for this component that declares the signal or constant
        /// </summary>
        /// <returns></returns>
        public abstract string generateWireDeclarationLine();

        /// <summary>
        /// Generates a header line used in a modules definition.
        /// It is the same as the format that is inputed by the user but
        /// built from the components attributes.
        /// </summary>
        /// <returns>A module header line e.g. input [7:0] mywire</returns>
        public abstract string generateHeaderLine();

        /// <summary>
        /// Generates the comment to add at the end of the line dependant on the setting
        /// configured in the settings attribute.
        /// </summary>
        /// <returns>A coment for the line e.g. "//output [7:0] | my Comment"</returns>
        public abstract string buildComment();

    }

    /// <summary>
    /// A component of the HDL header of the type "Parameter"
    /// </summary>
    public abstract class Parameter : ModuleComponent
    {
        public string value;

        public Parameter() { }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="otherParameter">The object to copy</param>
        public Parameter(Parameter otherParameter) : base(otherParameter)
        {
            value = otherParameter.value;
        }
    }

    /// <summary>
    /// A component of the HDL header that is an IO port (resembles IO Wire of Verilog)
    /// </summary>
    public abstract class Wire : ModuleComponent
    {

        /// <summary>
        /// The data direction of the IO
        /// </summary>
        public PortDirection direction;

        /// <summary>
        /// Contains the Bus size of e wire
        /// </summary>
        /// <example>"[7:0]" for Verilog or "(7 downto 0)" for VHDL</example>
        public string busSize = "";

        public Wire() { }
        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="otherWire">The object to copy</param>
        public Wire(Wire otherWire) : base(otherWire)
        {
            this.busSize = otherWire.busSize;
            this.direction = otherWire.direction;
        }

        /// <summary>
        /// Inverts the port direction
        /// Input --> Output
        /// Output --> Input
        /// InOut --> InOut
        /// UNKNOWN --> Throws InvalidOperationException
        /// </summary>
        public void invertDirection()
        {
            switch (this.direction)
            {
                case PortDirection.UNKNOWN:
                    throw new InvalidOperationException("Port direction type \"UNKNOWN\" can not be converted.");
                case PortDirection.Input:
                    this.direction = PortDirection.Output;
                    break;
                case PortDirection.Output:
                    this.direction = PortDirection.Input;
                    break;
                case PortDirection.InOut:
                    break;
            }
        }
    }
}
