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
                if (data.Length < 16) continue;
                var employeeId = BitConverter.ToInt16(data, 14);
                if (employeeId == userId)
                {
                    TokenStore.Delete(kv.Key);
                }
            }
        }       
        internal static short GetLocationId(string key)
        {
            var data = TokenStore.Get(key);
            if (data is null) return -1;

            return BitConverter.ToInt16(data, 12);
        }
        internal static void SetLocationId(string key, short locationId)
        {
            var data = TokenStore.Get(key);
            if (data is null) return;

            var bytes = BitConverter.GetBytes(locationId);
            data[12] = bytes[0];
            data[13] = bytes[1];
            TokenStore.Update(key, data);
        }
    }
}
