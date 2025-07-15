using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Province
    {
        [JsonPropertyName("id")] 
        public short Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("countryId")] 
        public short CountryId { get; set; } = 0;

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Province);
        public static Province Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Province);
    }
}
