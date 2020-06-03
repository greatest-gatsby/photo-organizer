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
        [Description("Adds schemes and sees if they are correctly consumed")]
        public void Add_ConsumesArgs()
        {
            string arg_new = @"scheme-add -f %YYYY#\%MM#\%II# -a year-month2    -d Nests";
            string result = Runner.RunProgram(arg_new);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("scheme-list");
            Assert.That(result, Contains.Substring("nests"));
            Assert.That(result, Contains.Substring("like no joke, this is way over the top"));

            const string arg_new_descless = @"scheme-add -f %E\%MMMM# -a Senseless";
            result = Runner.RunProgram(arg_new_descless);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("scheme-list");
            Assert.That(result, Contains.Substring("senseless"));
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
        public void Remove_RemovesScheme()
        {
            string result = Runner.RunProgram("scheme-list");
            Assert.That(result, Contains.Substring("year-month"));

            result = Runner.RunProgram("scheme-remove -a year-month");
            Assert.That(result.Contains("year-month"), Is.False);

        }
    }
}
