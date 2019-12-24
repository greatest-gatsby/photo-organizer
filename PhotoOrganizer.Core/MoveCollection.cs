using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Represents a batch of directories involved in a particular move operation
    /// </summary>
    public class MoveCollection
    {
        public List<DirectoryRecord> Sources { get; set; }

        public List<DirectoryRecord> Targets { get; set; }

    }
}
