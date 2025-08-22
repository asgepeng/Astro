using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models.Transactions
{
    public class Sale
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.Empty;
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("customerId")]
        public short CustomerId { get; set; } = 0;
        [JsonPropertyName("subTotal")]
        public long SubTotal { get; set; } = 0;
        [JsonPropertyName("discount")]
        public int Discount { get; set; } = 0;
        [JsonPropertyName("tax")]
        public int Tax { get; set; } = 0;
        [JsonPropertyName("cost")]
        public int Cost { get; set; } = 0;
        [JsonPropertyName("grandTotal1")]
        public long GrandTotal1 => SubTotal - Discount + Cost;
        [JsonPropertyName("grandTotal2")]
        public long GrandTotal2 => SubTotal - Discount + Cost + Tax;
        [JsonPropertyName("totalReceipt")]
        public long TotalReceipt { get; set; } = 0;
    }

    public class SaleItem
    {
        [JsonPropertyName("productId")]
        public short ProductId { get; set; } = 0;
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; } = 0;
        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public long Price { get; set; } = 0;
        [JsonPropertyName("discount")]
        public int Discount { get; set; } = 0;
        [JsonPropertyName("nettPrice")]
        public long NettPrice => Price - Discount;
        [JsonPropertyName("total")]
        public long Total => NettPrice * Quantity;
    }
}
