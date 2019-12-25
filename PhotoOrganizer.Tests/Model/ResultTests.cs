using System;
using System.Collections.Generic;
using System.Text;

using PhotoOrganizer.Core;
using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
    public class ResultTests
    {
        public void Result_Failure_PassesMessage()
        {
            const string msg = "eat ur veggies";
            var result = Result.Failure(msg);
            Assert.That(result.Successful, Is.False);
            Assert.That(result.Message, Is.EqualTo(msg));
        }
    }
}
