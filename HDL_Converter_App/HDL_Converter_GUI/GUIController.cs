using System;
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
                string inputHDL = inputPanel.getHDLInput();
                if (inputHDL != null)
                {
                    converter.hdlInput = inputHDL;
                    string declaration = converter.generateWireDeclaration();
                    System.Windows.Clipboard.SetText(declaration);
                    informGUI(new GUIEvent("Wire declaration copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                }
                else
                {
                    informGUI(new GUIEvent("No HDL Code provided! Please Insert HDL Code to convert into Textbox",
                        GUIEvent.Severity.Warning), new EventArgs());
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
                string inputHDL = inputPanel.getHDLInput();
                if (inputHDL != null)
                {
                    converter.hdlInput = inputHDL;
                    string instantiation = converter.generateModuleInstantiation();
                    System.Windows.Clipboard.SetText(instantiation);
                    informGUI(new GUIEvent("Module instantiation copied to clipboard", GUIEvent.Severity.Succsess), new EventArgs());
                }
                else
                {
                    informGUI(new GUIEvent("No HDL Code provided! Please Insert HDL Code to convert into Textbox",
                        GUIEvent.Severity.Warning), new EventArgs());
                }
            }
            catch (NotImplementedException)
            {
                informGUI(new GUIEvent("The configuration you selected is currently not suported",
                        GUIEvent.Severity.Error), new EventArgs());
            }
            catch(FormatException e)
            {
                informGUI(new GUIEvent(e.Message, GUIEvent.Severity.Error), new EventArgs()); 
            }
        }

    }
}
