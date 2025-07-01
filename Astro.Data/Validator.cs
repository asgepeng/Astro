using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Data
{
    public class Validator
    {
        private readonly DatabaseClient db;
        public Validator(IDatabase iDatabase)
        {
            db = (DatabaseClient)iDatabase;
        }
        public async Task<(ClaimsPrincipal?, string message)> ValidateTokenAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var commandText = """
                SELECT u.user_id, u.user_name, u.role_id
                FROM auths AS a
                INNER JOIN users AS u ON a.user_id = u.user_id
                WHERE a.token = @token
                """;
                
                ClaimsPrincipal? principal = null;
                string message = "Unauthorized request";
                await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                {
                    if (await reader.ReadAsync())
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Actor, reader.GetInt16(0).ToString()),
                            new Claim(ClaimTypes.Name, reader.GetString(1)),
                            new Claim(ClaimTypes.Role, reader.GetInt16(2).ToString()),
                            new Claim(ClaimTypes.NameIdentifier, token)
                        };
                        ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer");
                        principal = new ClaimsPrincipal(identity);
                        message = "Token is valid";
                    }
                }, commandText, new Npgsql.NpgsqlParameter("token", token));
                return (principal, message);
            }
            return (null, "Invalid token format");
        }
    }
}
