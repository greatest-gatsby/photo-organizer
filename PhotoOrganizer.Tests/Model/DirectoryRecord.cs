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
        [Description("Verifies DirectoryRecord.IdentifiableBy() correctly identifies records by path and alias")]
        public void DirectoryRecord_IdentifiableBy_Matches()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");
            Assert.IsTrue(rec1.IsIdentifiableBy("Truth"), "Failed to match Alias");
            Assert.IsTrue(rec1.IsIdentifiableBy("/boycott/china"), "Failed to match Path");
            Assert.IsFalse(rec1.IsIdentifiableBy("truth"), "Accepted case-insensitive query, which is N.E.B.");
            Assert.IsFalse(rec1.IsIdentifiableBy("china"), "Accepted a partial match, which is N.E.B.");
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