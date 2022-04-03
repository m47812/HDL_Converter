using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HDL_Converter_Classes.HDL_Structures
{

    public class VeriModule : HDLModule
    {

        public VeriModule() { }

        public VeriModule(string hdlModule)
        {
            this.initializeFormHDLCode(hdlModule);
        }

        protected override void initializeFormHDLCode(string hdlCode)
        {
            Tuple<int, int> topParenthesis = this.getTopParenteses('(', ')', hdlCode);
            string wireSection;
            if(topParenthesis.Item1 != 0)
            {
                string checkParamTag = hdlCode.Substring(0, topParenthesis.Item1);
                if (checkParamTag.Contains("#"))
                {
                    initializeParameters(hdlCode.Substring(topParenthesis.Item1, topParenthesis.Item2));
                    string remainingHDL = hdlCode.Substring((topParenthesis.Item2) + 1);
                    Tuple<int, int> wiresec = this.getTopParenteses('(', ')', remainingHDL);
                    wireSection = remainingHDL.Substring(wiresec.Item1, wiresec.Item2);
                }
                else
                {
                    wireSection = hdlCode.Substring(topParenthesis.Item1, topParenthesis.Item2);
                }
            }
            else
            {
                wireSection = hdlCode.Substring(topParenthesis.Item1, topParenthesis.Item2);
            }
            initializeWires(wireSection);
        }

        protected override void initializeParameters(string hdlCode)
        {
            throw new NotImplementedException();
        }

        protected override void initializeWires(string hdlCode)
        {
            throw new NotImplementedException();
        }

    
    }

    /// <summary>
    /// A Wire in the Verilog domain serving as an IO to a module
    /// </summary>
    public class VeriWire : Wire
    {
        /// <summary>
        /// Separates a string of HDL Code into Wire Components
        /// </summary>
        /// <param name="hdlCode">A HDL code segment (the part that is inbetween the parenthesi 
        /// containing the module IO declaration
        /// </param>
        /// <returns>A List of string arrays each array containing the module code at index 0
        /// and (if available) a comment at index 1</returns>
        public static List<string[]> separateElements(string hdlCode)
        {
            string[] splited = hdlCode.Split(',');
            List<string[]> retData = new List<string[]>();
            for(int i = 0; i < splited.Length; i++)
            {
                if (splited[i].Replace(System.Environment.NewLine," ").Trim() [0] == '/')
                {
                    string[] splitedAtNewLine = splited[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    retData.Last()[1] = splitedAtNewLine[0].Trim();
                    if (splitedAtNewLine.Length > 1)
                    {
                        string[] nextEntry = new string[2];
                        nextEntry[0] = splitedAtNewLine[1];
                        nextEntry[1] = "";
                        retData.Add(nextEntry);
                    }
                }
                else
                {
                    string[] nextEntry = new string[2];
                    nextEntry[0] = splited[i].Replace(System.Environment.NewLine, " ").Trim();
                    nextEntry[1] = "";
                    retData.Add(nextEntry);
                }
            }

            //Since Verilog does not have a "," for the last element comment and code
            //would end up in one string this if statement checks for and corrects this.
            if (retData.Last()[0].Contains("//"))
            {
                string[] correctedLastElement = retData.Last()[0].Split('/');
                retData.Last()[0] = correctedLastElement[0].Trim();
                string lastComment = "";
                for (int i = 2; i < correctedLastElement.Length; i++) lastComment += correctedLastElement[i];
                retData.Last()[1] = "//"+lastComment;
            }
            return retData;
        }

        /// <summary>
        /// Generates the module instantiation line for one wire
        /// </summary>
        /// <returns></returns>
        public override string generateInstantiationLine()
        {
            string commandLine;
            if (this.settings.emptyIOs)
            {
                commandLine = '.' + this.name + "()";
            }
            else
            {
                commandLine = '.' + this.name + '('+this.name+")";
            }
            return commandLine;
        }

        public override string generateWireDeclarationLine()
        {
            if(this.busSize != "")
            {
                return "wire " + this.busSize + " " + this.name + ";";
            }
            else
            {
                 return "wire " + this.name + ";";
            }
        }
    }

    /// <summary>
    /// A Parameter in the Verilog domain serving as a configuration parameter for the module
    /// </summary>
    public class VeriParameter : Parameter
    {
        public override string generateInstantiationLine()
        {
            string commandLine;
            if (this.settings.emptyIOs)
            {
                commandLine = '.' + this.name + "()";
            }
            else
            {
                commandLine = '.' + this.name + '(' + this.name + ")";
            }
            return commandLine;
        }

        public override string generateWireDeclarationLine()
        {
            return "localparam " + this.name +" = "+this.value+ ";";
        }
    }
}
