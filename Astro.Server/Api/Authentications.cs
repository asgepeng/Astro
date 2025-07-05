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
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;

namespace Astro.Server.Api
{
    internal enum LoginResult
    {
        Success, AccountLocked, InvalidCredential, NotFound, CreateTokenFailed
    }
    internal static class Authentications
    {
        internal static void MapAuthEndPoints(this WebApplication app)
        {
            app.MapPost("/auth/login", SignInAsync);
            app.MapPost("/auth/logout", SignOutAsync).RequireAuthorization();
            app.MapPost("/auth/change-password", ChangePasswordAsync).RequireAuthorization();
            app.MapGet("/auth/permissions", GetPermissionsAsync).RequireAuthorization();
        }
        internal static async Task<IResult> SignInAsync(Credential credential, IDatabase db, HttpContext context)
        {
            if (string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password))
            {
                return Results.Ok(AuthResponse.Fail());
            }

            var user = await GetUserByLoginAsync(credential.Username, db);
            if (user is null)
            {
                await CreateLoginHistory(null, LoginResult.NotFound, db, context);
                return Results.Ok(AuthResponse.Fail());
            }

            if (user.LockoutEnabled && user.IsLockedOut())
            {
                await CreateLoginHistory(user, LoginResult.AccountLocked, db, context);
                return Results.Ok(AuthResponse.Lockout(user.LockoutEnd));
            }

            var credentialVerified = user.VerifyPassword(credential.Password);
            if (!credentialVerified)
            {
                var loginResult = LoginResult.InvalidCredential;
                var response = AuthResponse.Fail();
                if (user.LockoutEnabled)
                {
                    await IncrementAccessFailedCountAsync(user, db);
                    if (user.AccessFailedCount >= 3)
                    {
                        loginResult = LoginResult.AccountLocked;
                        response.Message = "Your account has been temporarily locked due to multiple failed login attempts.";
                    }
                }
                await CreateLoginHistory(user, loginResult, db, context);
                return Results.Ok(response);
            }

            var token = await CreateTokenAsync(user, db, context);
            if (string.IsNullOrEmpty(token))
            {
                await CreateLoginHistory(user, LoginResult.CreateTokenFailed, db, context);
                return Results.Ok(AuthResponse.Fail("Token generation failed. Please try again later."));
            }
            var role = new Role()
            {
                Id = user.RoleId,
                Name = await GetRoleNameAsync(user.RoleId, db)
            };
            var userInfo = new UserInfo(user.Id, user.GetFullName(), role );
            await CreateLoginHistory(user, LoginResult.Success, db, context);
            return Results.Ok(AuthResponse.Success(token, userInfo));
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
        internal static async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, IDatabase db, HttpContext context)
        {
            var userId = (short)AppHelpers.GetUserID(context);
            var user = await GetUserByIdAsync(userId, db);
            if (user is null) return Results.Ok(CommonResult.Fail("User not found"));

            var oldPasswrodVerified = user.VerifyPassword(request.OldPassword);
            if (!oldPasswrodVerified) return Results.Ok(CommonResult.Fail("Invalid old password"));

            var newPasswordHashed = Astro.Models.Password.HashPassword(request.NewPassword);
            var commandText = """
                UPDATE users
                SET password_hash = @newPassword,
                    concurrency_stamp = CURRENT_TIMESTAMP
                WHERE user_id = @userId;
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter("userId", user.Id), new NpgsqlParameter("newPassword", newPasswordHashed));
            return success? Results.Ok(CommonResult.Ok("Password changed successfully")) : Results.Ok(CommonResult.Fail("Failed to change password"));
        }
        internal static async Task<IResult> GetPermissionsAsync(IDatabase db, HttpContext context)
        {
            return await Task.FromResult(Results.Ok());
        }

        private static async Task<User?> GetUserByLoginAsync(string login, IDatabase db)
        {
            User? user = null;
            var commandText = """
                UPDATE users
                SET access_failed_count = 0,
                    lockout_end = NULL
                WHERE
                    user_name = @login
                    AND is_deleted = false
                    AND lockout_end < CURRENT_TIMESTAMP;
                SELECT
                    user_id,
                    user_firstname,
                    user_lastname,
                    role_id,
                    user_name,
                    normalized_user_name,
                    email,
                    email_confirmed,
                    phone_number,
                    phone_number_confirmed,
                    date_of_birth,
                    sex,
                    marital_status,
                    street_address,
                    city_id,
                    zip_code,
                    two_factor_enabled,
                    access_failed_count,
                    lockout_enabled,
                    lockout_end,
                    security_stamp,
                    concurrency_stamp,
                    password_hash
                FROM
                    users
                WHERE user_name = @login
                    AND is_deleted = false
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user = User.Create(reader);
                }
            }, commandText, new NpgsqlParameter("login", login));
            return user;
        }
        private static async Task<User?> GetUserByIdAsync(short id, IDatabase db)
        {
            if (id <= 0) return null;

            User? user = null;
            var commandText = """
                SELECT
                    user_id,
                    user_firstname,
                    user_lastname,
                    role_id,
                    user_name,
                    normalized_user_name,
                    email,
                    email_confirmed,
                    phone_number,
                    phone_number_confirmed,
                    date_of_birth,
                    sex,
                    marital_status,
                    street_address,
                    city_id,
                    zip_code,
                    two_factor_enabled,
                    access_failed_count,
                    lockout_enabled,
                    lockout_end,
                    security_stamp,
                    concurrency_stamp,
                    password_hash
                FROM
                    users
                WHERE user_id = @userId
                    AND is_deleted = false
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user = User.Create(reader);
                }
            }, commandText, new NpgsqlParameter("userId", id));
            File.WriteAllText("db.GetUserByIdAsync.txt", id.ToString());
            return user;
        }
        private static async Task IncrementAccessFailedCountAsync(User user, IDatabase db)
        {
            var commandText = """
                UPDATE users
                SET
                    access_failed_count = access_failed_count + 1,
                    lockout_end = CASE WHEN (access_failed_count + 1) >= 3 THEN CURRENT_TIMESTAMP + INTERVAL '5 minutes' ELSE NULL END
                WHERE 
                    user_id = @userId;
                SELECT access_failed_count, lockout_end
                FROM users
                WHERE
                    user_id = @userId;
                """;

            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user.AccessFailedCount = reader.GetInt16(0);
                    if (!reader.IsDBNull(1))
                    {
                        user.LockoutEnd = reader.GetDateTime(1);
                    }
                }
            }, commandText, new NpgsqlParameter("userId", user.Id));
        }
        private static async Task<string> CreateTokenAsync(User user, IDatabase db, HttpContext context)
        {
            var token = Guid.NewGuid().ToString();
            var ipv4 = AppHelpers.GetIpAddress(context.Request);
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var commandText = """
                UPDATE users
                SET
                    access_failed_count = 0,
                    lockout_end = NULL
                WHERE
                    user_id = @user_id;
                INSERT INTO auths
                    (token, user_id, token_expired_date, ipv4_address, user_agent)
                VALUES
                    (@token, @user_id, @token_expired_date, @ipv4_address, @user_agent)
                """;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("token", token),
                new NpgsqlParameter("user_id", user.Id),
                new NpgsqlParameter("token_expired_date", DateTime.Now.AddHours(9)),
                new NpgsqlParameter("ipv4_address", ipv4),
                new NpgsqlParameter("user_agent", userAgent)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? token : string.Empty;
        }
        private static async Task CreateLoginHistory(User? user, LoginResult loginResult, IDatabase db, HttpContext context)
        {
            var commandText = """
                INSERT INTO login_attempts
                    (user_id, ipv4_address, user_agent, is_success, notes)
                VALUES
                    (@userId, @ipv4, @userAgent, @success, @notes);
                """;
            var getDescription = string (LoginResult result) =>
            {
                switch (loginResult)
                {
                    case LoginResult.Success: return "Login success";
                    case LoginResult.AccountLocked: return "Account locked";
                    case LoginResult.InvalidCredential: return "Invalid Credential";
                    case LoginResult.NotFound: return "Not registered account";
                    case LoginResult.CreateTokenFailed: return "Token generation failed";
                    default: return "";
                }
            };
            var notes = getDescription(loginResult);
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var userId = user is null ? 0 : user.Id;
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userId", userId),
                new NpgsqlParameter("ipv4", AppHelpers.GetIpAddress(context.Request)),
                new NpgsqlParameter("success", loginResult == LoginResult.Success),
                new NpgsqlParameter("userAgent", userAgent),
                new NpgsqlParameter("notes", notes)
            };
            await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private static async Task<string> GetRoleNameAsync(short roleId, IDatabase db)
        {
            var result = await db.ExecuteScalarAsync("SELECT role_name FROM roles WHERE role_id = @roleId", new Npgsql.NpgsqlParameter("roleId", roleId));
            return result as string ?? string.Empty;
        }
    }
}
