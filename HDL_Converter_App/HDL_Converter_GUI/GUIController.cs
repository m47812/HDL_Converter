using System;
using System.Collections.Generic;
using System.IO;
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
    /// A Class used to transfer information for inform GUI events.
    /// </summary>
    public class GUIEvent
    {
        public string text;
        public Severity severity;
        public enum Severity
        {
            Succsess = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        public GUIEvent(string text, Severity severity)
        {
            this.text = text;
            this.severity = severity;
        }
    }
    /// <summary>
    /// The GUI Controller Serves as a connection between the WPF based GUI and
    /// the backend in the HDL Converter classes.
    /// </summary>
    public class GUIController
    {
        /// <summary>
        /// Gets raised by GUI Controller to notify GUI components of events happend in Backend.
        /// </summary>
        public event EventHandler informGUI;

        private Converter converter;
        private Settings settings = new Settings();
        private InputPanel inputPanel;

        public GUIController(InputPanel inputPanel)
        {
            converter = new Converter(this.settings);
            this.inputPanel = inputPanel;
            converter.settings = this.settings;
            inputPanel.registerSettings(settings);
        }

        /// <summary>
        /// This is called by GUI components if a wire declaration shall be generated.
        /// </summary>
        public void generateWireDeclarationRequest()
        {
            try
            {
                if (setupHDLInput())
                {
                    string declaration = converter.generateWireDeclaration();
                    System.Windows.Clipboard.SetText(declaration);
                    informGUI(new GUIEvent("Wire declaration copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                }           
            }
            catch (NotImplementedException)
            {
                informGUI(new GUIEvent("The configuration you selected is currently not suported",
                        GUIEvent.Severity.Error), new EventArgs());
            }
            catch (FormatException e)
            {
                informGUI(new GUIEvent(e.Message, GUIEvent.Severity.Error), new EventArgs());
            }
        }

        /// <summary>
        /// This is called by GUI components if a module instantiation shall be generated.
        /// </summary>
        public void generateModuleInstantiationRequest()
        {
            try
            {
                if (setupHDLInput())
                {
                    string instantiation = converter.generateModuleInstantiation();
                    System.Windows.Clipboard.SetText(instantiation);
                    informGUI(new GUIEvent("Module instantiation copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                } 
            }
            catch (NotImplementedException)
            {
                informGUI(new GUIEvent("The configuration you selected is currently not suported",
                        GUIEvent.Severity.Error), new EventArgs());
            }
            catch (FormatException e)
            {
                informGUI(new GUIEvent(e.Message, GUIEvent.Severity.Error), new EventArgs());
            }
        }

        public void generateTestbenchRequest(int state, string path)
        {
            if (setupHDLInput())
            {               
                switch (state)
                {
                    case 0://FILE
                        try
                        {
                            converter.storeTestbenchToFile(path);
                            informGUI(new GUIEvent("Testbench was stored to file", GUIEvent.Severity.Succsess), new EventArgs());
                        }catch(FileLoadException e)
                        {
                            informGUI(new GUIEvent(e.Message, GUIEvent.Severity.Error), new EventArgs());
                        }
                        break;
                    case 1://Verify
                        string testbenchVerify = converter.generateTestbenchVerify();
                        System.Windows.Clipboard.SetText(testbenchVerify);
                        informGUI(new GUIEvent("Testbench verify was copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                        break;
                    case 2://TOP
                        string testbenchTop = converter.generateTestbenchTopLevel();
                        System.Windows.Clipboard.SetText(testbenchTop);
                        informGUI(new GUIEvent("Testbench top level was copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                        break;
                }  
            }
        }


        private bool setupHDLInput()
        {
            string inputHDL = inputPanel.getHDLInput();
            if (inputHDL != null)
            {
                if (checkCorrectHDLLanguage(inputHDL))
                {
                    converter.hdlInput = inputHDL;
                    return true;
                }               
            }
            else
            {
                informGUI(new GUIEvent("No HDL Code provided! Please Insert HDL Code to convert into Textbox",
                    GUIEvent.Severity.Warning), new EventArgs());
            }
            return false;
        }
        private bool checkCorrectHDLLanguage(string inputHDL)
        {
            MessageBoxResult result;
            if (settings.language == HDLLanguage.Verilog && inputHDL.ToLower().Contains("entity")){
                result = MessageBox.Show("Verilog was selected as language but the inputed code contains VHDL keywords." +
                    " Do you want to continue?", "Warning selected HDL", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else if(settings.language == HDLLanguage.VHDL && inputHDL.ToLower().Contains("module"))
            {
                result = MessageBox.Show("VHDL was selected as language but the inputed code contains Verilog keywords." +
                    " Do you want to continue?", "Warning selected HDL", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else
            {
                return true;
            }
            if (result == MessageBoxResult.Yes)
                return true;
            return false;
        }
    }
}
