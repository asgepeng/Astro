using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Contact
    {
        [JsonPropertyName("id")] 
        public int Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("addresses")]
        public List<Address> Addresses { get; set; } = new List<Address>();
        [JsonPropertyName("phones")]
        public List<Phone> Phones { get; set; } = new List<Phone>();
        [JsonPropertyName("emails")]
        public List<Email> Emails { get; set; } = new List<Email>();

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Contact);
        public static Contact? Create(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Contact);
    }
}
