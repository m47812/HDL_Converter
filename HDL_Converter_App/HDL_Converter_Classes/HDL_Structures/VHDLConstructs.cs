using System;
using System.Collections.Generic;
using System.Text;

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
    class VHDLModule : HDLModule
    {
        public override string generateModuleInstantiation()
        {
            throw new NotImplementedException();
        }

        public override string generateWireDeclaration()
        {
            throw new NotImplementedException();
        }

        protected override void initializeFormHDLCode(string hdlCode)
        {
            //Find entity boundries
            string entity_L = hdlCode.ToLower();
            int startIndex = entity_L.IndexOf("entity") + 6;
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
                this.initializeParameters(entity.Substring(genericBoundries.Item1 + 1, genericBoundries.Item1 - genericBoundries.Item2));
            }
            //Extract Port signals
            startIndex = entity_L.IndexOf("port");
            if(startIndex != -1)
            {
                Tuple<int, int> portBoundries = this.getTopParenteses('(', ')', entity_L.Substring(startIndex));
                this.initializeWires(entity.Substring(portBoundries.Item1 + 1, portBoundries.Item1 - portBoundries.Item2));
            }           
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
    /// A Wire in the VHDL (signal in VHDL) domain serving as an IO to a module
    /// </summary>
    public class VHDLWire : Wire
    {
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

        public override string generateInstantiationLine()
        {
            throw new NotImplementedException();
        }

        public override string generateWireDeclarationLine()
        {
            throw new NotImplementedException();
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
        public override string buildComment()
        {
            if (settings.includeInputComments && this.comment != "") return (" --" + this.comment);
            else return "";
        }

        public override string generateInstantiationLine()
        {
            throw new NotImplementedException();
        }

        public override string generateWireDeclarationLine()
        {
            throw new NotImplementedException();
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
            string[] lineSeparatedHDL = hdlCode.Split(new string[] { System.Environment.NewLine}, StringSplitOptions.None);
            foreach (string lineSepar in lineSeparatedHDL)
            {
                string line = lineSepar;
                if (line[line.Length - 1] == ';') line = line.Remove(line.Length - 1);
                if(line != "")
                {
                    string[] nextElement = new string[2];
                    if (line.Contains("--"))
                    {
                        string[] data = line.Split(new string[] {"--"}, StringSplitOptions.None);
                        nextElement[0] = data[0].Trim();
                        nextElement[1] = "";
                        for(int i = 1; i < data.Length; i++)
                        {
                            nextElement[1] += data[i];
                            if (data.Length > 2 && i < (data.Length - 1)) nextElement[1] += "--"; //In case -- is used in the comment
                        }  
                    }
                    else
                    {
                        nextElement[0] = line.Trim();
                        nextElement[1] = "";
                    }
                    retList.Add(nextElement);
                }
            }
            return retList;
        }
    }
}
