using System;
using System.Collections.Generic;
using System.Text;

using PhotoOrganizer.Core;
using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
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
