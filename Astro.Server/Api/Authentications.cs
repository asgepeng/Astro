using Astro.Data;
using Astro.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Astro;
using Astro.Security;
using Npgsql;
using Alaska.Data;

namespace Astro.Server.Api
{
    internal static class Authentications
    {
        internal static void MapAuthEndPoints(this WebApplication app)
        {
            app.MapPost("/auth/login", SignInAsync);
            app.MapPost("/auth/logout", SignOutAsync).RequireAuthorization();
        }
        internal static async Task<IResult> SignInAsync(LoginRequest request, IDatabase db, HttpContext context)
        {
            if (request.Username == "" || request.Password == "") return Results.Ok(LoginResponse.LoginFailed());
            var commandText = """
                UPDATE logins
                    SET is_locked = false, failed_count = 0
                WHERE is_locked = true
                    AND locked_expired < @now;
                SELECT 
                    u.user_id, u.user_name, r.role_id,  r.role_name, l.login_password, l.is_locked, l.failed_count
                FROM logins AS l
                    INNER JOIN users AS u ON l.user_id = l.user_id
                    INNER JOIN roles AS r ON u.role_id = r.role_id
                WHERE l.login_name = @username AND u.is_deleted = false
                """;
            UserInfo? userInfo = null;
            string storedPassword = "";

            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    userInfo = new UserInfo(reader.GetInt16(0), reader.GetString(1), new Role() { Id = reader.GetInt16(2), Name = reader.GetString(3) });
                    userInfo.IsLocked = reader.GetBoolean(5);
                    userInfo.TryCount = reader.GetInt16(6);

                    storedPassword = reader.GetString(4);
                }
            }, commandText, new NpgsqlParameter("username", request.Username), new NpgsqlParameter("now", DateTime.Now));

            if (userInfo != null && userInfo.IsLocked)
            {
                var response = LoginResponse.LoginFailed();
                response.Message = "Akun anda terkunci, mohon tunggu 1 menit sejak terakhir percobaan login";
                return Results.Ok(response);
            }
            if (userInfo is null)
            {
                var notFound = LoginResponse.LoginFailed();
                notFound.Message = "Pengguna tidak ditemukan";
                return Results.Ok(notFound);
            }
            if (storedPassword.Trim() == "") return Results.Ok(LoginResponse.LoginFailed());

            if (BCrypt.Net.BCrypt.Verify(request.Password, storedPassword) && userInfo != null)
            {

                commandText = """
                    INSERT INTO auths
                        (token, user_id, token_expired_date, ipv4_address, user_agent)
                    VALUES
                        (@token, @user_id, @token_expired_date, @ipv4_address, @user_agent)
                    """;
                var token = Guid.NewGuid().ToString();
                var ipv4 = AppHelpers.GetIpAddress(context.Request);
                var parameters = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("token", token),
                    new NpgsqlParameter("user_id", userInfo.Id),
                    new NpgsqlParameter("token_expired_date", DateTime.Now.AddHours(9)),
                    new NpgsqlParameter("ipv4_address", ipv4),
                    new NpgsqlParameter("user_agent", context.Request.Headers["User-Agent"].ToString())
                };
                await db.ExecuteNonQueryAsync(commandText, parameters);
                var response = LoginResponse.LoginSuccess(token);
                response.UserInfo = userInfo;
                return Results.Ok(response);
            }
            else
            {
                if (userInfo != null)
                {
                    commandText = """
                    UPDATE logins
                    SET failed_count = @failed_count, is_locked = @is_locked, locked_expired=@expired
                    WHERE login_name = @username
                    """;

                    userInfo.TryCount += 1;
                    var locked_expired = DateTime.Now.AddMinutes(1);
                    var parameters = new Npgsql.NpgsqlParameter[]
                    {
                        new NpgsqlParameter("failed_count", userInfo.TryCount),
                        new NpgsqlParameter("username", request.Username),
                        new NpgsqlParameter("is_locked", userInfo.TryCount >= 3),
                        new NpgsqlParameter("expired", locked_expired)
                    };
                    await db.ExecuteNonQueryAsync(commandText, parameters);
                    if (failedCount >= 3)
                    {
                        var response = LoginResponse.LoginFailed();
                        response.Message = "Akun anda terkunci sampai dengan " + locked_expired.ToString("dd-MM-yyyy HH:mm:ss");
                        return Results.Ok(response);
                    }
                }
                var nf = LoginResponse.LoginFailed();
                nf.Message = "Kombinasi password dan username tidak cocok";
                return Results.Ok(nf);
            }
            
        }

        internal static async Task<IResult> SignOutAsync(IDatabase db, HttpContext context)
        {
            var token = AppHelpers.GetToken(context.Request).Trim();
            var commandText = """
                DELETE FROM auths
                WHERE token = @token
                """;
            await db.ExecuteNonQueryAsync(commandText, new Npgsql.NpgsqlParameter("token", token));
            return Results.Ok();
        }
    }
}
