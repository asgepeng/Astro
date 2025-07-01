using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int ID { get; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("sku")]
        public long Sku { get; set; }
        [JsonPropertyName("category")]
        public short Category { get; set; }
        [JsonPropertyName("type")]
        public short Type { get; set; }
        public short Active { get; set; }
        public int Stock { get; set; }
        [JsonPropertyName("minStock")]
        [DisplayName("Min Stock")]
        public int MinStock { get; set; }
        [JsonPropertyName("maxStock")]
        [DisplayName("Max Stock")]
        public int MaxStock { get; set; }
        [JsonPropertyName("unit")]
        public short Unit { get; set; }
        [JsonPropertyName("price")]
        public long Price { get; set; }
        [JsonPropertyName("costAverage")]
        [DisplayName("Cost Average")]
        public long CostAverage { get; set; }
        [JsonPropertyName("images")]
        public string Images { get; set; }
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Product);
        public static Product? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Product);
    }
}
