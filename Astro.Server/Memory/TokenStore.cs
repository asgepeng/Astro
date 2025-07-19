using Astro.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Memory
{
    internal static class TokenStore
    {
        private static ConcurrentDictionary<string, byte[]> dict = new();
        internal static void Set(string token, byte[] value)
        {
            dict[token] = value;
        }
        internal static byte[]? Get(string token)
        {
            return dict.TryGetValue(token, out var value) ? value : null;
        }
        internal static void Delete(string token)
        {
            dict.TryRemove(token, out _);
        }
        internal static void Update(string token, byte[] value)
        {
            dict[token] = value;
        }
        internal static void Delete(short userId)
        {
            foreach (var kv in dict)
            {
                var data = kv.Value;
                if (data.Length < 2)
                    continue;

                using var stream = new MemoryStream(data);
                using var reader = new BinaryReader(stream);
                stream.Position = data.Length - 2;
                var uid = reader.ReadInt16();

                if (uid == userId)
                {
                    dict.TryRemove(kv.Key, out _);
                }
            }
        }
        internal static string ToView()
        {
            var sb = new StringBuilder();
            sb.Append("<table><thead><tr><th>Token</th><th>Value</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var kv in dict)
            {
                sb.Append("<tr>");
                sb.Append("<td>").Append(kv.Key).AppendLine("</td>");
                sb.Append("<td>").Append(kv.Value.ToHexString()).Append("</td>");
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }
    }
}
