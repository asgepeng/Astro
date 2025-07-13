using System.Data.Common;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Astro.Models
{
    public class User 
    {
        private readonly string _passwordStored = string.Empty;
        private User (DbDataReader reader)
        {
            Id = reader.GetInt16(0);
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);
            RoleId = reader.GetInt16(3);
            UserName = reader.GetString(4);
            NormalizedUserName = reader.GetString(5);
            Email = reader.GetString(6);
            EmailConfirmed = reader.GetBoolean(7);
            PhoneNumber = reader.GetString(8);
            PhoneNumberConfirmed = reader.GetBoolean(9);
            DateOfBirth = reader.GetDateTime(10);
            Sex = reader.GetInt16(11);
            MaritalStatus = reader.GetInt16(12);
            StreetAddress = reader.GetString(13);
            CityId = reader.GetInt32(14);
            ZipCode = reader.GetString(15);
            TwoFactorEnabled = reader.GetBoolean(16);
            AccessFailedCount = reader.GetInt16(17);
            LockoutEnabled = reader.GetBoolean(18);
            LockoutEnd = reader.IsDBNull(19) ? null : reader.GetDateTime(19);
            //SecurityStamp = reader.GetGuid(20);
            ConcurrencyStamp = reader.IsDBNull(21) ? null : reader.GetDateTime(21);
            UsePasswordExpiration = reader.GetBoolean(22);
            PasswordExpirationDate = reader.IsDBNull(23) ? null : reader.GetDateTime(23);
            _passwordStored = reader.GetString(24);
        }
        private User(IO.Reader reader)
        {
            Id = reader.ReadInt16();
            FirstName = reader.ReadString();
            LastName = reader.ReadString();
            RoleId = reader.ReadInt16();
            UserName = reader.ReadString();
            NormalizedUserName = reader.ReadString();
            Email = reader.ReadString();
            EmailConfirmed = reader.ReadBoolean();
            PhoneNumber = reader.ReadString();
            PhoneNumberConfirmed = reader.ReadBoolean();
            DateOfBirth = reader.ReadDateTime();
            Sex = reader.ReadInt16();
            MaritalStatus = reader.ReadInt16();
            StreetAddress = reader.ReadString();
            CityId = reader.ReadInt32();
            StateId = reader.ReadInt16();
            CountryId = reader.ReadInt16();
            ZipCode = reader.ReadString();
            TwoFactorEnabled = reader.ReadBoolean();
            AccessFailedCount = reader.ReadInt16();
            LockoutEnabled = reader.ReadBoolean();
            LockoutEnd = reader.ReadNullableDateTime();           
            SecurityStamp = reader.ReadGuid();
            ConcurrencyStamp = reader.ReadNullableDateTime();
            UsePasswordExpiration = reader.ReadBoolean();
            PasswordExpirationDate = reader.ReadNullableDateTime();
        }
        internal User(object[] data)
        {
            Id = (short)data[0];
            FirstName = (string)data[1];
            LastName = (string)data[2];
            
        }
        [JsonConstructor] public User() { }
        [JsonPropertyName("id")]
        public short Id { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("roleId")]
        public short RoleId { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;
        [JsonPropertyName("usePasswordExpiration")]
        public bool UsePasswordExpiration { get; set; } = false;
        [JsonPropertyName("passwordExpirationDate")]
        public DateTime? PasswordExpirationDate { get; set; }
        [JsonPropertyName("normalizedUserName")]
        public string NormalizedUserName { get; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("emailConfirmed")]
        public bool EmailConfirmed { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
        [JsonPropertyName("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; } = DateTime.Today;
        [JsonPropertyName("sex")]
        public short Sex { get; set; }
        [JsonPropertyName("maritalStatus")]
        public short MaritalStatus { get; set; }
        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; set; } = string.Empty;
        [JsonPropertyName("cityId")]
        public int CityId { get; set; }
        [JsonPropertyName("stateId")]
        public int StateId { get; set; }
        [JsonPropertyName("countryId")]
        public int CountryId { get; set; }
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; } = string.Empty;
        [JsonPropertyName("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }
        [JsonPropertyName("accessFailedCount")]
        public short AccessFailedCount { get; set; }
        [JsonPropertyName("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }
        [JsonPropertyName("lockoutEnd")]
        public DateTime? LockoutEnd { get; set; }
        [JsonPropertyName("securityStamp")]
        public Guid? SecurityStamp { get; set; }
        [JsonPropertyName("concurrencyStamp")]
        public DateTime? ConcurrencyStamp { get; set; }
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(this._passwordStored)) return false;

            return BCrypt.Net.BCrypt.Verify(password, _passwordStored);
        }
        public bool IsLockedOut()
        {
            return LockoutEnabled && LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
        }
        public bool IsPasswordExpired()
        {
            return UsePasswordExpiration && PasswordExpirationDate.HasValue && PasswordExpirationDate.Value < DateTime.UtcNow;
        }
        public bool HasExceededFailedAttempts(int threshold = 3)
        {
            return AccessFailedCount >= threshold;
        }
        public string GetFullName()
        {
            return $"{FirstName} {LastName}".Trim();
        }

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.User);
        public static User? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.User);
        public static User Create(DbDataReader reader) => new User(reader);
        public static User Create(IO.Reader reader) => new User(reader);
    }

    public class Password
    {
        public static string HashPassword(string pwd) => BCrypt.Net.BCrypt.HashPassword(pwd);
    }

    public class ChangePasswordRequest
    {
        [JsonConstructor]
        public ChangePasswordRequest(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
        [JsonPropertyName("oldPassword")] public string OldPassword { get; } = string.Empty;
        [JsonPropertyName("newPassword")] public string NewPassword { get; } = string.Empty;

        public bool VerifyOldPassword(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.VerifyPassword(OldPassword);
        }

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.ChangePasswordRequest);
        public static ChangePasswordRequest? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ChangePasswordRequest);
    }
}
