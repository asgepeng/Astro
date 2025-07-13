using Alaska.Data;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace Astro.Server.Api
{
    internal static class Units
    {
        internal static void MapUnitEndPoints(this WebApplication app)
        {
            app.MapGet("/data/units", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/units/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/units", CreateAsync).RequireAuthorization();
            app.MapPut("/data/units", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/units/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            if (isWinformApp)
            {
                var commandText = """
                select u.unit_id, u.unit_name, u.created_date, concat(c.user_firstname, ' ', c.user_lastname) as created_by
                from units as u
                inner join users as c on u.creator_id = c.user_id
                order by u.unit_name
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
            else
            {
                var list = new List<Unit>();
                return Results.Ok(list);
            }
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = """
                select unit_id, unit_name
                from units
                where unit_id = @id
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
        private static async Task<IResult> CreateAsync(Unit unit, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into units (unit_name, creator_id)
                values (@name, @creatorId)
                returning unit_id
                """;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@name", unit.Name.Trim()),
                new NpgsqlParameter("@creatorId", AppHelpers.GetUserID(context))
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Unit created successfully.")) 
                           : Results.Ok(CommonResult.Fail("Failed to create unit. Please try again."));
        }
        private static async Task<IResult> UpdateAsync(Unit unit, IDatabase db, HttpContext context)
        {
            var commandText = """
                update units
                set unit_name = @name
                where unit_id = @id
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter("name", unit.Name.Trim()), new NpgsqlParameter("@id", unit.Id));
            return success ? Results.Ok(CommonResult.Ok("Unit updated successfully.")) 
                           : Results.Ok(CommonResult.Fail("Failed to update unit. Please try again."));
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "delete from units where unit_id =@id";
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter("@id", id));
            return success ? Results.Ok(CommonResult.Ok("Unit deleted successfully.")) 
                           : Results.Ok(CommonResult.Fail("Failed to delete unit. Please try again."));
        }
    }
}
