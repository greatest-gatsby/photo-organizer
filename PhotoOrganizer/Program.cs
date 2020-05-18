using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using CommandLine;

using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var parser = new Parser(conf => {
                conf.CaseInsensitiveEnumValues = true;
                conf.HelpWriter = Console.Out;
            });
            var retvalue = parser.ParseArguments<DirectoryAddOptions, DirectoryListOptions, DirectoryRemoveOptions>(args)
                .WithParsed<DirectoryAddOptions>(opts => AddDirectory(opts))
                .WithParsed<DirectoryListOptions>(opts => ListDirectories(opts))
                .WithParsed<DirectoryRemoveOptions>(opts => RemoveDirectory(opts))
                .WithNotParsed(err => Console.WriteLine("failed"));
            
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
            Console.WriteLine("DIRECTORIES");
            // ADD usage
            Console.WriteLine("add <source | target> $path [$name]");

            // MOVE usage
            Console.WriteLine("move $source_identifier [$addl_source, ...] $target_identifier");

            // LIST usage
            Console.WriteLine("list [source | target]");

            // REMOVE usage
            Console.WriteLine("remove <$name | $path>");

            // Scheme management
            Console.WriteLine("{0}TARGET SCHEMES", Environment.NewLine);

            Console.WriteLine("scheme add $format [$name] [description]");

            Console.WriteLine("scheme list");

            Console.WriteLine("scheme remove <$format | $name>");

            Console.WriteLine("scheme help");

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

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim('"'); // dotnet groups the args when in quotes, but does not strip the quotes
            }
            return args;
        }

        /// <summary>
        /// Adds another directory to the watchlist
        /// </summary>
        /// <param name="args">CLI arguments used to invoke the program</param>
        static int AddDirectory(DirectoryAddOptions opts)
        {
            // Create record
            DirectoryRecord record = new DirectoryRecord(opts.DirType, opts.Directory, opts.Alias);
                        
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
        static int ListDirectories(DirectoryListOptions opts)
        {
            var set = SaveData.GetDirectories(opts.DirType);
            if (set.Successful)
            {
                foreach (DirectoryRecord rec in (DirectoryRecord[])(set.Data))
                {
                    string lineTail = String.IsNullOrEmpty(rec.Alias) ? "" : "\t" + rec.Alias;
                    Console.Write(rec.Type.ToString("g") + "\t" + rec.Path + lineTail + Environment.NewLine);
                }
            }
            return 0;
        }

        /// <summary>
        /// Removes the given directory from the directories file
        /// </summary>
        /// <returns>Status code program should echo before exiting</returns>
        static int RemoveDirectory(DirectoryRemoveOptions opts)
        {
            // After command is consumed, expecting:
            // <$name | $path>
            string id = String.IsNullOrEmpty(opts.Alias) ? opts.Directory : opts.Alias;
            var result = SaveData.RemoveDirectory(id);
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
        /// Executes a move operation on stored directories
        /// </summary>
        /// <param name="args">CLI args</param>
        /// <returns>Status code program should echo before exiting</returns>
        static int ExecuteMove(string[] args)
        {
            // After command is consumed, expecting:
            // $source_identifier [$addl_source, ...] $target_identifier

            // reject wrong number of args
            if (args.Length < 2)
            {
                Console.WriteLine("Expected 2 arguments, got {0}", args.Length);
                return 1;
            }

            var result = Organizer.Move(args);
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
            if (args.Length < 1)
            {
                PrintUsage();
                return 1;
            }

            string command = ConsumeFirst(ref args);

            switch (command)
            {
                case "add":
                    return AddScheme(args);
                case "list":
                    return ListSchemes(args);
                case "remove":

                    break;
                case "help":
                    PrintUsage();
                    return 0;
                default:
                    PrintUsage();
                    return 1;
            }
            return 0;
        }

        static int AddScheme(string[] args)
        {
            DirectoryScheme scheme = null;
            if (!DirectoryScheme.TryParse(args, out scheme))
            {
                return 1;
            }

            var res = SaveData.AddScheme(scheme);
            if (res.Successful)
            {
                return 0;
            }
            else
            {
                Console.WriteLine(res.Message);
                return 1;
            }
        }

        static int RemoveSchemes(string[] args)
        {
            DirectoryScheme scheme = null;
            if (!DirectoryScheme.TryParse(args, out scheme))
            {
                return 1;
            }

            var res = SaveData.RemoveScheme(scheme);

            if (res.Successful)
            {
                return 0;
            }
            else
            {
                Console.WriteLine(res.Message);
                return 1;
            }
        }

        static int ListSchemes(string[] args)
        {
            if (args.Length != 0)
            {
                Console.WriteLine("Expected 0 arguments, got {0}", args.Length);
                return 1;
            }

            var res = SaveData.ListSchemes();
            if (res.Successful)
            {
                return 0;
            }
            else
            {
                Console.WriteLine(res.Message);
                return 1;
            }
        }
    }
}
