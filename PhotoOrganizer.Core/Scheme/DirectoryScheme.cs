using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Represents a way of organizing images in a given directory
    /// </summary>
    public class DirectoryScheme
    {
        /// <summary>
        /// The custom format string describing the scheme's structure.
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Human-friendly alias used to refer to this scheme.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for this scheme.
        /// </summary>
        public string Description { get; set; }

        public override string ToString()
        {
            return this.FormatString + "\t" + this.Name + "\t" + this.Description + Environment.NewLine;
        }

        public DirectoryScheme()
        {

        }

        public DirectoryScheme(string format, string alias, string desc = "")
        {
            this.FormatString = format;
            this.Name = alias;
            this.Description = String.IsNullOrEmpty(desc) ? null : desc;
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

            DirectoryScheme scheme = new DirectoryScheme(
                format: split[0],
                alias: split[1]
                );
            
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

            DirectoryScheme scheme = new DirectoryScheme(
                format: input[0],
                alias: input[1]
                );
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

        /// <summary>
        /// Verifies that all the tokens in this scheme's format string are valid
        /// and that there are no partial tokens (i.e. unclosed brackets)
        /// </summary>
        /// <returns></returns>
        public bool ValidateTokens()
        {
            // iterate thru formatstring looking for the opening bracket
            for (int i = 0; i < this.FormatString.Length; i++)
            {
                if (FormatString[i] == '{')
                {
                    // find end bracket
                    int endBracket = FormatString.IndexOf('}', i);
                    if (endBracket == -1)
                    {
                        // closing-bracket not found. Error.
                        return false;
                    }
                    else
                    {
                        // closing bracket found. extract the token between them
                        string token = FormatString.Substring(i + 1, endBracket - i);
                        
                        // verify token is known
                        if (Array.IndexOf(SchemeTokens.All,token) == -1)
                        {
                            // token was not found in array of known tokens. Error.
                            return false;
                        }
                        else
                        {
                            // token found. advance counter to the last char of the token.
                            i = endBracket;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get an ordered array of the segments of the new path for a given image
        /// including replacing token keys with their corresponding values.
        /// Consider validating the FormatString before calling this method.
        /// </summary>
        /// <returns></returns>
        /// <param name="basePath">The string of the path that preceedes these segments. If empty, you must combine the paths manually.</param>
        /// <param name="imgRec">Image to be relocated</param>
        /// <exception cref="FormatException">Throws if FormatString contains an invalid/unknown token.</exception>
        public string[] GetPathSegments(ImageRecord imgRec, string basePath = "")
        {
            // put BasePath at the first element if it is not null or empty
            string[] segments = String.IsNullOrEmpty(basePath) ? 
                    FormatString.Split(Path.DirectorySeparatorChar) :
                    FormatString.Split(Path.DirectorySeparatorChar).Prepend(basePath).ToArray();

            // decode em
            for (int i = 0; i < segments.Length; i++)
            {
                // ignore the first cell if it's the basePath
                if (!String.IsNullOrEmpty(basePath) && i == 0)
                    continue;

                for (int j = 0; j < segments[i].Length; j++)
                {
                    int open = segments[i].IndexOf('{', j);
                    if (open == -1)
                    {
                        // no open bracket, so we can continue to the next segment
                        j = segments[i].Length; // break out of the inner loop entirely
                        continue;
                    }
                    else
                    {
                        // contains an open bracket, so let's find the token contained within
                        j = segments[i].IndexOf('}'); // move our inner loop to the location of our close bracket
                        string token = segments[i].Substring(open + 1, segments[i].Length - j - 1);
                        if (Array.BinarySearch(SchemeTokens.Stock, token) == -1)
                        {
                            // not a stock datetime token
                            // TODO: implement custom tokens like photo format, resolution, orientation, etc.
                        }
                        else
                        {
                            // stock datetime token - this is easy to process
                            segments[i] = segments[i].Replace("{" + token + "}", imgRec.File.CreationTime.ToString(token));
                        }
                    }
                }
            }

            return segments;
        }
    }
}
