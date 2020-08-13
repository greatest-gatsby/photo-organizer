using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Linq;

using PhotoOrganizer;
using System.Runtime.CompilerServices;

namespace PhotoOrganizer.Core
{
    public static partial class SaveData
    {
        /// <summary>
        /// Name of the directories file without the path or extension
        /// </summary>
        public static string DirectoriesFileName { get { return "directories"; } }

        /// <summary>
        /// Full path and filename to the directories file
        /// </summary>
        public static string DirectoriesFilePath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }

        /// <summary>
        /// All registered directories
        /// </summary>
        public static DirectoryRecord[] Directories { get; set; }

        /// <summary>
        /// Retrieves all Directories from the directories file on-disk. Calling this method manually
        /// is usually not needed, but doing so requires manually assigning the value to the Directories property.
        /// </summary>
        public static DirectoryRecord[] LoadDirectoriesFromDisk()
        {
            string content = File.ReadAllText(DirectoriesFilePath);
            if (String.IsNullOrEmpty(content))
            {
                return new DirectoryRecord[0];
            }
            else
            {
                return JsonSerializer.Deserialize<DirectoryRecord[]>(content);
            }
        }

        /// <summary>
        /// Writes all directories in the Directories property to disk.
        /// </summary>
        /// <returns></returns>
        public static Result WriteDirectoriesToDisk()
        {
            try
            {
                File.WriteAllText(DirectoriesFilePath, JsonSerializer.Serialize<DirectoryRecord[]>(Directories));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.ToString());
            }
            
        }

        /// <summary>
        /// Adds the given record to the Directories file
        /// </summary>
        /// <param name="record">DirectoryRecord to save to disk</param>
        public static Result AddDirectory(DirectoryRecord record)
        {
            if (GetDirectoryOrDefault(record.Identifier) == null)
            {
                Directories = Directories.Append(record).ToArray();
                var res = WriteDirectoriesToDisk();
                if (res.Successful)
                    return Result.Success();
                else
                    return Result.Failure(res.Message);
            }
            else
            {
                return Result.Failure("Error occurred while validating newly added directory");
            }
        }

        /// <summary>
        /// Removes a saved place by name or path
        /// </summary>
        /// <param name="identifier">CLI args containing the path or name</param>
        /// <returns></returns>
        public static Result RemoveDirectory(string identifier)
        {
            if (GetDirectoryOrDefault(identifier) == null)
            {
                return Result.Failure($"Error: Could not remove directory {identifier} because it was not found");
            }
            else
            {
                try
                {
                    Directories = Directories.Where(d => !d.IsIdentifiableBy(identifier)).ToArray();
                    File.WriteAllText(DirectoriesFilePath, JsonSerializer.Serialize<DirectoryRecord[]>(Directories));
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    return Result.Failure(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Verifies that all the DirectoryRecords in <paramref name="input"/> are registered directories.
        /// Stores a boolean indicating whether all input directories are registered: true if all are, else false.
        /// </summary>
        /// <param name="input">List of DirectoryRecords to verify</param>
        /// <returns>A Result object which reports whether the validation was successful.
        /// If the operation is succesful, the status of the validity is stored in the Data field.</returns>
        public static Result<bool> ValidateDirectories(List<DirectoryRecord> input)
        {
            if (input.All(i => Array.Find(Directories, d => d.Identifier == i.Identifier) != null))
                return Result<bool>.Success(true);
            else
                return Result<bool>.Success(false);
        }

        /// <summary>
        /// Verifies that all the DirectoryRecords in <paramref name="input"/> are registered directories.
        /// Stores a boolean indicating whether all input directories are registered: true if all are, else false.
        /// </summary>
        /// <param name="input">List of DirectoryRecords to verify</param>
        /// <returns>A Result object which reports whether the validation was successful.
        /// If the operation is succesful, the status of the validity is stored in the Data field.</returns>
        public static Result<bool> ValidateDirectories(DirectoryRecord[] input)
        {
            if (input.All(i => Array.Find(Directories, d => d.Identifier == i.Identifier) != null))
                return Result<bool>.Success(true);
            else
                return Result<bool>.Success(false);
        }

        /// <summary>
        /// Gets the DirectoryRecord that can be identified by the given alias or path,
        /// or null if no matching DirectoryRecord is found
        /// </summary>
        /// <param name="id">Identifier of the record to validate</param>
        /// <returns>The Directory Record if valid, else null</returns>
        public static DirectoryRecord GetDirectoryOrDefault(string id)
        {
            return Directories.FirstOrDefault(r => r.IsIdentifiableBy(id));
        }
    }
}
