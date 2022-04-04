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

namespace HDL_Converter_GUI.GUI_Components
{
    /// <summary>
    /// Interaktionslogik für OutputRequestPanel.xaml
    /// </summary>
    public partial class OutputRequestPanel : UserControl
    {
        private GUIController controller;

        public OutputRequestPanel()
        {
            InitializeComponent();
        }

        public void registerGUIController(GUIController controller)
        {
            this.controller = controller;
        }

        private void btGenerateInstantiation_Click(object sender, RoutedEventArgs e)
        {
            controller.generateModuleInstantiationRequest();
        }

        private void btGenerateWireDeclaration_Click(object sender, RoutedEventArgs e)
        {
            controller.generateWireDeclarationRequest();
        }
    }
}
