using Astro.Data;
using Astro.Models;
using Astro.Models.Transactions;
using Astro.Server.Extensions;
using Astro.Binaries;
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
            app.MapPost("/trans/purchases", async Task<IResult>(IDBClient db, HttpContext context) =>
            {
                using (var ms = await context.Request.GetMemoryStreamAsync())
                using (var reader = new BinaryDataReader(ms))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db, context);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPost("/trans/purchases/report", GetReportAsync).RequireAuthorization();
            app.MapPost("/trans/purchases/get-item", GetPurchaseItemByIdAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var listingMode = reader.ReadByte();
            if (listingMode == 0x00)
            {
                var commandText = """
                    SELECT p.purchaseid, p.invoicenumber, s.name AS suppliername, p.purchasedate, p.grandtotal, p.grandtotal - p.totalpaid AS ap, 
                        CASE p.status WHEN 2 THEN 'Lunas' WHEN 1 THEN 'Bayar Sebagian' ELSE 'Belum dibayar' END AS status, e.fullname AS creator, p.createddate
                    FROM purchases AS p
                    LEFT JOIN contacts AS s ON p.supplierid = s.contactid
                    INNER JOIN employees AS e ON p.creatorid = e.employeeid
                    WHERE p.purchasedate BETWEEN @start AND @end
                    AND p.locationid = @location
                    """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("start", reader.ReadDateTime(), DbType.DateTime),
                    db.CreateParameter("end", reader.ReadDateTime(), DbType.DateTime),
                    db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16)
                };
                return Results.File(await db.ExecuteBinaryTableAsync(commandText, parameters), "application/octet-stream");
            }
            return Results.BadRequest();
        }
        internal static async Task<IResult> CreateAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var purchase = Purchase.FromBinary(reader);
            if (purchase is null) return Results.BadRequest();

            var commandText = """
                SELECT 1 FROM purchases
                WHERE purchaseid = @id
                """;
            var exists = await db.HasRecordsAsync(commandText, db.CreateParameter("id", purchase.Id, DbType.Guid));
            if (exists) return Results.Ok(CommonResult.Fail("Data pembelian sudah tersimpan sebelumnya"));
            var creator = context.GetUserID();
            var parameters = new DbParameter[]
            {
                db.CreateParameter("id", purchase.Id, DbType.Guid),
                db.CreateParameter("invoicenumber", purchase.InvoiceNumber, DbType.AnsiString),
                db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16),
                db.CreateParameter("purchasedate", purchase.Date, DbType.DateTime),
                db.CreateParameter("supplierid", purchase.SupplierId, DbType.Int16),
                db.CreateParameter("subtotal", purchase.SubTotal, DbType.Int64),
                db.CreateParameter("discount", purchase.Discount, DbType.Int32),
                db.CreateParameter("cost", purchase.Cost, DbType.Int32),
                db.CreateParameter("grandtotal", purchase.GrandTotal, DbType.Int32),
                db.CreateParameter("tax", purchase.Tax, DbType.Int32),
                db.CreateParameter("totalpaid", purchase.TotalPaid, DbType.Int64),
                db.CreateParameter("status", purchase.GetStatusCode(), DbType.Int16),
                db.CreateParameter("notes", "", DbType.AnsiString),
                db.CreateParameter("creator", creator, DbType.Int16)
            };
            commandText = purchase.GenerateSql();
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            if (!success) return Results.Problem("Kesalahan yang belum diketahui terjadi, silakan coba lagi nanti");

            if (purchase.Payments.Count > 0)
            {
                var iCount = 0;
                commandText = """
                    INSERT INTO cashflows
                        (cashflowid, cashflowdate, refid, reftype, accountid, accounttype, amount, creatorid)
                    VALUES
                        (@paymentid, @purchasedate, @purchaseid, 1, @accountid, @accounttype, @amount, @creator)
                    """;
                foreach (var payment in purchase.Payments)
                {
                    var paymentParams = new DbParameter[]
                    {
                        db.CreateParameter("paymentid", payment.Id, DbType.Guid),
                        db.CreateParameter("purchasedate", purchase.Date, DbType.DateTime),
                        db.CreateParameter("purchaseid", purchase.Id, DbType.Guid),
                        db.CreateParameter("accountid", payment.AccountId, DbType.Int16),
                        db.CreateParameter("accountType", payment.AccountType, DbType.Int16),
                        db.CreateParameter("amount", payment.Amount * -1, DbType.Int64),
                        db.CreateParameter("creator", creator, DbType.Int16)
                    };
                    success = await db.ExecuteNonQueryAsync(commandText, paymentParams);
                    if (success) iCount++;
                }
                if (iCount == purchase.Payments.Count)
                {
                    return Results.Ok(CommonResult.Ok("Transaksi pembelian berhasil disimpan"));
                }
                else
                {
                    return Results.Ok(CommonResult.Fail("Gagal menyimpan pembayaran"));
                }
            }
            else
            {
                return Results.Ok(CommonResult.Ok("Data pembelian berhasil disimpan"));
            }
        }
        internal static async Task<IResult> GetReportAsync(IDBClient db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new BinaryDataReader(stream))
            {
                var dateStart = reader.ReadDateTime();
                var dateEnd = reader.ReadDateTime();
                var operatorId = reader.ReadInt16();

                return Results.File(Array.Empty<byte>(), "application/octet-stream");
            }
        }
        private static async Task<IResult> GetPurchaseItemByIdAsync(PurchaseItemRequest req, IDBClient db, HttpContext context)
        {
            using (var writer = new BinaryDataWriter())
            {
                var commandText = """
                    SELECT p.productid, p.name, p.sku, u.name AS unitname, i.buyprice
                    FROM products AS p
                    INNER JOIN units AS u ON p.unitid = u.unitid
                    INNER JOIN inventories AS i ON p.productid = i.productid  AND i.locationid = @location
                    WHERE p.isdeleted = false AND (p.productid = @id OR p.sku = @sku)
                    LIMIT 1
                    """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16),
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
            using (var writer = new Astro.Binaries.BinaryDataWriter())
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
