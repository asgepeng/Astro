using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    }
}
