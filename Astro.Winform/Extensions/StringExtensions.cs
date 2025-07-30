using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Extensions
{
    internal static class StringExtensions
    {
        internal static string Join(this IEnumerable<string> values, string separator)
        {
            if (values is null || !values.Any()) return string.Empty;
            return string.Join(separator, values);
        }
    }
}
