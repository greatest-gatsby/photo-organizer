using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class DirectoryRecord
    {
        public DirectoryRecord() { }

        /// <summary>
        /// Constructs a DirectoryRecord from a save file's XML element
        /// </summary>
        /// <param name="element"></param>
        public DirectoryRecord(XElement element)
        {
            DirectoryRecord rec = new DirectoryRecord();

            // Determine recursive nature
            if (element.Element("recursive") == null)
            {
                rec.IsRecursive = false;
            }
            rec.IsRecursive = bool.Parse(element.Attribute("recursive").Value) ? false : true;

            // Path
            rec.Path = element.Element("path").Value;

            // Alias
            if (element.Element("name") == null)
            {
                rec.Alias = String.Empty;
            }
            else
            {
                rec.Alias = rec.Path.Substring(rec.Path.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            }
        }

          
        /// <summary>
        /// Path to the directory
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Alternate name for the directory
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// If true, searches all subfolders for images
        /// </summary>
        public bool IsRecursive { get; set; }

        /// <summary>
        /// idk
        /// </summary>
        public DirectoryType Type { get; set; }

        public IEnumerable<string> RetrieveImages()
        {
            List<string> ret = new List<string>();
            

            return ret;
        }


        /// <summary>
        /// Constructs a string of the record in the easily parsable format
        /// </summary>
        /// <returns>A formatted line representing this record</returns>
        public override string ToString()
        {
            return Type.ToString("g") + "\t" +
                Path + "\t" +
                Alias + Environment.NewLine;
        }

        /// <summary>
        /// Parses a DirectoryRecord from the given input string
        /// </summary>
        /// <param name="input">Input string to parse</param>
        /// <returns>A corresponding DirectoryRecord. Parsing failures will throw exceptions!</returns>
        public static DirectoryRecord Parse(string input)
        {
            // Intensely process the string by splitting at tabs
            string[] processed = input.Split('\t');
            
            // Fail on obvious errors
            if (processed.Length > 3 || processed.Length < 2)
            {
                throw new FormatException("Expected 2 or 3 arguments, got " + processed.Length);
            }

            
            DirectoryRecord record = new DirectoryRecord();

            // Parse type
            

            // Don't validate path because who cares
            record.Path = processed[1];

            // Parse alias if included
            if (processed.Length == 3)
                record.Alias = processed[2];

            return record;
        }

        public static DirectoryRecord Parse(string[] input)
        {
            // Assumes the following input structure
            // <source | target> $path [$alias]
            DirectoryRecord record = new DirectoryRecord()
            {
                Path = input[2]
            };

            record.Type = DirectoryRecord.ParseType(input[0]);

            if (input.Length > 3)
                throw new FormatException("Expected 2 or 3 arguments, got " + input.Length);
            else if (input.Length == 3)
                record.Alias = input[2];

            return record;
        }

        /// <summary>
        /// Attempts to parse a DirectoryRecord from the given input string. This method does not throw.
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <param name="record">The DirectoryRecord to write over. If parsing fails, this will be set to null.</param>
        /// <returns>True if parsing succeeds, else false.</returns>
        public static bool TryParse(string input, out DirectoryRecord record)
        {
            try
            {
                record = DirectoryRecord.Parse(input);
                return true;
            }
            catch
            {
                record = null;
                return false;
            }
        }

        /// <summary>
        /// Parses a Directory Type from the given type string. Case and trim insensitive.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static DirectoryType ParseType(string input)
        {
            if (input.ToLower().Trim() == "source")
            {
                return DirectoryType.Source;
            }
            else if (input.ToLower().Trim() == "target")
            {
                return DirectoryType.Target;
            }
            else
                throw new FormatException("Unrecognized type " + input);
        }

        /// <summary>
        /// Parses a directory type from the given string, and returns the status of the parse attempt
        /// </summary>
        /// <param name="input">The string to parse into a DirectoryType</param>
        /// <param name="type">The DirectoryType whose value will be overwritten with the parsed value, if successful</param>
        /// <returns>True if parsing succeeeds, else False</returns>
        private static bool TryParseType(string input, out DirectoryType type)
        {
            try
            {
                type = DirectoryRecord.ParseType(input);
                return true;
            }
            catch
            {
                type = DirectoryType.Source;
                return false;
            }
        }

    }

    public enum DirectoryType
    {
        Source,
        Target
    }
}
