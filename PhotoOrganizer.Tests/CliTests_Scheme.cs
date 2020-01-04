using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using PhotoOrganizer.Core;

namespace PhotoOrganizer.Tests
{
    class CliTests_Scheme
    {
        static string testDataPath = String.Empty;

        public static string Schemes = "%Y\\%M\\%II#\tyear-month\tNests images by month, by year" + Environment.NewLine
            + "%Y\\%MMM#\\%D\\%E\\$II#\tOverly organized\tLike no joke, this is way over the top" + Environment.NewLine;

        [SetUp]
        public static void WriteTestData()
        {
            // Set path
            testDataPath = Path.Combine(Environment.CurrentDirectory, "test" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(testDataPath);

            // Write files
            File.WriteAllText(Path.Combine(testDataPath, SaveData.SchemesFileName), Schemes);

            // Update SaveData
            SaveData.DataDirectory = testDataPath;

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
        [Description("")]
        public void Add_ConsumesArgs()
        {
            string arg_new = "scheme add \"%YYYY#\\%MM#\\%II#\"\t\"year-month\"\t\"Nests images by month, by year\"";
            string result = Runner.RunProgram(arg_new);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("scheme list");
            Assert.That(result.EndsWith("Nests images by month, by year", StringComparison.OrdinalIgnoreCase));

            const string arg_new_descless = "scheme add %E\\%MMMM#\tSenseless scheme";
            result = Runner.RunProgram(arg_new_descless);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("list");
            Assert.That(result.EndsWith("Senseless scheme"));
        }
    }
}
