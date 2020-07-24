using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    public class Sorters
    {
        /// <summary>
        /// Compares <see cref="ImageRecord"/>s by file name and extension, not including directory
        /// </summary>
        public class FileNameMatches : IComparer<ImageRecord>
        {
            int IComparer<ImageRecord>.Compare(ImageRecord x, ImageRecord y)
            {
                return String.Compare(x.File.Name, y.File.Name);
            }
        }
    }
}
