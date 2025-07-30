using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Cryptography
{
    public static class SimpleEncryption
    {
        public static string Encrypt(string text, byte[] key)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] ^= key[i % key.Length];
            return Convert.ToBase64String(buffer);
        }

        public static string Decrypt(string encoded, byte[] key)
        {
            var buffer = Convert.FromBase64String(encoded);
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] ^= key[i % key.Length];
            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
