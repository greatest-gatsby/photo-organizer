using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    public class Scheme
    {
        /// <summary>
        /// Gets the scheme format string with the given name. If no scheme by that name is found, returns null.
        /// </summary>
        /// <param name="name">Name of the scheme to retrieve</param>
        /// <returns>The scheme formatting string if found, else returns null.</returns>
        static string ByName(string name)
        {
            if (File.Exists(SaveData.SchemesFilePath))
            {

            }

            return "";
        }
    }
}
