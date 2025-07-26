using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Extensions
{
    internal static class StringExtensions
    {
        internal static string SqlString(this string value)
        {
            return value.Replace("'", "''");
        }
        internal static int ToInt32(this string value, int defaultValue = 0)
        {
            return int.TryParse(value, out int outValue) ? outValue : defaultValue;
        }
    }
}
