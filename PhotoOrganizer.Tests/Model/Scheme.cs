using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using PhotoOrganizer.Core;

namespace PhotoOrganizer.Tests
{
    public class Scheme
    { 
        [Test]
        public void Scheme_Parses_FromString()
        {
            string parse = "%Y\\%M\\%II#\tyear-month\tNests images by month, by year";
            DirectoryScheme scheme = DirectoryScheme.Parse(parse);

            Assert.That(scheme.FormatString, Is.EqualTo("%Y\\%M\\%II#"));
            Assert.That(scheme.Description, Is.EqualTo("Nests images by month, by year"));
            Assert.That(scheme.Name, Is.EqualTo("year-month"));
        }

        [Test]
        public void Scheme_Parses_FromStringArray()
        {
            string[] parse = { "%Y\\%M\\%II#" , "year-month", "Nests images by month, by year" };
            DirectoryScheme scheme = DirectoryScheme.Parse(parse);

            Assert.That(scheme.FormatString, Is.EqualTo("%Y\\%M\\%II#"));
            Assert.That(scheme.Description, Is.EqualTo("Nests images by month, by year"));
            Assert.That(scheme.Name, Is.EqualTo("year-month"));
        }

        
        [Description("Verifies that Scheme.Parse() correctly matches expr-start character ('%') with terminating character ('#')")]
        public void Scheme_Parses_MatchesTermCharacter()
        {
            throw new NotImplementedException();
        }
    }
}
