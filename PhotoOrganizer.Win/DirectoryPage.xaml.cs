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
using PhotoOrganizer.Core;

namespace PhotoOrganizer.Win
{
    /// <summary>
    /// Interaction logic for DirectoryPage.xaml
    /// </summary>
    public partial class DirectoryPage : Page
    {
        public DirectoryPage()
        {
            InitializeComponent();
        }

        public DirectoryPage(DirectoryRecord rec) : this()
        {
            RecordName.Content = rec.Alias;
            RecordPath.Content = rec.Path;
            RecordRecursive.Content = rec.IsRecursive.ToString();
        }
    }
}
