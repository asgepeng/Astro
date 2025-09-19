using Astro.Binaries;
using Astro.Data;
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
            app.MapGet("/data/account-providers/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/account-providers", async Task<IResult>(IDBClient db, HttpContext context) =>
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
            app.MapPut("/data/account-providers", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/account-providers/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(BinaryDataReader reader, IDBClient db)
        {
            var listingMode = reader.ReadByte();
            if (listingMode == 0x00)
            {
                var commandText = """
                SELECT providerid, name, case providertype when 1 then 'Bank' when 2 then 'E-Wallet' when 3 then 'E-Money' else '-' end as providertype
                FROM accountproviders
                """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingMode == 0x01)
            {
                //pagination model, not implemented right now
                return Results.BadRequest();
            }
            else if (listingMode == 0x02)
            {
                var commandText = """
                    SELECT providerid, name
                    FROM accountproviders
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
                SELECT providerid, name, providertype
                FROM accountproviders
                WHERE providerid = @id;
                """;
            using (var writer = new BinaryDataWriter())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                    }
                }, commandText, db.CreateParameter("id", id));
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> CreateAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var providerid = reader.ReadInt16();
            var providername = reader.ReadString();
            var providertype = reader.ReadInt16();

            var sqlCheck = """
                SELECT name
                FROM accountproviders
                WHERE providerid=@id
                """;
            var result = await db.ExecuteScalarAsync<string>(sqlCheck, db.CreateParameter("id", providerid));
            if (result != null)
            {
                return Results.Problem("ID: " + providerid.ToString() + " saat sudah digunakan oleh '" + result.ToString() + "', silakan gunakan ID lain");
            }
            var commandText = """
                INSERT INTO accountproviders
                    (providerid, name, providertype)
                VALUES (@providerid, @name, @providertype)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("providerid", providerid),
                db.CreateParameter("name", providername),
                db.CreateParameter("providertype", providertype)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account provider was successfully created")) : Results.Problem("An error occured while creating account providers, please try again later");
        }
        private static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new BinaryDataReader(ms))
            {
                var commandText = """
                UPDATE accountproviders
                SET name = @name,
                    providertype = @providertype
                WHERE providerid = @id
                """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("id", reader.ReadInt16(), System.Data.DbType.Int16),
                    db.CreateParameter("name", reader.ReadString(), System.Data.DbType.String),
                    db.CreateParameter("providertype", reader.ReadInt16(), System.Data.DbType.Int16)
                };
                return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Account provider was successfully created")) : Results.Problem("An error occured while creating account providers, please try again later");
            }
        }
        private static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var sqlCheck = """
                SELECT COUNT(*) AS total
                FROM accounts
                WHERE providerid = @id
                """;
            var accountUse = await db.ExecuteScalarAsync<int>(sqlCheck, db.CreateParameter("id", id));
            if (accountUse > 0)
            {
                return Results.Problem("You cannot delete account provider that being used by account record");
            }
            var commandText = """
                DELETE FROM accountproviders
                WHERE providerid = @id
                """;
            return await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("id", id)) ? Results.Ok(CommonResult.Ok("Account provider was succesfully deleted")) : Results.Problem("An error occurred while deleting account provider, please try again later");
        }
    }
}
