using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Country
    {
        [JsonPropertyName("id")] public short Id { get; set; } = 0;
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("a2code")] public string A2Code { get; set; } = "";
        [JsonPropertyName("a3code")] public string A3Code { get; set; } = "";

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Country);
        public static Country? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Country);

    }
}
