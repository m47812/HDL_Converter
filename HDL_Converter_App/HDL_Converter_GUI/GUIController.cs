using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public GUIController(InputPanel inputPanel, OutputRequestPanel outReqPanel)
        {

        }

        /// <summary>
        /// This event is raised by GUI components if a wire declaration shall be generated.
        /// </summary>
        private void generateWireDeclarationRequest(object sender, EventArgs e)
        {

        }
    }
}
