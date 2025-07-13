using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Category);
        public static Category? Create(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Category);
    }
}
