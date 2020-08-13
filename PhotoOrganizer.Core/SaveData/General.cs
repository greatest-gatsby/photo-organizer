using System;
using System.IO;

namespace PhotoOrganizer.Core
{
    public static partial class SaveData
    {
        /// <summary>
        /// Location of data directory which will be checked if the current directory doesn't have organizer_data.xml. Default is in the following special directory.
        /// </summary>
        public static string DataDirectory { get; set; }

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
                    Console.WriteLine("Created config at " + DataDirectory);
                }
                catch (Exception)
                {
                    // Use current directory instead
                    try
                    {
                        File.WriteAllText(Path.Combine(Environment.CurrentDirectory, DirectoriesFileName), "");
                        DataDirectory = Environment.CurrentDirectory;
                        Console.WriteLine("Created config at " + DataDirectory);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Could not determine a writable path for config files", ex);
                    }
                }
            }

            Directories = LoadDirectoriesFromDisk();
            Schemes = LoadSchemesFromDisk();
        }
    }
}
