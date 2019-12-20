using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace PhotoOrganizer.Tests
{
    public class Runner
    {
        /// <summary>
        /// Run the program with the given array of command line arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string RunProgram(string[] args, bool trim = true, bool lowercase = true)
        {
            string result = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Program.Main(args);
                result = writer.ToString();
            }

            // Process cleaning options
            if (trim)
            {
                result = result.Trim('\r', '\n', ' ');
            }


            if (lowercase)
                result = result.ToLower();

            return result;
        }

        /// <summary>
        /// Run the program with the given command line argument string
        /// </summary>
        /// <param name="arg">String of arguments to pass to program</param>
        /// <returns></returns>
        public static string RunProgram(string arg, bool trim = true, bool lowercase = true)
        {
            string result = String.Empty;

            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Program.Main(arg.Split(' '));
                result = writer.ToString();
            }

            // Process cleaning options
            if (trim)
            {
                result = result.Trim('\r', '\n', ' ');
            }


            if (lowercase)
                result = result.ToLower();

            return result;
        }


    }
}
