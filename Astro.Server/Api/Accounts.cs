using Astro.Data;
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
        private static async Task<IResult> GetAllAsync(IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT acc.accountid, acc.accountname, acc.accountnumber, 
                CASE ap.providertype WHEN 1 THEN 'Bank' WHEN 2 THEN 'E-Wallet' WHEN 3 THEN 'E-Money' ELSE '-' END AS accounttype, 
                ap.name, u.fullname, acc.createddate, acc.editeddate
                FROM accounts AS acc
                INNER JOIN accountproviders AS ap ON acc.providerid = ap.providerid
                INNER JOIN employees AS u ON acc.creatorid = u.employeeid
                WHERE acc.isdeleted = false
                """;
            return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetAccount(id), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> CreateAsync(Account model, IDBClient db, HttpContext context)
        {
            var commandText = """
                insert into accounts
                    (accountname, accountnumber, providerid, creatorid)
                values
                    (@accountname, @accountnumber, @providerid, @creatorid)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("accountname", model.AccountName, DbType.AnsiString),
                db.CreateParameter("accountnumber", model.AccountNumber, DbType.AnsiString),
                db.CreateParameter("providerid", model.Provider, DbType.Int16),
                db.CreateParameter("creatorid", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account created succesfully")) : Results.Problem("An error occured while creating account, please try again later.");
        }
        private static async Task<IResult> UpdateAsync(Account model, IDBClient db, HttpContext context)
        {
            var commandText = """
                UPDATE accounts
                SET accountname =@accountname,
                    accountnumber = @accountnumber,
                    providerid = @providerid,
                    editorid = @editorid,
                    editeddate = current_timestamp
                WHERE accountid = @id;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("accountname", model.AccountName, DbType.AnsiString),
                db.CreateParameter("accountnumber", model.AccountNumber, DbType.AnsiString),
                db.CreateParameter("providerid", model.Provider, DbType.Int16),
                db.CreateParameter("editorid", Extensions.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("id", model.Id, DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ?
                Results.Ok(CommonResult.Ok("An account update successfully")) :
                Results.Problem("An error occured while updating account, please try again later.");
        }
        private static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = """
                UPDATE accounts
                SET isdeleted = true,
                    editorid = @editorid,
                    editeddate = current_timestamp
                WHERE accountid = @id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("editorid", Extensions.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("id", id, DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ?
                Results.Ok(CommonResult.Ok("Account successfully deleted")) :
                Results.Problem("An arror occured while deleting account, please try again later");
        }
    }
}
