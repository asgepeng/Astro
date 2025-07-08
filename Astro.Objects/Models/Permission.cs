using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Permission
    {
        [JsonPropertyName("id")] public short Id { get; set; } = 0;
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("allowCreate")] public bool AllowCreate { get; set; } = false;
        [JsonPropertyName("allowRead")] public bool AllowRead { get; set; } = false;
        [JsonPropertyName("allowEdit")] public bool AllowEdit { get; set; } = false;
        [JsonPropertyName("allowDelete")] public bool AllowDelete { get; set; } = false;

        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Permission);
        public static Permission? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Permission);
    }
}
