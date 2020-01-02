using System;
using System.IO;
using NUnit.Framework;
using PhotoOrganizer;

using PhotoOrganizer.Core;

namespace PhotoOrganizer.Tests
{
    public class DirectoryRecordTests
    {
        /// <summary>
        /// Path to the directory of the tempfile in use for this test run
        /// </summary>
        public string Directory { get; set; }
        
        /// <summary>
        /// Path to the temp file used for this test run
        /// </summary>
        public string FilePath { get; set; }

        [SetUp]
        public void Setup()
        {
            // Place directories on the system for testing
            FilePath = Path.GetTempFileName();
            Directory = new FileInfo(FilePath).DirectoryName;
        }

        [Test]
        [Description("Creates a directory record from parsing a prepared string, then verifies the record matches the known data.")]
        public void DirectoryRecord_Parses_FromString_3Args()
        {
            string data = "source\t" + Directory + "\t temp1";

            DirectoryRecord record = DirectoryRecord.Parse(data);
            
            // Test only the variables assigned from parsing, not from additional logic
            Assert.AreEqual(Directory, record.Path);
            Assert.AreEqual("temp1", record.Alias);
            Assert.AreEqual(DirectoryType.Source, record.Type);

            // Try with spaces in names, properly quoted
            const string spaces = "source\t/home/your it guy didnt think of this name/or path\ttruth hurts";
            record = DirectoryRecord.Parse(spaces);

            Assert.AreEqual("/home/your it guy didnt think of this name/or path", record.Path);
            Assert.AreEqual("truth hurts", record.Alias);
            Assert.AreEqual(DirectoryType.Source, record.Type);
        }

        [Test]
        [Description("Creates a directory record from parsing a prepared string, then verifies the record matches the known data.")]
        public void DirectoryRecord_Parses_FromString_2Args()
        {
            string data = "source\t" + Directory;

            DirectoryRecord record = DirectoryRecord.Parse(data);

            // Test only the variables assigned from parsing, not from additional logic
            Assert.AreEqual(Directory, record.Path);
            Assert.AreEqual(null, record.Alias);
            Assert.AreEqual(DirectoryType.Source, record.Type);

            // Try with spaces in names, properly quoted
            const string spaces = "target\t/mnt/every moment ever/i phone\tsubscribe 2 pewdiepie";
            record = DirectoryRecord.Parse(spaces);

            Assert.AreEqual("/mnt/every moment ever/i phone", record.Path);
            Assert.AreEqual("subscribe 2 pewdiepie", record.Alias);
            Assert.AreEqual(DirectoryType.Target, record.Type);
        }

        [Test]
        [Description("Attempts to create a directory record from parsing a prepared, invalid string, then verifies the parse failed")]
        public void DirectoryType_ParseFails_FromString()
        {
            string src = "source";
            Assert.Throws<FormatException>(() => DirectoryRecord.Parse(src));

            src = "target";
            Assert.Throws<FormatException>(() => DirectoryRecord.Parse(src));

            src = "source C:\\Windows altname bad_arg";
            Assert.Throws<FormatException>(() => DirectoryRecord.Parse(src));
        }

        [Test]
        [Description("Attempts to parse an invalid directory record, verifying that TryParse does not throw")]
        public void DirectoryType_TryParse_DoesntThrowOnInvalid()
        {
            string bad = "sauce C:\\Windows lol getoff";
            DirectoryRecord rec = null;
            bool? result = null;

            Assert.DoesNotThrow(() => { result = DirectoryRecord.TryParse(bad, out rec); });
            Assert.IsNotNull(result, "TryParse did not return a boolean value");
            Assert.IsFalse(result.Value, "TryParse accepted an invalid string");
        }

        [Test]
        [Description("Verifies DirectoryRecord.IdentifiableBy() correctly identifies records by path and alias")]
        public void DirectoryRecord_IdentifiableBy_Matches()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");
            Assert.IsTrue(rec1.IdentifiableBy("Truth"), "Failed to match Alias");
            Assert.IsTrue(rec1.IdentifiableBy("/boycott/china"), "Failed to match Path");
            Assert.IsFalse(rec1.IdentifiableBy("truth"), "Accepted case-insensitive query, which is N.E.B.");
            Assert.IsFalse(rec1.IdentifiableBy("china"), "Accepted a partial match, which is N.E.B.");
        }
    }

    public class UtilityTests
    {
        [Test]
        [Description("Verifies that strings given to the Result.Failure() method are formatted")]
        public void Result_Failure_FormatsArgs()
        {
            var res = Result.Failure("This is the base {0} l", "message");
            Assert.That(res.Message, Is.EqualTo("This is the base message l"));
        }

        [Test]
        [Description("Verifies that data given to the Result.Failure() method are filled in the returned object")]
        public void Result_Failure_FillsData()
        {
            var res = Result.Failure("This is the data", data: 722);
            Assert.That(res.Data, Is.EqualTo(722));
        }

        [Test]
        [Description("Verifies that strings given to the Result.Success() method are formatted")]
        public void Result_Success_FillsData()
        {
            var res = Result.Success("the fancy data object goes here");
            Assert.That(res.Data, Is.EqualTo("the fancy data object goes here"));
        }
    }
}