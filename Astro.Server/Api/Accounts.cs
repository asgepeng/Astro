using Astro.Data;
using Astro.Helpers;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class Accounts
    {
        internal static void MapAccountEndPoints(this WebApplication app)
        {
            app.MapGet("/data/accounts", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/accounts/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/accounts", CreateAsync).RequireAuthorization();
            app.MapPut("/data/accounts", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/accounts/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccountDataTable(), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccount(id), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> CreateAsync(Account model, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into accounts
                    (account_name, account_number, provider_id, creator_id)
                values
                    (@account_name, @account_number, @provider_id, @creator_id)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("account_name", model.AccountName),
                db.CreateParameter("account_number", model.AccountNumber),
                db.CreateParameter("provider_id", model.Provider),
                db.CreateParameter("creator_id", Application.GetUserID(context))
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account created succesfully")) : Results.Problem("An error occured while creating account, please try again later.");
        }
        private static async Task<IResult> UpdateAsync(Account model, IDatabase db, HttpContext context)
        {
            var commandText = """
                update accounts
                set account_name =@account_name,
                    account_number = @account_number,
                    provider_id = @provider_id,
                    editor_id = @editor_id,
                    edited_date = current_timestamp
                where account_id = @id;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("account_name", model.AccountName, DbType.String),
                db.CreateParameter("account_number", model.AccountNumber, DbType.String),
                db.CreateParameter("provider_id", model.Provider, DbType.Int16),
                db.CreateParameter("editor_id", Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("id", model.Id, DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ?
                Results.Ok(CommonResult.Ok("An account update successfully")) :
                Results.Problem("An error occured while updating account, please try again later.");
        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = """
                update accounts
                set is_deleted = true,
                    editor_id = @editor_id,
                    edited_date = current_timestamp
                where account_id = @id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("editor_id", Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("id", id, DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ?
                Results.Ok(CommonResult.Ok("Account successfully deleted")) :
                Results.Problem("An arror occured while deleting account, please try again later");
        }
    }
}
