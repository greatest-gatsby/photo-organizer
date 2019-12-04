using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class ImageRecord
    {
        public String FileName { get { return Name + Extension; } }
        public String Extension { get; set; }
        public String Name { get; set; }

        public ImageRecord(FileInfo info)
        {
            Name = info.Name;
            Extension = info.Extension;
        }
    }
}
