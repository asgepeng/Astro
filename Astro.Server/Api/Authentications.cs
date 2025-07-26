using Astro.Data;
using Astro.Models;
using Astro.Helpers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System.Data.Common;
using System.Data;
using Astro.Server.Binaries;
using Astro.Server.Memory;
using Microsoft.AspNetCore.Routing;
using Astro.Server.Extensions;

namespace Astro.Server.Api
{
    internal enum LoginResult
    {
        Success, AccountLocked, InvalidCredential, NotFound, CreateTokenFailed, PasswordExpired
    }
    internal static class Authentications
    {
        internal static void MapAuthEndPoints(this WebApplication app)
        {
            app.MapPost("/auth/login", SignInAsync);
            app.MapPost("/auth/logout", SignOut).RequireAuthorization();
            app.MapPost("/auth/change-password", ChangePasswordAsync).RequireAuthorization();
            app.MapGet("/auth/permissions", GetPermissionsAsync).RequireAuthorization();
        }
        internal static async Task<IResult> SignInAsync(Credential credential, IDatabase db, HttpContext context)
        {
            if (string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password))
            {
                return Results.Unauthorized();
            }

            var user = await GetUserByLoginAsync(credential.Username, db);
            if (user is null)
            {
                await CreateLoginHistory(null, LoginResult.NotFound, db, context);
                return Results.Unauthorized();
            }

            if (user.IsLockedOut())
            {
                await CreateLoginHistory(user, LoginResult.AccountLocked, db, context);
                return Results.Unauthorized();
            }
            var credentialVerified = user.VerifyPassword(credential.Password);
            if (!credentialVerified)
            {
                if (user.LockoutEnabled && await IncrementAccessFailedCountAsync(user, db))
                {
                    await CreateLoginHistory(user, LoginResult.AccountLocked, db, context);
                    return Results.Problem("Your account has been temporarily locked due to multiple failed login attempts.");
                }
                else
                {
                    await CreateLoginHistory(user, LoginResult.InvalidCredential, db, context);
                    return Results.Unauthorized();
                }
            }
            if (user.IsPasswordExpired())
            {
                await CreateLoginHistory(user, LoginResult.PasswordExpired, db, context);
                return Results.Forbid();
            }

            var token = RegisterNewToken(user, context);
            if (string.IsNullOrEmpty(token))
            {
                await CreateLoginHistory(user, LoginResult.CreateTokenFailed, db, context);
                return Results.Problem("We were unable to generate your access token. Please contact your administrator for assistance");
            }

            if (context.Request.IsDesktopAppRequest())
            {
                using (var writer =new IO.Writer())
                {
                    writer.WriteInt16(user.Id);
                    writer.WriteString((user.FirstName + " " + user.LastName).Trim());
                    writer.WriteInt16(user.RoleId);
                    writer.WriteString(await GetRoleNameAsync(user.RoleId, db));
                    writer.WriteString(token);

                    return Results.File(writer.ToArray(), "application/octet-stream");
                }
            }
            else
            {
                var role = await GetRoleAsync(user.RoleId, db);
                var userInfo = new UserInfo(user.Id, user.GetFullName(), role);

                await CreateLoginHistory(user, LoginResult.Success, db, context);
                return Results.Ok(AuthResponse.Success(token, userInfo));
            }
        }

        internal static IResult SignOut(IDatabase db, HttpContext context)
        {
            var token = Application.GetToken(context.Request).Trim();
            TokenStore.Delete(token);
            return Results.Ok();
        }
        internal static async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, IDatabase db, HttpContext context)
        {
            var userId = (short)Application.GetUserID(context);
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
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", user.Id),
                db.CreateParameter("newPassword", newPasswordHashed)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success? Results.Ok(CommonResult.Ok("Password changed successfully")) : Results.Ok(CommonResult.Fail("Failed to change password"));
        }
        internal static async Task<IResult> GetPermissionsAsync(IDatabase db, HttpContext context)
        {
            var roleId = Application.GetRoleID(context);
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetUserPermissions(roleId), "application.octet-stream");

            var commandText = """
                SELECT s.section_id, s.section_title, m.menu_id, m.menu_title, rtm.allow_create, rtm.allow_read, rtm.allow_update, rtm.allow_delete
                FROM role_to_menus rtm INNER JOIN
                    menus m ON rtm.menu_id = m.menu_id INNER JOIN
                    sections s ON m.section_id = s.section_id
                WHERE m.is_disabled = false AND rtm.role_id = @roleId
                ORDER BY s.section_id, m.menu_id
                """;
            var listMenu = new ListMenu();
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                Menu? parent = null;
                while (await reader.ReadAsync())
                {
                    var parentId = reader.GetInt16(0);
                    var parentName = reader.GetString(1);
                    if (parent is null) parent = listMenu.Add(parentId, parentName);
                    else
                    {
                        if (parent.Id != parentId) parent = listMenu.Add(parentId, parentName);
                    }
                    parent.Items.Add(new Menu()
                    {
                        Id = reader.GetInt16(2),
                        Title = reader.GetString(3)
                    });
                }
            },commandText, db.CreateParameter("roleId", roleId));
            return Results.Ok(listMenu);
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
                    use_password_expiration,
                    password_expiration_date,
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
            }, commandText, db.CreateParameter("login", login));
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
                    use_password_expiration,
                    password_expiration_date,
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
            }, commandText, db.CreateParameter("userId", id));
            return user;
        }
        private static async Task<bool> IncrementAccessFailedCountAsync(User user, IDatabase db)
        {
            var commandText = """
                UPDATE users
                SET
                    access_failed_count = access_failed_count + 1,
                    lockout_end = CASE WHEN (access_failed_count + 1) >= @threshold THEN CURRENT_TIMESTAMP + INTERVAL '5 minutes' ELSE NULL END
                WHERE 
                    user_id = @userId
                RETURNING access_failed_count, lockout_end;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", user.Id, DbType.Int16),
                db.CreateParameter("threshold", 3, DbType.Int32)
            };
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user.AccessFailedCount = reader.GetInt16(0);
                    if (!reader.IsDBNull(1))
                    {
                        user.LockoutEnd = reader.GetDateTime(1);
                    }
                    else
                    {
                        user.LockoutEnd = null;
                    }
                }
            },
            commandText, parameters);

            return user.HasExceededFailedAttempts(3);
        }
        private static string RegisterNewToken(User user, HttpContext context)
        {
            var token = Guid.NewGuid().ToString();
            var ipv4 = Application.GetIpAddress(context.Request);
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var data = Array.Empty<byte>();
            using (var writer = new IO.Writer())
            {
                writer.WriteDateTime(DateTime.Now.AddHours(9));
                writer.WriteString(ipv4);
                writer.WriteString(userAgent);

                writer.WriteString(user.GetFullName().Trim());
                writer.WriteInt16(user.Id);
                writer.WriteInt16(user.RoleId);

                data = writer.ToArray();
            }
            TokenStore.Set(token, data);
            return token;
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
                    case LoginResult.PasswordExpired: return "Password expired";
                    default: return "";
                }
            };
            var notes = getDescription(loginResult);
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var userId = user is null ? 0 : user.Id;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", userId),
                db.CreateParameter("ipv4", Application.GetIpAddress(context.Request)),
                db.CreateParameter("success", loginResult == LoginResult.Success),
                db.CreateParameter("userAgent", userAgent),
                db.CreateParameter("notes", notes)
            };

            await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private static async Task<string> GetRoleNameAsync(short roleId, IDatabase db)
        {
            var result = await db.ExecuteScalarAsync("SELECT role_name FROM roles WHERE role_id = @roleId",db.CreateParameter("roleId", roleId));
            return result as string ?? string.Empty;
        }
        private static async Task<Role> GetRoleAsync(short roleId, IDatabase db)
        {
            var role = new Role();
            var commandText = """
                SELECT role_id, role_name
                FROM roles
                WHERE role_id = @roleId
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    role.Id = reader.GetInt16(0);
                    role.Name = reader.GetString(1);
                }
            }, commandText, db.CreateParameter("roleId", roleId));
            return role;
        }
    }
}
