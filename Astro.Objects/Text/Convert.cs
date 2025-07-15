using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Astro.Text
{
    public static class Convert
    {
        private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static int ToInt32(string value, CultureInfo culterInfo = null)
        {
            if (culterInfo is null) culterInfo = new CultureInfo("id-ID");
            if (int.TryParse(value, NumberStyles.Integer, culterInfo, out int result))
            {
                return result;
            }
            return 0;
        }
        public static double ToDouble(string value, CultureInfo culterInfo = null)
        {
            if (culterInfo is null) culterInfo = new CultureInfo("id-ID");
            if (double.TryParse(value, NumberStyles.Float, culterInfo, out double result))
            {
                return result;
            }
            return 0.0;
        }
        public static string ToDecimalFormat(this long value)
        {
            return value.ToString("N0", new CultureInfo("id-ID"));
        }
        public static long ToInt64(this string value, CultureInfo culterInfo = null)
        {
            if (culterInfo is null) culterInfo = new CultureInfo("id-ID");
            if (long.TryParse(value, NumberStyles.AllowThousands, culterInfo, out long result))
            {
                return result;
            }
            return 0;
        }
        public static bool IsValidEmailFormat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return EmailRegex.IsMatch(value);
        }
    }
}
