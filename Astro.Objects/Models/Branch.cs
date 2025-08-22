using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Branch
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("address")]
        public string Address { get; set; } = "";
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = "";
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";
        public override string ToString() => Name;
    }
}
