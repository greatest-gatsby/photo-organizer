using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;

using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    public static class SaveData
    {
        /// <summary>
        /// Location of data directory which will be checked if the current directory doesn't have organizer_data.xml. Default is in the following special directory.
        /// </summary>
        public static string DataDirectory { get; set; }

        /// <summary>
        /// Name of the config file. Stored within the data directory
        /// </summary>
        public static string ConfigName { get; set; } = "organizer_data.xml";

        public static string DirectoriesFileName { get { return "directories"; } }

        public static string DirectoriesFilePath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }

        /// <summary>
        /// Returns the path to the in-use config file
        /// </summary>
        public static string ConfigPath { get { return Path.Combine(DataDirectory, DirectoriesFileName); } }
        
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
        public static bool AddDirectory(DirectoryRecord record)
        {
            // Check for duplicates
            if (File.Exists(DirectoriesFilePath))
            {
                foreach (string line in File.ReadAllLines(DirectoriesFilePath))
                {
                    DirectoryRecord found;
                    if (DirectoryRecord.TryParse(line, out found) && found.Path == record.Path)
                    {
                        return false;
                    }
                }
            }

            // Write it
            try
            {
                File.AppendAllText(DirectoriesFilePath, record.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a saved place by name or path
        /// </summary>
        /// <param name="args">CLI args containing the path or name</param>
        /// <returns></returns>
        public static bool RemoveSource(string[] args)
        {
            bool foundDirectory = false;
            // Exit on obvious errors, such as unexpected arguments
            if (args.Length != 2)
            {
                return false;
            }

            // Look for it
            StreamReader reader = File.OpenText(SaveData.DirectoriesFilePath);
            string newContents = String.Empty;
            int lineNumber = 1;
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


                if (args[1] == rec.Path || args[1] == rec.Alias)
                {
                    // Do not write this back to file!
                    foundDirectory = true;
                }
                else
                    newContents += rec.ToString();
            }
            reader.Close();

            File.WriteAllText(SaveData.DirectoriesFilePath, newContents);

            return foundDirectory;
        }

        public static bool Move()
        {
            return true;
        }

        /// <summary>
        /// Lists directories of the given type. If an invalid type is given, no message or error is given.
        /// </summary>
        /// <param name="scope">One of ALL, SOURCE, or TARGET</param>
        public static void ListDirectories(string scope = "all")
        {
            StreamReader reader = File.OpenText(SaveData.DirectoriesFilePath);
            while (!reader.EndOfStream)
            {
                DirectoryRecord rec;
                if (!DirectoryRecord.TryParse(reader.ReadLine(), out rec))
                {
                    continue; // skip failed parse lines
                }
                if ((scope.ToLower() == "source" && rec.Type == DirectoryType.Source) ||
                    (scope.ToLower() == "target" && rec.Type == DirectoryType.Target) ||
                    scope.ToLower() == "all")
                {
                    Console.WriteLine(rec.Type.ToString("g") + "\t" + rec.Path + "\t" + rec.Alias);
                }
            }
        }

        static readonly string BaseConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine
            + "<data>" + Environment.NewLine + "\t<sources>" + Environment.NewLine + "</sources>" + Environment.NewLine
            + "\t<targets>" + Environment.NewLine + "</targets>" + Environment.NewLine + "</data>";
    }
}
