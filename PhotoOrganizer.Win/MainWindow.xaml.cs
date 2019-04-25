using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using System.Xml.Serialization;
using PhotoOrganizer.Core;

namespace PhotoOrganizer.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string StatePath { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            if (LoadState())    // Load state. If successful, don't draw the regular startup objects (b/c the state has already been loaded)
                return;
            StackPanel stack = new StackPanel();
            WrapPanel wrap = new WrapPanel();
            Label welcome = new Label();
            welcome.Content = "Welcome to photo sorter\nUse auto directory below";
            wrap.Children.Add(welcome);
            stack.Children.Add(wrap);
            ItemView.Children.Add(stack);
        }
        #region watch_directories
        /// <summary>
        /// Event handler for when the "Add new directories" button is clicked. Opens the AddDirectory dialog.
        /// </summary>
        private void AddNewDirs_Selected(object sender, RoutedEventArgs e)
        {
            AddDirectory ad = new AddDirectory();
            ad.ShowInTaskbar = false;
            switch (ad.ShowDialog())
            {
                case true:
                    DirectoryRecord rec = new DirectoryRecord()
                    {
                        Alias = ad.Alias.Text,
                        Path = (string)ad.PathLbl.Content
                    };
                    TreeViewItem dir = new TreeViewItem();
                    dir.Selected += WatchDirectory_Click;
                    dir.Header = rec.Alias;
                    dir.DataContext = rec;
                    Directories.Items.Add(dir);
                    break;
                case false:
                    break;
                default:
                    break;
            }
            //TreeViewItem dir_item = 
        }

        private void WatchDirectory_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem itm = (TreeViewItem)sender;
            DirectoryRecord rec = (DirectoryRecord)(itm.DataContext);
            Frame frame = new Frame();
            frame.Content = new DirectoryPage(rec);
            ItemView.Children.Clear();
            ItemView.Children.Add(frame);
        }
        #endregion

        /// <summary>
        /// Loads state from disk or memory and returns true on success
        /// </summary>
        /// <returns>True if state was loaded, else False</returns>
        private bool LoadState()
        {
            return true;
        }

        private bool SaveState()
        {
            XmlSerializer writer = new XmlSerializer(typeof(DirectoryRecord));
            FileStream file = new FileStream(StatePath, FileMode.OpenOrCreate);
            foreach (TreeViewItem itm in Directories.Items)
            {
                DirectoryRecord rec = (DirectoryRecord)(itm.DataContext);
                writer.Serialize(file, rec);
            }
            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(StatePath))
            {
                if (!SetStatePath())
                    e.Cancel = true;
            }
            else
                SaveState();
        }

        private bool SetStatePath()
        {
            Microsoft.Win32.SaveFileDialog diag = new Microsoft.Win32.SaveFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                OverwritePrompt = true,
                Title = "Select save file...",
                CreatePrompt = true,
                DefaultExt = ".xml",
                FileName = "Catalog"
            };
            switch(diag.ShowDialog())
            {
                case true:
                    StatePath = diag.FileName;
                    return true;
                default:
                    return false;
            }

        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(StatePath))
                SetStatePath();
            SaveState();
        }
    }
}
