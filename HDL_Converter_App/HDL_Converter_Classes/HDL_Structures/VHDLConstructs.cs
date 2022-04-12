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
            throw new NotImplementedException();
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
