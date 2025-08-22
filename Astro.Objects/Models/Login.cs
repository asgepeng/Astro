using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Login
    {
        private readonly string passwordHash;
        private Login(IDataReader reader)
        {
            EmployeeId = reader.GetInt16(0);
            UserName = reader.GetString(1);
            RoleId = reader.GetInt16(2);
            passwordHash = reader.GetString(3);
            AccessFailedCount = reader.GetInt16(4);
            LockoutEnabled = reader.GetBoolean(5);
            LockoutEnd = !reader.IsDBNull(6) ? reader.GetDateTime(6) : null;
            UsePasswordExpiration = reader.GetBoolean(7);
            PasswordExpirationDate = !reader.IsDBNull(8) ? reader.GetDateTime(8) : null;
        }
        [JsonPropertyName("employeeId")]
        public short EmployeeId { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; } = "";
        [JsonPropertyName("roleId")]
        public short RoleId { get; set; }
        [JsonPropertyName("accessFailedCount")]
        public short AccessFailedCount { get; set; } = 0;
        [JsonPropertyName("lockoutEnabled")]
        public bool LockoutEnabled { get; set; } = false;
        [JsonPropertyName("lockoutEnd")]
        public DateTime? LockoutEnd { get; set; }
        [JsonPropertyName("usePasswordExpiration")]
        public bool UsePasswordExpiration { get; set; }
        [JsonPropertyName("passwordExpirationDate")]
        public DateTime? PasswordExpirationDate { get; set; }

        public bool IsLockedOut() => LockoutEnabled && LockoutEnd > DateTime.UtcNow;
        public bool IsPasswordExpired() => UsePasswordExpiration && PasswordExpirationDate < DateTime.UtcNow;
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(this.passwordHash)) return false;
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        public bool HasExceededFailedAttempts(int threshold = 3) => AccessFailedCount >= threshold;
        public static Login Create(IDataReader reader) => new Login(reader);
    }
}
