using Astro.Data;
using Astro.Helpers;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class AccountProdivers
    {
        internal static void MapAccountProviderEndPoints(this WebApplication app)
        {
            app.MapGet("/data/account-providers", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/account-providers/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/account-providers", CreateAsync).RequireAuthorization();
            app.MapPut("/data/account-providers", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/account-providers/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccountProviderTableAsync(), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccountProviderAsync(id), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> CreateAsync(AccountProvider provider, IDatabase db, HttpContext context)
        {
            var sqlCheck = """
                SELECT provider_name
                FROM account_providers
                WHERE provider_id=@id
                """;
            var result = await db.ExecuteScalarAsync(sqlCheck, db.CreateParameter("id", provider.Id));
            if (result != null)
            {
                return Results.Problem("ID: " + provider.Id.ToString() + " currently being used by '" + result.ToString() + "', please use another id");
            }
            var commandText = """
                INSERT INTO account_providers
                    (provider_id, provider_name, provider_type)
                VALUES (@provider_id, @provider_name, @provider_type)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("provider_id", provider.Id),
                db.CreateParameter("provider_name", provider.Name),
                db.CreateParameter("provider_type", provider.Type)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account provider was successfully created")) : Results.Problem("An error occured while creating account providers, please try again later");
        }
        private static async Task<IResult> UpdateAsync(AccountProvider provider, IDatabase db, HttpContext context)
        {
            var commandText = """
                UPDATE account_providers
                SET provider_name = @provider_name,
                    provider_type = @provider_type
                WHERE provider_id = @id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("provider_name", provider.Name, System.Data.DbType.String),
                db.CreateParameter("provider_type", provider.Type, System.Data.DbType.Int16),
                db.CreateParameter("id", provider.Id, System.Data.DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account provider was successfully created")) : Results.Problem("An error occured while creating account providers, please try again later");
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var sqlCheck = """
                SELECT COUNT(*) AS total
                FROM accounts
                WHERE provider_id = @id
                """;
            var accountUse = await db.ExecuteScalarIntegerAsync(sqlCheck, db.CreateParameter("id", id));
            if (accountUse > 0)
            {
                return Results.Problem("You cannot delete account provider that being used by account record");
            }
            var commandText = """
                DELETE FROM account_providers
                WHERE provider_id = @id
                """;
            return await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("id", id)) ? Results.Ok(CommonResult.Ok("Account provider was succesfully deleted")) : Results.Problem("An error occurred while deleting account provider, please try again later");
        }
    }
}
