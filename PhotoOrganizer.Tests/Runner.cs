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
        /// Run the program
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string RunProgram(string[] args)
        {
            string result = string.Empty;
            using (StringWriter writer = new StringWriter())
            {
                Console.SetOut(writer);
                Program.Main(args);
                result = writer.ToString();
            }
            
            //assembly.GetType()

            return result;
        }
    }
}
