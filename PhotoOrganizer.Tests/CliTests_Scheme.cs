﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using NUnit.Framework;

using PhotoOrganizer.Core;
using System.Linq;

namespace PhotoOrganizer.Tests
{
    class CliTests_Scheme
    {
        static string testDataPath = String.Empty;

        static DirectoryScheme[] TestSchemes;

        [SetUp]
        public static void WriteTestData()
        {
            // Set path
            testDataPath = Path.Combine(Environment.CurrentDirectory, "test" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(testDataPath);

            DirectoryScheme d1 = new DirectoryScheme("{Y}\\{M}\\{II}", "year-month", "Nests images by month, by year");
            DirectoryScheme d2 = new DirectoryScheme("{Y}\\{MMM}\\{D}\\{E}\\{II}", "Overly organized", "Like no joke, this is way over the top");
            var list = new DirectoryScheme[] { d1, d2 };

            // Write files
            File.WriteAllText(Path.Combine(testDataPath, SaveData.SchemesFileName), JsonSerializer.Serialize<DirectoryScheme[]>(list));

            // Update SaveData
            SaveData.DataDirectory = testDataPath;
            SaveData.Schemes = SaveData.LoadSchemesFromDisk();
            TestSchemes = list;
        }

        [TearDown]
        public static void RemoveTestData()
        {
            // Remove dir and files
            try
            {
                File.Delete(Path.Combine(testDataPath, SaveData.DirectoriesFileName));
            }
            catch (IOException)
            {
                System.Threading.Thread.Sleep(200);
                File.Delete(Path.Combine(testDataPath, SaveData.DirectoriesFileName));
            }
        }

        [Test]
        [Description("Adds schemes and sees if they are correctly consumed")]
        public void Add_ConsumesArgs_Desc()
        {
            const string format = "{YYYY}\\{MM}\\{II}", alias = "year-month2", description = "Nests";
            string arg_new = $"scheme-add -f {format} -a {alias} -d {description}";
            string result = Runner.RunProgram(arg_new);
            Assert.AreEqual(String.Empty, result, "Adding a scheme triggered program output unexpectedly");

            var newScheme = SaveData.Schemes.FirstOrDefault(s => s.FormatString == format &&
                s.Description == description && s.Name == alias);
            Assert.That(newScheme, Is.Not.Null, "Couldn't find the scheme added in SaveData");
        }

        [Test]
        [Description("Adds a scheme with no description to see if they are correctly consumed")]
        public void Add_ConsumeArgs_NoDesc()
        {
            const string format = "{E}\\{MMMM}", alias = "Senseless";
            string arg_new_descless = $"scheme-add -f {format} -a {alias}";
            string result = Runner.RunProgram(arg_new_descless);
            Assert.AreEqual(String.Empty, result, "Adding a scheme triggered program output unexpectedly");

            var newScheme = SaveData.Schemes.FirstOrDefault(s => s.FormatString == format && s.Name == alias);
            Assert.That(newScheme, Is.Not.Null, "Couldn't find the scheme added in SaveData");
        }

        [Test]
        [Description("Verifies all the schemes on-disk are written to console")]
        public void List_DisplaysSchemes()
        {
            string result = Runner.RunProgram("scheme-list");

            Assert.That(result, Contains.Substring("nests images by month, by year"));
            Assert.That(result, Contains.Substring("like no joke, this is way over the top"));
            Assert.That(result.Contains("failed to parse"), Is.False);
        }

        [Test]
        [Description("Verifies that invoking the remove command actually removes the scheme.")]
        public void Remove_RemovesScheme()
        {
            string result = Runner.RunProgram("scheme-list");
            Assert.That(result, Contains.Substring("year-month"));

            result = Runner.RunProgram("scheme-remove -a year-month");
            Assert.That(result.Contains("year-month"), Is.False);

        }
    }
}
