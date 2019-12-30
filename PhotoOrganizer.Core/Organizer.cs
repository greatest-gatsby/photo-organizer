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
            // or
            // $source_identifier [$addl_source, ...] TO $target_identifier [$addl_target, ...]
            if (args.Length < 2)
            {
                return Result.Failure("Expected 2 or more arguments, got {0}", args.Length);
            }

            string[] sources = null;
            string[] targets = null;

            // Determine where sources end and targets begin
            int toLoc = Array.FindIndex<string>(args, r => r.ToLower() == "to");
            if (toLoc == -1)
            {
                sources = new string[args.Length - 1];
                Array.Copy(args, 0, sources, 0, sources.Length);
                targets = new string[1];
                Array.Copy(args, sources.Length, targets, 0, 1);
            }
            else
            {
                if (toLoc != Array.FindLastIndex<string>(args, r => r.ToLower() == "to"))
                {
                    return Result.Failure("Too many uses of reserved keyword 'to'");
                }
                else
                {
                    sources = new string[toLoc - 1];
                    Array.Copy(args, 0, sources, 0, toLoc - 1);
                    targets = new string[args.Length - toLoc];
                    Array.Copy(args, toLoc + 1, targets, 0, args.Length - toLoc);
                }
            }

            MoveCollection collection = null;
            if (!MoveCollection.TryBuildCollection(sources, targets, out collection))
            {
                return Result.Failure("Unable to build collection");
            }

            throw new NotImplementedException();
        }
    }
}
