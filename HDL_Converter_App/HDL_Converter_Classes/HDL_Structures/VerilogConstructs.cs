using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HDL_Converter_Classes.HDL_Structures
{

    public class VeriModule : HDLModule
    {

        public VeriModule() { }

        public VeriModule(string hdlModule, Settings settings)
        {
            this.settings = settings;
            this.initializeFormHDLCode(hdlModule);
        }

        public override string generateModuleInstantiation()
        {
            string outputString = this.name + System.Environment.NewLine;
            List<ModuleComponent>[] components = new List<ModuleComponent>[2]
            { this.parameters.Cast<ModuleComponent>().ToList(), this.wires.Cast<ModuleComponent>().ToList()};
            if (this.parameters.Count != 0) outputString += '#';
            foreach(List<ModuleComponent> component in components)
            {       
                if (component.Count != 0)
                {
                    outputString += '('+ System.Environment.NewLine;
                    int lastCounter = component.Count;
                    foreach(ModuleComponent elem in component)
                    {
                        lastCounter--;
                        outputString += "\t"+ elem.generateInstantiationLine() +(lastCounter==0?"":",")
                            + elem.buildComment() + System.Environment.NewLine;
                    }
                    outputString += ')';
                }
                outputString += System.Environment.NewLine + "inst_" + this.name + System.Environment.NewLine;
            }
            outputString += ';';
            return outputString;
        }

        public override string generateWireDeclaration()
        {
            string outputString = "";
            int lastItemCheck = this.wires.Count;
            foreach(VeriWire wire in this.wires)
            {
                lastItemCheck--;
                outputString += wire.generateWireDeclarationLine() + wire.buildComment();
                if (lastItemCheck != 0) outputString += System.Environment.NewLine;
            }
            return outputString;
        }

        protected override void initializeFormHDLCode(string hdlCode)
        {
            int nameIndexStart = hdlCode.ToLower().IndexOf("module") + 6;
            int nameIndexEnd = hdlCode.IndexOf('(') - 1;
            this.name = hdlCode.Substring(nameIndexStart, nameIndexEnd - nameIndexStart).
                Replace('#', ' ').Replace(System.Environment.NewLine, " ").Trim();
            Tuple<int, int> topParenthesis = this.getTopParenteses('(', ')', hdlCode);
            string wireSection;
            if(topParenthesis.Item1 != 0)
            {
                string checkParamTag = hdlCode.Substring(0, topParenthesis.Item1);
                if (checkParamTag.Contains("#"))
                {
                    initializeParameters(hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1));
                    string remainingHDL = hdlCode.Substring((topParenthesis.Item2) + 1);
                    Tuple<int, int> wiresec = this.getTopParenteses('(', ')', remainingHDL);
                    wireSection = remainingHDL.Substring(wiresec.Item1+1, wiresec.Item2-wiresec.Item1-1);
                }
                else
                {
                    wireSection = hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1);
                }
            }
            else
            {
                wireSection = hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1);
            }
            initializeWires(wireSection);
        }

        protected override void initializeParameters(string hdlCode)
        {
            List<string[]> paramRaw = VeriDataProcessing.separateElements(hdlCode);
            this.parameters = new List<Parameter>();
            foreach(string[] parLine in paramRaw)
            {
                if(parLine[0] != "")
                {
                    VeriParameter param = new VeriParameter();
                    param.settings = this.settings;
                    param.comment = parLine[1];
                    param.initializeFromCodeLine(parLine[0]);
                    this.parameters.Add(param);
                }
            }
        }

        protected override void initializeWires(string hdlCode)
        {
            List<string[]> wiresRaw = VeriDataProcessing.separateElements(hdlCode);
            this.wires = new List<Wire>();
            foreach (string[] wireLine in wiresRaw)
            {
                if (wireLine[0] != "")
                {
                    VeriWire wire = new VeriWire();
                    wire.settings = this.settings;
                    wire.comment = wireLine[1];
                    wire.initializeFromCodeLine(wireLine[0]);
                    this.wires.Add(wire);
                }
            }
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
            if (codeLine.Contains("(*"))
            {
                //Removes the Mark Debug Statement from IDE
                int removeIndex = codeLine.IndexOf("*)");
                codeLine = codeLine.Substring(removeIndex + 2).Trim();
            }
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
                string lowerCodeline = codeLine.ToLower();
                int nameIndex = lowerCodeline.IndexOf("wire")+4;
                int regIndex = lowerCodeline.IndexOf("reg") + 3;
                if(regIndex >= 3)
                {
                    this.name = codeLine.Substring(regIndex).Trim();
                }else if(nameIndex >= 4)
                {
                    this.name = codeLine.Substring(nameIndex).Trim();
                }
                else
                {
                    this.name = codeLine.Substring(codeLine.Trim().IndexOf(' ')).Trim();
                }
            }            
        }

        public override string buildComment()
        {
            int commentType = settings.includeInputComments ? 1 : 0;
            if (settings.addWireDirectionComment) commentType += 2;
            if (this.comment == "" && commentType > 0 && commentType != 2) commentType -= 1;
            string outComment = " //";
            switch (commentType)
            {
                case 0:
                    return "";
                case 1:
                    outComment += this.comment;
                    break;
                case 2:
                    outComment += this.direction.ToString() + ' ' + this.busSize;
                    break;
                case 3:
                    outComment += this.direction.ToString() + ' ' + this.busSize + " | " + this.comment;
                    break;
            }
            return outComment;
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

        /// <summary>
        /// This Function Initializes the obejcts attributes from a line of HDL Code (Excluding Comments)
        /// </summary>
        /// <param name="codeLine">HDL Code for one Wire or Parameter without comment (output format of separate elements function)</param>
        public override void initializeFromCodeLine(string codeLine)
        {
            int valueIndex = codeLine.IndexOf('=');
            this.value = codeLine.Substring(valueIndex + 1).Trim();
            int paramIndex = codeLine.ToLower().IndexOf("parameter")+9;
            this.name = codeLine.Substring(paramIndex, valueIndex - 1-paramIndex).Trim();
        }
        public override string buildComment()
        {
            if (settings.includeInputComments && this.comment != "") return (" //" + this.comment);
            else return "";
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
            string[] initialEntry = new string[2];
            initialEntry[0] = "";
            initialEntry[1] = "";
            retList.Add(initialEntry);
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
