using Microsoft.Win32;
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
using System.Windows.Shapes;

namespace PhotoOrganizer.Win
{
    /// <summary>
    /// Interaction logic for AddDirectory.xaml
    /// </summary>
    public partial class AddDirectory : Window
    {
        private string Path = String.Empty;
        string LastMode = "AUTO";

        public AddDirectory()
        {
            InitializeComponent();
            AutomaticModeRd.IsChecked = true;
            LastMode = "MANUAL";
            LoadOptions();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            string dialogmsg = "Select Source Directory";
            OpenFileDialog dialog = new OpenFileDialog()
            {
                ValidateNames = false,
                CheckPathExists = true,
                CheckFileExists = false,
                FileName = dialogmsg
            };
            dialog.ShowDialog();
            Path = dialog.FileName.Remove(dialog.FileName.Length - dialogmsg.Length);
            PathLbl.Content = Path;
            Alias.Text = Path.Split('\\')[Path.Split('\\').Length - 2];
        }

        /// <summary>
        /// Loads Options panel for each mode
        /// </summary>
        private void LoadOptions()
        {
            // Only load manual options if we're already on auto
            if (ManualModeRd.IsChecked.Value && LastMode == "AUTO")
            {
                LastMode = "MANUAL";
                OptionsGroup.Header = "Manual Options";
                Options.Children.RemoveRange(0, Options.Children.Count); // clear items bc we are doing different ones

            }
            else if (AutomaticModeRd.IsChecked.Value && LastMode == "MANUAL")
            {
                LastMode = "AUTO";
                OptionsGroup.Header = "Automatic Options";
                Options.Children.RemoveRange(0, Options.Children.Count); // clear items to replace them

                // Add browse controls
                WrapPanel browse = new WrapPanel();
                Label browselbl = new Label()
                {
                    Content = "Select directory: "
                };
                Button browsebtn = new Button()
                {
                    Content = "Browse",
                };
                browsebtn.Click += Browse_Click;
                browse.Children.Add(browselbl);
                browse.Children.Add(browsebtn);
                Options.Children.Add(browse);

                // Include subdirectories controls
                WrapPanel subdir = new WrapPanel();
                Label subdirlbl = new Label
                {
                    Content = "Include all subdirectories"
                };
                CheckBox subdirchk = new CheckBox()
                {
                    IsChecked = true
                };
                subdir.Children.Add(subdirchk);
                subdir.Children.Add(subdirlbl);
                Options.Children.Add(subdir);
            }
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            LoadOptions();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Alias.Text))
                return;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
