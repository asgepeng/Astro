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
        [JsonPropertyName("id")] public short Id { get; set; } = 0;
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
        [JsonPropertyName("items")] public ListMenu Items { get; set; } = new ListMenu();
    }
    public class ListMenu : List<Menu>
    {
        public Menu Add(short id, string title)
        {
            var menu = new Menu()
            {
                Id = id,
                Title = title
            };
            base.Add(menu);
            return menu;
        }
        public static ListMenu? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ListMenu);
    }
}
