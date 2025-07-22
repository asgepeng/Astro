using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models
{
    public class Puchase
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("invoice")]
        public string Invoice { get; set; } = string.Empty;
        [JsonPropertyName("supplier")]
        public short Supplier { get; set; } = 0;
        
    }

    public class PurchaseItem
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyName("purchaseId")]
        public Guid PurchaseId { get; set; }
        [JsonPropertyName("productId")]
        public short ProductId { get; set; }
    }
}
