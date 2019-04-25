using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class DirectoryRecord
    {
        public enum DirectoryMode
        {
            Auto,
            Manual
        }
        public string Path { get; set; }
        public string Alias { get; set; }
        public bool IsRecursive { get; set; }
        public DirectoryMode Mode { get; set; }

    }
}
