using Alaska.Data;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            var commandText = """
                select p.product_id, p.product_name, p.product_sku, c.category_name,  p.stock, unt.unit_name, p.price, 
                   	concat(u.user_firstname, ' ', u.user_lastname) as creator, p.created_date
                from products as p
                inner join categories as c on p.category_id = c.category_id
                inner join units as unt on p.unit_id = unt.unit_id
                inner join users as u on p.creator_id = u.user_id
                where p.is_deleted = false
                """;
            if (isWinformApp)
            {
                var data = Array.Empty<byte>();
                using (var writer = new IO.Writer())
                {
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            writer.WriteInt16(reader.GetInt16(0));
                            writer.WriteString(reader.GetString(1));
                            writer.WriteString(reader.GetString(2));
                            writer.WriteString(reader.GetString(3));
                            writer.WriteInt32(reader.GetInt32(4));
                            writer.WriteString(reader.GetString(5));
                            writer.WriteInt64(reader.GetInt64(6));
                            writer.WriteString(reader.GetString(7));
                            writer.WriteDateTime(reader.GetDateTime(8));
                        }
                    }, commandText);
                    data = writer.ToArray();
                }
                return Results.File(data, "application/octet-stream");
            }
            else
            {
                var sb = new StringBuilder();
                return Results.Content(sb.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            var commandText = """
                select product_id, product_name, product_description, product_sku, category_id, product_type,
                	is_active, stock, min_stock, max_stock, unit_id, price, cost_average, images
                from products
                where product_id = @id and is_deleted = false;
                """;
            if (isWinformApp)
            {
                using (var writer = new IO.Writer())
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
                        }
                    }, commandText, new NpgsqlParameter("id", id));

                    commandText = """
                        select category_id, category_name
                        from categories
                        where is_deleted = false
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
                        select unit_id, unit_name
                        from units
                        order by unit_name
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
            else
            {
                return Results.NotFound();
            }
        }
        internal static async Task<IResult> CreateAsync(Product product, IDatabase db, HttpContext context)
        {
            if (product is null) return Results.Ok(CommonResult.Fail("product cannot be null.."));
            if (await IsBarcodeUsed(db, product)) return Results.Ok(CommonResult.Fail($"Barcode '{product.Sku}' is already used by another product."));
            var commandText = """
                INSERT INTO products (
                    product_name,
                    product_description,
                    product_sku,
                    category_id,
                    product_type,
                    is_active,
                    stock,
                    min_stock,
                    max_stock,
                    unit_id,
                    price,
                    cost_average,
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
                    @is_active,
                    @stock,
                    @min_stock,
                    @max_stock,
                    @unit_id,
                    @price,
                    @cost_average,
                    @images,
                    @creator_id,
                    @created_date
                );                
                """;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("product_name", product.Name),
                new NpgsqlParameter("product_description", product.Description ?? string.Empty),
                new NpgsqlParameter("product_sku", product.Sku ?? string.Empty),
                new NpgsqlParameter("category_id", product.Category),
                new NpgsqlParameter("product_type", product.Type),
                new NpgsqlParameter("is_active", product.Active),
                new NpgsqlParameter("stock", product.Stock),
                new NpgsqlParameter("min_stock", product.MinStock),
                new NpgsqlParameter("max_stock", product.MaxStock),
                new NpgsqlParameter("unit_id", product.Unit),
                new NpgsqlParameter("price", product.Price),
                new NpgsqlParameter("cost_average", product.CostAverage),
                new NpgsqlParameter("images", product.Images ?? string.Empty),
                new NpgsqlParameter("creator_id", AppHelpers.GetUserID(context)),
                new NpgsqlParameter("created_date", DateTime.UtcNow)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Product was successfully saved")) : Results.Problem("An error occured while saving the product. Please try again later.");
        }
        internal static async Task<IResult> UpdateAsync(Product product, IDatabase db, HttpContext context)
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
                    is_active = @is_active,
                    stock = @stock,
                    min_stock = @min_stock,
                    max_stock = @max_stock,
                    unit_id = @unit_id,
                    price = @price,
                    cost_average = @cost_average,
                    images = @images
                WHERE product_id = @product_id AND is_deleted = false;
                """;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("product_id", product.ID),
                new NpgsqlParameter("product_name", product.Name),
                new NpgsqlParameter("product_description", product.Description ?? string.Empty),
                new NpgsqlParameter("product_sku", product.Sku ?? string.Empty),
                new NpgsqlParameter("category_id", product.Category),
                new NpgsqlParameter("product_type", product.Type),
                new NpgsqlParameter("is_active", product.Active),
                new NpgsqlParameter("stock", product.Stock),
                new NpgsqlParameter("min_stock", product.MinStock),
                new NpgsqlParameter("max_stock", product.MaxStock),
                new NpgsqlParameter("unit_id", product.Unit),
                new NpgsqlParameter("price", product.Price),
                new NpgsqlParameter("cost_average", product.CostAverage),
                new NpgsqlParameter("images", product.Images ?? string.Empty)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Product succesfully updated")) : Results.Problem("An error occurred while updating the product. Please try again later.");
        }
        internal static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "update products set is_deleted = true, editor_id=@editor_id where product_id = @product_id";
            var parameter = new NpgsqlParameter[]
            {
                new NpgsqlParameter("product_id", id),
                new NpgsqlParameter("editor_id", AppHelpers.GetUserID(context))
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameter);
            return success ? Results.Ok(CommonResult.Ok("Product have benn succesfully deleted")) : Results.Problem("An error occurred while deleting the product. Please try again later.");
        }
        private static async Task<bool> IsBarcodeUsed(IDatabase db, Product product)
        {
            var commandText = """
                select 1 from products
                where product_sku = @product_sku and product_id != @product_id and is_deleted = false;
                """;
            return await db.AnyRecordsAsync(commandText, new NpgsqlParameter("product_id", product.ID), new NpgsqlParameter("product_sku", product.Sku));
        }
    }
}
