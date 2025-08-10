using Astro.Data;
using Astro.Models;
using Astro.Server.Extensions;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class Purchases
    {
        internal static void MapPurchaseEndPoints(this WebApplication app)
        {
            app.MapPost("/trans/purchases", CreateAsync).RequireAuthorization();
            app.MapPost("/trans/purchases/report", GetReportAsync).RequireAuthorization();
            app.MapPost("/trans/purchases/get-items", GetPurchaseItemAsync).RequireAuthorization();
        }
        internal static async Task<IResult> CreateAsync(IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest())
            {
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
                        db.CreateParameter("id", purchaseId),
                        db.CreateParameter("location", reader.ReadInt16()),
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
                        db.CreateParameter("user_id", reader.ReadInt16(), DbType.Int16),
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
        internal static async Task<IResult> GetReportAsync(IDatabase db, HttpContext context)
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
        private static async Task<IResult> GetPurchaseItemAsync(IDatabase db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new IO.Reader(stream))
            {
                var parameterType = (int)reader.ReadByte();
                if (parameterType == 0)
                {
                    var productId = reader.ReadInt16();
                    return Results.File(await GetPurchaseItemByIdAsync(db, productId), "application/octet-stream");
                }
                else if (parameterType == 1)
                {
                    var productSku = reader.ReadString();
                    return Results.File(await GetPurchaseItemBySkuAsync(db, productSku));
                }
            }

            return Results.Ok();
        }
        private static async Task<byte[]> GetPurchaseItemByIdAsync(IDatabase db, short id)
        {
            return await Task.FromResult(Array.Empty<byte>());
        }
        private static async Task<byte[]> GetPurchaseItemBySkuAsync(IDatabase db, string sku)
        {
            return await Task.FromResult(Array.Empty<byte>());
        }
    }
}
