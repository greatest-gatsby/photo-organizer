using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
    public class Scheme
    { 
        [Test]
        public void Scheme_Parses_FromString()
        {
            string parse = @"%Y\%M\%II";
            throw new NotImplementedException();
        }

        [Test]
        [Description("Verifies that Scheme.Parse() correctly matches expr-start character ('%') with terminating character ('#')")]
        public void Scheme_Parses_MatchesTermCharacter()
        {
            throw new NotImplementedException();
        }
    }
}
