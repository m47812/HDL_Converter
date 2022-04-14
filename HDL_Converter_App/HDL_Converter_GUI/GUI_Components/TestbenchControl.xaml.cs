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


namespace HDL_Converter_GUI.GUI_Components
{
    /// <summary>
    /// Interaktionslogik für TestbenchControl.xaml
    /// </summary>
    public partial class TestbenchControl : UserControl
    {
        private string target_path;
        public TestbenchControl()
        {
            InitializeComponent();
        }

        private void click_folder_location(object sender, EventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select folder to store testbench in";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    this.target_path = dialog.SelectedPath;
                    this.laFolderLocation.Content = "Path: " + this.target_path;
                    btGenerateTestbench.IsEnabled = true;
                }
            }
        }

        private void click_generate_testbench(object sender, EventArgs e)
        {

        }
    }
}
