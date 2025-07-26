using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class StockOpname
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("product")]
        public int ProductId { get; set; } = 0;
    }
}
