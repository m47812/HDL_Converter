using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Core.HDL_Structures
{

    /// <summary>
    /// A Wire in the Verilog domain serving as an IO to a module
    /// </summary>
    class VeriWire : Wire
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
    }

    /// <summary>
    /// A Parameter in the Verilog domain serving as a configuration parameter for the module
    /// </summary>
    class VeriParameter : Parameter
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
            return "localparam  " + this.name +" = "+this.value+ ";";
        }
    }
}
