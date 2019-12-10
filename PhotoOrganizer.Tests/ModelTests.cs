using System.IO;
using NUnit.Framework;
using PhotoOrganizer;

using PhotoOrganizer.Core;

namespace PhotoOrganizer.Tests
{
    public class DirectoryRecordTests
    {
        public string Directory { get; set; }

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
        public void DirectoryRecord_Parses_FromString_3()
        {
            string data = "source\t" + Directory + "\t temp1";
            
            DirectoryRecord record = DirectoryRecord.Parse(data);
            
            // Test only the variables assigned from parsing, not from additional logic
            Assert.AreEqual(Directory, record.Path);
            Assert.AreEqual("temp1", record.Alias);
            Assert.AreEqual(DirectoryType.Source, record.Type);
        }

        [Test]
        [Description("Creates a directory record from parsing a prepared string, then verifies the record matches the known data.")]
        public void DirectoryRecord_Parses_FromString_2()
        {
            string data = "source\t" + Directory;

            DirectoryRecord record = DirectoryRecord.Parse(data);

            //Test only the variables assigned from parsing, not from additional logic
            Assert.AreEqual(Directory, record.Path);
            Assert.AreEqual(null, record.Alias);
            Assert.AreEqual(DirectoryType.Source, record.Type);
        }

        public void DirectoryType_Parses_FromString()
        {
            string src = "source";
            
            
        }
    }
}