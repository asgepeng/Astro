using Astro.Data;
using Astro.Helpers;
using Astro.Models;
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
        }
        internal static async Task<IResult> CreateAsync(IDatabase db, HttpContext context)
        {
            if (Application.IsWinformApp(context.Request))
            {
                var sb = new StringBuilder();
                sb.AppendLine("""
                    INSERT INTO purchases
                        (purchase_id, purchase_date, supplier_id, bruto, discount, cost, tax, tax_rate, netto, paid_amount, paid_method_count, user_id, created_date)
                    VALUES
                        (@purchase_id, @purchase_date, @supplier_id, @bruto, @discount, @cost, @tax, @tax_rate, @netto, @paid_amount, @paid_method_count, @user_id, @created_date);
                    """);
                using (var stream = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    if (stream.Length == 0) return Results.BadRequest();

                    using (var reader = new IO.Reader(stream))
                    {
                        var parameters = new List<DbParameter>()
                        {
                            db.CreateParameter("purchase_id", reader.ReadGuid(), DbType.Guid),
                            db.CreateParameter("purchase_date", reader.ReadDateTime(), DbType.DateTime),
                            db.CreateParameter("supplier_id", reader.ReadInt16()),
                            db.CreateParameter("bruto", reader.ReadInt64(), DbType.Int64),
                            db.CreateParameter("discount", reader.ReadInt32(), DbType.Int32),
                            db.CreateParameter("cost", reader.ReadInt32(), DbType.Int32),
                            db.CreateParameter("tax", reader.ReadInt32(), DbType.Int32),
                            db.CreateParameter("tax_rate", reader.ReadInt32(),DbType.Int32),
                            db.CreateParameter("netto", reader.ReadInt64(), DbType.Int64),
                            db.CreateParameter("paid_amount", reader.ReadInt64(), DbType.Int64),
                            db.CreateParameter("paid_method_count", reader.ReadInt16(),DbType.Int16),
                            db.CreateParameter("user_id", reader.ReadInt16(), DbType.Int16),
                            db.CreateParameter("created_date", reader.ReadDateTime(), DbType.DateTime)
                        };
                        var itemCount = reader.ReadInt32();
                        if (itemCount == 0) return Results.BadRequest("items is null");
                        sb.AppendLine("""
                        INSERT INTO purchase_items
                            (purchase_id, product_id, price, qty, discount)
                        VALUES
                        """);
                        while (itemCount > 0)
                        {
                            sb
                                .Append(" (@purchase_id, @p").Append(itemCount)
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
            }
            return Results.Ok();
        }
    }
}
