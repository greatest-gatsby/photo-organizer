using System;
using System.Collections.Generic;
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
            var retvalue = parser.ParseArguments<DirectoryAddOptions, DirectoryListOptions, DirectoryRemoveOptions,
                                                 SchemeAddOptions, SchemeListOptions, SchemeRemoveOptions,
                                                 ExecuteMoveOptions>(args)
                // directory funcs
                .WithParsed<DirectoryAddOptions>(opts => AddDirectory(opts))
                .WithParsed<DirectoryListOptions>(opts => ListDirectories(opts))
                .WithParsed<DirectoryRemoveOptions>(opts => RemoveDirectory(opts))
                
                // scheme funcs
                .WithParsed<SchemeAddOptions>(opts => AddScheme(opts))
                .WithParsed<SchemeRemoveOptions>(opts => RemoveSchemes(opts))
                .WithParsed<SchemeListOptions>(opts => ListSchemes(opts))

                // execute
                .WithParsed<ExecuteMoveOptions>(opts => ExecuteMove(opts))
                
                // everything else is an error
                .WithNotParsed(err => Console.WriteLine("failed"));
            
            return 0;

        }

        #region directory
        /// <summary>
        /// Adds another directory to the watchlist
        /// </summary>
        /// <param name="args">CLI arguments used to invoke the program</param>
        static int AddDirectory(DirectoryAddOptions opts)
        {
            // verify the scheme format id if necessary
            if (opts.DirType == DirectoryType.Target && !String.IsNullOrEmpty(opts.SchemeIdentifier))
            {

            }

            // Create record
            DirectoryRecord record = new DirectoryRecord(opts.DirType,
                opts.Directory, String.Join(' ', opts.Alias))
            {
                RecursiveSource = opts.Recursive
            };
                        
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
                    Console.Write(rec.Type.ToString("g") +
                        ((rec.RecursiveSource && rec.Type == DirectoryType.Source) ? " (R)" : "") + "\t" +
                        rec.Path + lineTail + Environment.NewLine);
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

        #endregion


        #region scheme
        static int AddScheme(SchemeAddOptions opts)
        {
            DirectoryScheme scheme = new DirectoryScheme(format: opts.FormatString, alias: opts.Alias, desc: opts.Description);

            var res = SaveData.AddScheme(scheme);
            if (res.Successful)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        static int RemoveSchemes(SchemeRemoveOptions opts)
        {
            
            var res = SaveData.RemoveScheme(opts.Alias);

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

        static int ListSchemes(SchemeListOptions opts)
        {
            var res = SaveData.GetSchemes();
            if (res.Successful)
            {
                var set = res.Data;
                foreach (var itm in set)
                {
                    Console.WriteLine(itm.ToString());
                }
                return 0;
            }
            else
            {
                Console.WriteLine(res.Message);
                return 1;
            }
        }
        #endregion

        #region exec
        static int ExecuteMove(ExecuteMoveOptions opts)
        {
            var res = Organizer.TryMove(opts.SourceIdentifier.Trim('"'), opts.TargetIdentifier.Trim('"'));
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
        #endregion
    }
}
