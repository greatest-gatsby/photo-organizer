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

        /// <summary>
        /// Builds a MoveCollection from the given sources and targets, veryfing they are saved in the directories file.
        /// </summary>
        /// <param name="sources">Array of identifiers of source directories</param>
        /// <param name="targets">Array of identifiers of target directories</param>
        /// <returns>A MoveCollection for the passed arguments</returns>
        /// <exception cref="ArgumentException">One or more identifiers is not found in the directories file</exception>
        public static MoveCollection BuildCollection(string[] sources, string[] targets)
        {
            if (sources.Length < 1)
            {
                throw new ArgumentException("One or more sources required for collection");
            }
            if (targets.Length < 1)
            {
                throw new ArgumentException("One or more sources required for collection");
            }

            // Get directories
            var result = SaveData.GetDirectories();
            if (!result.Successful)
            {
                throw new ArgumentException(result.Message);
            }

            var recs = result.GetData<DirectoryRecord[]>();
            MoveCollection collection = new MoveCollection();

            // Build directory record for each source
            for (int i = 0; i < sources.Length; i++)
            {
                collection.Sources.Add((SourceDirectory)DirectoryRecord.Parse(sources[i]));
            }
            // Verify that all targets are registered
            for (int i = 0; i < targets.Length; i++)
            {
                collection.Targets.Add((TargetDirectory)DirectoryRecord.Parse(sources[i]));
            }

            //var validate = SaveData.ValidateDirectories(collection.Sources);
            throw new NotImplementedException();
            return collection;
        }

        public static bool TryBuildCollection(string[] sources, string[] targets, out MoveCollection collection)
        {
            try
            {
                collection =  BuildCollection(sources, targets);
                return true;
            }
            catch
            {
                collection = null;
                return false;
            }
        }
    }
}
