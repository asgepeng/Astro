using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Permission
    {
        [JsonPropertyName("id")] public int Id { get; set; } = 0;
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("allowAdd")] public bool AllowAdd { get; set; } = false;
        [JsonPropertyName("allowEdit")] public bool AllowEdit { get; set; } = false;
        [JsonPropertyName("allowDelete")] public bool AllowDelete { get; set; } = false;
    }
}
