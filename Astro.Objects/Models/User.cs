using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Astro.Models
{
    public class User 
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("firstname")] public string Firstname { get; set; } = "";
        [JsonPropertyName("middlename")] public string Middlename { get; set; } = "";
        [JsonPropertyName("lastname")] public string Lastname { get; set; } = "";
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.User);
        public static User? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.User);
    }
}
