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
        [Description("Verifies DirectoryRecord.IdentifiableBy() correctly identifies records by path")]
        public void DirectoryRecord_IdentifiableBy_MatchesPath()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");
            Assert.IsTrue(rec1.IsIdentifiableBy("/boycott/china"), "Failed to match Path");
        }

        [Test]
        [Description("Verifies DirectoryRecord.IdentifiableBy() correctly identifies records by alias")]
        public void DirectoryRecord_IdentifiableBy_MatchesAlias()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");
            Assert.IsTrue(rec1.IsIdentifiableBy("Truth"), "Failed to match alias");
        }

        [Test]
        [Description("Verifies that DirectoryRecord.IdentifiableBy() is case sensitive")]
        public void DirectoryRecord_IdentifiableBy_IsCaseSensitive()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");
            Assert.IsFalse(rec1.IsIdentifiableBy("truth"), "Accepted case-insensitive query, which is N.E.B.");
        }

        [Test]
        [Description("Verifies that DirectoryRecord.IdentifiableBy() requires a full match instead of partial match")]
        public void DirectoryRecord_IdentifiableBy_NoPartialMatch()
        {
            DirectoryRecord rec1 = new DirectoryRecord(DirectoryType.Source, "/boycott/china", "Truth");

            Assert.IsFalse(rec1.IsIdentifiableBy("china"), "Accepted a partial match, which is N.E.B.");
        }
    }
}