using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data;
using System.Security.Claims;
using System.Data.Common;

using Astro.Data;
using Astro.Helpers;

namespace Astro.Server.Middlewares
{
    public class TokenValidator
    {
        private readonly IDatabase db;
        public TokenValidator(IDatabase iDatabase)
        {
            db = iDatabase;
        }
        public async Task ValidateAsync(MessageReceivedContext context)
        {

            string token = Application.GetToken(context.Request);
            string userAgent = context.Request.Headers.UserAgent.ToString();
            string ipv4 = Application.GetIpAddress(context.Request);

            if (Guid.TryParse(token, out Guid key))
            {
                ClaimsPrincipal? principal = null;
                if (!string.IsNullOrEmpty(token))
                {
                    var commandText = """
                    SELECT u.user_id, u.user_name, u.role_id
                    FROM auths AS a
                        INNER JOIN users AS u ON a.user_id = u.user_id
                    WHERE a.token = @token 
                        AND a.user_agent = @user_agent 
                        AND a.ipv4_address = @ipv4
                    """;
                    var parameters = new DbParameter[]
                    {
                        db.CreateParameter("token", key, DbType.Guid),
                        db.CreateParameter("user_agent", userAgent),
                        db.CreateParameter("ipv4", ipv4)
                    };

                    await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                    {
                        if (await reader.ReadAsync())
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Actor, reader.GetInt16(0).ToString()),
                                new Claim(ClaimTypes.Name, reader.GetString(1)),
                                new Claim(ClaimTypes.Role, reader.GetInt16(2).ToString())
                            };
                            ClaimsIdentity identity = new ClaimsIdentity(claims, "Bearer");
                            principal = new ClaimsPrincipal(identity);
                        }
                    }, commandText, parameters);
                }
                context.Principal = principal;
                if (principal != null)
                {
                    context.Success();
                }
                else
                {
                    context.Fail("Unauthorized request");
                }
            }
            else
            {
                context.Fail("Invalid token format");
            }
        }
    }
}
