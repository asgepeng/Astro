using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Option<T> where T : IConvertible
    {
        [JsonPropertyName("id")] public T Id { get; set; } = default(T);
        [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
        [JsonPropertyName("selected")] public bool Selected { get; set; } = false;
    }
}
