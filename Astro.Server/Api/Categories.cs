using Astro.Helpers;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.Data;
using Astro.Server.Binaries;
using System.Text;
using Astro.Server.Extensions;

namespace Astro.Server.Api
{
    internal static class Categories
    {
        internal static void MapCategoryEndPoints(this WebApplication app)
        {
            app.MapGet("/data/categories", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/categories/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/categories", CreateAsync).RequireAuthorization();
            app.MapPut("/data/categories", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/categories/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<bool> CategoryNameExists(Category category, IDatabase db)
        {
            var commandText = "select 1 from categories where category_name = @name and is_deleted = false and category_id != @id";
            var parameters = new DbParameter[]
            {
                db.CreateParameter("name", category.Name, DbType.String),
                db.CreateParameter("id", category.Id, DbType.Int16)
            };

            return await db.AnyRecordsAsync(commandText, parameters);
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetCategoryDataTable(), "application/octet-stream");

            var sb = new StringBuilder();
            await sb.AppendCategoryTableAsync(db);
            return Results.Content(sb.ToString(), "text/html");
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetCategory(id), "application/octet-stream");
            else
                return Results.Ok(CommonResult.Fail("This endpoint is not available for web applications."));
        }
        private static async Task<IResult> CreateAsync(Category category, IDatabase db, HttpContext context)
        {
            if (await CategoryNameExists(category, db)) return Results.Ok(CommonResult.Fail("A category with this name already exists. Please choose a different name."));

            var commandText = """
                insert into categories (category_name, creator_id)
                values (@name, @creatorId)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@name", category.Name, DbType.String),
                db.CreateParameter("@creatorId", Helpers.Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category created successfully.")) : Results.Problem("An error occured while creating the category. Please try again later.");
        }
        private static async Task<IResult> UpdateAsync(Category category, IDatabase db, HttpContext context)
        {
            if (await CategoryNameExists(category, db)) return Results.Ok(CommonResult.Fail("A category with this name already exists. Please choose a different name."));

            var commandText = """
                update categories
                set category_name = @name, creator_id = @creatorId
                where category_id = @id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@name", category.Name, DbType.String),
                db.CreateParameter("@creatorId", Helpers.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("@id", category.Id, DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category updated successfully.")) : Results.Problem("An error occured while updating the category. Please try again later.");
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "update categories set is_deleted = true, editor_id=@editor, edited_date = current_timestamp where category_id=@id";
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@editor", Helpers.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("@id", id, DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category deleted successfully.")) : Results.Problem("An error occured while deleting the category. Please try again later.");
        }
    }
}
