using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Unit
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Unit);
        public static Unit? Create(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<Unit>(json, AppJsonSerializerContext.Default.Unit);
    }
}
