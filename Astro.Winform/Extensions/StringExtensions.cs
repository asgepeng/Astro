using Svg;
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
        internal static Image ToImage(this string value)
        {
            SvgDocument doc;
            using (var str = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(value)))
            {
                doc = SvgDocument.Open<SvgDocument>(str);
            }
            return doc.Draw(20, 20);
        }
    }
}
