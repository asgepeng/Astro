using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class AddressInfo
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("streetAddress")] public string StreetAddress { get; set; } = "";
    }

    public class EmailInfo
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
    }
    public class PhoneInfo
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
    }
}
