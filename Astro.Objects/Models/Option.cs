using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Option
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
        [JsonPropertyName("selected")] public bool Selected { get; set; } = false;
    }
    public class ListOption : List<Option>
    {
        public Option Add(int id, string text)
        {
            var option = new Option { Id = id, Text = text };
            this.Add(option);
            return option;
        }
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.ListOption);
        public static ListOption Create(string json) => JsonSerializer.Deserialize<ListOption>(json, AppJsonSerializerContext.Default.ListOption) ?? new ListOption();
    }
}
