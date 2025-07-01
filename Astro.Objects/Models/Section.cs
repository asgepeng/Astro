using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Menu
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
    }
    public class Section
    {
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("items")] public List<Menu> Items { get; } = new List<Menu>();
    }

    public class ListSection : List<Section>
    {
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.ListSection);
        public static ListSection Create(string json)=> JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ListSection);
    }
}
