using Astro.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Astro.ViewModel
{
    public class RoleViewModel
    {
        [JsonPropertyName("role")] public Role Role { get; } = new Role();
        [JsonPropertyName("permissions")] public List<Permission> Permissions { get; } = new List<Permission>();
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.RoleViewModel);
        public static RoleViewModel? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.RoleViewModel);
    } 
}
