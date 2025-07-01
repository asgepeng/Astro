using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class City
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("provinceId")] public ushort ProvinceId { get; set; } = 0;

        public static City? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.City);
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.City);
    }
}
