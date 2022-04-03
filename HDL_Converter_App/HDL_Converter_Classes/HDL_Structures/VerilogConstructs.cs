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

        /// <summary>
        /// This Function Initializes the obejcts attributes from a line of HDL Code (Excluding Comments)
        /// </summary>
        /// <param name="codeLine">HDL Code for one Wire or Parameter without comment (output format of separate elements function)</param>
        public override void initializeFromCodeLine(string codeLine)
        {
            string[] lowerData = codeLine.ToLower().Split(' ');
            if (lowerData[0].Contains("input"))
            {
                this.direction = PortDirection.Input;
            }else if (lowerData[0].Contains("output"))
            {
                this.direction = PortDirection.Output;
            }else if (lowerData[0].Contains("inout"))
            {
                this.direction = PortDirection.InOut;
            }
            else
            {
                throw new FormatException("Wire had invalid data direction");
            }
            string restOfData = "";
            for (int i = 1; i < lowerData.Length; i++) restOfData += lowerData[i];
            if (restOfData.Contains('['))
            {
                string busSize = "";
                bool enabled = false;
                foreach (char s in restOfData)
                {
                    if (s == '[') enabled = true;
                    if (enabled) busSize += s;
                    if (s == ']') break;
                }
                this.busSize = busSize;
                this.name = codeLine.Split(']')[1].Trim();
            }
            else
            {
                this.busSize = "";
                int nameIndex = codeLine.ToLower().IndexOf("wire")+4;
                this.name = codeLine.Substring(nameIndex).Trim();
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

        public override void initializeFromCodeLine(string codeLine)
        {
            throw new NotImplementedException();
        }
    }

    public static class VeriDataProcessing
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
            List<string[]> retList = new List<string[]>();
            string[] hdlSplited = hdlCode.Replace("//", "//{Comment}").Split(new string[] { System.Environment.NewLine, "//" }, StringSplitOptions.None);
            foreach (string element in hdlSplited)
            {
                if (element.Trim() == "") continue;
                if (!element.Contains("{Comment}"))
                {
                    string[] splitedAtComma = element.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string commaSpearated in splitedAtComma)
                    {
                        string[] nextEntry = new string[2];
                        nextEntry[0] = commaSpearated.Replace(',', ' ').Trim();
                        nextEntry[1] = "";
                        retList.Add(nextEntry);
                    }                    
                }
                else
                {
                    if (retList.Last()[1] != "")
                    {
                        string[] nextEntry = new string[2];
                        nextEntry[0] = "";
                        nextEntry[1] = "";
                        retList.Add(nextEntry);
                    }
                    retList.Last()[1] = element.Replace("{Comment}", " ").Trim();
                }
            }
            return retList;
        }
    }
}
