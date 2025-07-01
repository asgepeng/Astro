using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Address
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("streetAddress")] public string StreetAddress { get; set; } = "";
        [JsonPropertyName("city")] public string City { get; set; } = "";
        [JsonPropertyName("stateOrProvince")] public string StateOrProvince { get; set; } = "";
    }
}
