using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Represents a way of organizing images in a given directory
    /// </summary>
    public class DirectoryScheme
    {
        public string FormatString { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return this.FormatString + "\t" + this.Name + "\t" + this.Description;
        }

        /// <summary>
        /// Parses a DirectoryScheme from the given input string
        /// </summary>
        /// <param name="input">Input string to parse</param>
        /// <returns>A corresponding DirectoryScheme. Parsing failures will throw exceptions!</returns>
        public static DirectoryScheme Parse(string input)
        {
            // Assumes following input structure
            // $expression \t $name \t $description
            string[] split = input.Split('\t');
            
            // reject bad arg count
            if (split.Length < 2 || split.Length > 3)
            {
                throw new ArgumentException($"Expected 2 or 3 arguments, got {split.Length}");
            }

            DirectoryScheme scheme = new DirectoryScheme();
            scheme.FormatString = split[0];
            scheme.Name = split[1];

            if (split.Length == 3)
            {
                scheme.Description = split[2];
            }


            return scheme;
        }

        /// <summary>
        /// Parses a DirectoryScheme from the given input string array
        /// </summary>
        /// <param name="input">Input string array to parse</param>
        /// <returns>A corresponding DirectoryScheme. Parsing failures will throw exceptions!</returns>
        public static DirectoryScheme Parse(string[] input)
        {
            // Assumes following input structure
            // [0] expression
            // [1] name
            // [2] description (opt.)

            if (input.Length < 2 || input.Length > 3)
            {
                throw new ArgumentException($"Expected 2 or 3 arguments, got {input.Length}");
            }

            DirectoryScheme scheme = new DirectoryScheme();
            scheme.FormatString = input[0];
            scheme.Name = input[1];

            if (input.Length == 3)
            {
                scheme.Description = input[2];
            }

            return scheme;
        }

        /// <summary>
        /// Attempts to parse a DirectoryScheme from the given input string. This method does not throw.
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <param name="scheme">The DirectoryScheme to write over. If parsing fails, this will be set to null.</param>
        /// <returns>True if parsing succeeds, else False.</returns>
        public static bool TryParse(string input, out DirectoryScheme scheme)
        {
            try
            {
                scheme = DirectoryScheme.Parse(input);
                return true;
            }
            catch
            {
                scheme = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to parse a DirectoryScheme from the given input 
        /// </summary>
        /// <param name="input">Input string array to parse</param>
        /// <param name="scheme">The DirectoryScheme to write over. If parsing fails, this will be set to null.</param>
        /// <returns>True if parsing suceeds, else false.</returns>
        public static bool TryParse(string[] input, out DirectoryScheme scheme)
        {
            try
            {
                scheme = DirectoryScheme.Parse(input);
                return true;
            }
            catch
            {
                scheme = null;
                return false;
            }
        }
    }
}
