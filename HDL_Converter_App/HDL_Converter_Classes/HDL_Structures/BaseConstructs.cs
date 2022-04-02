﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Classes.HDL_Structures
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
    /// A Class resembling a Module in HDL it contains lists of all wires
    /// and parameters that make up the interface for the module.
    /// </summary>
    public abstract class HDLModule
    {
        public List<Parameter> parameters;
        public List<Wire> wires;

        protected abstract void initializeFormHDLCode(string hdlCode);
        protected abstract void initializeParameters(string hdlCode);
        protected abstract void initializeWires(string hdlCode);

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
                    if(cnt_opened == 0)
                    {
                        endIndex = i;
                    }
                }
            }
            if (cnt_opened != 0) throw new FormatException("Uneven Parenteses");
            return new Tuple<int, int>(startIndex, endIndex);
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
    public abstract class Parameter : ModuleComponent
    {
        public string value;
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

    }
}