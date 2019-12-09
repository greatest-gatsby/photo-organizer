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
using PhotoOrganizer.Core;

namespace PhotoOrganizer.Win
{
    /// <summary>
    /// Interaction logic for DirectoryPage.xaml
    /// </summary>
    public partial class DirectoryPage : Page
    {
        DirectoryRecord dirRecord { get; set; }

        public DirectoryPage()
        {
            InitializeComponent();
        }

        public DirectoryPage(DirectoryRecord rec) : this()
        {
            dirRecord = rec;
            DrawFromRecord();
            PopulateImageList();
            
        }

        /// <summary>
        /// Fill image list with images according to data from DirectoryRecord object
        /// </summary>
        private void PopulateImageList()
        {
            DirectoryInfo info = new DirectoryInfo(dirRecord.Path);
            SearchOption opt;
            if (dirRecord.IsRecursive)
                opt = SearchOption.AllDirectories;
            else
                opt = SearchOption.TopDirectoryOnly;
            foreach (FileInfo file in info.GetFiles("*", opt))
            {
                ImageRecord photo = new ImageRecord(file);
                ListBoxItem item = new ListBoxItem()
                {
                    DataContext = photo,
                    Content = photo.Name
                };
                ImageList.Items.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawFromRecord()
        {
            RecordName.Content = dirRecord.Alias;
            RecordPath.Content = dirRecord.Path;
            RecordRecursive.Content = dirRecord.IsRecursive.ToString();
        }
    }
}
