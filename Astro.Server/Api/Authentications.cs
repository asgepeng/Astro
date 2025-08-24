using Astro.Data;
using Astro.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System.Data.Common;
using System.Data;
using Astro.Server.Binaries;
using Astro.Server.Memory;
using Microsoft.AspNetCore.Routing;
using Astro.Server.Extensions;
using Astro.Extensions;
using System.Dynamic;

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
            app.MapGet("/auth/stores", GetUserLocationsAsync).RequireAuthorization();
            app.MapPut("/auth/stores/{location}", ChangeLocationAsync).RequireAuthorization();
        }
        internal static async Task<IResult> SignInAsync(Credential credential, IDBClient db, HttpContext context)
        {
            if (string.IsNullOrEmpty(credential.Username) || string.IsNullOrEmpty(credential.Password))
            {
                return Results.Unauthorized();
            }
            var loginInfo = await GetLoginInfoAsync(credential.Username, db);
            if (loginInfo is null)
            {
                await CreateLoginHistory(null, LoginResult.NotFound, db, context);
                return Results.Unauthorized();
            }

            if (loginInfo.IsLockedOut())
            {
                await CreateLoginHistory(loginInfo, LoginResult.AccountLocked, db, context);
                return Results.Unauthorized();
            }
            var credentialVerified = loginInfo.VerifyPassword(credential.Password);
            if (!credentialVerified)
            {
                if (loginInfo.LockoutEnabled && await IncrementAccessFailedCountAsync(loginInfo, db))
                {
                    await CreateLoginHistory(loginInfo, LoginResult.AccountLocked, db, context);
                    return Results.Problem("Your account has been temporarily locked due to multiple failed login attempts.");
                }
                else
                {
                    await CreateLoginHistory(loginInfo, LoginResult.InvalidCredential, db, context);
                    return Results.Unauthorized();
                }
            }
            if (loginInfo.IsPasswordExpired())
            {
                await CreateLoginHistory(loginInfo, LoginResult.PasswordExpired, db, context);
                return Results.Forbid();
            }

            var token = await RegisterNewToken(loginInfo, db, context);
            if (string.IsNullOrEmpty(token))
            {
                await CreateLoginHistory(loginInfo, LoginResult.CreateTokenFailed, db, context);
                return Results.Problem("We were unable to generate your access token. Please contact your administrator for assistance");
            }

            if (context.Request.IsDesktopAppRequest())
            {
                using (var writer =new Streams.Writer())
                {
                    var commandText = """
                        SELECT e.fullname, e.roleid, r.name AS rolename
                        FROM employees AS e
                        INNER JOIN roles AS r ON e.roleid = r.roleid
                        WHERE e.employeeid = @id
                        """;
                    writer.WriteInt16(loginInfo.EmployeeId);
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        if (await reader.ReadAsync())
                        {
                            writer.WriteString(reader.GetString(0));
                            writer.WriteInt16(reader.GetInt16(1));
                            writer.WriteString(reader.GetString(2));
                        }
                    }, commandText, db.CreateParameter("id", loginInfo.EmployeeId, DbType.Int16));
                    writer.WriteString(token);

                    var iPos = writer.ReserveInt32();
                    var command = """
                        SELECT l.locationid, l.name
                        FROM employeelocations AS el
                        INNER JOIN locations AS l ON el.locationid = l.locationid
                        WHERE el.employeeid = @id
                        """;
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        var iCount = 0;
                        while (await reader.ReadAsync())
                        {
                            writer.WriteInt16(reader.GetInt16(0));
                            writer.WriteString(reader.GetString(1));
                            iCount++;
                        }
                        writer.WriteInt32(iCount, iPos);
                    }, command, db.CreateParameter("id", loginInfo.EmployeeId));
                    return Results.File(writer.ToArray(), "application/octet-stream");
                }
            }
            else
            {
                var commandText = """
                    SELECT e.fullname, e.roleid, r.name AS rolename
                    FROM employees AS e
                    INNER JOIN roles AS r ON e.roleid = r.roleid
                    WHERE e.employeeid = @id
                    """;
                UserInfo? userInfo = null;
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        userInfo = new UserInfo(loginInfo.EmployeeId, reader.GetString(0), new Role() { Id = reader.GetInt16(1), Name = reader.GetString(2) });
                    }
                }, commandText, db.CreateParameter("id", loginInfo.EmployeeId, DbType.Int16));

                await CreateLoginHistory(loginInfo, LoginResult.Success, db, context);
                return Results.Ok(AuthResponse.Success(token, userInfo));
            }
        }
        private static async Task<IResult> ChangeLocationAsync(short location, IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT 1
                FROM employeelocations
                WHERE employeeid = @id AND locationid = @location
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("id", context.GetUserID(), DbType.Int16),
                db.CreateParameter("location", location, DbType.Int16)
            };
            var dataExists = await db.HasRecordsAsync(commandText, parameters);
            if (dataExists)
            {
                TokenStore.SetLocationId(context.Request.GetToken(), location);
            }
            return Results.Ok(dataExists);
        }
        internal static IResult SignOut(IDBClient db, HttpContext context)
        {
            var token = context.Request.GetToken();
            TokenStore.Delete(token);
            return Results.Ok();
        }
        internal static async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, IDBClient db, HttpContext context)
        {
            var userId = (short)context.GetUserID();
            var loginInfo = await GetLoginInfoAsync(userId, db);
            if (loginInfo is null) return Results.Ok(CommonResult.Fail("User not found"));

            var oldPasswrodVerified = loginInfo.VerifyPassword(request.OldPassword);
            if (!oldPasswrodVerified) return Results.Ok(CommonResult.Fail("Invalid old password"));

            var newPasswordHashed = Astro.Models.Password.HashPassword(request.NewPassword);
            var commandText = """
                UPDATE logins
                SET passwordhash = @pwd
                WHERE employeeid = @id;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("id", userId, DbType.Int16),
                db.CreateParameter("pwd", newPasswordHashed, DbType.AnsiString)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success? Results.Ok(CommonResult.Ok("Password changed successfully")) : Results.Ok(CommonResult.Fail("Failed to change password"));
        }
        internal static async Task<IResult> GetPermissionsAsync(IDBClient db, HttpContext context)
        {
            var roleId = Extensions.Application.GetRoleID(context);
            if (context.Request.IsDesktopAppRequest()) return Results.File(await db.GetUserPermissions(roleId), "application.octet-stream");

            var commandText = """
                SELECT s.sectionid, s.title, m.icon, m.menuid, m.title AS menutitle, rtm.allowcreate, rtm.allowread, rtm.allowupdate, rtm.allowdelete
                FROM rolemenus rtm INNER JOIN
                    menus m ON rtm.menuid = m.menuid INNER JOIN
                    sections s ON m.sectionid = s.sectionid
                WHERE m.isdisabled = false AND rtm.roleid = @roleId
                ORDER BY s.sectionid, m.menuid
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

        private static async Task<Login?> GetLoginInfoAsync(string login, IDBClient db)
        {
            Login? loginInfo = null;
            var commandText = """
                UPDATE logins
                SET accessfailedcount = 0,
                    lockoutend = NULL
                WHERE lockoutend < CURRENT_TIMESTAMP;
                SELECT
                    l.employeeid,
                    e.fullname,
                    e.roleid,
                    l.passwordhash,
                    l.accessfailedcount,
                    l.lockoutenabled,
                    l.lockoutend,
                    l.usepasswordexpiration,
                    l.passwordexpirationdate
                FROM
                    employees AS e
                	INNER JOIN logins AS l ON e.employeeid = l.employeeid
                WHERE (e.email = @login OR e.phone = @login)
                    AND e.isdeleted = false;
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    loginInfo = Login.Create(reader);
                }
            }, commandText, db.CreateParameter("login", login));
            return loginInfo;
        }
        private static async Task<Login?> GetLoginInfoAsync(short id, IDBClient db)
        {
            Login? loginInfo = null;
            var commandText = """
                UPDATE logins
                SET accessfailedcount = 0,
                    lockoutend = NULL
                WHERE lockoutend < CURRENT_TIMESTAMP;
                SELECT
                    l.employeeid,
                    e.fullname,
                    e.roleid,
                    l.passwordhash,
                    l.accessfailedcount,
                    l.lockoutenabled,
                    l.lockoutend,
                    l.usepasswordexpiration,
                    l.passwordexpirationdate
                FROM
                    employees AS e
                	INNER JOIN logins AS l ON e.employeeid = l.employeeid
                WHERE l.employeeid = @id
                    AND e.isdeleted = false;
                """;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    loginInfo = Login.Create(reader);
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return loginInfo;
        }
        private static async Task<bool> IncrementAccessFailedCountAsync(Login loginInfo, IDBClient db)
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
                db.CreateParameter("userId", loginInfo.EmployeeId, DbType.Int16),
                db.CreateParameter("threshold", 3, DbType.Int32)
            };
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    loginInfo.AccessFailedCount = reader.GetInt16(0);
                    loginInfo.LockoutEnd = !reader.IsDBNull(1) ? reader.GetDateTime(1) : null;
                }
            },
            commandText, parameters);

            return loginInfo.HasExceededFailedAttempts(3);
        }
        private static async Task<string> RegisterNewToken(Login loginInfo, IDBClient db, HttpContext context)
        {
            var token = Guid.NewGuid().ToString();
            var ipv4 = Extensions.Application.GetIpAddress(context.Request).ToInet();
            var userAgent = context.Request.Headers.UserAgent.ToString();
            var data = new byte[18];

            var commandText = """
                    SELECT locationid
                    FROM employeelocations AS el
                    WHERE employeeid = @id AND isprimary = true
                    LIMIT 1
                    """;
            var locationId = await db.ExecuteScalarAsync<short>(commandText, db.CreateParameter("id", loginInfo.EmployeeId, DbType.Int16));
            var expiredData = BitConverter.GetBytes(DateTime.Now.AddHours(9).Ticks);

            data.Copy(expiredData, 0);
            data.Copy(ipv4, 8);
            data.Copy(BitConverter.GetBytes(locationId), 12);
            data.Copy(BitConverter.GetBytes(loginInfo.EmployeeId), 14);
            data.Copy(BitConverter.GetBytes(loginInfo.RoleId), 16);

            TokenStore.Set(token, data);
            return token;
        }
        private static async Task CreateLoginHistory(Login? loginInfo, LoginResult loginResult, IDBClient db, HttpContext context)
        {
            var commandText = """
                INSERT INTO login_attempts
                    (userid, ipv4, useragent, issuccess, notes)
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
            var userId = loginInfo is null ? 0 : loginInfo.EmployeeId;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("userId", userId, DbType.Int16),
                db.CreateParameter("ipv4", context.Request.GetIpAddress().ToInet(), DbType.Binary),
                db.CreateParameter("success", loginResult == LoginResult.Success, DbType.Boolean),
                db.CreateParameter("userAgent", userAgent, DbType.AnsiString),
                db.CreateParameter("notes", notes, DbType.AnsiString)
            };

            await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private static async Task<IResult> GetUserLocationsAsync(IDBClient db, HttpContext context)
        {
            using (var writer = new Streams.Writer())
            {
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.ReserveInt32();
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                    writer.WriteInt32(iCount, 0);
                }, "SELECT locationid, name FROM locations");
                var iPos = writer.ReserveInt32();
                var commandText = """
                    SELECT 0 AS employeeid, fullname
                    FROM employees
                    WHERE employeeid = @id AND isdeleted = false
                    UNION ALL
                    SELECT a.accountid, CONCAT(ap.name, ' - ', a.accountname) AS name 
                    FROM accounts AS a 
                    INNER JOIN accountproviders AS ap ON a.providerid = ap.providerid
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    iCount = 0;
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", context.GetUserID(), DbType.Int16));
                writer.WriteInt32(iCount, iPos);
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
    }
}
