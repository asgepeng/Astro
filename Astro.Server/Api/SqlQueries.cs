using Astro.Cryptography;
using Astro.Data;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class SqlQueries
    {
        internal static void MapSqlQueryEndPoints(this WebApplication app)
        {
            app.MapPost("/api/sql", GetDataTableAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(IDBClient db, HttpContext context)
        {
            var commandText = string.Empty;
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Astro.Binaries.BinaryDataReader(stream))
            {
                var guid = reader.ReadGuid();
                if (guid == Guid.Empty) return Results.BadRequest("Bad request: missing GUID.");

                var key = guid.ToByteArray();
                var encrypted = reader.ReadString();
                commandText = encrypted.Decrypt(key);
            }
            var data = Array.Empty<byte>();
            using (var writer = new Astro.Binaries.BinaryDataWriter())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    await writer.WriteDataTableAsync(reader); 
                }, commandText, db.CreateParameter("location", context.Request.GetLocationID(), System.Data.DbType.Int16));
                if (writer.GetLength() == 0)
                {
                    writer.WriteByte(2);
                    writer.WriteString("An error occured while executing your Sql syntax, please check your syntax and try again.");
                }
                data = writer.ToArray();
                return Results.File(data, "application/octet-stream");
            }            
        }
    }
}
