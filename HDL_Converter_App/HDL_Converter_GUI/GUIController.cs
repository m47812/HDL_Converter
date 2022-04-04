﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HDL_Converter_Classes;
using HDL_Converter_Classes.HDL_Structures;
using HDL_Converter_GUI.GUI_Components;

namespace HDL_Converter_GUI
{
    /// <summary>
    /// The GUI Controller Serves as a connection between the WPF based GUI and
    /// the backend in the HDL Converter classes.
    /// </summary>
    public class GUIController
    {
        private Converter converter = new Converter();
        private Settings settings = new Settings();
        private InputPanel inputPanel;

        public GUIController(InputPanel inputPanel) 
        {
            this.inputPanel = inputPanel;
            converter.settings = this.settings;
        }

        /// <summary>
        /// This is called by GUI components if a wire declaration shall be generated.
        /// </summary>
        public void generateWireDeclarationRequest()
        {
            string inputHDL = inputPanel.getHDLInput();
            if (inputHDL != null)
            {
                converter.hdlInput = inputHDL;
                converter.generateWireDeclaration();
                //TODO Return Result
            }
            else
            {
                MessageBox.Show("No HDL Code provided! Please Insert HDL Code to convert into Textbox",
                    "No HDL Input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// This is called by GUI components if a module instantiation shall be generated.
        /// </summary>
        public void generateModuleInstantiationRequest()
        {
            string inputHDL = inputPanel.getHDLInput();
            if(inputHDL != null)
            {
                converter.hdlInput = inputHDL;
                converter.generateModuleInstantiation();
                //TODO Return Result
            }
            else
            {
                MessageBox.Show("No HDL Code provided! Please Insert HDL Code to convert into Textbox",
                     "No HDL Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}