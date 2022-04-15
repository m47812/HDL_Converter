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
        private enum TestbenchRequestState
        {
            TOFILE = 0,
            ClIP_VERIFY = 1,
            CLIP_TOP = 2
        }
        private TestbenchRequestState state = TestbenchRequestState.TOFILE;
        private GUIController controller;
        private string target_path = "";
        public TestbenchControl()
        {
            InitializeComponent();
        }

        public void registerGUIController(GUIController controller)
        {
            this.controller = controller;
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
            controller.generateTestbenchRequest((int)this.state, this.target_path);
        }


        private void rb_check_change(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            switch (button.Name)
            {
                case "rbFile":
                    buttonUpdate(true);
                    this.state = TestbenchRequestState.TOFILE;
                    break;
                case "rbTopLevel":
                    buttonUpdate(false);
                    this.state = TestbenchRequestState.CLIP_TOP;
                    break;
                case "rbVerify":
                    buttonUpdate(false);
                    this.state = TestbenchRequestState.ClIP_VERIFY;
                    break;
                default:
                    throw new InvalidOperationException("Check of unknown raio button occured");
            }
        }

        private void buttonUpdate(bool switechedToFileState)
        {
            if (switechedToFileState)
            {
                btFolderDialog.IsEnabled = true;
                if (target_path != "") btGenerateTestbench.IsEnabled = true;
                else btGenerateTestbench.IsEnabled = false;
            }
            else
            {
                btFolderDialog.IsEnabled = false;
                btGenerateTestbench.IsEnabled = true;
            }
        }
    }
}
