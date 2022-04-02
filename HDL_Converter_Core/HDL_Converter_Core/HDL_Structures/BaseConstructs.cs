using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Core.HDL_Structures
{
    /// <summary>
    /// A enum for defining the data direction of a Wire
    /// </summary>
    public enum PortDirection{
        Input = 0,
        Output = 1,
        InOut = 2
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
    }

    /// <summary>
    /// A component of the HDL header of the type "Parameter"
    /// </summary>
    abstract class Parameter : ModuleComponent
    {
        public string value;
    }

    /// <summary>
    /// A component of the HDL header that is an IO port (resembles IO Wire of Verilog)
    /// </summary>
    abstract class Wire : ModuleComponent
    {
        /// <summary>
        /// The data direction of the IO
        /// </summary>
        public PortDirection direction;

        /// <summary>
        /// Contains the Bus size of e wire
        /// </summary>
        /// <example>"[7:0]" for Verilog or "(7 downto 0)" for VHDL</example>
        public string busSize;

    }
}
