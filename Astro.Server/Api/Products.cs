using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.Text;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Astro.Server.Memory;
using Astro.Binaries;

namespace Astro.Server.Api
{
    internal static class Products
    {
        internal static void MapProductEndPoints(this WebApplication app)
        {
            app.MapGet("/data/products/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/products", async Task<IResult> (IDBClient db, HttpContext context) =>
            {
                using (var stream = await context.Request.GetMemoryStreamAsync())
                using (var reader = new Astro.Binaries.BinaryDataReader(stream))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db, context);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else { return Results.BadRequest(); }
                }
            }).RequireAuthorization();
            app.MapPut("/data/products", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/products/{id}", DeleteAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetDataTableAsync(Astro.Binaries.BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var listingType = reader.ReadByte();
            if (listingType == 0x00) // normal table
            {
                var commandText = """
                SELECT p.productid, p.name, p.sku, c.name AS categoryname, i.stock, u.name AS unitname, i.price, e.fullname, p.createddate
                FROM products AS p
                    INNER JOIN categories AS c ON p.categoryid = c.categoryid
                    INNER JOIN units AS u ON p.unitid = u.unitid
                    INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                    INNER JOIN employees AS e ON p.creatorid = e.employeeid
                WHERE p.isdeleted = false
                ORDER BY p.name
                """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText, db.CreateParameter("location", context.Request.GetLocationID())), "application/octet-stream");
            }
            else if (listingType == 0x01) // pagination
            {
                var numPerPage = reader.ReadInt16();
                var pageNumber = reader.ReadInt16();
                var searchKeyword = reader.ReadString(); 
                var sb = new StringBuilder();
                sb.Append("""
                    SELECT p.productid, p.name, p.sku, c.name AS categoryname, i.stock, u.name AS unitname, i.price, e.fullname, p.createddate
                    FROM products AS p
                        INNER JOIN categories AS c ON p.categoryid = c.categoryid
                        INNER JOIN units AS u ON p.unitid = u.unitid
                        INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                        INNER JOIN employees AS e ON p.creatorid = e.employeeid
                    """);
                return Results.File(await db.ExecuteBinaryTableAsync(sb.ToString(), new DbParameter[]
                {
                    db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16)
                }), "application/octet-stream");
            }
            else if (listingType == 0x04) // purchase items
            {
                var keywordType = reader.ReadByte();
                if (keywordType == 0x01)
                {

                }
                return Results.BadRequest();
            }
            else
            {
                return Results.BadRequest();
            }
            
        }
        internal static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT p.productid, p.name, p.description, p.sku, p.categoryid, p.producttype,
                	i.isactive, i.stock, i.minstock, i.maxstock, p.unitid, i.price, i.cogs, p.images, p.taxable, p.taxfactor, p.parentid
                FROM products AS p
                INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                WHERE p.productid = @id and p.isdeleted = false;
                """;
            using (var writer = new BinaryDataWriter())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt16(reader.GetInt16(4));
                        writer.WriteInt16(reader.GetInt16(5));
                        writer.WriteBoolean(reader.GetBoolean(6));
                        writer.WriteInt32(reader.GetInt32(7));
                        writer.WriteInt16(reader.GetInt16(8));
                        writer.WriteInt16(reader.GetInt16(9));
                        writer.WriteInt16(reader.GetInt16(10));
                        writer.WriteInt64(reader.GetInt64(11));
                        writer.WriteInt64(reader.GetInt64(12));
                        writer.WriteString(reader.GetString(13));
                        writer.WriteBoolean(reader.GetBoolean(14));
                        writer.WriteDecimal(reader.GetDecimal(15));
                        writer.WriteInt16(reader.GetInt16(16));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16), db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16));

                commandText = """
                    SELECT categoryid, name
                    FROM categories
                    WHERE isdeleted = false
                    """;
                var iCount = 0;
                var iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);

                commandText = """
                    SELECT unitid, name
                    FROM units
                    ORDER BY name
                    """;
                iCount = 0;
                iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);

                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        internal static async Task<IResult> CreateAsync(Astro.Binaries.BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var product = GetProductDataInput(reader);
            if (await IsBarcodeUsed(db, product)) return Results.Ok(CommonResult.Fail($"Barcode '{product.Sku}' is already used by another product."));

            var commandText = """
            INSERT INTO products (
                name,
                description,
                sku,
                categoryid,
                producttype,
                unitid,
                images,
                taxable,
                taxfactor,
                creatorid,
                createddate
            )
            VALUES (
                @name,
                @description,
                @sku,
                @categoryid,
                @producttype,
                @unitid,
                @images,
                @taxable,
                @taxfactor,
                @creatorid,
                @createddate
            ) RETURNING productid;           
            """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("productid", product.ID, DbType.Int16),
                db.CreateParameter("name", product.Name, DbType.AnsiString),
                db.CreateParameter("description", product.Description, DbType.AnsiString),
                db.CreateParameter("sku", product.Sku, DbType.AnsiString),
                db.CreateParameter("categoryid", product.Category, DbType.Int16),
                db.CreateParameter("producttype", product.Type, DbType.Int16),
                db.CreateParameter("unitid", product.Unit, DbType.Int16),
                db.CreateParameter("images", product.Images, DbType.AnsiString),
                db.CreateParameter("taxable", product.Taxable, DbType.Boolean),
                db.CreateParameter("taxfactor", product.TaxFactor, DbType.Decimal),
                db.CreateParameter("creatorid", context.GetUserID(), DbType.Int16),
                db.CreateParameter("createddate", DateTime.UtcNow, DbType.DateTime)
            };
            product.ID = await db.ExecuteScalarAsync<short>(commandText, parameters);
            if (product.ID == default(short)) return Results.Problem("An error occured while saving the product. Please try again later.");

            commandText = """
                INSERT INTO inventories
                (locationid, productid, isactive, stock, minstock, maxstock, price, cogs)
                VALUES
                (@location, @productid, @isactive, 0, @minstock, @maxstock, @price, 0);
                """;
            parameters = new DbParameter[]
            {
                db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16),
                db.CreateParameter("productid", product.ID, DbType.Int16),
                db.CreateParameter("isactive", product.Active, DbType.Boolean),
                db.CreateParameter("minstock", product.MinStock, DbType.Int16),
                db.CreateParameter("maxstock", product.MaxStock, DbType.Int16),
                db.CreateParameter("price", product.Price, DbType.Int64)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok(product.ID.ToString())) : Results.Problem("An error occured while saving the product. Please try again later.");
        }
        internal static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Astro.Binaries.BinaryDataReader(ms))
            {
                var reqtuestType = reader.ReadByte();
                if (reqtuestType == 0x01)
                {
                    var locationId = context.Request.GetLocationID();
                    var product = GetProductDataInput(reader);
                    if (await IsBarcodeUsed(db, product)) return Results.Ok(CommonResult.Fail($"Barcode '{product.Sku}' is already used by another product (id: {product.ID})."));
                    var commandText = """
                    UPDATE products
                    SET name = @name,
                        description = @description,
                        sku = @sku,
                        categoryid = @categoryid,
                        producttype = @producttype,
                        unitid = @unitid,
                        images = @images,
                        taxable = @taxable,
                        taxfactor = @taxfactor
                    WHERE productid = @productid AND isdeleted = false;
                    UPDATE inventories
                    SET isactive = @isactive,
                        minstock = @minstock,
                        maxstock = @maxstock,
                        price = @price
                    WHERE productid = @productid AND locationid = @location
                    """;
                    var parameters = new DbParameter[]
                    {
                        db.CreateParameter("productid", product.ID, DbType.Int16),
                        db.CreateParameter("name", product.Name, DbType.String),
                        db.CreateParameter("description", product.Description ?? string.Empty, DbType.String),
                        db.CreateParameter("sku", product.Sku ?? string.Empty, DbType.String),
                        db.CreateParameter("categoryid", product.Category, DbType.Int16),
                        db.CreateParameter("producttype", product.Type, DbType.Int16),
                        db.CreateParameter("taxable", product.Taxable, DbType.Boolean),
                        db.CreateParameter("taxfactor", product.TaxFactor, DbType.Decimal),
                        db.CreateParameter("isactive", product.Active, DbType.Boolean),
                        db.CreateParameter("stock", product.Stock, DbType.Int32),
                        db.CreateParameter("minstock", product.MinStock, DbType.Int16),
                        db.CreateParameter("maxstock", product.MaxStock, DbType.Int16),
                        db.CreateParameter("unitid", product.Unit, DbType.Int16),
                        db.CreateParameter("price", product.Price, DbType.Int64),
                        db.CreateParameter("images", product.Images ?? string.Empty, DbType.String),
                        db.CreateParameter("location", locationId, DbType.Int16)
                    };

                    var success = await db.ExecuteNonQueryAsync(commandText, parameters);
                    return success ? Results.Ok(CommonResult.Ok("Product succesfully updated")) : Results.Problem("An error occurred while updating the product. Please try again later.");
                }
                else
                {
                    return Results.BadRequest();
                }                
            }
        }
        internal static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = "UPDATE products SET isdeleted = true, editorid=@editorid WHERE productid = @productid";
            var parameter = new DbParameter[]
            {
                db.CreateParameter("productid", id, DbType.Int16),
                db.CreateParameter("editorid", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameter);
            return success ? Results.Ok(CommonResult.Ok("Product have benn succesfully deleted")) : Results.Problem("An error occurred while deleting the product. Please try again later.");
        }
        private static async Task<bool> IsBarcodeUsed(IDBClient db, Product product)
        {
            var commandText = """
                SELECT 1 FROM products
                WHERE sku = @sku and productid != @productid 
                AND isdeleted = false;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("productid", product.ID, DbType.Int16),
                db.CreateParameter("sku", product.Sku, DbType.String)
            };
            return await db.HasRecordsAsync(commandText, parameters);
        }
        private static Product GetProductDataInput(Astro.Binaries.BinaryDataReader reader) => new Product()
        {
            ID = reader.ReadInt16(),
            Name = reader.ReadString(),
            Description = reader.ReadString(),
            Sku = reader.ReadString(),
            Category = reader.ReadInt16(),
            Type = reader.ReadInt16(),
            Unit = reader.ReadInt16(),
            Images = reader.ReadString(),
            Taxable = reader.ReadBoolean(),
            TaxFactor = reader.ReadDecimal(),
            MinStock = reader.ReadInt16(),
            MaxStock = reader.ReadInt16(),
            Price = reader.ReadInt64(),
            Active = reader.ReadBoolean()
        };
    }
}
