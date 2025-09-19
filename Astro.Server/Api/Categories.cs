using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.Data;
using Astro.Binaries;
using Astro.Server.Extensions;

namespace Astro.Server.Api
{
    internal static class Categories
    {
        internal static void MapCategoryEndPoints(this WebApplication app)
        {
            app.MapGet("/data/categories/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/categories", async Task<IResult>(IDBClient db, HttpContext context) =>
            {
                using (var ms = await context.Request.GetMemoryStreamAsync())
                using (var reader = new BinaryDataReader(ms))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/categories", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/categories/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<bool> CategoryNameExists(Category category, IDBClient db)
        {
            var commandText = """
                SELECT 1 
                FROM categories 
                WHERE name = @name AND isdeleted = false AND categoryid != @id";
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("name", category.Name, DbType.String),
                db.CreateParameter("id", category.Id, DbType.Int16)
            };

            return await db.HasRecordsAsync(commandText, parameters);
        }

        private static async Task<IResult> GetDataTableAsync(BinaryDataReader reader, IDBClient db)
        {
            var listingMode = reader.ReadByte();
            if (listingMode == 0x00)
            {
                var commandText = """
                    SELECT c.categoryid, c.name, c.createddate, u.fullname
                    FROM categories AS c
                    INNER JOIN employees AS u on c.creatorid = u.employeeid
                    WHERE c.isdeleted = false
                    ORDER BY c.name
                    """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingMode == 0x02)
            {
                var commandText = """
                    SELECT categoryid, name
                    FROM categories
                    WHERE isdeleted = false
                    ORDER BY name
                    """;
                using (var writer = new BinaryDataWriter())
                {
                    var iPos = writer.ReserveInt32();
                    var iCount = 0;
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
            else return Results.BadRequest();
        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = """
                select c.categoryid, c.name
                from categories c
                where c.isdeleted = false and c.categoryid = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new BinaryDataWriter())
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                    data = writer.ToArray();
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return Results.File(data, "application/octet-stream");
        }
        private static async Task<IResult> CreateAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var category = new Category()
            {
                Name = reader.ReadString()
            };
            if (await CategoryNameExists(category, db)) return Results.Ok(CommonResult.Fail("A category with this name already exists. Please choose a different name."));

            var commandText = """
                INSERT INTO categories (name, creatorid)
                VALUES (@name, @creatorId)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@name", category.Name, DbType.String),
                db.CreateParameter("@creatorId", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category created successfully.")) : Results.Problem("An error occured while creating the category. Please try again later.");
        }
        private static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new BinaryDataReader(ms))
            {
                var category = new Category()
                {
                    Id = reader.ReadInt16(),
                    Name = reader.ReadString()
                };
                if (await CategoryNameExists(category, db)) return Results.Ok(CommonResult.Fail("A category with this name already exists. Please choose a different name."));

                var commandText = """
                    UPDATE categories
                    SET name = @name, editorid = @creatorId
                    WHERE categoryid = @id
                    """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("@name", category.Name, DbType.String),
                    db.CreateParameter("@creatorId", Extensions.Application.GetUserID(context), DbType.Int16),
                    db.CreateParameter("@id", category.Id, DbType.Int16)
                };
                var success = await db.ExecuteNonQueryAsync(commandText, parameters);
                return success ? Results.Ok(CommonResult.Ok("Category updated successfully.")) : Results.Problem("An error occured while updating the category. Please try again later.");
            }
        }
        private static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = "update categories set is_deleted = true, editor_id=@editor, edited_date = current_timestamp where category_id=@id";
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@editor", Extensions.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("@id", id, DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Category deleted successfully.")) : Results.Problem("An error occured while deleting the category. Please try again later.");
        }
    }
}
