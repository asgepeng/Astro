﻿using Astro.Helpers;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.Text;
using Astro.Server.Binaries;

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
            if (Application.IsWinformApp(context.Request)) return Results.File(await db.GetProductDataTable(), "application/octet-stream");
            else
            {
                var sb = new StringBuilder();
                return Results.Content(sb.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (Application.IsWinformApp(context.Request)) return Results.File(await db.GetProduct(id), "application/octet-stream");
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
            var parameters = new DbParameter[]
            {
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
                db.CreateParameter("cost_average", product.CostAverage, DbType.Int64),
                db.CreateParameter("images", product.Images ?? string.Empty, DbType.String),
                db.CreateParameter("creator_id", Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("created_date", DateTime.UtcNow, DbType.DateTime)
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
                db.CreateParameter("cost_average", product.CostAverage, DbType.Int64),
                db.CreateParameter("images", product.Images ?? string.Empty, DbType.String)
            };

            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Product succesfully updated")) : Results.Problem("An error occurred while updating the product. Please try again later.");
        }
        internal static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "update products set is_deleted = true, editor_id=@editor_id where product_id = @product_id";
            var parameter = new DbParameter[]
            {
                db.CreateParameter("product_id", id, DbType.Int16),
                db.CreateParameter("editor_id", Application.GetUserID(context), DbType.Int16)
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
            var parameters = new DbParameter[]
            {
                db.CreateParameter("product_id", product.ID, DbType.Int16),
                db.CreateParameter("product_sku", product.Sku, DbType.String)
            };
            return await db.AnyRecordsAsync(commandText, parameters);
        }
    }
}
