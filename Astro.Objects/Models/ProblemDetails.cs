using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class ProblemDetails
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        public static ProblemDetails? Create(string json) => string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ProblemDetails);
    }
}
