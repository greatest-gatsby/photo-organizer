using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;

using PhotoOrganizer;

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
        public string Message { get; }

        /// <summary>
        /// Returns true if the operation succeeded, else false.
        /// </summary>
        public bool Successful { get; }

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
        /// Returns a Result indicating SUCCESS
        /// </summary>
        public static Result Success() { 
            {
                return new Result();
            }
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
    }

    public static class SaveData
    {
        /// <summary>
        /// Location of data directory which will be checked if the current directory doesn't have organizer_data.xml. Default is in the following special directory.
        /// </summary>
        public static string DataDirectory { get; set; }


        public static string DirectoriesFileName { get { return "directories"; } }

        public static string DirectoriesFilePath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }

        public static string SchemesFileName { get { return "schemes"; } }

        public static string SchemesFilePath { get { return Path.Combine(DataDirectory, SchemesFileName); } }

        /// <summary>
        /// Returns the path to the in-use config file
        /// </summary>
        public static string ConfigPath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }
        
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
                catch (Exception ex)
                {
                    // Use current directory instead
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, DirectoriesFileName), "");
                    DataDirectory = Environment.CurrentDirectory;
                    Console.WriteLine("Created config at " + ConfigPath);
                }
            }
        }

        /// <summary>
        /// Adds the given record to the Directories file
        /// </summary>
        /// <param name="category">Category of data to append</param>
        /// <param name="element">XElement to insert in file</param>
        public static Result AddDirectory(DirectoryRecord record)
        {
            // Check for duplicates
            if (File.Exists(DirectoriesFilePath))
            {
                foreach (string line in File.ReadAllLines(DirectoriesFilePath))
                {
                    DirectoryRecord found;
                    if (DirectoryRecord.TryParse(line, out found) && found.Path == record.Path)
                    {
                        return Result.Failure("Already have a record for {0}", found.Identifier);
                    }
                }
            }

            // Write it
            try
            {
                File.AppendAllText(DirectoriesFilePath, record.ToString());
                return Result.Success();
            }
            catch
            {
                return Result.Failure("Error occured during file write");
            }
        }

        /// <summary>
        /// Removes a saved place by name or path
        /// </summary>
        /// <param name="args">CLI args containing the path or name</param>
        /// <returns></returns>
        public static Result RemoveDirectory(string args)
        {
            bool found = false;
            // Exit on obvious errors, such as unexpected arguments
            if (String.IsNullOrEmpty(args))
            {
                return Result.Failure("Expected path or name, but got empty string");
            }

            // Look for it
            StreamReader reader = File.OpenText(SaveData.DirectoriesFilePath);
            string newContents = String.Empty;
            int lineNumber = 1;
            try
            {
                while (!reader.EndOfStream)
                {
                    DirectoryRecord rec = null;
                    if (!DirectoryRecord.TryParse(reader.ReadLine(), out rec))
                    {
                        Console.WriteLine("Failed to parse directory on line {0}", lineNumber);
                        lineNumber++;
                        continue;
                    }
                    lineNumber++;

                    // See if this rec we are looking at matches the query given by the user
                    if (args == rec.Path || (!String.IsNullOrEmpty(args) && args == rec.Alias))
                    {
                        // Do not write back to file
                        found = true;
                    }
                    else
                        newContents += rec.ToString();
                }
            }
            finally
            {
                reader.Close();
            }

            // If found, then write the modified file
            if (found)
            {
                try
                {
                    File.WriteAllText(SaveData.DirectoriesFilePath, newContents);
                }
                catch (IOException)
                {

                }
                return Result.Success();
            }
            // Otherwise don't bother rewriting the same contents
            else
            {
                return Result.Failure("No saved directory named '{0}'", args);
            }
        }

        public static Result Move()
        {
            return Result.Success();
        }

        /// <summary>
        /// Lists directories of the given type. If an invalid type is given, no message or error is given.
        /// </summary>
        /// <param name="scope">One of ALL, SOURCE, or TARGET</param>
        public static Result ListDirectories(string scope = "all")
        {
            StreamReader reader = null;
            try
            {
                reader = File.OpenText(SaveData.DirectoriesFilePath);
                bool parseAll = (scope == "all") ? true : false;

                // This lets us reuse that same message thrown by the parsing method
                DirectoryType type = DirectoryType.Source;
                try
                {
                    if (!parseAll)
                        type = DirectoryRecord.ParseType(scope);
                }
                catch (Exception ex)
                {
                    return Result.Failure(ex.Message);
                }

                while (!reader.EndOfStream)
                {
                    DirectoryRecord rec;
                    if (!DirectoryRecord.TryParse(reader.ReadLine(), out rec))
                    {
                        continue; // skip failed parse lines
                    }
                    if (parseAll || type == rec.Type)
                    {
                        string lineTail = String.IsNullOrEmpty(rec.Alias) ? "" : "\t" + rec.Alias;
                        Console.Write(rec.Type.ToString("g") + "\t" + rec.Path + lineTail + Environment.NewLine);
                    }
                }
            }
            finally
            {
                reader?.Close();
            }
            

            return Result.Success();
        }
    }
}
