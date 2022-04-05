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
    /// Interaktionslogik für SettingsPanel.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        private Settings settings;
        public SettingsPanel()
        {
            InitializeComponent();
        }
        public void registerSettings(Settings settings)
        {
            this.settings = settings;
        }

        private void CbPrevComments_Checked(object sender, RoutedEventArgs e)
        {
            this.settings.includeInputComments = (bool)cbPrevComments.IsChecked;
        }

        private void CbDirectionComments_Checked(object sender, RoutedEventArgs e)
        {
            this.settings.addWireDirectionComment = (bool)cbDirectionComments.IsChecked;
        }

        private void CbEmptyIO_Checked(object sender, RoutedEventArgs e)
        {
            this.settings.emptyIOs = (bool) cbEmptyIO.IsChecked;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.settings is null) return;
            switch (cbbLanguage.SelectedIndex)
            {
                case 0:
                    this.settings.language = HDLLanguage.Verilog;
                    break;
                case 1:
                    this.settings.language = HDLLanguage.VHDL;
                    break;
            }
        }
    }
}
