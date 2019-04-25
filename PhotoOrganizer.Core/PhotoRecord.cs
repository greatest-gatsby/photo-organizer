using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PhotoOrganizer.Core
{
    [Serializable]
    public class PhotoRecord
    {
        #region properties
        /// <summary>
        /// Standardized name of the photograph, after any formatting or processing
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Collection of copies of/derivative images of the photograph
        /// </summary>
        ObservableCollection<Image>  Images { get; set; }
        #endregion
    }

    public static class Utilities
    {
        public static IEnumerable<string> GetDirectories(string basePath)
        {
            return new List<string>();
        }
    }
}
