using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Product
    {
        private Product(IDataReader reader)
        {
            ID = reader.GetInt16(0);
            Name = reader.GetString(1);
            Description = reader.GetString(2);
            Sku = reader.GetString(3);
            Category = reader.GetInt16(4);
            Type = reader.GetInt16(5);
            Active = reader.GetBoolean(6);
            Stock = reader.GetInt32(7);
            MinStock = reader.GetInt32(8);
            MaxStock = reader.GetInt32(9);
            Unit = reader.GetInt16(10);
            Price = reader.GetInt64(11);
            COGs = reader.GetInt64(12);
            Images = reader.GetString(13);
        }
        private Product(Streams.Reader reader)
        {
            ID = reader.ReadInt16();
            Name = reader.ReadString();
            Description = reader.ReadString();
            Sku = reader.ReadString();
            Category = reader.ReadInt16();
            Type = reader.ReadInt16();
            Active = reader.ReadBoolean();
            Stock = reader.ReadInt32();
            MinStock = reader.ReadInt16();
            MaxStock = reader.ReadInt16();
            Unit = reader.ReadInt16();
            Price = reader.ReadInt64();
            COGs = reader.ReadInt64();
            Images = reader.ReadString();
            Taxable = reader.ReadBoolean();
            TaxFactor = reader.ReadDecimal();
        }
        [JsonConstructor] public Product() { }
        [JsonPropertyName("id")]
        public short ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("sku")]
        public string Sku { get; set; }
        [JsonPropertyName("category")]
        public short Category { get; set; }
        [JsonPropertyName("type")]
        public short Type { get; set; }
        public bool Active { get; set; }
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
        public long COGs { get; set; }
        [JsonIgnore]
        public long Margin => Price - COGs;
        [JsonPropertyName("images")]
        public string Images { get; set; } = string.Empty;
        [JsonPropertyName("taxable")]
        public bool Taxable { get; set; } = false;
        [JsonPropertyName("taxFactor")]
        public decimal TaxFactor { get; set; } = 0.0M;
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.Product);
        public static Product? Create(string json) => JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.Product);
        public static Product Create(Streams.Reader reader) => new Product(reader);
    }
}
