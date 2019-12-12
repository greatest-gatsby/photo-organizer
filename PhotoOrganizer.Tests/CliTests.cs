using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
    public class CliTests
    {
        #region LIST
        [Test]
        [Description("Invokes the 'list' CL-arg with an invalid directory type")]
        public static void List_InvalidType_PrintsError()
        {
            const string arg_plural = "list sources";
            const string arg_wrong = "list undertaker";

            string result = Runner.RunProgram(arg_plural);
            Assert.AreEqual("unrecognized type sources", result.ToLower(), "Program accepted the plural of a valid type");

            result = Runner.RunProgram(arg_wrong);
            Assert.AreEqual("unrecognized type undertaker", result.ToLower(), "Program accepted an invalid type");
        }

        [Test]
        [Description("Invokes the 'list' CL-arg with a valid directory type")]
        public static void List_ValidType_PrintsEntries()
        {
            const string arg_source = "list source";
            const string arg_target = "list target";
            const string arg_base = "list";

            string result = Runner.RunProgram(arg_source);
            Assert.AreNotEqual("unrecognized type source", result);

            result = Runner.RunProgram(arg_target);
            Assert.AreNotEqual("unrecognized type target", result);

            result = Runner.RunProgram(arg_base);
            Assert.IsFalse(result.Contains("unrecognized type")); // super bad test theoretically but practically its fine, maybe
        }

        #endregion
    }
}
