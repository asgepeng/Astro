using System.Text.Json;
using System.Text.Json.Serialization;

namespace Astro.Models
{
    public class Role
    {
        [JsonPropertyName("id")] public short Id { get; set; } = 0;
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Role);
        public static Role? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Role);
    }

}
