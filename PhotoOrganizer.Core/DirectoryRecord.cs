using System;
using System.Collections.Generic;
using System.Text;
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
        /// 
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public XElement ToXml(string tagName)
        {
            XElement element = new XElement(tagName);

            element.Add(new XElement("name") { Value = this.Alias });

            element.Add(new XElement("recursive") { Value = this.IsRecursive.ToString() });

            element.Add(new XElement("path") { Value = this.Path });

            return element;
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
        public DirectoryMode Mode { get; set; }

        public IEnumerable<string> RetrieveImages()
        {
            List<string> ret = new List<string>();
            

            return ret;
        }

    }

    public enum DirectoryMode
    {
        Auto,
        Manual
    }
}
