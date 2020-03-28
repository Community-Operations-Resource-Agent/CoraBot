using System;
using System.Globalization;

namespace Shared
{
    public static class DateTimeHelpers
    {
        static string[] hourFormats = { "htt", "h tt" };
        static string[] hourMinuteFormats = { "htt", "h tt", "h:mmtt", "h:mm tt" };

        /// <summary>
        /// Parses a datetime string to match only hours.
        /// </summary>
        public static bool ParseHour(string input, out DateTime dateTime)
        {
            return Parse(input, hourFormats, out dateTime);
        }

        /// <summary>
        /// Parses a datetime string to match only hours.
        /// </summary>
        public static bool ParseHourAndMinute(string input, out DateTime dateTime)
        {
            return Parse(input, hourMinuteFormats, out dateTime);
        }

        /// <summary>
        /// Parses a datetime string to match one of the provided formats.
        /// </summary>
        public static bool Parse(string input, string[] formats, out DateTime dateTime)
        {
            return DateTime.TryParseExact(input, formats,  CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
    }
}
