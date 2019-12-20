using System;
using System.IO;
using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    public class Program
    {
        public static int Main(string[] args)
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

            string command = ConsumeFirst(ref args);

            // Read command
            switch (command)
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
                case "scheme":

                    break;
                default:
                    PrintUsage();
                    return 1;
            }

            return 0;

        }

        /// <summary>
        /// Removes and returns the first item in the array, and modifies the args array IN PLACE
        /// </summary>
        /// <param name="args">CLI args to work with. The first ([0]) will be consumed.</param>
        /// <returns>The value at args[0]. The array will also be modified now.</returns>
        public static string ConsumeFirst(ref string[] args)
        {
            string val = args[0];
            Array.Copy(args, 1, args, 0, args.Length - 1);
            Array.Resize<string>(ref args, args.Length - 1);
            return val;
        }

        /// <summary>
        /// Prints CLI usage information
        /// </summary>
        static void PrintUsage()
        {
            // Directories/typical usage
            // ADD usage
            Console.WriteLine("add <source | target> $path [$name]");

            // MOVE usage
            Console.WriteLine("move <source | target> $name");

            // LIST usage
            Console.WriteLine("list [source | target]");

            // REMOVE usage
            Console.WriteLine("remove <$name | $path>");

            // Scheme management
            Console.WriteLine("{0}TARGET SCHEMES", Environment.NewLine);

            Console.WriteLine("scheme add $format [$name] [description]");

            Console.WriteLine("scheme list");

            Console.WriteLine("scheme remove <$format | $name>");

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
            // After command is consumed, expecting:
            // <source | target> $path [$name]

            // Verify array size
            if (args.Length < 2)
            {
                Console.WriteLine("Expected 2 or 3 arguments after 'add', got {0}--did you specify directory type?", args.Length);
                return 1;
            }
            else if (args.Length > 3)
            {
                Console.WriteLine("Expected 2 or 3 arguments after 'add', got {0}", args.Length);
                return 1;
            }

            // Verify type
            DirectoryType type;
            if (!DirectoryRecord.TryParseType(args[0], out type))
            {
                Console.WriteLine(DirectoryRecord.WrongType(args[0]));
                return 1;
            }

            // Create record
            DirectoryRecord record = new DirectoryRecord()
            {
                Type = type,
                Path = args[1]
            };
            
            // Add alias if included
            if (args.Length == 3)
            {
                record.Alias = args[2];
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
            // After command is consumed, expecting:
            // [source | target]

            // Reject too many arguments
            if (args.Length > 1)
            {
                Console.WriteLine("Expected 1 optional argument after 'list', got {0}", args.Length);
                return 1;
            }
            // Certain type listings
            else if (args.Length == 1)
            {
                // List only those of a certain type
                var result = SaveData.ListDirectories(args[0]);
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

        /// <summary>
        /// Removes the given directory from the directories file
        /// </summary>
        /// <param name="args">CLI args</param>
        /// <returns>Status code program should echo before exiting</returns>
        static int RemoveDirectory(string[] args)
        {
            // After command is consumed, expecting:
            // <$name | $path>

            var result = SaveData.RemoveDirectory(args[0]);
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

        /// <summary>
        /// Consumes arguments concerning schemes
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Status code program should echo before exiting</returns>
        static int Schemes(string[] args)
        {
            // Reject too few args
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            switch (args[1])
            {
                case "add":

                    break;
                case "list":

                    break;
                case "remove":

                    break;
                default:
                    PrintUsage();
                    return 1;
            }
            return 0;
        }

        static int AddScheme(string[] args)
        {
            return 0;
        }
    }
}
