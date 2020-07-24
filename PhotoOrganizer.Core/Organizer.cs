using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Linq.Expressions;
using System.IO;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// The functions that help you interact with the library by consuming string arguments and performing work
    /// </summary>
    public class Organizer
    {
        /// <summary>
        /// Executes a move operation, returning a successful result if the operation succeeds,
        /// or a failure result if the operation fails with the Data property populated with the error message.
        /// </summary>
        /// <param name="sourceId">Identifier of the source directory</param>
        /// <param name="targetId">Identifier of the target directory</param>
        /// <returns>True if the operation succeeds and False if it fails.</returns>
        public static Result TryMove(string sourceId, string targetId)
        {
            // get directory records and validate
            var source = SaveData.ValidateDirectoryIdentifier(sourceId);
            var target = SaveData.ValidateDirectoryIdentifier(targetId);
            if (source == null)
            {
                return Result.Failure(String.Format("Unrecognized source {0}", sourceId));
            }
            if (target == null)
            {
                return Result.Failure(String.Format("Unrecognized target {0}", targetId));
            }

            // now collect all the images
            var sourceImgs = new DirectoryInfo(source.Path)
                .GetFiles()
                .Select<FileInfo, ImageRecord>(fi => new ImageRecord(fi))
                .ToArray();
            var targetImgs = new DirectoryInfo(target.Path)
                .GetFiles()
                .Select<FileInfo, ImageRecord>(fi => new ImageRecord(fi))
                .ToArray();

            // reject empty sets
            if (sourceImgs.Length == 0)
            {
                return Result.Success();
            }

            // sort -- this will speed up lookups
            // ImageRecord implements IComparable(object)
            Array.Sort(sourceImgs, new Sorters.FileNameMatches());
            Array.Sort(targetImgs, new Sorters.FileNameMatches());

            // remove all images from the source list which are already in the target list
            for (int i = 0; i < sourceImgs.Length; i++)
            {
                // goal: find the source image in the targetImgs list
                // complication: how do we determine the images are the same without reading byte for byte?
                // complication: how do we account for variant versions? such as '_2' or '(2)' or 'edited'

                var loc = Array.BinarySearch(targetImgs, sourceImgs[i], new Sorters.FileNameMatches());
                if (loc == -1)
                {
                    // target doesn't have this image, so we need to copy it over
                }
                else
                {
                    // targetImgs has the img, so we skip ?
                    sourceImgs[loc] = null;
                }
            }

            // copy over all images from the sourceImgs array
            // respect the target directory scheme...perhaps retrieve tokens in order in a looped switch statement
            // and build the corresponding path string
            for (int i = 0; i < sourceImgs.Length; i++)
            {
                if (sourceImgs[i] == null)
                    continue;

                string destPath = target.GetNewLocation(sourceImgs[i]);

                try
                {
                    File.Move(sourceImgs[i].File.FullName, destPath);
                }
                catch
                {
                    Console.WriteLine("Failed to copy image {0}", destPath);
                }

            }

            return Result.Success();
        }

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
