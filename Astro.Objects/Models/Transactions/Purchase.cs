using Astro.Binaries;
using Astro.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public long SubTotal { get; private set; }
        [JsonPropertyName("discount")]
        public int Discount { get; private set; }
        [JsonPropertyName("cost")]
        public int Cost { get; private set; }
        public long GrandTotal => SubTotal - Discount + Cost;
        [JsonPropertyName("tax")]
        public int Tax { get; set; }
        [JsonPropertyName("totalPaid")]
        public long TotalPaid { get; private set; }
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
            Cost = 0;
            foreach (var item in this.Costs)
            {
                Cost += item.Value;
            }
            double costAndDiscount = this.Cost - this.Discount;
            double discountPerItem = 0;
            if (costAndDiscount > 0)
            {
                double totalItem = 0;
                foreach (var item in this.Items) totalItem += item.Quantity;
                discountPerItem = costAndDiscount / totalItem;
            }
            foreach (var item in this.Items) item.Discount = 0;
            TotalPaid = 0;
            foreach (var p in this.Payments)
            {
                TotalPaid += p.Amount;
            }
        }
        public short GetStatusCode()
        {
            if (this.TotalPaid >= (GrandTotal + Tax)) return 2;
            else if (this.TotalPaid > 0) return 1;
            else return 0;
        }
        public void CopyTo(BinaryDataWriter writer)
        {
            writer.WriteInt16((short)this.Items.Count);
            writer.WriteGuid(this.Id);
            writer.WriteString(this.InvoiceNumber);
            writer.WriteDateTime(this.Date);
            writer.WriteInt16(this.SupplierId);
            writer.WriteInt32(this.Discount);
            writer.WriteInt64(this.TotalPaid);

            foreach (var item in this.Items)
            {
                writer.WriteInt16(item.Id);
                writer.WriteString(item.Name);
                writer.WriteString(item.Sku);
                writer.WriteString(item.Unit);
                writer.WriteInt32(item.Quantity);
                writer.WriteInt64(item.Price);
            }

            writer.WriteInt32(this.Costs.Count);
            foreach (var cost in this.Costs)
            {
                writer.WriteString(cost.Name);
                writer.WriteInt32(cost.Value);
            }

            writer.WriteInt32(this.Payments.Count);
            foreach (var payment in this.Payments)
            {
                writer.WriteGuid(payment.Id);
                writer.WriteInt16(payment.AccountId);
                writer.WriteInt16(payment.AccountType);
                writer.WriteInt64(payment.Amount);
            }
        }
        public static Purchase? FromBinary(BinaryDataReader reader)
        {
            if (!reader.Read()) return null;

            var itemCount = reader.ReadInt16();
            if (itemCount <= 0) return null;

            var purchase = new Purchase()
            {
                Id = reader.ReadGuid(),
                InvoiceNumber = reader.ReadString(),
                Date = reader.ReadDateTime(),
                SupplierId = reader.ReadInt16(),
                Discount = reader.ReadInt32(),
                TotalPaid = reader.ReadInt64()
            };

            for (int i = 0; i < itemCount; i++)
            {
                purchase.Items.Add(new PurchaseItem()
                {
                    Id = reader.ReadInt16(),
                    Name = reader.ReadString(),
                    Sku = reader.ReadString(),
                    Unit = reader.ReadString(),
                    Quantity = reader.ReadInt32(),
                    Price = reader.ReadInt64()
                });
            }
            var costCount = reader.ReadInt32();
            for (int i = 0; i < costCount; i++)
            {
                purchase.Costs.Add(new Cost()
                {
                    Name = reader.ReadString(),
                    Value = reader.ReadInt32()
                });
            }

            var paymentCount = reader.ReadInt32();
            for (int i = 0; i < paymentCount; i++)
            {
                purchase.Payments.Add(new Payment()
                {
                    Id = reader.ReadGuid(),
                    AccountId = reader.ReadInt16(),
                    AccountType = reader.ReadInt16(),
                    Amount = reader.ReadInt64()
                });
            }
            purchase.Calculate();
            return purchase;
        }
        public string GenerateSql()
        {
            this.Calculate();
            var sb = new StringBuilder();
            sb.AppendLine("""
                INSERT INTO purchases
                    (purchaseid, invoicenumber, locationid, purchasedate, supplierid, subtotal, discount, cost, grandtotal, tax, totalpaid, status, notes, creatorid, createddate)
                VALUES
                    (@id, @invoicenumber, @location, @purchasedate, @supplierid, @subtotal, @discount, @cost, @grandtotal, @tax, @totalpaid, @status, @notes, @creator, CURRENT_TIMESTAMP);
                """);
            sb.Append("""
                INSERT INTO stockflows (
                    flowdate, locationid, refid, reftype, 
                    productid, productname, productsku, productunit, 
                    cogs, stock, quantity, price, discount, creatorid)
                SELECT
                    @purchasedate AS flowdate, @location AS locationid, @id AS refid, 1 AS reftype,
                    pd.productid, pd.productname, pd.qty, pd.productunit,
                    i.cogs, i.stock, pd.qty, pd.price, pd.discount, @creator
                FROM (
                VALUES 
                """);
            for (int i = 0; i < this.Items.Count; i++)
            {
                var item = this.Items[i];
                if (i > 0) sb.Append(", ");
                sb.Append("(")
                    .Append(item.Id).Append(", ")
                    .Append(item.Name.ToSqlVarchar()).Append(", ")
                    .Append(item.Sku.ToSqlVarchar()).Append(", ")
                    .Append(item.Unit.ToSqlVarchar()).Append(", ")
                    .Append(item.Price).Append(", ")
                    .Append(item.Discount).Append(", ")
                    .Append(item.Quantity).Append(")");
            }
            sb.Append("""
                ) AS pd (productid, productname, productsku, productunit, price, discount, qty)
                INNER JOIN inventories AS i ON pd.productid = i.productid AND i.locationid = @location;
                UPDATE inventories AS i
                SET 
                    stock = i.stock + pd.qty,
                    cogs = CASE 
                               WHEN i.stock > 0 
                                   THEN ((i.stock * i.cogs) + (pd.qty * pd.price)) / (i.stock + pd.qty)
                               ELSE pd.price 
                           END
                FROM (
                    VALUES 
                """);
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                var item = this.Items[i];
                sb.Append("(")
                    .Append(item.Id).Append(", ")
                    .Append(item.Quantity).Append(", ")
                    .Append(item.Price).Append(")");
            }
            sb.AppendLine("""
                ) AS pd(productid, qty, price)
                WHERE pd.productid = i.productid 
                  AND i.locationid = @location;
                """);
            return sb.ToString();
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
        public long Total => Quantity * Price;
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
        public string GenerateSql()
        {
            var sb = new StringBuilder();
            sb.Append("""
                INSERT INTO stockflows (
                    flowdate, locationid, refid, reftype, 
                    productid, productname, productsku, productunit, 
                    cogs, stock, quantity, price, discount, creatorid)
                SELECT
                    @purchasedate AS flowdate, @location AS locationid, @id AS refid, 1 AS reftype,
                    pd.productid, pd.productname, pd.qty, pd.productunit,
                    i.cogs, i.stock, pd.qty, pd.price, 0, @creator
                FROM (
                VALUES 
                """);
            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i];
                if (i > 0) sb.Append(", ");
                sb.Append("(")
                    .Append(item.Id).Append(", ")
                    .Append(item.Name.ToSqlVarchar()).Append(", ")
                    .Append(item.Sku.ToSqlVarchar()).Append(", ")
                    .Append(item.Unit.ToSqlVarchar()).Append(", ")
                    .Append(item.Price).Append(", ")
                    .Append(item.NettPrice).Append(", ")
                    .Append(item.Quantity).Append(")");
            }
            sb.Append("""
                ) AS pd (productid, productname, productsku, productunit, price, netprice, qty)
                INNER JOIN inventories AS i ON pd.productid = i.productid AND i.locationid = @location;
                UPDATE inventories AS i
                SET 
                    stock = i.stock + pd.qty,
                    cogs = CASE 
                               WHEN i.stock > 0 
                                   THEN ((i.stock * i.cogs) + (pd.qty * pd.price)) / (i.stock + pd.qty)
                               ELSE pd.price 
                           END
                FROM (
                    VALUES 
                """);
            for (int i=0; i < this.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                var item = this[i];
                sb.Append("(")
                    .Append(item.Id).Append(", ")
                    .Append(item.Quantity).Append(", ")
                    .Append(item.Price).Append(")");
            }
            sb.Append("""
                ) AS pd(productid, qty, price)
                WHERE pd.productid = i.productid 
                  AND i.locationid = @location;
                """);

            return sb.ToString();
        }
    }

    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public short AccountId { get; set; } = 0;
        public short AccountType { get; set; } = 0;
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
