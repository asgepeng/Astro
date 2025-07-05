using System.Security.Cryptography;
using System.Text;
namespace Astro.Security
{
    public class Encryption
    {
        public static byte[] HashBytes(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return bytes;
            }
        }
    }

    public static class Password
    {
        public static (byte[] passwordHash, byte[] salt) Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be empty.", nameof(input));

            using (SHA256 sha256 = SHA256.Create())
            {
                // Generate salt
                var salt = Guid.NewGuid().ToByteArray(); //16 bytes
                                                         // Combine input + salt
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] combined = new byte[inputBytes.Length + salt.Length];
                Buffer.BlockCopy(inputBytes, 0, combined, 0, inputBytes.Length);
                Buffer.BlockCopy(salt, 0, combined, inputBytes.Length, salt.Length);
                
                return (combined, salt);
            }
        }

        public static bool Match(string input, byte[] storedHash, byte[] salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] combined = new byte[inputBytes.Length + salt.Length];
                Buffer.BlockCopy(inputBytes, 0, combined, 0, inputBytes.Length);
                Buffer.BlockCopy(salt, 0, combined, inputBytes.Length, salt.Length);
                Print(storedHash, combined);
                return combined.SequenceEqual(storedHash);
            }
        }
        public static void Print(byte[] p1, byte[] p2)
        {
            var builder = new StringBuilder();
            foreach (var b in p1)
            {
                builder.Append(b.ToString("X2"));
            }
            builder.AppendLine("");
            foreach (var b in p2)
            {
                builder.Append(b.ToString("X2"));
            }
            File.WriteAllText("E:\\combined.txt", builder.ToString());
        }
    }

}
