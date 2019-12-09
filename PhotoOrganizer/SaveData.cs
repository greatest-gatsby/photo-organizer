using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;

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

        /// <summary>
        /// Returns the path to the in-use config file
        /// </summary>
        public static string ConfigPath { get { return Path.Combine(DataDirectory, ConfigName); } }
        
        static SaveData()
        {
            string appResourcePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "photo organizer" + Path.DirectorySeparatorChar);

            // Determine where the save data is -- either application data or current directory
            if (File.Exists(Path.Combine(appResourcePath, ConfigName)))
            {
                DataDirectory = appResourcePath;
            }
            else
            {
                // Try to create data directory in appdata folder
                try
                {
                    Directory.CreateDirectory(appResourcePath);
                    // Now try to write the config base
                    File.WriteAllText(Path.Combine(appResourcePath, "organizer_data.xml"), BaseConfig);
                    DataDirectory = appResourcePath;
                    Console.WriteLine("Created config at " + ConfigPath);
                }
                catch (Exception ex)
                {
                    // Use current directory instead
                    File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "organizer_data.xml"), BaseConfig);
                    DataDirectory = Environment.CurrentDirectory;
                    Console.WriteLine("Created config at " + ConfigPath);
                }
            }
        }

        /// <summary>
        /// Adds the XElement as a new child of the node named by <paramref name="category"/>
        /// </summary>
        /// <param name="category">Category of data to append</param>
        /// <param name="element">XElement to insert in file</param>
        public static bool AddSource(string category, XElement element)
        {
            XElement config = XDocument.Load(ConfigPath).Element("data");
            
            var set = config.Descendants().Where(d => d.Value == element.Element("path").Value);
            if (set.Any())
            {
                Console.WriteLine("SOURCE already saved");
                return false;
            }
            else
            {
                config.Element(category).Add(element);
                config.Save(ConfigPath);
                return true;
            }
        }

        /// <summary>
        /// Lists directories of the given type. If an invalid type is given, no message or error is given.
        /// </summary>
        /// <param name="scope">One of ALL, SOURCE, or TARGET</param>
        public static void ListDirectories(string scope = "all")
        {
            List<XElement> dirs = new List<XElement>();
            XElement config = XDocument.Load(ConfigPath).Root;
            scope = scope.ToLower();

            // Read elements from config
            if (scope == "all" || scope == "source")
            {
                dirs.AddRange(config.Element("sources").Elements());
            }
            if (scope == "target" || scope == "all")
            {
                dirs.AddRange(config.Element("targets").Elements());
            }
            
            // Handle empty config
            if (scope != "target" && scope != "source" && scope != "all")
            {
                Console.WriteLine("Unrecognized argument '{0}'", scope);
                return;
            }
            else
            {
                Console.WriteLine("NAME\t\tPATH\t\tTYPE");
                foreach (XElement ele in dirs)
                {
                    Console.WriteLine(ele.Element("name").Value + "\t" + ele.Element("path").Value + "\t" + ele.Name);
                }
            }
        }

        static readonly string BaseConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine
            + "<data>" + Environment.NewLine + "\t<sources>" + Environment.NewLine + "</sources>" + Environment.NewLine
            + "\t<targets>" + Environment.NewLine + "</targets>" + Environment.NewLine + "</data>";
    }
}
