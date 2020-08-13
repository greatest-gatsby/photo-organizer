using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class ImageRecord
    {
        public FileInfo File { get; set; }

        public string NewPath { get; }

        public ImageRecord(FileInfo info, DirectoryRecord directory)
        {
            File = info;
        }

        // TODO - Add method to determine if '_MG_4567' is the same as 'IMG_4567'
        // TODO - Recognize versioning inside the name
    }
}
