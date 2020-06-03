using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    [Verb("directory-add", HelpText = "Add managed directories.")]
    public class DirectoryAddOptions
    {
        [Option('t',"type", Required = true)]
        public DirectoryType DirType { get; set; }

        [Option('d', "directory", Required = true)]
        public string Directory { get; set; }

        [Option('a', "alias", Required = false)]
        public IEnumerable<string> Alias { get; set; }
    }

    [Verb("directory-list", HelpText = "List managed directories.")]
    public class DirectoryListOptions
    {
        [Option('t', "type", Required = false)]
        public DirectoryType? DirType { get; set; }
    }

    [Verb("directory-remove", HelpText = "Remove a managed directory by path or alias.")]
    public class DirectoryRemoveOptions
    {
        [Option('d', "directory", Group = "target")]
        public string Directory { get; set; }

        [Option('a', "alias", Group = "target")]
        public string Alias { get; set; }
    }
}
