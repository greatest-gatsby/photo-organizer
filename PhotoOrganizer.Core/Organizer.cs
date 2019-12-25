using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// The functions that help you interact with the library by consuming string arguments and performing work
    /// </summary>
    public class Organizer
    {
        /// <summary>
        /// Executes a move using the supplied arguments
        /// </summary>
        /// <param name="sources">Array of source directory paths or names</param>
        /// <param name="targets">Array of target directory paths or names</param>
        /// <param name="scheme">Directory scheme to use when transfering files to the targets</param>
        /// <returns></returns>
        public static Result Move(string[] sources, string[] targets, DirectoryScheme scheme = null)
        {
            // Build MoveCollection
            MoveCollection collection = new MoveCollection();
            var get = SaveData.GetDirectories();
            if (!get.Successful)
            {
                return get;
            }

            // Verify all input source directories are registered in the config file
            var validateSources = SaveData.ValidateDirectories<SourceDirectory>(sources);
            if (validateSources.Successful)
            {
                collection.Sources.AddRange((List<SourceDirectory>)validateSources.Data);
            }
            else
            {
                return Result.Failure((string)validateSources.Data);
            }

            // Verify all input target directories are registered in the config file
            var validateTargets = SaveData.ValidateDirectories<TargetDirectory>(targets);
            if (validateTargets.Successful)
            {
                collection.Targets.AddRange((List<TargetDirectory>)validateTargets.Data);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a move using the CLI args
        /// </summary>
        /// <param name="args">CLI args with the 'move' command REMOVED</param>
        /// <returns>A result indicating outcome of operation</returns>
        public static Result Move(string[] args)
        {
            // After command is consumed, expecting:
            // $source_identifier [$addl_source, ...] $target_identifier
            if (args.Length < 2)
            {
                return Result.Failure("Expected 2 or more arguments, got {0}", args.Length);
            }

            // Get directories
            var result = SaveData.GetDirectories();
            if (!result.Successful)
            {
                return result;
            }

            var recs = result.GetData<DirectoryRecord[]>();
            // Verify that the last arg is a target
            if (!recs.Any(r => (r.Type == DirectoryType.Target) && (r.Identifier == args[args.Length - 1])))
            {
                return Result.Failure("No saved target directory {0}", args[args.Length - 1]);
            }

            // Verify that the other args are all sources
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!recs.Any(r => (r.Type == DirectoryType.Source) && (r.Identifier == args[i])))
                {
                    return Result.Failure("No saved source directory {0}", args[i]);
                }
            }
            throw new NotImplementedException();
        }
    }
}
