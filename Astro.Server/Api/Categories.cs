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
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("name", category.Name),
                new NpgsqlParameter("id", category.Id)
            };
            return await db.AnyRecordsAsync(commandText, parameters);
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            if (isWinformApp)
            {
                var commandText = """
                    select c.category_id, c.category_name, c.created_date, concat(u.user_firstname, ' ', u.user_lastname) as created_by
                    from categories c
                    inner join users u on c.creator_id = u.user_id
                    where c.is_deleted = false
                    order by c.category_name
                    """;
                var data = Array.Empty<byte>();
                using (var writer = new IO.Writer())
                {
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            writer.WriteInt16(reader.GetInt16(0));
                            writer.WriteString(reader.GetString(1));
                            writer.WriteDateTime(reader.GetDateTime(2));
                            writer.WriteString(reader.GetString(3));
                        }
                    }, commandText);
                    data = writer.ToArray();
                }
                return Results.File(data, "application/octet-stream");
            }
            return Results.Ok();
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = """
                select c.category_id, c.category_name
                from categories c
                where c.is_deleted = false and c.category_id = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new IO.Writer())
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                    data = writer.ToArray();
                }
            }, commandText, new NpgsqlParameter("id", id));
            return Results.File(data, "application/octet-stream");
        }
        private static async Task<IResult> CreateAsync(Category category, IDatabase db, HttpContext context)
        {
            if (await CategoryNameExists(category, db)) return Results.Ok(CommonResult.Fail("A category with this name already exists. Please choose a different name."));

            var commandText = """
                insert into categories (category_name, creator_id)
                values (@name, @creatorId)
                """;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@name", category.Name),
                new NpgsqlParameter("@creatorId", AppHelpers.GetUserID(context))
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
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@name", category.Name),
                new NpgsqlParameter("@creatorId", AppHelpers.GetUserID(context)),
                new NpgsqlParameter("@id", category.Id)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category updated successfully.")) : Results.Problem("An error occured while updating the category. Please try again later.");
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "update categories set is_deleted = true, editor_id=@editor, edited_date = current_timestamp where category_id=@id";
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@editor", AppHelpers.GetUserID(context)),
                new NpgsqlParameter("@id", id)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category deleted successfully.")) : Results.Problem("An error occured while deleting the category. Please try again later.");
        }
    }
}
