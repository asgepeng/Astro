using Astro.Helpers;
using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using Astro.Server.Binaries;
using Astro.Server.Extensions;

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
            var isWinformApp = context.Request.IsDesktopAppRequest();
            if (isWinformApp) return Results.File(await db.GetUnitDataTable(), "application/octet-stream");
            else
            {
                var list = new List<Unit>();
                return Results.Ok(list);
            }
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetUnit(id), "application/octet-stream");
            else return Results.Ok(CommonResult.Fail("This endpoint is not available for web applications."));
        }
        private static async Task<IResult> CreateAsync(Unit unit, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into units (unit_name, creator_id)
                values (@name, @creatorId)
                returning unit_id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("@name", unit.Name.Trim(), DbType.String),
                db.CreateParameter("@creatorId", Application.GetUserID(context), DbType.Int16)
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
            var parameters = new DbParameter[]
            {
                db.CreateParameter("name", unit.Name.Trim(), DbType.String),
                db.CreateParameter("id", unit.Id, DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Unit updated successfully.")) 
                           : Results.Ok(CommonResult.Fail("Failed to update unit. Please try again."));
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "delete from units where unit_id =@id";
            var success = await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("@id", id, DbType.Int16));
            return success ? Results.Ok(CommonResult.Ok("Unit deleted successfully.")) 
                           : Results.Ok(CommonResult.Fail("Failed to delete unit. Please try again."));
        }
    }
}
