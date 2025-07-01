using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("username")] public string Username { get; set; } = "";
        [JsonPropertyName("password")] public string Password { get; set; } = "";
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.LoginRequest);
        public static LoginRequest? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.LoginRequest);
    }
    public class LoginResponse
    {
        [JsonConstructor]
        internal LoginResponse(string? token = null)
        {
            Success = !string.IsNullOrEmpty(token);
            Token = string.IsNullOrEmpty(token) ? "" : token;
        }
        [JsonPropertyName("success")] public bool Success { get; } = false;
        [JsonPropertyName("token")] public string Token { get; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";
        [JsonPropertyName("userInfo")] public UserInfo? UserInfo { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.LoginResponse);
        public static LoginResponse LoginSuccess(string token) => new LoginResponse(token);

        public static LoginResponse LoginFailed() => new LoginResponse(null);
        public static LoginResponse? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.LoginResponse);
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
        [JsonPropertyName("isLocked")] public bool IsLocked { get; set; } = false;
        [JsonPropertyName("tryCount")] public short TryCount { get; set; } = 0;
        [JsonPropertyName("role")] public Role Role { get; }
    }
}
