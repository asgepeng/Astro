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
        private static async Task<IResult> GetDataTableAsync(IDatabase db, HttpContext context)
        {
            var commandText = string.Empty;
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new IO.Reader(stream))
            {
                var key = reader.ReadByte();
                var encrypted = reader.ReadString();
                commandText = Cryptography.SimpleEncryption.Decrypt(encrypted, key);
            }

            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    await writer.WriteDataTableAsync(reader); 
                }, commandText);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
    }
}
