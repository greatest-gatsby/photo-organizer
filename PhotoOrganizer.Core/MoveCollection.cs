using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Represents a batch of directories involved in a particular move operation.
    /// Contains the core logic of the move operation.
    /// </summary>
    public class MoveCollection
    {
        public List<SourceDirectory> Sources { get; set; }

        public List<TargetDirectory> Targets { get; set; }

        public Result Move()
        {
            return Result.Success();
        }

    }
}
