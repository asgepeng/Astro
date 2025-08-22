using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astro.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex EmailRegex = new Regex( @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static void Log(this string value, string? optionalPath = null)
        {
            string filePath = optionalPath ?? Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
            File.AppendAllText(filePath, DateTime.UtcNow + "\n" + value + "\n");
        }
        public static bool ToBoolean(this string value)
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("on", StringComparison.OrdinalIgnoreCase);
        }
        public static byte ToByte(this string value)
        {
            if (byte.TryParse(value, out byte byteValue))
            {
                return byteValue;
            }
            throw new FormatException($"The string '{value}' cannot be converted to a byte.");
        }
        public static byte[] ToInet(this string value)
        {
            var arr = value.Split('.');
            var bytes = new byte[4];
            if (arr.Length != 4) return bytes;

            for (int i = 0; i < 4; i++)
            {
                byte.TryParse(arr[i], out bytes[i]);
            }
            return bytes;
        }
        public static short ToInt16(this string value)
        {
            return short.TryParse(value, out short shortValue) ? shortValue : (short)0;
        }
        public static int ToInt32(this string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            throw new FormatException($"The string '{value}' cannot be converted to an integer.");
        }
        public static long ToInt64(this string value)
        {
            return long.TryParse(value, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint, 
                System.Globalization.CultureInfo.InvariantCulture, out long longValue) ? longValue : 0L;
        }
        public static string ToSqlVarchar(this string value) => "'" + value.EscapeSql() + "'";
        public static string ToSqlNVarchar(this string value) => "N'" + value.EscapeSql() + "'";
        public static string EscapeSql(this string value) => value.Replace("'", "''");
        public static bool IsValidEmailFormat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return EmailRegex.IsMatch(value);
        }
    }
}
