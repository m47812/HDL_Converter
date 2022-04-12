using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Classes.HDL_Structures
{
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
    class VHDLWire : Wire
    {
        public override string buildComment()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A Parameter in the VHDL domain (generic in VHDL) serving as a configuration parameter for the module
    /// </summary>
    class VHDLParameter : Parameter
    {
        public override string buildComment()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
