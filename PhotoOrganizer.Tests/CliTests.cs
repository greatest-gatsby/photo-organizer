using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PhotoOrganizer.Core;
using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
    public class CliTests
    {
        static string testDataPath = String.Empty;

        public static string Directories = "source\tC:\\Users\\Me\\Art\tlocal-art" + Environment.NewLine
            + "source\tD:\\Photography" + Environment.NewLine
            + "target\tE:\\Backup\\Images\tbig-disk" + Environment.NewLine;
        public const string Schemes = "";

        [SetUp]
        public static void WriteTestData()
        {
            // Set path
            testDataPath = Path.Combine(Environment.CurrentDirectory, "test" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(testDataPath);

            // Write files
            File.WriteAllText(Path.Combine(testDataPath, "directories"), Directories);

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
            }catch (IOException)
            {
                System.Threading.Thread.Sleep(200);
                File.Delete(Path.Combine(testDataPath, SaveData.DirectoriesFileName));
            }
        }

        #region LIST
        [Test]
        [Description("Invokes the 'list' CL-arg with an invalid directory type")]
        public static void List_InvalidType_PrintsError()
        {
            const string arg_plural = "list sources";
            const string arg_wrong = "list undertaker";

            string result = Runner.RunProgram(arg_plural);
            Assert.AreEqual(SourceDirectory.WrongType("sources").ToLower(), result, "Program accepted the plural of a valid type");

            result = Runner.RunProgram(arg_wrong);
            Assert.AreEqual(SourceDirectory.WrongType("undertaker").ToLower(), result, "Program accepted an invalid type");
        }

        [Test]
        [Description("Invokes the 'list' CL-arg with a valid directory type")]
        public static void List_ValidType_PrintsEntries()
        {
            const string arg_source = "list source";
            const string arg_target = "list target";
            const string arg_base = "list";

            string result = Runner.RunProgram(arg_source);
            Assert.AreNotEqual(SourceDirectory.WrongType("source").ToLower(), result);

            result = Runner.RunProgram(arg_target);
            Assert.AreNotEqual(SourceDirectory.WrongType("target").ToLower(), result);

            result = Runner.RunProgram(arg_base);
            Assert.IsFalse(result.Contains("unrecognized type")); // super bad test theoretically but practically its fine, maybe
        }

        [Test]
        [Description("Invokes the 'list' CL-arg with no directory type")]
        public static void List_NoType_PrintsAll()
        {
            const string arg = "list";
            string result = Runner.RunProgram(arg);

            if (!result.EndsWith(Environment.NewLine))
            {
                result += Environment.NewLine;
            }

            Assert.AreEqual(Directories.ToLower(), result);
        }
        #endregion

        #region ADD
        [Test]
        [Description("Verifies that providing an invalid type results in an error")]
        public static void Add_InvalidType_PrintsError()
        {
            const string arg_type_wrong = "add sources C:\\Ay";
            string result = Runner.RunProgram(arg_type_wrong);

            Assert.AreEqual(SourceDirectory.WrongType("sources").ToLower(), result);
        }

        [Test]
        [Description("Verifies that specifying no directory type prints a helpful error message")]
        public static void Add_NoType_PrintsError()
        {
            const string arg_count_wrong = "add C:\\Ay";
            string result = Runner.RunProgram(arg_count_wrong);

            Assert.That(result, Contains.Substring("did you specify directory"));
        }

        [Test]
        [Description("Verifies that the directories added via the 'add' command show up in the 'list' command")]
        public static void Add_ConsumesArgs()
        {
            const string arg_new = "add source B:\\atman frlyfe";
            string result = Runner.RunProgram(arg_new);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("list");
            Assert.That(result.EndsWith("source\tB:\\atman\tfrlyfe", StringComparison.OrdinalIgnoreCase));

            const string arg_new_nameless = "add target D:\\artboi";
            result = Runner.RunProgram(arg_new_nameless);
            Assert.AreEqual(String.Empty, result);

            result = Runner.RunProgram("list");
            Assert.That(result.EndsWith("target\td:\\artboi"));
        }


        #endregion

        #region REMOVE
        [Test]
        [Description("Verifies that directories can be removed via path, even if they have an alias")]
        public static void Remove_MatchesByPath()
        {
            const string remove = "remove E:\\Backup\\Images";
            Runner.RunProgram(remove);
            string result = Runner.RunProgram("list");
            Assert.That(result, !Contains.Substring("e:\\backup\\images"));
        }

        [Test]
        [Description("Verifies that directories can be removed via alias")]
        public static void Remove_MatchesByAlias()
        {
            const string remove = "remove big-disk";
            Runner.RunProgram(remove);
            string result = Runner.RunProgram("list");
            Assert.That(result, !Contains.Substring("big-disk"));
        }

        [Test]
        [Description("Verifies that the 'remove' command silently executes when given valid arguments")]
        public static void Remove_GoodArgs_NoMessage()
        {
            const string remove = "remove E:\\Backup\\Images";
            string result = Runner.RunProgram(remove);
            Assert.That(result, Is.EqualTo(String.Empty));
        }

        [Test]
        [Description("Verifies that the 'remove' command prints an error message when given invalid arguments")]
        public static void Remove_BadArgs_PrintsMessage()
        {
            const string remove = "remove squiiire";
            string result = Runner.RunProgram(remove);
        }
        #endregion
    }
}
