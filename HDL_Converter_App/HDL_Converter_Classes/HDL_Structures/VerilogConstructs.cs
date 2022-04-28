using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HDL_Converter_Classes.Properties;

namespace HDL_Converter_Classes.HDL_Structures
{

    public class VeriModule : HDLModule
    {

        public VeriModule() { }

        /// <summary>
        /// Initializes a Module from HDL code in string format.
        /// </summary>
        /// <param name="hdlModule"> hdl (header) code in string fromat</param>
        /// <param name="settings">settings object that will be used for the output</param>
        public VeriModule(string hdlModule, Settings settings) : base(hdlModule, settings)
        {        
        }
        /// <summary>
        /// Creates a copy of the module passed to it.
        /// </summary>
        /// <param name="otherModule"></param>
        public VeriModule(VeriModule otherModule)
        {
            this.settings = otherModule.settings;
            this.name = otherModule.name;
            foreach (VeriWire otherWire in otherModule.wires)            
                this.wires.Add(new VeriWire(otherWire));
            foreach (VeriParameter otherParam in otherModule.parameters)
                this.parameters.Add(new VeriParameter(otherParam));            
        }

        public override string generateModuleInstantiation()
        {
            string outputString;
            bool hasParameters;
            if (this.parameters.Count > 0)
            {
                outputString = this.name + System.Environment.NewLine;
                hasParameters = true;
            }
            else
            {
                outputString = this.name + " inst_"+this.name+ System.Environment.NewLine;
                hasParameters = false;
            }
            List<ModuleComponent>[] components = new List<ModuleComponent>[2]
            { this.parameters.Cast<ModuleComponent>().ToList(), this.wires.Cast<ModuleComponent>().ToList()};
            if (this.parameters.Count != 0) outputString += '#';
            foreach (List<ModuleComponent> component in components)
            {    
                if (component.Count != 0)
                {
                    if(hasParameters && outputString.Last() != '#') outputString += System.Environment.NewLine + " inst_" + this.name + System.Environment.NewLine;
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
            }
            outputString += ';';
            return outputString;
        }
       
        /// <summary>
        /// Generates the top level file of a testbench. It contains the declaration of constants,
        /// wires, a instance of the module passed by the user and a instance of the verify module used to verify
        /// the Module
        /// </summary>
        /// <returns>The top level file content as a string</returns>
        public override string generateTestbenchTopLevel()
        {
            string hdlCode = Resources.VERILOG_TESTBENCH_TOP;
            hdlCode = hdlCode.Replace("$NAME$", "tb_"+this.name);
            hdlCode = hdlCode.Replace("$WIREDECLARATIONS$", this.generateWireDeclaration());
            hdlCode = hdlCode.Replace("$CONSTANTS$", this.generateParameterDeclaration());
            hdlCode = hdlCode.Replace("$SECTIONCOMMENT$", "Module Instantiations");
            hdlCode = hdlCode.Replace("$DATE$", System.DateTime.Now.ToString());

            bool emtyIO = this.settings.emptyIOs;
            this.settings.emptyIOs = false;
            string instance = this.generateModuleInstantiation();
            this.settings.emptyIOs = emtyIO;
            string instanciation = instance.Replace("inst_" + this.name, "DUT") + System.Environment.NewLine
                + System.Environment.NewLine + instance.Replace("inst_" + this.name, "Verify").Replace(this.name,"verify_"+this.name);
            hdlCode = hdlCode.Replace("$INSTANCES$", instanciation);
            return hdlCode;
        }

        /// <summary>
        /// Generates the Verify module file content. It is a copy of the original moule passed by the user
        /// but with inverted data diretion (input --> output, output --> input). The User can insert his
        /// stimuli and assertion code in this file.
        /// </summary>
        /// <returns>The verify file as a string</returns>
        public override string generateTestbenchVerify()
        {
            VeriModule verify = new VeriModule(this);
            verify.invertAllWires();
            verify.name = "verify_" + this.name;
            string header = verify.generateModuleHeader();
            string template = Resources.VERILOG_TEMPLATE_NO_HEADER;
            template = template.Replace("$NAME$", verify.name);
            template = template.Replace("$DATE$", System.DateTime.Now.ToString());
            template = template.Replace("$SECTIONCOMMENT$", "Verification Code");
            template = template.Replace("$INSTANCES$", "").Replace("$WIREDECLARATIONS$", "");
            template = template.Replace("$HEADER$", header);
            return template;
        }

        public override string generateParameterDeclaration()
        {
            string outputString = "";
            int lastItemCheck = this.parameters.Count;
            foreach (VeriParameter param in this.parameters)
            {
                lastItemCheck--;
                outputString += param.generateWireDeclarationLine() + param.buildComment();
                if (lastItemCheck != 0) outputString += System.Environment.NewLine;
            }
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
            string outsideDeclarationHDL;
            if(topParenthesis.Item1 != 0)
            {
                string checkParamTag = hdlCode.Substring(0, topParenthesis.Item1);
                if (checkParamTag.Contains("#"))
                {
                    initializeParameters(hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1));
                    string remainingHDL = hdlCode.Substring((topParenthesis.Item2) + 1);
                    Tuple<int, int> wiresec = this.getTopParenteses('(', ')', remainingHDL);
                    wireSection = remainingHDL.Substring(wiresec.Item1+1, wiresec.Item2-wiresec.Item1-1);
                    outsideDeclarationHDL = remainingHDL.Substring(wiresec.Item2 + 1);
                }
                else
                {
                    wireSection = hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1);
                    outsideDeclarationHDL = hdlCode.Substring(topParenthesis.Item2 + 1);
                }
            }
            else
            {
                wireSection = hdlCode.Substring(topParenthesis.Item1+1, topParenthesis.Item2 - topParenthesis.Item1-1);
                outsideDeclarationHDL = hdlCode.Substring(topParenthesis.Item2 + 1);
            }
            initializeWires(wireSection);
            initializeUnknownWires(outsideDeclarationHDL);
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

        private void initializeUnknownWires(string remainingHDL)
        {
            foreach(VeriWire wire in this.wires)
            {
                if(wire.direction == PortDirection.UNKNOWN)
                {
                    wire.initializeUnknown(remainingHDL);
                }
            }
        }

        /// <summary>
        /// Generates a Modules Header
        /// </summary>
        /// <example>
        /// module myModule
        /// #(
        /// parameter someParameter = 1
        /// )
        /// (
        /// input someWire;
        /// );
        /// </example>
        /// <returns>A Module Header (see Example)</returns>
        public override string generateModuleHeader()
        {
            string header = Resources.VERILOG_MODULE_HEADER;
            header = header.Replace("$NAME$", this.name);
            header = header.Replace("$PORT$", this.generateHeaderPort());
            if (this.parameters.Count > 0)
                header = header.Replace("$PARAMETERS$", this.generateHeaderParameters());
            else
                header = header.Replace("$PARAMETERS$", "");
            return header;
        }

        /// <summary>
        /// Generates the wires in a module Header
        /// </summary>
        /// <returns>All wires in the module as a string</returns>
        public override string generateHeaderPort()
        {
            string port = "";
            int elementCount = this.wires.Count;
            foreach(VeriWire wire in this.wires)
            {
                elementCount--;
                port += wire.generateHeaderLine();
                if (elementCount > 0)
                    port += ", " + wire.buildComment() + System.Environment.NewLine;
                else
                    port += " " + wire.buildComment();
            }
            return port;
        }

        /// <summary>
        /// Generates all parameters in a module header
        /// </summary>
        /// <returns>String of parameters containd within "#(" and ")"</returns>
        public override string generateHeaderParameters()
        {
            string port = "#(" + System.Environment.NewLine;
            int elementCount = this.parameters.Count;
            foreach (VeriParameter wire in this.parameters)
            {
                elementCount--;
                port += wire.generateHeaderLine();
                if (elementCount > 0)
                    port += ", " + wire.buildComment() + System.Environment.NewLine;
                else
                    port += " " + wire.buildComment();
            }
            port += System.Environment.NewLine + ")";
            return port;
        }
    }

    /// <summary>
    /// A Wire in the Verilog domain serving as an IO to a module
    /// </summary>
    public class VeriWire : Wire
    {
        /// <summary>
        /// Marks if the wire has the signed property
        /// </summary>
        public bool signed = false;

        public VeriWire() { }

        public VeriWire(VeriWire otherWire) : base(otherWire)
        {
            this.signed = otherWire.signed;
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
            string signedString = this.signed ? "signed " : "";
            if(this.busSize != "")
            {
                return "wire "+ signedString + this.busSize + " " + this.name + ";";
            }
            else
            {
                 return "wire "+ signedString + this.name + ";";
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
            string[] lowerData = codeLine.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Covers the case that port direction and bus size is declared outside of header
            if (lowerData.Length == 1)
            {
                this.name = codeLine.Trim();
                this.direction = PortDirection.UNKNOWN;
                return;
            }
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
            this.signed = codeLine.Replace(this.name, "").ToLower().Contains("signed");
        }

        /// <summary>
        /// This function initializes the wire in the case that the module header 
        /// does not contain data direction and bus size information.
        /// </summary>
        /// <param name="remainingHDL">HDL Code without the header</param>
        /// <returns>true if function was successfull at finding direction information</returns>
        public bool initializeUnknown(string remainingHDL)
        {
            int firstIndex = remainingHDL.IndexOf(this.name);
            if (firstIndex == -1) return false;
            for(int i = 1;firstIndex != i ; i++)
            {
                string information = remainingHDL.Substring(firstIndex - i, i);
                if(information.Contains(System.Environment.NewLine) || information.Contains(';'))
                {
                    //Recursive if the first mention was not the direction information
                    return this.initializeUnknown(remainingHDL.Substring(firstIndex + 1));
                }
                string l_info = information.ToLower();
                if (l_info.Contains("input"))
                {
                    this.direction = PortDirection.Input;
                }else if (l_info.Contains("output"))
                {
                    this.direction = PortDirection.Output;
                }else if (l_info.Contains("inout"))
                {
                    this.direction = PortDirection.InOut;
                }
                if(this.direction != PortDirection.UNKNOWN)
                {
                    int openIndex = information.IndexOf('[');
                    int closeIndex = information.IndexOf(']');
                    if(openIndex != -1 && closeIndex != -1)
                    {
                        this.busSize = information.Substring(openIndex, closeIndex - openIndex +1);
                    }
                    return true;
                }
            }
            return false;
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

        public override string generateHeaderLine()
        {
            string retVal = "";
            switch (this.direction)
            {
                case PortDirection.UNKNOWN:
                    break;
                case PortDirection.Input:
                    retVal += "input ";
                    break;
                case PortDirection.Output:
                    retVal += "output ";
                    break;
                case PortDirection.InOut:
                    retVal += "inout ";
                    break;
            }
            if (this.signed) retVal += "signed ";
            if (this.busSize != "") retVal += this.busSize + " ";
            retVal += this.name;
            return retVal;
        }
    }

    /// <summary>
    /// A Parameter in the Verilog domain serving as a configuration parameter for the module
    /// </summary>
    public class VeriParameter : Parameter
    {
        public VeriParameter() { }

        /// <summary>
        /// Copy constructor creates a new object with same properties
        /// </summary>
        /// <param name="otherParameter">object to copy</param>
        public VeriParameter(VeriParameter otherParameter) : base(otherParameter) { }
        

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

        public override string generateHeaderLine()
        {
            string retVal = "parameter " + this.name;
            if (this.value != "") retVal += " = " + this.value;
            return retVal;
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
