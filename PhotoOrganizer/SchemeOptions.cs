using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

using CommandLine;

namespace PhotoOrganizer
{
    [Verb("scheme-add", HelpText = "Add directory scheme.")]
    class SchemeAddOptions
    {
        [Option('a', "alias", Required = true)]
        public string Alias { get; set; }

        [Option('f', "format", Required = true)]
        public string FormatString { get; set; }

        [Option('d', "description", Required = false)]
        public string Description { get; set; }
    }

    [Verb("scheme-list", HelpText = "List directory schemes.")]
    class SchemeListOptions
    {

    }

    [Verb("scheme-remove", HelpText = "Remove a directory scheme.")]
    class SchemeRemoveOptions
    {
        [Option('a', "alias", Required = true)]
        public string Alias { get; set; }
    }
}