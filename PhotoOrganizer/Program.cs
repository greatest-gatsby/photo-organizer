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

            // Extract and remove application-wide options like --config
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
                    SaveData.Move();
                    break;
                case "list":
                    ListDirectories(args);
                    break;
                case "remove":
                    RemoveDirectory(args);
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
            Console.WriteLine("add <source | target> $path [$name]");

            // MOVE usage
            Console.WriteLine("move <source | target> $name");

            // LIST usage
            Console.WriteLine("list [source | target]");

            // REMOVE usage
            Console.WriteLine("remove <$name | $path>");

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
        static int AddDirectory(string[] args)
        {
            // Verify array size
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("Expected 3 or 4 arguments, got {0}", args.Length);
            }

            // Now, assumes the following input structure
            // <source | target> $path [$alias]

            // Verify type
            DirectoryType type;
            if (!DirectoryRecord.TryParseType(args[1], out type))
            {
                Console.WriteLine("Unknown directory type {0}", args[1]);
            }

            // Create record
            DirectoryRecord record = new DirectoryRecord()
            {
                Type = type,
                Path = args[2]
            };
            
            // Add alias if included
            if (args.Length == 4)
            {
                record.Alias = args[3];
            }

            // Attempt to save
            var result = SaveData.AddDirectory(record);
            if (result.Successful)
            {
                return 0;
            }
            else
            {
                Console.WriteLine(result.Message + "TAG");
                return 1;
            }
            
        }

        /// <summary>
        /// Prints all the directories in a nice format
        /// </summary>
        /// <param name="args"></param>
        static int ListDirectories(string[] args)
        {
            // Reject too many arguments
            if (args.Length > 2)
            {
                Console.WriteLine("Expected 1 or 2 arguments, got {0}", args.Length);
                return 1;
            }
            // Certain type listings
            else if (args.Length == 2)
            {
                // List only those of a certain type
                var result = SaveData.ListDirectories(args[1]);
                if (result.Successful)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine(result.Message);
                    return 1;
                }
            }
            // All types listing
            else
            {
                var result = SaveData.ListDirectories();
                if (result.Successful)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine(result.Message);
                    return 1;
                }
            }
        }

        static int RemoveDirectory(string[] args)
        {
            var result = SaveData.RemoveDirectory(args);
            if (result.Successful)
            {
                return 0;
            }
            else
            {
                Console.WriteLine(result.Message);
                return 1;
            }
        }
    }
}
