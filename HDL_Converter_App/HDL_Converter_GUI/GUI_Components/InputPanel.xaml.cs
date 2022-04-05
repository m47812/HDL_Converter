using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HDL_Converter_Classes.HDL_Structures;

namespace HDL_Converter_GUI.GUI_Components
{
    /// <summary>
    /// Interaktionslogik für InputPanel.xaml
    /// </summary>
    public partial class InputPanel : UserControl
    {
        public InputPanel()
        {
            InitializeComponent();
        }

        public void registerSettings(Settings settings)
        {
            this.settingsPanel.registerSettings(settings);
        }

        public string getHDLInput()
        {
            string hdlCode = tbHDLInput.Text;
            if (hdlCode != "" && hdlCode != null) return hdlCode;
            return null;
        }
    }
}
