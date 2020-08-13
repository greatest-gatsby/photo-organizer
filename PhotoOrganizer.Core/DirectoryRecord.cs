using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class DirectoryRecord
    {
        protected DirectoryRecord() { }

        public DirectoryRecord(DirectoryType type, string path, string alias = "", DirectoryScheme scheme = null)
        {
            Type = type;
            Path = path;
            Alias = String.IsNullOrEmpty(alias) ? null : alias;

            if (Type == DirectoryType.Target)
            {
                if (scheme == null)
                {
                    // get default scheme
                }
                else
                {
                    Scheme = scheme;
                }
            }
        }

        /// <summary>
        /// Alternate name for the directory
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Path to the directory
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Returns the best way for a user to recognize this record: its alias if it exists, else its path
        /// </summary>
        public string Identifier
        {
            get { return String.IsNullOrEmpty(Alias) ? Path : Alias; }
        }

        /// <summary>
        /// Determines whether the given string can be used to identify this record. Checks path and alias.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsIdentifiableBy(string value)
        {
            return (value == this.Alias) || (value == this.Path);
        }

        /// <summary>
        /// Shows what kind of directory this is
        /// </summary>
        public DirectoryType Type { get; set; }

        public DirectoryScheme Scheme { get; set; }


        public bool RecursiveSource { get; set; } = false;


        /// <summary>
        /// Constructs a string of the record in the easily parsable format
        /// </summary>
        /// <returns>A formatted line representing this record</returns>
        public override string ToString()
        {
            return Type.ToString("g") + 
                ((RecursiveSource  && Type == DirectoryType.Source) ? " (R)" : "") + "\t" +
                Path + "\t" +
                Alias + Environment.NewLine;
        }

        /// <summary>
        /// Parses a Directory Type from the given type string. Case and trim insensitive.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="FormatException">Thrown if the input does not match a known DirectoryType</exception>
        /// <returns></returns>
        public static DirectoryType ParseType(string input)
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
                throw new FormatException(SourceDirectory.WrongType(input));
        }

        /// <summary>
        /// Parses a directory type from the given string, and returns the status of the parse attempt.
        /// </summary>
        /// <param name="input">The string to parse into a DirectoryType</param>
        /// <param name="type">The DirectoryType whose value will be overwritten with the parsed value, if successful</param>
        /// <returns>True if parsing succeeeds, else False</returns>
        public static bool TryParseType(string input, out DirectoryType type)
        {
            try
            {
                type = SourceDirectory.ParseType(input);
                return true;
            }
            catch
            {
                type = DirectoryType.Source;
                return false;
            }
        }

        /// <summary>
        /// Prints a standardized message for incorrect type arguments
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string WrongType(string type)
        {
            return "Unknown directory type " + type;
        }

        /// <summary>
        /// Gets the location for the given image based on this Directory
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public string GetNewLocation(ImageRecord img)
        {
            if (this.Scheme == null)
            {
                return System.IO.Path.Combine(this.Path, img.File.Name);
            }
            else
            {
                return System.IO.Path.Combine(
                    this.Scheme.GetPathSegments(img, this.Path).ToArray()
                );
            }
            
        }

        /// <summary>
        /// Gets the locations for the given images based on this Directory
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public string[] GetNewLocations(ImageRecord[] imgs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all applicable images from this directory. This returns all images within the directory
        /// TOP LEVEL ONLY unless this is a SOURCE directory and the recursive option is enabled.
        /// </summary>
        /// <returns></returns>
        public ImageRecord[] GetRecordsForContents()
        {
            var opt = this.RecursiveSource ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return new System.IO.DirectoryInfo(this.Path)
                .GetFiles("*", opt)
                .Select<System.IO.FileInfo, ImageRecord>(fi => new ImageRecord(fi, this))
                .ToArray();
        }
    }

    public class SourceDirectory : DirectoryRecord
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public SourceDirectory() 
        {
            Type = DirectoryType.Source;
        }

        /// <summary>
        /// If true, searches all subfolders for images
        /// </summary>
        public bool IsRecursive { get; set; }

        public IEnumerable<string> RetrieveImages()
        {
            List<string> ret = new List<string>();
            

            return ret;
        }
    }

    public enum DirectoryType
    {
        Source,
        Target
    }
}
