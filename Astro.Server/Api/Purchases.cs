using Astro.Data;
using Astro.Models;
using Astro.Models.Transactions;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Astro.Server.Api
{
    internal static class Purchases
    {
        internal static void MapPurchaseEndPoints(this WebApplication app)
        {
            app.MapPost("/trans/purchases", CreateAsync).RequireAuthorization();
            app.MapPost("/trans/purchases/report", GetReportAsync).RequireAuthorization();
            app.MapPost("/trans/purchases/get-item", GetPurchaseItemByIdAsync).RequireAuthorization();
        }
        internal static async Task<IResult> CreateAsync(IDBClient db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest())
            {
                using (var writer = new IO.Writer())
                {
                    writer.WriteBoolean(true);
                    writer.WriteString("Transaksi pembelian sukses disimpan");
                    writer.WriteString("Berhasil");
                    return Results.File(writer.ToArray(), "application/octet-stream");
                }
                var sb = new StringBuilder();
                sb.AppendLine("""
                    INSERT INTO purchases
                        (purchase_id, purchase_date, supplier_id, bruto, discount, cost, tax, tax_rate, netto, paid_amount, paid_method_count, user_id, created_date)
                    VALUES
                        (@id, @purchase_date, @supplier_id, @bruto, @discount, @cost, @tax, @tax_rate, @netto, @paid_amount, @paid_method_count, @user_id, @created_date);
                    """);
                using (var stream = await context.Request.GetMemoryStreamAsync())
                using (var reader = new IO.Reader(stream))
                {
                    var purchaseId = reader.ReadGuid();
                    var invoiceNumber = await db.ExecuteScalarAsync<string>("SELECT invoice_number FROM purchases WHERE purchase_id = @id", db.CreateParameter("id", purchaseId, DbType.Guid));
                    if (invoiceNumber != string.Empty) return Results.Ok(true);

                    var parameters = new List<DbParameter>()
                    {
                        db.CreateParameter("location", reader.ReadInt16()),
                        db.CreateParameter("id", purchaseId),
                        db.CreateParameter("purchase_date", reader.ReadDateTime(), DbType.DateTime),
                        db.CreateParameter("supplier_id", reader.ReadInt16()),
                        db.CreateParameter("bruto", reader.ReadInt64(), DbType.Int64),
                        db.CreateParameter("discount", reader.ReadInt32(), DbType.Int32),
                        db.CreateParameter("cost", reader.ReadInt32(), DbType.Int32),
                        db.CreateParameter("tax", reader.ReadInt32(), DbType.Int32),
                        db.CreateParameter("tax_rate", reader.ReadInt32(),DbType.Int32),
                        db.CreateParameter("netto", reader.ReadInt64(), DbType.Int64),
                        db.CreateParameter("total_paid", reader.ReadInt64(), DbType.Int64),
                        db.CreateParameter("paid_method_count", reader.ReadInt16(),DbType.Int16),
                        db.CreateParameter("user_id", context.GetUserID(), DbType.Int16),
                        db.CreateParameter("created_date", reader.ReadDateTime(), DbType.DateTime)
                    };
                    var itemCount = reader.ReadInt32();
                    if (itemCount == 0) return Results.BadRequest("items is null");
                    sb.AppendLine("""
                        INSERT INTO stockflows
                            (location_id, ref_type, ref_id, cogs, stock, qty, price, discount)
                        VALUES
                            (@location, 1, @id, @cogs, @stock, @qty, @price, @discount);
                        """);
                    while (itemCount > 0)
                    {
                        sb
                            .Append(" (@id, @p").Append(itemCount)
                            .Append(", @prc").Append(itemCount)
                            .Append(", @q").Append(itemCount)
                            .Append(", @d").Append(itemCount).Append(")");
                        if (itemCount > 1)
                        {
                            sb.Append(",");
                        }
                        parameters.AddRange(new DbParameter[]
                        {
                            db.CreateParameter("p" + itemCount.ToString(), reader.ReadInt16(), DbType.Int16),
                            db.CreateParameter("prc" + itemCount.ToString(), reader.ReadInt32(), DbType.Int32),
                            db.CreateParameter("q" + itemCount.ToString(), reader.ReadInt16(), DbType.Int16),
                            db.CreateParameter("d" + itemCount.ToString(), reader.ReadInt16(), DbType.Int32)
                        });
                        itemCount--;
                    }
                    sb.Append(";");

                    var paymentCount = reader.ReadInt32();
                    if (paymentCount > 0)
                    {
                        sb.AppendLine("""
                            INSERT INTO payments (purchase_id, payment_type, account_id, payment_amount)
                            VALUES
                            """);
                        while (paymentCount > 0)
                        {
                            sb.AppendFormat("(@purchase_id, @pt{0}, @acc{0}, @pa{0})", paymentCount);
                            if (paymentCount > 1) { sb.Append(","); }
                            parameters.AddRange(new DbParameter[]
                            {
                                db.CreateParameter($"pt{paymentCount}", reader.ReadInt16(), DbType.Int16),
                                db.CreateParameter($"acc{paymentCount}", reader.ReadInt16(), DbType.Int16),
                                db.CreateParameter($"pa{paymentCount}", reader.ReadInt64(), DbType.Int64)
                            });
                            paymentCount--;
                        }
                    }
                    return await db.ExecuteNonQueryAsync(sb.ToString(), parameters.ToArray()) ?
                        Results.Ok(CommonResult.Ok("Purchase was successfully saved")) :
                        Results.Problem("An error occured while saving purchase transaction, please try again later");
                }
            }
            return Results.Ok();
        }
        internal static async Task<IResult> GetReportAsync(IDBClient db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest())
            {
                using (var stream = await context.Request.GetMemoryStreamAsync())
                using (var reader = new IO.Reader(stream))
                {
                    var dateStart = reader.ReadDateTime();
                    var dateEnd = reader.ReadDateTime();
                    var operatorId = reader.ReadInt16();

                    return Results.File(Array.Empty<byte>(), "application/octet-stream");
                }
            }
            return Results.Ok();
        }
        private static async Task<IResult> GetPurchaseItemByIdAsync(PurchaseItemRequest req, IDBClient db, HttpContext context)
        {
            using (var writer = new IO.Writer())
            {
                var commandText = """
                    SELECT p.product_id, p.product_name, p.product_sku, u.unit_name, i.buyprice
                    FROM products AS p
                    INNER JOIN units AS u ON p.unit_id = u.unit_id
                    INNER JOIN inventories AS i ON p.product_id = i.product_id  AND i.location_id = @location
                    WHERE p.is_deleted = false AND (p.product_id = @id OR p.product_sku = @sku)
                    LIMIT 1
                    """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("location", req.Location, DbType.Int16),
                    db.CreateParameter("id", req.Id, DbType.Int16),
                    db.CreateParameter("sku", req.Sku)
                };
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteInt32(0);
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt64(reader.GetInt64(4));
                    }
                }, commandText, parameters);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> GetPurchaseItemBySkuAsync(PurchaseItemRequest req, IDBClient db, string sku)
        {
            using (var writer = new IO.Writer())
            {
                var commandText = """
                    SELECT p.product_id, p.product_name, p.product_sku, u.unit_name, i.buyprice
                    FROM products AS p
                    INNER JOIN units AS u ON p.unit_id = u.unit_id
                    INNER JOIN inventories AS i ON p.product_id = i.product_id  AND i.location_id = @location
                    WHERE p.is_deleted = false AND (p.product_id = @id OR p.product_sku=@sku)
                    LIMIT 1
                    """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("location", req.Location, DbType.Int16),
                    db.CreateParameter("id", req.Id, DbType.Int16),
                    db.CreateParameter("sku", req.Sku, DbType.String)
                };
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteInt32(0);
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt64(reader.GetInt64(4));
                    }
                }, commandText, parameters);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
    }
}
