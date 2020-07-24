using NUnit.Framework;
using PhotoOrganizer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PhotoOrganizer.Tests
{
    public class CliTests_Execute
    {
        static string testDataPath = String.Empty;
        static DirectoryInfo targetDir = new DirectoryInfo("../../../Images/target/");
        static DirectoryInfo sourceDir = new DirectoryInfo("../../../Images/source/");

        [SetUp]
        public static void WriteTestData()
        {
            // Set path
            testDataPath = Path.Combine(Environment.CurrentDirectory, "test" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(testDataPath);

            DirectoryRecord d1 = new DirectoryRecord(DirectoryType.Target, targetDir.FullName, "local-art");
            DirectoryRecord d2 = new DirectoryRecord(DirectoryType.Source, sourceDir.FullName);
            DirectoryRecord d3 = new DirectoryRecord(DirectoryType.Target, "E:\\Backup\\Images", "big-disk");
            var list = new DirectoryRecord[] { d1, d2, d3 };

            DirectoryScheme s1 = new DirectoryScheme("{Y}\\{M}\\{II}", "year-month", "Nests images by month, by year");
            DirectoryScheme s2 = new DirectoryScheme("{Y}\\{MMM}\\{D}\\{E}\\{II}", "Overly organized", "Like no joke, this is way over the top");
            var schemes = new DirectoryScheme[] { s1, s2 };

            // Write files
            File.WriteAllText(Path.Combine(testDataPath, "directories"), JsonSerializer.Serialize<DirectoryRecord[]>(list));
            File.WriteAllText(Path.Combine(testDataPath, "schemes"), JsonSerializer.Serialize<DirectoryScheme[]>(schemes));

            var dir = new DirectoryInfo("../../../Images");

            // write images
            var set = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var file in set)
            {
                File.Copy(file.FullName, Path.Combine(dir.FullName, "source", file.Name));
            }

            // Update SaveData
            SaveData.DataDirectory = testDataPath;
            SaveData.ReadDirectoriesFile();
        }

        [OneTimeTearDown]
        public static void RemoveTestDataEvery()
        {
            foreach (var file in targetDir.GetFiles())
            {
                file.Delete();
            }
        }

        [TearDown]
        public static void RemoveTestData()
        {
            
            foreach (var file in sourceDir.GetFiles())
            {
                file.Delete();
            }
        }

        [Test]
        public static void ExecMove_Success_ReturnsEmptyString()
        {
            string result = Runner.RunProgram($"exec-move -s \"{sourceDir.FullName}\" -t local-art");
            Assert.That(result, Is.EqualTo(String.Empty));
        }

        [Test]
        public static void ExecMove_Success_MovesAllImages()
        {
            Dictionary<string, long> images = new Dictionary<string, long>();
            foreach (var img in sourceDir.GetFiles())
            {
                images.Add(img.Name, img.Length);
            }
            
            Runner.RunProgram($"exec-move -s \"{sourceDir.FullName}\" -t local-art");
            var sourceSet = sourceDir.GetFiles();
            Assert.That(sourceSet.Length, Is.Zero, $"Expected no images in sourceDir, got {sourceSet.Length}");

            var targetSet = targetDir.GetFiles();
            if (targetSet.Length != images.Keys.Count)
                Assert.Fail("Wrong number of source images versus target images: {0} v {1}", sourceSet.Length, targetSet.Length);
            foreach (var img in targetSet)
            {
                if (!images.ContainsKey(img.Name))
                    Assert.Fail("Encountered unexpected image {0}", img.FullName);
            }
        }
    }
}
