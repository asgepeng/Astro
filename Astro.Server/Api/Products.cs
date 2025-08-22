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

namespace Astro.Server.Api
{
    internal static class Products
    {
        internal static void MapProductEndPoints(this WebApplication app)
        {
            app.MapGet("/data/products", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/products/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/products", CreateAsync).RequireAuthorization();
            app.MapPut("/data/products", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/products/{id}", DeleteAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetAllAsync(
            [FromQuery] int? pg,
            [FromQuery] int? pgsize,
            [FromQuery] int? order,
            [FromQuery] int? sort,
            [FromQuery] string? src,
            IDBClient db, HttpContext context)
        {

            var locationID = TokenStore.GetLocationId(context.Request.GetToken());
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetProductDataTable(locationID), "application/octet-stream");
            else
            {
                var pagination = new Pagination()
                {
                    Page = pg.HasValue ? pg.Value : 1,
                    PageSize = pgsize.HasValue ? pgsize.Value : 5,
                    OrderBy = order.HasValue ? order.Value : 0,
                    SortOrder = sort.HasValue ? sort.Value : 0,
                    Search = src is null ? string.Empty : src.Trim()
                };
                var sb = new StringBuilder();
                await sb.AppendProductTableAsync(db, pagination);
                return Results.Content(sb.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetProduct(id, context), "application/octet-stream");
            else
            {
                return Results.NotFound();
            }
        }
        internal static async Task<IResult> CreateAsync(IDBClient db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new IO.Reader(stream))
            {
                var requestType = reader.ReadByte();
                var locationId = reader.ReadInt16();
                if (requestType == 0)
                {
                    return Results.File(await db.GetProductDataTable(locationId), "application/octet-stream");
                }
                else if (requestType == 1)
                {
                    var product = new Product()
                    {
                        ID = reader.ReadInt16(),
                        Name = reader.ReadString(),
                        Description = reader.ReadString(),
                        Sku = reader.ReadString(),
                        Category = reader.ReadInt16(),
                        Type = reader.ReadInt16(),
                        Unit = reader.ReadInt16(),
                        Images = reader.ReadString(),
                        Active =reader.ReadBoolean(),
                        Stock = reader.ReadInt32(),
                        MinStock= reader.ReadInt16(),
                        MaxStock= reader.ReadInt16(),
                        Price = reader.ReadInt64(),
                        COGs = reader.ReadInt64()
                    };
                    if (await IsBarcodeUsed(db, product)) return Results.Ok(CommonResult.Fail($"Barcode '{product.Sku}' is already used by another product."));

                    if (product.ID == 0)
                    {
                        var commandText = """
                        INSERT INTO products (
                            product_name,
                            product_description,
                            product_sku,
                            category_id,
                            product_type,
                            unit_id,
                            images,
                            creator_id,
                            created_date
                        )
                        VALUES (
                            @product_name,
                            @product_description,
                            @product_sku,
                            @category_id,
                            @product_type,
                            @unit,
                            @images,
                            @creator_id,
                            @created_date
                        ) RETURNING product_id;           
                        """;
                        var parameters = new DbParameter[]
                        {
                            db.CreateParameter("product_name", product.Name, DbType.String),
                            db.CreateParameter("product_description", product.Description ?? string.Empty, DbType.String),
                            db.CreateParameter("product_sku", product.Sku ?? string.Empty, DbType.String),
                            db.CreateParameter("category_id", product.Category, DbType.Int16),
                            db.CreateParameter("product_type", product.Type, DbType.Int16),
                            db.CreateParameter("unit", product.Unit, DbType.Int16),
                            db.CreateParameter("images", product.Images ?? string.Empty, DbType.String),
                            db.CreateParameter("creator_id", Extensions.Application.GetUserID(context), DbType.Int16),
                            db.CreateParameter("created_date", DateTime.UtcNow, DbType.DateTime)
                        };
                        product.ID = await db.ExecuteScalarAsync<short>(commandText, parameters);
                        if (product.ID == default(short)) return Results.Problem("An error occured while saving the product. Please try again later.");

                        commandText = """
                        INSERT INTO inventories
                        (location_id, product_id, is_active, stock, min_stock, max_stock, price, cogs)
                        VALUES
                        (@location, @product_id, @is_active, @stock, @min_stock, @max_stock, @price, @cogs);
                        """;
                        parameters = new DbParameter[]
                        {
                            db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16),
                            db.CreateParameter("product_id", product.ID, DbType.Int16),
                            db.CreateParameter("is_active", product.Active, DbType.Boolean),
                            db.CreateParameter("stock", product.Stock, DbType.Int32),
                            db.CreateParameter("min_stock", product.MinStock, DbType.Int16),
                            db.CreateParameter("max_stock", product.MaxStock, DbType.Int16),
                            db.CreateParameter("price", product.Price, DbType.Int64),
                            db.CreateParameter("cogs", product.COGs, DbType.Int64)
                        };
                        var success = await db.ExecuteNonQueryAsync(commandText, parameters);
                        return success ? Results.Ok(CommonResult.Ok(product.ID.ToString())) : Results.Problem("An error occured while saving the product. Please try again later.");
                    }
                    else
                    {
                        var commandText = """
                            UPDATE products
                            SET product_name = @product_name,
                                product_description = @product_description,
                                product_sku = @product_sku,
                                category_id = @category_id,
                                product_type = @product_type,
                                unit_id = @unit_id,
                                images = @images
                            WHERE product_id = @product_id AND is_deleted = false;
                            UPDATE inventories
                            SET is_active = @is_active,
                                stock = @stock,
                                min_stock = @min_stock,
                                max_stock = @max_stock,
                                price = @price,
                                cogs = @cost_average
                            WHERE product_id = @product_id AND location_id = @location
                            """;
                        var parameters = new DbParameter[]
                        {
                            db.CreateParameter("product_id", product.ID, DbType.Int16),
                            db.CreateParameter("product_name", product.Name, DbType.String),
                            db.CreateParameter("product_description", product.Description ?? string.Empty, DbType.String),
                            db.CreateParameter("product_sku", product.Sku ?? string.Empty, DbType.String),
                            db.CreateParameter("category_id", product.Category, DbType.Int16),
                            db.CreateParameter("product_type", product.Type, DbType.Int16),
                            db.CreateParameter("is_active", product.Active, DbType.Boolean),
                            db.CreateParameter("stock", product.Stock, DbType.Int32),
                            db.CreateParameter("min_stock", product.MinStock, DbType.Int16),
                            db.CreateParameter("max_stock", product.MaxStock, DbType.Int16),
                            db.CreateParameter("unit_id", product.Unit, DbType.Int16),
                            db.CreateParameter("price", product.Price, DbType.Int64),
                            db.CreateParameter("cost_average", product.COGs, DbType.Int64),
                            db.CreateParameter("images", product.Images ?? string.Empty, DbType.String),
                            db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16)
                        };

                        var success = await db.ExecuteNonQueryAsync(commandText, parameters);
                        return success ? Results.Ok(CommonResult.Ok("Product succesfully updated")) : Results.Problem("An error occurred while updating the product. Please try again later.");
                    }
                }
                else
                {
                    return Results.BadRequest();
                }
            }
        }
        internal static async Task<IResult> UpdateAsync(Product product, IDBClient db, HttpContext context)
        {
            if (product == null) return Results.BadRequest("Product cannot be null.");
            if (await IsBarcodeUsed(db, product)) return Results.Ok(CommonResult.Fail($"Barcode '{product.Sku}' is already used by another product (id: {product.ID})."));
            var commandText = """
                UPDATE products
                SET product_name = @product_name,
                    product_description = @product_description,
                    product_sku = @product_sku,
                    category_id = @category_id,
                    product_type = @product_type,
                    unit_id = @unit_id,
                    images = @images
                WHERE product_id = @product_id AND is_deleted = false;
                UPDATE inventories
                SET is_active = @is_active,
                    stock = @stock,
                    min_stock = @min_stock,
                    max_stock = @max_stock,
                    price = @price,
                    cogs = @cost_average
                WHERE product_id = @product_id AND location_id = @location
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("product_id", product.ID, DbType.Int16),
                db.CreateParameter("product_name", product.Name, DbType.String),
                db.CreateParameter("product_description", product.Description ?? string.Empty, DbType.String),
                db.CreateParameter("product_sku", product.Sku ?? string.Empty, DbType.String),
                db.CreateParameter("category_id", product.Category, DbType.Int16),
                db.CreateParameter("product_type", product.Type, DbType.Int16),
                db.CreateParameter("is_active", product.Active, DbType.Boolean),
                db.CreateParameter("stock", product.Stock, DbType.Int32),
                db.CreateParameter("min_stock", product.MinStock, DbType.Int16),
                db.CreateParameter("max_stock", product.MaxStock, DbType.Int16),
                db.CreateParameter("unit_id", product.Unit, DbType.Int16),
                db.CreateParameter("price", product.Price, DbType.Int64),
                db.CreateParameter("cost_average", product.COGs, DbType.Int64),
                db.CreateParameter("images", product.Images ?? string.Empty, DbType.String),
                db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16)
            };

            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Product succesfully updated")) : Results.Problem("An error occurred while updating the product. Please try again later.");
        }
        internal static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = "update products set is_deleted = true, editor_id=@editor_id where product_id = @product_id";
            var parameter = new DbParameter[]
            {
                db.CreateParameter("product_id", id, DbType.Int16),
                db.CreateParameter("editor_id", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameter);
            return success ? Results.Ok(CommonResult.Ok("Product have benn succesfully deleted")) : Results.Problem("An error occurred while deleting the product. Please try again later.");
        }
        private static async Task<bool> IsBarcodeUsed(IDBClient db, Product product)
        {
            var commandText = """
                select 1 from products
                where product_sku = @product_sku and product_id != @product_id and is_deleted = false;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("product_id", product.ID, DbType.Int16),
                db.CreateParameter("product_sku", product.Sku, DbType.String)
            };
            return await db.HasRecordsAsync(commandText, parameters);
        }
    }
}
