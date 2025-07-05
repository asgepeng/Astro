using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Credential
    {
        [JsonConstructor] public Credential( string username, string password) 
        {
            Username = username;
            Password = password;
        }
        [JsonPropertyName("username")] public string Username { get; }
        [JsonPropertyName("password")] public string Password { get; }
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Credential);
        public static Credential? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Credential);
    }
    public class AuthResponse
    {
        [JsonConstructor]
        internal AuthResponse(string? accessToken = null, UserInfo userInfo = null)
        {
            AccessToken = accessToken;
            UserInfo = userInfo;
        }
        public bool IsAuthenticated() => !string.IsNullOrEmpty(AccessToken) && UserInfo != null;
        [JsonPropertyName("accessToken")] public string? AccessToken { get; } = null;
        [JsonPropertyName("userInfo")] public UserInfo? UserInfo { get; }
        [JsonPropertyName("message")] public string Message { get; set; } = "";

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.AuthResponse);
        public static AuthResponse Success(string token, UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Token is required.", nameof(token));
            if (userInfo == null) throw new ArgumentNullException(nameof(userInfo));
            return new AuthResponse(token, userInfo);
        }
        public static AuthResponse Lockout(DateTime? lockoutEnd)
        {
            var message = lockoutEnd.HasValue ? $"Your account is locked until {lockoutEnd:dd/MM/yyyy HH:mm:ss}. Please try again later." : "";
            return new AuthResponse() { Message = message };
        }
        public static AuthResponse Fail(string message = "") => new AuthResponse() { Message = message };
        public static AuthResponse? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.AuthResponse);
    }
    public class UserInfo
    {
        [JsonConstructor] public UserInfo(short id, string name, Role role)
        {
            Id = id;
            Name = name;
            Role = role;
        }
        [JsonPropertyName("id")] public short Id { get; } 
        [JsonPropertyName("name")] public string Name { get; }
        [JsonPropertyName("role")] public Role Role { get; }
    }

    public class LockInfo
    {
        [JsonConstructor] public LockInfo(DateTime lockedExpired, short failedCount)
        {
            LockedExpired = lockedExpired;
            FailedCount = failedCount;
        }
        [JsonPropertyName("lockedExpired")] public DateTime LockedExpired { get; set; } = DateTime.Now;
        [JsonPropertyName("failedCount")] public short FailedCount { get; set; } = 0;
    }

    public class LoginInfo
    {
        public LoginInfo(string passwordStored, short trialCount, bool isLocked, DateTime lockExpirationDate)
        {
            StoredPassword = passwordStored;
            IsLocked = isLocked;
            LockExpirationDate = LockExpirationDate;
            TrialCount = trialCount;
        }
        public string StoredPassword { get; }
        public short TrialCount { get; private set; }
        public bool IsLocked { get; }
        public DateTime LockExpirationDate { get; }
    }
}
