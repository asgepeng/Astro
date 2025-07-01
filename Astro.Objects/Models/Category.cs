using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public ushort Id { get; set; } = 0;
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
