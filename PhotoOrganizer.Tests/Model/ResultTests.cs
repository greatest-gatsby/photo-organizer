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
        [Description("Verifies that data given to the Result<T>.Failure() method are filled in the returned object.")]
        public void ResultT_Failure_FillsData()
        {
            var res = Result<int>.Failure("This is the data", 722);
            Assert.That(res.Data, Is.EqualTo(722), "The data was wrongly altered");
            Assert.IsTrue(res.Data is int, "Data type was not preserved");
        }

        [Test]
        [Description("Verifies that data given to the Result<T>.Failure() method are filled in the returned object.")]
        public void ResultT_Failure_PreservesMessage()
        {
            var res = Result<int>.Failure("Something went wrong but this is what we had before the crash", 722);
            Assert.That(res.Message, Is.EqualTo("Something went wrong but this is what we had before the crash"),
                "The message was wrongly altered");
        }

        [Test]
        [Description("Verifies that Result<T>.Failure() returns a Result indicating FAILURE.")]
        public void ResultT_Failure_IndicatesFailure()
        {
            var res = Result<int>.Failure("You had a failure");
            Assert.That(res.Successful, Is.False, "Result was wrongly marked as successful");
        }

        [Test]
        [Description("Verifies that strings given to the Result<T>.Success() method are formatted.")]
        public void ResultT_Success_FillsData()
        {
            var res = Result<string>.Success("the fancy data object goes here");
            Assert.That(res.Data, Is.EqualTo("the fancy data object goes here"));
        }

        [Test]
        [Description("Verifiies that Result<T>.Success() returns a Result indicating SUCCESS.")]
        public void ResultT_Success_IndicatesSuccess()
        {
            var res = Result<string>.Success();
            Assert.That(res.Successful, Is.True, "Result was wrongly marked as unsuccessful");
            res = null;

            res = Result<string>.Success("this is the data item");
            Assert.That(res.Successful, Is.True, "Result was wrongly marked as unsuccessful");
        }

        [Test]
        [Description("Verifies that Result<T>.Failure() returns a Result indicating FAILURE.")]
        public void Result_Failure_PreservesMessage()
        {
            var res = Result.Failure("You had a failure");
            Assert.That(res.Message, Is.EqualTo("You had a failure"), "The message was wrongly altered");
        }

        [Test]
        [Description("Verifies that Result<T>.Failure() returns a Result indicating FAILURE.")]
        public void Result_Failure_IndicatesFailure()
        {
            var res = Result.Failure("You had a failure");
            Assert.That(res.Successful, Is.False, "Result was wrongly marked as successful");
        }

        [Test]
        [Description("Verifiies that Result<T>.Success() returns a Result indicating SUCCESS.")]
        public void Result_Success_IndicatesSuccess()
        {
            var res = Result<string>.Success();
            Assert.That(res.Successful, Is.True, "Result was wrongly marked as unsuccessful");
            res = null;

            res = Result<string>.Success("this is the data item");
            Assert.That(res.Successful, Is.True, "Result was wrongly marked as unsuccessful");
        }
    }
}
