using Astro.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Api
{
    internal static class Logins
    {
        internal static void MapLoginEndPoints(this WebApplication app)
        {

        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            using (var builder = new IO.Writer())
            {
                var commandText = """
                    SELECT u.user_id, CONCAT(u.user_firstname, ' ', u.user_lastname) AS fullname
                    FROM users AS u
                    WHERE u.user_id = @id
                    """;
                commandText = """
                    SELECT u.user_id, CONCAT(u.user_firstname, ' ', u.user_lastname) AS fullname
                    FROM users AS u
                    WHERE u.user_id NOT IN (
                        SELECT user_id
                        FROM logins
                    )
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {

                }, commandText);
                return Results.File(builder.ToArray(), "application/octet-strea");
            }
        }
    }
}
