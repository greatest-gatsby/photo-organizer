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
        public static Result<FileOperationCollection> BuildMoveCollection(DirectoryRecord source, DirectoryRecord target)
        {
            FileOperationCollection collection = new FileOperationCollection();

            collection.SourceImages = source.GetRecordsForContents();

            // treat empty sets as already complete, since they technically are
            if (collection.SourceImages.Length == 0)
            {
                return Result<FileOperationCollection>.Success(collection);
            }

            // get target images now that we have verified there are source images to work with
            ImageRecord[] targetImgs = target.GetRecordsForContents();

            // sort -- this will speed up lookups
            Array.Sort(collection.SourceImages, new Sorters.FileNameMatches());
            Array.Sort(targetImgs, new Sorters.FileNameMatches());

            // remove all images from the source list which are already in the target list
            for (int i = 0; i < collection.SourceImages.Length; i++)
            {
                // goal: find the source image in the targetImgs list
                // complication: how do we determine the images are the same without comparing byte for byte?
                // complication: how do we account for variant versions? such as '_2' or '(2)' or 'edited'

                var loc = Array.BinarySearch(targetImgs, collection.SourceImages[i], new Sorters.FileNameMatches());
                if (loc <= 0)
                {
                    // target doesn't have this image, so we need to copy it over
                }
                else
                {
                    // targetImgs has the img, so we skip ?
                    collection.SourceImages[loc] = null;
                }
            }

            // trim the nulls
            collection.SourceImages = collection.SourceImages.Where(i => i != null).ToArray();
            

            return Result<FileOperationCollection>.Success(collection);
        }

        /// <summary>
        /// Executes a move operation, returning a successful result if any individual move succeeds,
        /// or a failure result if the operation fails with the Data property populated with the error message.
        /// On <see cref="Result{FileOperationCollection}.Success(FileOperationCollection)"/>, any
        /// <see cref="ImageRecord"/> left in <see cref="FileOperationCollection.SourceImages"/> is a FAILED move.
        /// </summary>
        /// <param name="sourceId">Identifier of the source directory</param>
        /// <param name="targetId">Identifier of the target directory</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the move operation.</returns>
        public static Result<FileOperationCollection> TryMove(DirectoryRecord source, DirectoryRecord target)
        {
            // get FileOpCollection
            var res = BuildMoveCollection(source, target);
            if (!res.Successful)
            {
                return Result<FileOperationCollection>.Failure(res.Message);
            }

            // copy over all images from the sourceImgs array
            // respect the target directory scheme...perhaps retrieve tokens in order in a looped switch statement
            // and build the corresponding path string
            for (int i = 0; i < res.Data.SourceImages.Length; i++)
            {
                if (res.Data.SourceImages[i] == null)
                    continue;

                string destPath = target.GetNewLocation(res.Data.SourceImages[i]);

                try
                {
                    File.Move(res.Data.SourceImages[i].File.FullName, destPath);
                }
                catch (Exception ex)
                {
                    return Result<FileOperationCollection>.Failure(ex.ToString());
                }

            }

            return Result<FileOperationCollection>.Success(res.Data);
        }
    }
}
