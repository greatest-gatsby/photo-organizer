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
        [Option('t',"type", Required = true, HelpText = "Type of directory to add, either source or target.")]
        public DirectoryType DirType { get; set; }

        [Option('d', "directory", Required = true, HelpText = "Full path to directory.")]
        public string Directory { get; set; }

        [Option('a', "alias", Required = false)]
        public IEnumerable<string> Alias { get; set; }

        [Option('s', "scheme", Required = false, HelpText = "Alias of the directory scheme to use. If none is provided, the default will be used.")]
        public string SchemeIdentifier { get; set; }

        [Option('r', "recursive", Required = false, HelpText = "Search this directory recursively. Only applicable to source directories.")]
        public bool Recursive { get; set; }
    }

    [Verb("directory-list", HelpText = "List managed directories.")]
    public class DirectoryListOptions
    {
        [Option('t', "type", Required = false, HelpText = "Type of directory to show, either source or target.")]
        public DirectoryType? DirType { get; set; }
    }

    [Verb("directory-remove", HelpText = "Remove a managed directory by path or alias.")]
    public class DirectoryRemoveOptions
    {
        [Option('d', "directory", Group = "target", HelpText = "Full path of directory to remove.")]
        public string Directory { get; set; }

        [Option('a', "alias", Group = "target", HelpText = "Alias of directory to remove.")]
        public string Alias { get; set; }
    }
}
