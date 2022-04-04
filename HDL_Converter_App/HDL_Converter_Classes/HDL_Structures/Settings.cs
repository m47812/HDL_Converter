using System;
using System.Collections.Generic;
using System.Text;

namespace HDL_Converter_Classes.HDL_Structures
{
    public enum HDLLanguage
    {
        Verilog = 0,
        VHDL = 1
    }

    /// <summary>
    /// A class containing all settings for the conversion output
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Defines the HDL Language that is used (given by the Input HDL)
        /// </summary>
        public HDLLanguage language = HDLLanguage.Verilog;
        /// <summary>
        /// Defines if module instantiation will be empty or containing a wire of the same name.
        /// </summary>
        /// <example>
        /// if true:
        ///     .myWire()
        /// if false:
        ///     .myWire(myWire)
        /// </example>
        public bool emptyIOs = false;

        /// <summary>
        /// Defines if comments schall be added in the instantiation
        /// </summary>
        public bool includeInputComments = true;
        public bool addWireDirectionComment = true;
    }
}
