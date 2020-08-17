using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PhotoOrganizer.Core
{
    public static partial class SaveData
    {
        /// <summary>
        /// Name of the schemes file without the path or extension
        /// </summary>
        public static string SchemesFileName { get { return "schemes"; } }

        /// <summary>
        /// Full path and filename to the schemes file
        /// </summary>
        public static string SchemesFilePath { get { return Path.Combine(DataDirectory, SchemesFileName); } }

        /// <summary>
        /// All registered Directory Schemes
        /// </summary>
        public static DirectoryScheme[] Schemes { get; set; }

        /// <summary>
        /// Retrieves all Directory Schemes from the directories file on-disk. Calling this method manually
        /// is usually not needed, but doing so requires manually assigning the value to the Schemes property.
        /// </summary>
        /// <returns></returns>
        public static DirectoryScheme[] LoadSchemesFromDisk()
        {
            if (!File.Exists(SchemesFilePath))
            {
                return new DirectoryScheme[0];
            }
            string content = File.ReadAllText(SchemesFilePath);
            if (String.IsNullOrEmpty(content))
            {
                return new DirectoryScheme[0];
            }
            else
            {
                return JsonSerializer.Deserialize<DirectoryScheme[]>(content);
            }
        }

        /// <summary>
        /// Writes all of <see cref="SaveData.Schemes"/> to disk
        /// </summary>
        /// <returns></returns>
        public static Result WriteSchemesToDisk()
        {
            try
            {
                File.WriteAllText(SchemesFilePath, JsonSerializer.Serialize<DirectoryScheme[]>(Schemes));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.ToString());
            }
        }

        /// <summary>
        /// Adds the given DirectoryScheme to the schemes file
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static Result AddScheme(DirectoryScheme scheme)
        {
            if (GetDirectorySchemeOrDefault(scheme.FormatString) == null)
            {
                Schemes = Schemes.Append(scheme).ToArray();
                return SaveData.WriteSchemesToDisk();
            }
            else
            {
                return Result.Failure("Already have a scheme with the same format or name");
            }
        }

        /// <summary>
        /// Removes the given Scheme from the save file
        /// </summary>
        /// <param name="scheme">Scheme to remove from the file</param>
        /// <returns></returns>
        public static Result RemoveScheme(string identifier)
        {
            var scheme = GetDirectorySchemeOrDefault(identifier);
            if (scheme == null)
            {
                return Result.Failure($"No scheme '{identifier}'");
            }
            else
            {
                Schemes = Schemes.Where(s => !s.IsIdentifiableBy(identifier)).ToArray();
                return SaveData.WriteSchemesToDisk();
            }
        }

        /// <summary>
        /// Returns the <see cref="DirectoryScheme"/> corresponding to the given identifer (either Alias or Format String)
        /// or null if no such scheme is found.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static DirectoryScheme GetDirectorySchemeOrDefault(string schemeIdentifier)
        {
            return Schemes.FirstOrDefault(s => s.Name == schemeIdentifier || s.FormatString == schemeIdentifier);
        }
    }
}
