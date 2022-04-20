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
            this.ucTestbenchPanel.registerGUIController(controller);
            controller.informGUI += update_console;
        }

        private void btGenerateInstantiation_Click(object sender, RoutedEventArgs e)
        {
            controller.generateModuleInstantiationRequest();
        }

        private void btGenerateWireDeclaration_Click(object sender, RoutedEventArgs e)
        {
            controller.generateWireDeclarationRequest();
        }

        private void update_console(object sender, EventArgs e)
        {
            GUIEvent eventData = sender as GUIEvent;
            laOutputText.Content = eventData.text;
            switch (eventData.severity)
            {
                case GUIEvent.Severity.Succsess:
                    laOutputText.Foreground = Brushes.Green;
                    break;
                case GUIEvent.Severity.Error:
                    laOutputText.Foreground = Brushes.Red;
                    break;
                case GUIEvent.Severity.Warning:
                    laOutputText.Foreground = Brushes.OrangeRed;
                    break;
                default:
                    laOutputText.Foreground = Brushes.Black;
                    break;
            }
        }
    }
}
