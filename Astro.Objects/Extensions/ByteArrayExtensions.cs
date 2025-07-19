using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            var sb = new StringBuilder();
            if (bytes.Length == 0)
            {
                sb.Append("[]");
            }
            else
            {
                sb.Append("0x");
            }
            foreach (var b in bytes )
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
