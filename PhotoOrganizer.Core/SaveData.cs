﻿using System;
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
    /// <summary>
    /// Utility class useful for giving descriptive, non-throwing reports on operation status.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// The message explaing the error. If the operation succeeded, then this returns String.Empty()
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Returns true if the operation succeeded, else false.
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// Contains data associated with the operation, if any
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// Creates a FAILING result with the given message
        /// </summary>
        /// <param name="message">Message to explain failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        private Result(string message, params object[] args)
        {
            Message = String.Format(message, args);
            Successful = false;
        }

        /// <summary>
        /// Creates a PASSING result 
        /// </summary>
        private Result()
        {
            Message = String.Empty;
            Successful = true;
        }

        /// <summary>
        /// Gets the data associated with this operation, if any
        /// </summary>
        /// <typeparam name="T">Type of the data which will be returned</typeparam>
        /// <returns>The data associated with this operation in the requested object type</returns>
        public T GetData<T>()
        {
            return (T)Data;
        }

        /// <summary>
        /// Returns a Result with the given data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result Success(object data = null)
        {
            return new Result()
            {
                Data = data
            };
        }

        /// <summary>
        /// Returns a Result indicating FAILURE with the given message
        /// </summary>
        /// <param name="message">The message to explain the failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        /// <returns>A result indicating FAILURE with the given message</returns>
        public static Result Failure(string message, params object[] args) {
            {
                return new Result(message, args);
            }
        }

        /// <summary>
        /// Returns a Result indicating FAILURE with the given message
        /// </summary>
        /// <param name="message">The message to explain the failure</param>
        /// <param name="args">Optional array of objects to use when formatting the string.</param>
        /// <returns>A result indicating FAILURE with the given message</returns>
        public static Result Failure(string message, object data, params object[] args)
        {
            {
                if (args == null || args.Length == 0)
                    return new Result(message, data) { Data = data };
                else
                    return new Result(message, args) { Data = data };
            }
        }
    }

    public static class SaveData
    {
        /// <summary>
        /// Location of data directory which will be checked if the current directory doesn't have organizer_data.xml. Default is in the following special directory.
        /// </summary>
        public static string DataDirectory { get; set; }

        /// <summary>
        /// Name of the directories file without the path or extension
        /// </summary>
        public static string DirectoriesFileName { get { return "directories"; } }

        /// <summary>
        /// Full path and filename to the directories file
        /// </summary>
        public static string DirectoriesFilePath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }

        /// <summary>
        /// Name of the schemes file without the path or extension
        /// </summary>
        public static string SchemesFileName { get { return "schemes"; } }

        /// <summary>
        /// Full path and filename to the schemes file
        /// </summary>
        public static string SchemesFilePath { get { return Path.Combine(DataDirectory, SchemesFileName); } }

        /// <summary>
        /// Returns the path to the in-use config file
        /// </summary>
        public static string ConfigPath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }

        /// <summary>
        /// All registered directories
        /// </summary>
        public static DirectoryRecord[] Directories { get; set; }

        /// <summary>
        /// Initializes access to save data files
        /// </summary>
        static SaveData()
        {
            string appResourcePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "photo organizer" + Path.DirectorySeparatorChar);

            // Determine where the save data is -- either application data or current directory
            if (File.Exists(Path.Combine(appResourcePath, DirectoriesFileName)))
            {
                DataDirectory = appResourcePath;
            }
            else
            {
                // Try to create data directory in appdata folder
                try
                {
                    if (!Directory.Exists(appResourcePath))
                        Directory.CreateDirectory(appResourcePath);
                    // Now try to write the directories file
                    File.WriteAllText(Path.Combine(appResourcePath, DirectoriesFileName), "");
                    DataDirectory = appResourcePath;
                    Console.WriteLine("Created config at " + ConfigPath);
                }
                catch (Exception)
                {
                    // Use current directory instead
                    try
                    {
                        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, DirectoriesFileName), "");
                        DataDirectory = Environment.CurrentDirectory;
                        Console.WriteLine("Created config at " + ConfigPath);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Could not determine a writable path for config files", ex);
                    }
                }
            }

            ReadDirectoriesFile();
        }

        /// <summary>
        /// Reloads all in-memory directories from the directories file. Calling this method manually
        /// is usually not needed.
        /// </summary>
        public static void ReadDirectoriesFile()
        {
            string content = File.ReadAllText(DirectoriesFilePath);
            if (String.IsNullOrEmpty(content))
            {
                Directories = new DirectoryRecord[0];
            }
            else
            {
                Directories = JsonSerializer.Deserialize<DirectoryRecord[]>(content);
            }
        }

        /// <summary>
        /// Adds the given record to the Directories file
        /// </summary>
        /// <param name="record">DirectoryRecord to save to disk</param>
        public static Result AddDirectory(DirectoryRecord record)
        {
            var list = new List<DirectoryRecord>();
            if (File.Exists(DirectoriesFilePath))
            {
                string content = File.ReadAllText(DirectoriesFilePath);
                if (!String.IsNullOrEmpty(content))
                {
                    list = JsonSerializer.Deserialize<List<DirectoryRecord>>(content);
                }
            }
            else
            {
                list = new List<DirectoryRecord>();
            }
            
            list.Add(record);
            foreach (var d in list)
            {
                Directories.Append(d);
            }
            File.WriteAllText(DirectoriesFilePath, JsonSerializer.Serialize<DirectoryRecord[]>(list.ToArray()));

            return Result.Success();
        }

        /// <summary>
        /// Removes a saved place by name or path
        /// </summary>
        /// <param name="identifier">CLI args containing the path or name</param>
        /// <returns></returns>
        public static Result RemoveDirectory(string identifier)
        {
            if (File.Exists(DirectoriesFilePath))
            {
                var list = JsonSerializer.Deserialize<List<DirectoryRecord>>(File.ReadAllText(DirectoriesFilePath));
                int loc = list.FindIndex(dr => dr.IsIdentifiableBy(identifier));
                if (loc == -1)
                {
                    // no directoryrecord found with that identifier
                    return Result.Failure("Could not remove directory because it was not found");
                }
                else
                {
                    // found itttt
                    list.RemoveAt(loc);
                    File.WriteAllText(DirectoriesFilePath, JsonSerializer.Serialize<List<DirectoryRecord>>(list));

                    // stuff
                    Directories = Directories.Where(d => !d.IsIdentifiableBy(identifier)).ToArray();

                    return Result.Success();
                }
            }
            else
            {
                return Result.Failure("Could not remove directory because no save data was found");
            }
        }

        /// <summary>
        /// Gets DirectoryRecords for all the directories in the directories config file that match the given type
        /// </summary>
        /// <param name="scope">One of ALL, SOURCE, or TARGET</param>
        /// <returns>An <see cref="DirectoryRecord[]"></see></returns>
        public static Result GetDirectories(DirectoryType? type = null)
        {
            if (File.Exists(DirectoriesFilePath))
            {
                string content = File.ReadAllText(DirectoriesFilePath);
                if (String.IsNullOrEmpty(content))
                {
                    return Result.Success(new DirectoryRecord[0]);
                }
                else
                {
                    return Result.Success(JsonSerializer.Deserialize<DirectoryRecord[]>(content));
                }
            }
            else
            {
                return Result.Success(new DirectoryRecord[0]);
            }
        }

        /// <summary>
        /// Verifies that all the DirectoryRecords in <paramref name="input"/> are registered directories.
        /// Stores a boolean indicating whether all input directories are registered: true if all are, else false.
        /// </summary>
        /// <param name="input">List of DirectoryRecords to verify</param>
        /// <returns>A Result object which reports whether the validation was successful.
        /// If the operation is succesful, the status of the validity is stored in the Data field.</returns>
        public static Result ValidateDirectories(List<DirectoryRecord> input)
        {
            if (input.All(i => Array.Find(Directories, d => d.Identifier == i.Identifier) != null))
                return Result.Success(true);
            else
                return Result.Success(false);
        }

        /// <summary>
        /// Verifies that all the DirectoryRecords in <paramref name="input"/> are registered directories.
        /// Stores a boolean indicating whether all input directories are registered: true if all are, else false.
        /// </summary>
        /// <param name="input">List of DirectoryRecords to verify</param>
        /// <returns>A Result object which reports whether the validation was successful.
        /// If the operation is succesful, the status of the validity is stored in the Data field.</returns>
        public static Result ValidateDirectories(DirectoryRecord[] input)
        {
            if (input.All(i => Array.Find(Directories, d => d.Identifier == i.Identifier) != null))
                return Result.Success(true);
            else
                return Result.Success(false);
        }

        /// <summary>
        /// Verifies that the given identifier corresponds to directories registed in the application.
        /// If the identifier is valid, the Directory Record is returned. If not, null is returned.
        /// </summary>
        /// <param name="id">Identifier of the record to validate</param>
        /// <returns>The Directory Record if valid, else null</returns>
        public static DirectoryRecord ValidateDirectoryIdentifier(string id)
        {
            return Directories.FirstOrDefault(r => r.IsIdentifiableBy(id));
        }

        /// <summary>
        /// Lists all DirectorySchemes in the save file. Reading stops at the first
        /// invalid line, or at the end of the file.
        /// </summary>
        /// <returns>A Successful result with a List`DirectoryRecord` in Data, else a Failure result.</returns>
        public static Result GetSchemes()
        {
            if (File.Exists(SchemesFilePath))
            {
                return Result.Success(JsonSerializer.Deserialize<List<DirectoryScheme>>(File.ReadAllText(SchemesFilePath)));
            }
            else
            {
                return Result.Success(new List<DirectoryScheme>());
            }
        }

        /// <summary>
        /// Adds the given DirectoryScheme to the schemes file
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static Result AddScheme(DirectoryScheme scheme)
        {
            List<DirectoryScheme> collection = null;
            if (File.Exists(SchemesFilePath))
            {
                // verify there is no saved scheme with the same name or format string
                collection = JsonSerializer.Deserialize<List<DirectoryScheme>>(File.ReadAllText(SchemesFilePath));
                if (collection.Any(s => s.FormatString == scheme.FormatString || s.Name == scheme.Name))
                {
                    return Result.Failure("Already have a scheme with the same format or name");
                }
            }
            else
            {
                collection = new List<DirectoryScheme>();
            }

            // add this scheme and write all to disk
            collection.Add(scheme);
            File.WriteAllText(SchemesFilePath, JsonSerializer.Serialize<List<DirectoryScheme>>(collection));
            return Result.Success();
        }

        /// <summary>
        /// Removes the given Scheme from the save file
        /// </summary>
        /// <param name="scheme">Scheme to remove from the file</param>
        /// <returns></returns>
        public static Result RemoveScheme(string identifier)
        {
            if (File.Exists(SchemesFilePath))
            {
                var schemes = JsonSerializer.Deserialize<List<DirectoryScheme>>(File.ReadAllText(SchemesFilePath));
                int loc = schemes.FindIndex(s => s.FormatString == identifier || s.Name == identifier);
                if (loc == -1)
                {
                    // specified scheme not found
                    return Result.Failure("Specified scheme was not removed because it could not be found");
                }
                else
                {
                    schemes.RemoveAt(loc);
                    File.WriteAllText(SchemesFilePath, JsonSerializer.Serialize<List<DirectoryScheme>>(schemes));
                    return Result.Success();
                }
            }
            else
            {
                return Result.Failure("Schemes file is empty - cannot remove the specified scheme");
            }
        }
    }
}
