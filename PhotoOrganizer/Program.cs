using System;
using System.IO;
using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    class Program
    {
        static int Main(string[] args)
        {
            // Handle empty args
            if (args.Length == 0)
            {
                PrintUsage();
                return 1;
            }

            // Extract, then remove, application-wide options like --config
            args = ParseApplicationArguments(args);
            if (args == null)
                return 1;

            // Read command
            switch (args[0].ToLower())
            {
                case "add":
                    AddDirectory(args);
                    break;
                case "move":
                    break;
                case "list":
                    ListDirectories(args);
                    break;
                default:
                    PrintUsage();
                    return 1;
            }

            return 0;

        }

        /// <summary>
        /// Prints CLI usage information
        /// </summary>
        static void PrintUsage()
        {
            // ADD usage
            Console.WriteLine("add <source | target> $location [$name]");

            // MOVE usage
            Console.WriteLine("move <source | target> $name");

            // LIST usage
            Console.WriteLine("list [source | target]");

            Console.WriteLine("");
        }

        /// <summary>
        /// Parses application-wide arguments and returns all the others
        /// </summary>
        /// <param name="args">CLI arguments</param>
        /// <returns>Returns an array of all arguments except those that are application-wide.
        /// If there is an error in these arguments, the function will print an error message and return null.</returns>
        static string[] ParseApplicationArguments(string[] args)
        {
            // config
            int customConfig = Array.FindIndex<string>(args, a => a.ToLower() == "--config");
            if (customConfig != -1)
            {
                // check location was provided
                if (customConfig < args.Length - 1)
                {
                    Console.WriteLine("--config option used, but no path provided");
                    return null;
                }
                customConfig += 1; // point to the location now

                // check location is valid
                if (!File.Exists(args[customConfig]))
                {
                    Console.WriteLine("Config not found at '" + args[customConfig] + "'");
                    return null;
                }
                else
                {
                    SaveData.DataDirectory = args[customConfig];
                }
            }
            return args;
        }

        /// <summary>
        /// Adds another directory to the watchlist
        /// </summary>
        /// <param name="args">CLI arguments used to invoke the program</param>
        static void AddDirectory(string[] args)
        {
            string location, name, dirType;

            // reject wrong argument count
            if (args.Length > 4)
            {
                Console.WriteLine("Found unknown arguments-- expected 4 or fewer, got " + args.Length);
                return;
            }
            else if (args.Length < 2)
            {
                Console.WriteLine("Missing expected arguments");
                return;
            }

            // Set variables to default, config, or CLI values
            if (args[1].ToLower() == "source" || args[1].ToLower() == "target")
            {
                location = args[2];
                dirType = args[1].ToUpper();

                // Set name to last term in location unless custom name provided
                if (args.Length == 4)
                    name = args[3];
                else
                {
                    name = args[2].Substring(args[2].LastIndexOf(Path.DirectorySeparatorChar));
                    name = name.Replace(Path.DirectorySeparatorChar.ToString(), "");
                    if (String.IsNullOrWhiteSpace(name))
                    {
                        name = args[2].Substring(args[2].LastIndexOf(Path.DirectorySeparatorChar));
                    }
                }
            }
            else
            {
                Console.WriteLine("Directory not declared SOURCE or TARGET");
                return;
            }

            // Verify path exists
            if (!Directory.Exists(location))
            {
                Console.WriteLine("Directory '" + location + "' not found");
                return;
            }

            // Valid. Add it!
            DirectoryRecord rec = new DirectoryRecord()
            {
                Alias = name,
                IsRecursive = true,
                Mode = DirectoryMode.Auto,
                Path = location
            };

            // Save, and see if it isn't a duplicate
            if (SaveData.AddSource(args[1].ToLower() + 's', rec.ToXml(args[1].ToLower())))
                Console.WriteLine("Added " + dirType + " " + location);
            else
                Console.WriteLine("Already had '" + rec.Path + "'");
        }

        /// <summary>
        /// Prints all the directories in a nice format
        /// </summary>
        /// <param name="args"></param>
        static void ListDirectories(string[] args)
        {
            if (args.Length > 2)
            {
                Console.WriteLine("Only expected 2 arguments");
            }
            else if (args.Length == 2)
            {
                if (args[1].ToLower() == "source" || args[1].ToLower() == "target")
                    SaveData.ListDirectories(args[1]);
                else
                    Console.WriteLine("Unknown option " + args[1]);
            }
            else
                SaveData.ListDirectories();
            
                    //SaveData.DataDirectory
        }
    }
}
