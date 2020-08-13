using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Organizes the file moves/copies that must be made for a given execution
    /// </summary>
    public class FileOperationCollection
    {
        public ImageRecord[] SourceImages { get; set; }

        public DirectoryRecord SourceDirectory { get; set; }

        public DirectoryRecord TargetDirectory { get; set; }
    }
}
