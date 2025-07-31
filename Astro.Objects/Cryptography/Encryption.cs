using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Astro.Extensions;

namespace Astro.Cryptography
{
    public enum EncryptionType
    {
        Simple = 1,
        AES128 = 2,
        AES192 = 3,
        AES256 = 4,
    }
    public static class Encryption
    {
        public static string Encrypt(this string text, byte[] key, EncryptionType encryptionType = EncryptionType.Simple)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            byte[] aesKey;
            byte[] aesIV;

            switch (encryptionType)
            {
                case EncryptionType.AES128:
                    aesKey = key.SubBytes(0, 16);
                    aesIV = key.SubBytes(16, 16);
                    break;
                case EncryptionType.AES192:
                    aesKey = key.SubBytes(0, 24);
                    aesIV = key.SubBytes(24, 16);
                    break;
                case EncryptionType.AES256:
                    aesKey = key.SubBytes(0, 32);
                    aesIV = key.SubBytes(32, 16);
                    break;
                default:
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] ^= key[i % key.Length];
                    return Convert.ToBase64String(buffer);
            }

            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = aesIV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            byte[] encryptedBytes = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Convert.ToBase64String(encryptedBytes);
        }
        public static string Decrypt(this string base64Text, byte[] key, EncryptionType encryptionType = EncryptionType.Simple)
        {
            byte[] buffer = Convert.FromBase64String(base64Text);
            byte[] aesKey;
            byte[] aesIV;

            switch (encryptionType)
            {
                case EncryptionType.AES128:
                    aesKey = key.SubBytes(0, 16);
                    aesIV = key.SubBytes(16, 16);
                    break;
                case EncryptionType.AES192:
                    aesKey = key.SubBytes(0, 24);
                    aesIV = key.SubBytes(24, 16);
                    break;
                case EncryptionType.AES256:
                    aesKey = key.SubBytes(0, 32);
                    aesIV = key.SubBytes(32, 16);
                    break;
                default:
                    for (int i = 0; i < buffer.Length; i++)
                        buffer[i] ^= key[i % key.Length];
                    return Encoding.UTF8.GetString(buffer);
            }

            using var aes = Aes.Create();
            aes.Key = aesKey;
            aes.IV = aesIV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
