using System;
using System.Collections.Generic;
using HDL_Converter_Classes.HDL_Structures;

namespace HDL_Converter_Classes
{
    /// <summary>
    /// The Main Model Class that is used to perform Conversions it
    /// is created by the GUI or Core app and will be used for the data
    /// processing.
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// Settings to configure all Conversion options
        /// </summary>
        public Settings settings;
        /// <summary>
        /// The HDL Code given by the User that is used for Conversion
        /// to the other Formats.
        /// </summary>
        private string hdlInput_data;
        /// <summary>
        /// Public variable to set the Input date that invalidates that processing
        /// has been done on the data.
        /// </summary>
        public string hdlInput
        {
            get
            {
                return hdlInput_data;
            }
            set
            {
                hdlInput_data = value;
                inputProcessed = false;
            }
        }
        /// <summary>
        /// Flag that is false until the input data (HDL Code) is processed.
        /// Is used so that data processing is only computed once.
        /// </summary>
        private bool inputProcessed = false;

        /// <summary>
        /// Containes the processed HDL Input Code
        /// </summary>
        private HDLModule module;

        public Converter(Settings settings)
        {
            this.settings = settings;
        }

        

        public string generateWireDeclaration()
        {
            if (!inputProcessed) this.processInputHDL();
            return module.generateWireDeclaration();
        }

        public string generateModuleInstantiation()
        {
            if (!inputProcessed) this.processInputHDL();
            return null; //Make Compiler Happy (Placeholder)
        }

        private void processInputHDL()
        {
            switch (settings.language)
            {
                case HDLLanguage.Verilog:
                    this.module = new VeriModule(this.hdlInput, this.settings);
                    break;
                default:
                    throw new NotImplementedException("HDL Language not supported");
            }
        }
    }
}
