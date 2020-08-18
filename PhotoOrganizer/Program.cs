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

            string alias = String.Join(' ', opts.Alias).Trim('"');
            opts.Directory = opts.Directory.Trim('"');

            // Create record
            DirectoryRecord record = new DirectoryRecord(opts.DirType, opts.Directory, alias)
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
                Console.WriteLine(result.Message);
                return 1;
            }
            
        }

        /// <summary>
        /// Prints all the directories in a nice format
        /// </summary>
        /// <param name="args"></param>
        static int ListDirectories(DirectoryListOptions opts)
        {
            foreach (DirectoryRecord rec in SaveData.Directories)
            {
                string lineTail = String.IsNullOrEmpty(rec.Alias) ? "" : "\t" + rec.Alias;
                Console.Write(rec.Type.ToString("g") +
                    ((rec.RecursiveSource && rec.Type == DirectoryType.Source) ? " (R)" : "") + "\t" +
                    rec.Path + lineTail + Environment.NewLine);
            }
            return 0;
        }

        /// <summary>
        /// Removes the given directory from the directories file
        /// </summary>
        /// <returns>Status code program should echo before exiting</returns>
        static int RemoveDirectory(DirectoryRemoveOptions opts)
        {
            string id = String.IsNullOrEmpty(opts.Alias) ? opts.Directory.Trim('"') : opts.Alias.Trim('"');
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
            opts.FormatString = opts.FormatString.Trim('"');
            opts.Alias = opts.Alias.Trim('"');
            
            if (!String.IsNullOrEmpty(opts.Description))
                opts.Description = opts.Description.Trim('"');
            
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
            opts.Alias = opts.Alias.Trim('"');
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
            foreach (var itm in SaveData.Schemes)
            {
                Console.WriteLine(itm.ToString());
            }
            return 0;
        }
        #endregion

        #region exec
        static int ExecuteMove(ExecuteMoveOptions opts)
        {
            opts.SourceIdentifier = opts.SourceIdentifier.Trim('"');
            opts.TargetIdentifier = opts.TargetIdentifier.Trim('"');
            DirectoryRecord source = SaveData.GetDirectoryOrDefault(opts.SourceIdentifier);
            if (source == null)
            {
                Console.WriteLine("No saved source '{0}'", opts.SourceIdentifier);
                return 1;
            }

            DirectoryRecord target = SaveData.GetDirectoryOrDefault(opts.TargetIdentifier);
            if (target == null)
            {
                Console.WriteLine("No saved target {0}", opts.TargetIdentifier);
                return 1;
            }

            var res = Organizer.TryMove(source, target);
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
