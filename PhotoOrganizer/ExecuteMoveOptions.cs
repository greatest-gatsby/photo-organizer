using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;

using PhotoOrganizer.Core;

namespace PhotoOrganizer
{
    [Verb("exec-move", HelpText = "Execute a move operation")]
    class ExecuteMoveOptions
    {
        [Option('s', "source", Required = true, HelpText = "Alias or path of source directory.")]
        public string SourceIdentifier { get; set; }

        [Option('t', "target", Required = true, HelpText = "Alias or path of target directory.")]
        public string TargetIdentifier { get; set; }
    }

}
