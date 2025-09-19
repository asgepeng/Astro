using Astro.Binaries;
using Astro.Data;
using Astro.Models;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;

namespace Astro.Server.Api
{
    internal static class Accounts
    {
        internal static void MapAccountEndPoints(this WebApplication app)
        {
            app.MapGet("/data/accounts/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/accounts", async Task<IResult>(IDBClient db, HttpContext context) =>
            {
                using (var ms = await context.Request.GetMemoryStreamAsync())
                using (var reader = new BinaryDataReader(ms))
                {
                    if (ms.Length == 0) return Results.BadRequest();

                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/accounts", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/accounts/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(Astro.Binaries.BinaryDataReader reader, IDBClient db)
        {
            var listingType = reader.ReadByte();
            if (listingType == 0x00)
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
            else if(listingType == 0x02)
            {
                var commandText = """
                    SELECT acc.accountid, CONCAT(ap.name, ' - ', acc.accountname) AS name 
                    FROM accounts AS acc
                    INNER JOIN accountproviders AS ap ON acc.providerid = ap.providerid
                    WHERE acc.isdeleted = false
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
                SELECT a.accountid, a.accountname, a.accountnumber, a.providerid, p.providertype
                FROM accounts AS a
                INNER JOIN accountproviders AS p ON a.providerid = p.providerid
                WHERE a.accountid = @id AND a.isdeleted = false
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Astro.Binaries.BinaryDataWriter())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteInt16(reader.GetInt16(3));
                        writer.WriteInt16(reader.GetInt16(4));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));

                var iCount = 0;
                var iPos = writer.ReserveInt32();
                commandText = """
                    SELECT providerid, name, providertype
                    FROM accountproviders
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> CreateAsync(Astro.Binaries.BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var commandText = """
                INSERT INTO accounts
                    (accountname, accountnumber, providerid, creatorid)
                VALUES
                    (@accountname, @accountnumber, @providerid, @creatorid)
                """;
            var accountId = reader.ReadInt16();
            if (accountId > 0) return Results.BadRequest();

            var parameters = new DbParameter[]
            {
                db.CreateParameter("accountname", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("accountnumber", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("providerid", reader.ReadInt16(), DbType.Int16),
                db.CreateParameter("creatorid", context.GetUserID(), DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account created succesfully")) : Results.Problem("An error occured while creating account, please try again later.");
        }
        private static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Astro.Binaries.BinaryDataReader(ms))
            {
                if (reader.ReadByte() != 0x01) return Results.BadRequest();
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
                    db.CreateParameter("id", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("accountname", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("accountnumber", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("providerid", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("editorid", context.GetUserID(), DbType.Int16)               
                };
                return await db.ExecuteNonQueryAsync(commandText, parameters) ?
                Results.Ok(CommonResult.Ok("An account update successfully")) :
                Results.Problem("An error occured while updating account, please try again later.");
            }
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
