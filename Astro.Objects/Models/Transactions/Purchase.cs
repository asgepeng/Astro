using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Astro.Models.Transactions
{
    public class Purchase
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("location")]
        public short Location { get; set; } = 0;
        [JsonPropertyName("invoiceNumber")]
        public string InvoiceNumber { get; set; } = string.Empty;
        [JsonPropertyName("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("supplierId")]
        public short SupplierId { get; set; }
        [JsonPropertyName("subTotal")]
        public long SubTotal { get; set; }
        [JsonPropertyName("discount")]
        public int Discount { get; set; }
        [JsonPropertyName("tax")]
        public int Tax { get; set; }
        [JsonPropertyName("cost")]
        public int Cost { get; set; } = 0;
        public long GrandTotal => SubTotal - Discount + Cost + Tax;
        [JsonPropertyName("totalPaid")]
        public long TotalPaid { get; set; }
        [JsonPropertyName("accountPayableAmount")]
        public long AccountPayableAmount => GrandTotal - TotalPaid;        
        [JsonPropertyName("items")]
        public PurchaseItemCollection Items { get; } = new PurchaseItemCollection();
        [JsonPropertyName("payments")]
        public PaymentCollection Payments { get; } = new PaymentCollection();
        [JsonPropertyName("costs")]
        public CostCollection Costs { get; } = new CostCollection();
        public void Calculate()
        {
            SubTotal = 0;
            foreach (var item in this.Items)
            {
                SubTotal += item.Total;
            }
        }
        public byte[] ToByteArray()
        {
            using (var writer = new IO.Writer())
            {
                writer.WriteInt16(this.Location);
                writer.WriteGuid(this.Id);
                writer.WriteString(this.InvoiceNumber);
                writer.WriteDateTime(this.Date);
                writer.WriteInt16(this.SupplierId);
                writer.WriteInt64(this.SubTotal);
                writer.WriteInt32(this.Discount);
                writer.WriteInt32(this.Tax);
                writer.WriteInt32(this.Cost);
                writer.WriteInt64(this.GrandTotal);
                writer.WriteInt64(this.TotalPaid);
                writer.WriteInt64(this.AccountPayableAmount);
                writer.WriteDateTime(DateTime.UtcNow);
                writer.WriteInt32(this.Items.Count);
                foreach (var item in this.Items)
                {
                    writer.WriteInt16(item.Id);
                    writer.WriteString(item.Name);
                    writer.WriteString(item.Unit);
                    writer.WriteInt32(item.Quantity);
                    writer.WriteInt64(item.Price);
                    writer.WriteInt32(item.Discount);
                    writer.WriteInt64(item.Total);
                }
                writer.WriteInt32(this.Payments.Count);
                foreach (var payment in this.Payments)
                {
                    writer.WriteInt16(payment.AccountId);
                    writer.WriteInt64(payment.Amount);
                }
                return writer.ToArray();
            }
        }
    }

    public class PurchaseItem
    {
        public short Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public long Price { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public long NettPrice => Price - Discount;
        public long Total => Quantity * NettPrice;
    }
    public class PurchaseItemCollection : Collection<PurchaseItem>
    {
        public void Add(PurchaseItem item)
        {
            foreach (var i in this)
            {
                if (item.Id == i.Id)
                {
                    i.Quantity += item.Quantity;
                    return;
                }
            }
            base.Add(item);
        }
    }

    public class Payment
    {
        public short AccountId { get; set; } = 0;
        public long Amount { get; set; } = 0;
    }
    public class PaymentCollection : Collection<Payment>
    {

    }

    public class PurchaseItemRequest
    {
        [JsonPropertyName("id")]
        public short Id { get; set; } = 0;
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        [JsonPropertyName("location")]
        public short Location { get; set; } = 0;
        public override string ToString() => JsonSerializer.Serialize(this, AppJsonSerializerContext.Default.PurchaseItemRequest);
    }
}
