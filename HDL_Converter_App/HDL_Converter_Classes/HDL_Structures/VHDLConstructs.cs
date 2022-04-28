using HDL_Converter_Classes.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HDL_Converter_Classes.HDL_Structures
{
    /// <summary>
    /// The Data Types of signals and generics in VHDL
    /// Not used yet needed for Verilog <---> VHDL conversions 
    /// </summary>
    public enum VHDLDataTypes
    {
        UNKNOWN_TYPE = 0,
        std_ulogic_vector = 1,
        std_logic_vector = 2,
        std_ulogic = 3,
        std_logic = 4,
        unsigned = 5,
        signed = 6,
        integer = 7,
        natural = 8
    }

    /// <summary>
    /// A Module in VHDL (Entity) containing IO signals and generics
    /// </summary>
    public class VHDLModule : HDLModule
    {
        public VHDLModule() { }
        /// <summary>
        /// Initializes a Module from HDL code in string format.
        /// </summary>
        /// <param name="hdlModule"> hdl (header) code in string fromat</param>
        /// <param name="settings">settings object that will be used for the output</param>
        public VHDLModule(string hdlModule, Settings settings) : base(hdlModule, settings)
        {
        }

        /// <summary>
        /// Copy Constructor: Creates a copy of the other Module creating new objects for wires
        /// and parameters but using same settings object
        /// </summary>
        /// <param name="otherModule">Module to copy</param>
        public VHDLModule(VHDLModule otherModule)
        {
            this.settings = otherModule.settings;
            this.name = otherModule.name;
            foreach (VHDLWire otherWire in otherModule.wires)
                this.wires.Add(new VHDLWire(otherWire));
            foreach (VHDLParameter otherParam in otherModule.parameters)
                this.parameters.Add(new VHDLParameter(otherParam));
        }

        /// <summary>
        /// Generates the generics of a modules entity
        /// </summary>
        /// <returns>The generics wrapped in "generic ()"</returns>
        public override string generateHeaderParameters()
        {
            StringBuilder headerParam = new StringBuilder("generic (" + System.Environment.NewLine);
            foreach (var parameter in this.parameters)
            {
                headerParam.Append(parameter.generateHeaderLine());
                if (parameter != this.parameters.Last()) headerParam.Append(';');
                headerParam.AppendLine(parameter.buildComment());
            }
            headerParam.Append(");");
            return headerParam.ToString();
        }

        /// <summary>
        /// Generates the port of a modules entity
        /// </summary>
        /// <returns>The port signals wrapped in "port ()"</returns>
        public override string generateHeaderPort()
        {
            StringBuilder headerPort = new StringBuilder("port (" + System.Environment.NewLine);
            foreach(var wire in this.wires)
            {
                headerPort.Append(wire.generateHeaderLine());
                if (wire != this.wires.Last()) headerPort.Append(';');
                headerPort.AppendLine(wire.buildComment());
            }
            headerPort.Append(");");
            return headerPort.ToString();
        }

        /// <summary>
        /// Generates a modules entity
        /// </summary>
        /// <returns>Entity of the module</returns>
        public override string generateModuleHeader()
        {
            return this.headerOrComponentbuild(Resources.VHDL_ENTITY);
        }

        /// <summary>
        /// Generates a component declaration (same as entity but using the word component instestead)
        /// </summary>
        /// <returns>component declaration</returns>
        public string generateComponent()
        {
            return this.headerOrComponentbuild(Resources.VHDL_COMPONENT);
        }

        /// <summary>
        /// Creates the instantiation code of the module
        /// </summary>
        /// <returns>instantiation code</returns>
        public override string generateModuleInstantiation()
        {
            StringBuilder template = new StringBuilder(Resources.VHDL_INSTANCE);
            template.Replace("$NAME$", this.name);
            if (this.parameters.Count > 0)
            {
                StringBuilder parmeterscontainer = new StringBuilder("    generic map(" + System.Environment.NewLine+"*)");
                StringBuilder parametersBuilt = new StringBuilder();
                foreach(var parameter in this.parameters)
                {
                    parametersBuilt.Append(parameter.generateInstantiationLine());
                    if (parameter != this.parameters.Last()) parametersBuilt.Append(',');
                    parametersBuilt.Append(parameter.buildComment());
                    parametersBuilt.Append(System.Environment.NewLine);
                }
                parmeterscontainer.Replace("*",parametersBuilt.ToString());
                parmeterscontainer.Replace("\n", "\n    ");
                parmeterscontainer.Append(System.Environment.NewLine);              
                template.Replace("$PARAMETERS$", parmeterscontainer.ToString());
            }
            else
            {
                template.Replace("$PARAMETERS$", "");
            }
            if(this.wires.Count > 0)
            {
                StringBuilder portcontainer = new StringBuilder("    port map(" + System.Environment.NewLine + "*)");
                StringBuilder portBuilt = new StringBuilder();
                foreach (var wire in this.wires)
                {
                    portBuilt.Append(wire.generateInstantiationLine());
                    if (wire != this.wires.Last()) portBuilt.Append(',');
                    portBuilt.Append(wire.buildComment());
                    portBuilt.Append(System.Environment.NewLine);
                }
                portcontainer.Replace("*", portBuilt.ToString());
                portcontainer.Replace("\n", "\n    ");
                template.Replace("$PORT$", portcontainer.ToString());
            }
            else
            {
                template.Replace("$PORT$", "");
            }
            return template.ToString();
        }

        /// <summary>
        /// Generates the code declaring all generics as constants
        /// </summary>
        /// <returns>constants given by generics</returns>
        public override string generateParameterDeclaration()
        {
            StringBuilder constants = new StringBuilder();
            foreach(var parameter in this.parameters)
            {
                constants.Append(parameter.generateWireDeclarationLine());
                constants.AppendLine(parameter.buildComment());
            }
            return constants.ToString();
        }

        /// <summary>
        /// Creates a testbench toplevel file content with a instance of the original module and the verify
        /// with inverted ports and connects the two modules
        /// </summary>
        /// <returns>Top level file content</returns>
        public override string generateTestbenchTopLevel()
        {
            VHDLModule verify = new VHDLModule(this);
            verify.invertAllWires();
            verify.name = "verify_" + this.name;
            StringBuilder builder = new StringBuilder(Resources.VHDL_TESTBENCH_TOP);
            StringBuilder components = new StringBuilder();
            components.AppendLine(this.generateComponent());
            components.AppendLine();
            components.AppendLine(verify.generateComponent());
            builder.Replace("$COMPONENTS$", components.ToString());

            StringBuilder instances = new StringBuilder();
            instances.AppendLine(this.generateModuleInstantiation());
            instances.AppendLine();
            instances.AppendLine(verify.generateModuleInstantiation());
            builder.Replace("$INSTANCES$",instances.ToString());

            builder.Replace("$NAME$", "tb_" + this.name);
            builder.Replace("$SECTIONCOMMENT$", "Testbench Modules");
            builder.Replace("$CONSTANTS$", this.generateParameterDeclaration());
            builder.Replace("$WIREDECLARATIONS$", this.generateWireDeclaration());
            builder.Replace("$DATE$", System.DateTime.Now.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// Creates the testbench verify module containing the same entity as the original module but
        /// with inverted data directions
        /// </summary>
        /// <returns>File content of the verify module</returns>
        public override string generateTestbenchVerify()
        {
            VHDLModule verify = new VHDLModule(this);
            verify.invertAllWires();
            verify.name = "verify_" + this.name;
            StringBuilder builder = new StringBuilder(Resources.VHDL_MODULE_SYNTAX);
            builder.Replace("$INSTANCES$", "");
            builder.Replace("$COMPONENTS$", System.Environment.NewLine);
            builder.Replace("$CONSTANTS$", System.Environment.NewLine);
            builder.Replace("$WIREDECLARATIONS$", System.Environment.NewLine);
            builder.Replace("$SECTIONCOMMENT$", "Verification Code");
            builder.Replace("$NAME$", verify.name);
            builder.Replace("$ENTITY$", verify.generateModuleHeader());
            builder.Replace("$DATE$", System.DateTime.Now.ToString());
            return builder.ToString();
        }

        /// <summary>
        /// Declares all signals contained in the modules entity
        /// </summary>
        /// <returns>Declaration of all signals in the entity</returns>
        public override string generateWireDeclaration()
        {
            StringBuilder signals = new StringBuilder();
            foreach(var wire in this.wires)
            {
                signals.Append(wire.generateWireDeclarationLine());
                signals.AppendLine(wire.buildComment());
            }
            return signals.ToString();
        }

        /// <summary>
        /// Initializes the module from a given HDL Code (containing the enity of a module)
        /// </summary>
        /// <param name="hdlCode">HDL code containing the entity</param>
        /// <exception cref="FormatException">When the module does not contain a proper entity beginning or ending</exception>
        protected override void initializeFormHDLCode(string hdlCode)
        {
            //Find entity boundries
            string entity_L = hdlCode.ToLower();
            int startIndex = entity_L.IndexOf("entity") + 6;
            this.name = hdlCode.Substring(startIndex, hdlCode.Substring(startIndex).IndexOf(" is")).Trim();
            if (startIndex == -1) throw new FormatException("Beginning of entity not found");
            int endIndex = entity_L.IndexOf("end entity", startIndex);
            if (endIndex == -1) throw new FormatException("Entity end not found (only one whitspace between \"entity\" and \"end\" tollerated)");
            string entity = hdlCode.Substring(startIndex, endIndex - startIndex);
            entity_L = entity_L.Substring(startIndex, endIndex - startIndex);
            //Extract Generics (if given)
            startIndex = entity_L.IndexOf("generic");
            if(startIndex != -1)
            {
                Tuple<int, int> genericBoundries = this.getTopParenteses('(', ')', entity_L.Substring(startIndex));
                this.initializeParameters(entity.Substring(startIndex+genericBoundries.Item1 + 1, genericBoundries.Item2 - genericBoundries.Item1-1));
            }
            //Extract Port signals
            startIndex = entity_L.IndexOf("port");
            if(startIndex != -1)
            {
                Tuple<int, int> portBoundries = this.getTopParenteses('(', ')', entity_L.Substring(startIndex));
                this.initializeWires(entity.Substring(startIndex+portBoundries.Item1 + 1, portBoundries.Item2 - portBoundries.Item1-1));
            }           
        }

        /// <summary>
        /// Subfunction of initializeFormHDLCode that initializes the generics
        /// </summary>
        /// <param name="hdlCode">generics section of the entity</param>
        protected override void initializeParameters(string hdlCode)
        {
            List<string[]> paramRaw = VHDLDataProcessing.separateElements(hdlCode);
            this.parameters = new List<Parameter>();
            foreach (string[] parLine in paramRaw)
            {
                if (parLine[0] != "")
                {
                    VHDLParameter param = new VHDLParameter();
                    param.settings = this.settings;
                    param.comment = parLine[1];
                    param.initializeFromCodeLine(parLine[0]);
                    this.parameters.Add(param);
                }
            }
        }

        /// <summary>
        /// Subfunction of initializeFormHDLCode that initializes the ports signals
        /// </summary>
        /// <param name="hdlCode">port section of the entity</param>
        protected override void initializeWires(string hdlCode)
        {
            List<string[]> wiresRaw = VHDLDataProcessing.separateElements(hdlCode);
            this.wires = new List<Wire>();
            foreach (string[] wireLine in wiresRaw)
            {
                if (wireLine[0] != "")
                {
                    VHDLWire wire = new VHDLWire();
                    wire.settings = this.settings;
                    wire.comment = wireLine[1];
                    wire.initializeFromCodeLine(wireLine[0]);
                    this.wires.Add(wire);
                }
            }
        }

        /// <summary>
        /// Subfunction that can build the entity or component declaration from given template
        /// combined in one function since entity and component output only differ by one word (entity or component)
        /// </summary>
        /// <param name="templateString">The entity or component template</param>
        /// <returns>Entity or component declaration</returns>
        private string headerOrComponentbuild(string templateString)
        {
            StringBuilder template = new StringBuilder(templateString);
            template.Replace("$NAME$", this.name);
            if (this.parameters.Count > 0)
                template.Replace("$PARAMETERS$", this.generateHeaderParameters()+System.Environment.NewLine);
            else
                template.Replace("$PARAMETERS$", "");
            if (this.wires.Count > 0)
                template.Replace("$PORT$", this.generateHeaderPort());
            else
                template.Replace("$PORT$", "");
            return template.ToString();
        }
    }

    /// <summary>
    /// A Wire in the VHDL (signal in VHDL) domain serving as an IO to a module
    /// </summary>
    public class VHDLWire : Wire
    {
        public VHDLWire() { }
        public VHDLWire(VHDLWire otherWire) : base(otherWire) { }

        public override string buildComment()
        {
            int commentType = settings.includeInputComments ? 1 : 0;
            if (settings.addWireDirectionComment) commentType += 2;
            if (this.comment == "" && commentType > 0 && commentType != 2) commentType -= 1;
            string outComment = " --";
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
            string retString = this.name + " : ";
            switch (this.direction)
            {
                case PortDirection.UNKNOWN:
                    break;
                case PortDirection.Input:
                    retString += "in ";
                    break;
                case PortDirection.Output:
                    retString += "out ";
                    break;
                case PortDirection.InOut:
                    retString += "inout ";
                    break;
            }
            retString += this.busSize;
            return retString;
        }

        public override string generateInstantiationLine()
        {
            string retString = this.name + " => ";
            if (!this.settings.emptyIOs) retString += this.name;
            return retString;
        }

        public override string generateWireDeclarationLine()
        {
            return "signal " + this.name + " : " + this.busSize + ";";
        }

        public override void initializeFromCodeLine(string codeLine)
        {
            if (!codeLine.Contains(":")) throw new FormatException("Invalid File Line: " + codeLine);//Sanity Check
            string[] type_name_sep = codeLine.Split(':');
            this.name = type_name_sep[0].Trim();
            string[] vec_dir_sep = type_name_sep[1].Trim().Split(new string[] { " " ,"\t"}, StringSplitOptions.RemoveEmptyEntries);
            switch (vec_dir_sep[0].ToLower())
            {
                case "in":
                    this.direction = PortDirection.Input;
                    break;
                case "out":
                    this.direction = PortDirection.Output;
                    break;
                case "inout":
                    this.direction = PortDirection.InOut;
                    break;
                default:
                    throw new FormatException("Invalid data direction in line: " + codeLine);
            }
            this.busSize = "";
            for (int i = 1; i < vec_dir_sep.Length; i++) this.busSize += vec_dir_sep[i] + " ";
            this.busSize = this.busSize.Trim();
        }
    }

    /// <summary>
    /// A Parameter in the VHDL domain (generic in VHDL) serving as a configuration parameter for the module
    /// </summary>
    public class VHDLParameter : Parameter
    {
        public string dataType;

        public VHDLParameter() { }
        public VHDLParameter(VHDLParameter otherParameter) : base(otherParameter)
        {
            this.dataType = otherParameter.dataType;
        }
        public override string buildComment()
        {
            if (settings.includeInputComments && this.comment != "") return (" --" + this.comment);
            else return "";
        }

        public override string generateHeaderLine()
        {
            string headerline = this.name + " \t: " + this.dataType;
            if(this.value != "") headerline += " := " + this.value;
            return headerline;
        }

        public override string generateInstantiationLine()
        {
            string retString = this.name + " \t=> \t";
            if (!this.settings.emptyIOs) retString += this.name;
            return retString;
        }

        public override string generateWireDeclarationLine()
        {
            return "constant " + this.name + " : " + this.dataType + " := " + this.value + ";";
        }

        public override void initializeFromCodeLine(string codeLine)
        {
            if (!codeLine.Contains(":")) throw new FormatException("Invalid File Line: " + codeLine);//Sanity Check
            string[] type_name_sep = codeLine.Split(':');
            this.name = type_name_sep[0].Trim();
            type_name_sep[1] = type_name_sep.Length > 2 ? type_name_sep[1] + ':' + type_name_sep[2] : type_name_sep[1];
            if (type_name_sep[1].Contains(":="))
            {
                string[] value_type_sep = type_name_sep[1].Split(new string[] { ":=" }, StringSplitOptions.None);
                this.value = value_type_sep[1].Trim();
                this.dataType = value_type_sep[0].Trim();
            }
            else
            {
                this.value = "";
                this.dataType = type_name_sep[1].Trim();
            }
        }
    }

    public static class VHDLDataProcessing
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
            string[] lineSeparatedHDL = hdlCode.Split(new string[] { System.Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string lineSepar in lineSeparatedHDL)
            {
                string line = lineSepar.Trim();
                if(line != "")
                {
                    string[] nextElement = new string[2];
                    if (line.Contains("--"))
                    {
                        string[] data = line.Split(new string[] {"--"}, StringSplitOptions.None);
                        nextElement[0] = data[0].Replace(';', ' ').Trim();
                        nextElement[1] = "";
                        for(int i = 1; i < data.Length; i++)
                        {
                            nextElement[1] += data[i];
                            if (data.Length > 2 && i < (data.Length - 1)) nextElement[1] += "--"; //In case -- is used in the comment
                        }  
                    }
                    else
                    {
                        nextElement[0] = line.Replace(';', ' ').Trim();
                        nextElement[1] = "";
                    }
                    retList.Add(nextElement);
                }
            }
            return retList;
        }
    }
}
