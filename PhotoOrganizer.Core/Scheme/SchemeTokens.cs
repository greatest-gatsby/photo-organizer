using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoOrganizer.Core
{
    /// <summary>
    /// Contains all the tokens recognized by the program.
    /// This is a subset of the strings recognized by the dotnetcore's custom datetime library
    /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
    /// </summary>
    public static class SchemeTokens
    {
        public const string DayName = "dddd";
        public const string DayNameAbbreviation = "ddd";
        public const string DayNumeral = "d";
        public const string DayNumeralLeadingZero = "dd";

        public const string Hour12h = "h";
        public const string Hour12hLeadingZero = "hh";
        public const string Hour24h = "H";
        public const string Hour24hLeadingZero = "HH";

        public const string Minute = "m";
        public const string MinuteLeadingZero = "mm";

        public const string MonthName = "MMMM";
        public const string MonthNameAbbreviation = "MMM";
        public const string MonthNumeral = "M";
        public const string MonthNumeralLeadingZero = "MM";

        public const string Year2Digit = "y";
        public const string Year2DigitLeadingZero = "yy";
        public const string Year3DigitLeadingZero = "yyy";
        public const string Year4DigitLeadingZero = "yyyy";

        static SchemeTokens()
        {
            Array.Sort(All);
        }

        public static string[] All { get; } = new string[]
        {
            DayName, DayNameAbbreviation, DayNumeral, DayNumeralLeadingZero,
            Hour12h, Hour12hLeadingZero, Hour24h, Hour24hLeadingZero,
            Minute, MinuteLeadingZero, MonthName, MonthNameAbbreviation,
            MonthNumeral, MonthNumeralLeadingZero, Year2Digit, Year2DigitLeadingZero,
            Year3DigitLeadingZero, Year4DigitLeadingZero
        };

        public static string[] Stock { get; } = new string[]
        {
            DayName, DayNameAbbreviation, DayNumeral, DayNumeralLeadingZero,
            Hour12h, Hour12hLeadingZero, Hour24h, Hour24hLeadingZero,
            Minute, MinuteLeadingZero, MonthName, MonthNameAbbreviation,
            MonthNumeral, MonthNumeralLeadingZero, Year2Digit, Year2DigitLeadingZero,
            Year3DigitLeadingZero, Year4DigitLeadingZero
        };
    }
}
