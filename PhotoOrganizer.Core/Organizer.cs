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
            ImageRecord[] sourceImgs = source.GetRecordsForContents();
            ImageRecord[] targetImgs = target.GetRecordsForContents();

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
    }
}
