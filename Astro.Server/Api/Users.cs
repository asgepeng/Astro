using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.Text;
using System.Data;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Routing;
using Astro.Binaries;

namespace Astro.Server.Api
{
    internal static class Users
    {
        internal static void MapUserEndPoints(this WebApplication app)
        {
            app.MapGet("/data/users/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/users", async Task<IResult>(IDBClient db, HttpContext context) =>{
                using (var stream  = await context.Request.GetMemoryStreamAsync())
                using (var reader = new BinaryDataReader(stream))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/users", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/users/{id}", DeleteAsync).RequireAuthorization();
            app.MapGet("/data/users/role-options", GetRoleOptionsAsync).RequireAuthorization();
            app.MapPost("/data/users/reset-password", ResetPasswordAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(BinaryDataReader reader, IDBClient db)
        {
            var listingMode = reader.ReadByte();
            if (listingMode == 0x00)
            {
                var commandText = """
                SELECT e.employeeid, e.fullname, CASE l.creatorid WHEN 0 THEN 'System' ELSE creator.fullname END AS creator, l.createddate
                FROM logins AS l
                INNER JOIN employees AS e ON l.employeeid = e.employeeid
                LEFT JOIN employees AS creator ON l.creatorid = creator.employeeid
                WHERE e.isdeleted = false AND e.employeeid > 1
                """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octetstream");
            }
            else return Results.BadRequest();
        }
        private static async Task<IResult> CreateAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var parameters = new DbParameter[]
            {
                db.CreateParameter("employeeid", reader.ReadInt16(), DbType.Int16),
                db.CreateParameter("lockoutenabled", reader.ReadBoolean(), DbType.Boolean),
                db.CreateParameter("usepasswordexpiration", reader.ReadBoolean(), DbType.Boolean),
                db.CreateParameter("Password", Password.HashPassword(reader.ReadString()), DbType.AnsiString),
                db.CreateParameter("creator", context.GetUserID(), DbType.Int16)
            };
            var commandText = """
                INSERT INTO logins (employeeid, passwordhash, lockoutenabled, usepasswordexpiration, creatorid)
                VALUES (@employeeid, @password, @lockoutenabled, @usepasswordexpiration, @creator);
                """;
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("Succesfully")) : Results.Problem("An error occured while saving data login, please try again later");
        }
        internal static async Task<IResult> GetAllAsync(IDBClient db, HttpContext context)
        {
            if (context.Request.IsDesktopAppRequest())
            {
                return Results.File(await db.GetUserDataTable(), "application/octet-stream");
            }
            else
            {
                var sb = new StringBuilder();
                await sb.AppendUserTableAsync(db);
                return Results.Content(sb.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            using (var writer = new BinaryDataWriter())
            {
                var commandText = """
                    SELECT e.employeeid, e.fullname, l.lockoutenabled, l.usepasswordexpiration
                    FROM logins AS l
                    INNER JOIN employees AS e ON l.employeeid = e.employeeid
                    WHERE l.employeeid = @id
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        var exists = reader.HasRows;
                        writer.WriteBoolean(exists);                        // data found
                        if (exists)
                        {
                            writer.WriteInt16(reader.GetInt16(0));          // employee ID
                            writer.WriteString(reader.GetString(1));        // employee name
                            writer.WriteBoolean(reader.GetBoolean(2));      // lockout enabled
                            writer.WriteBoolean(reader.GetBoolean(3));      // use password expiration
                        }
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        internal static async Task<IResult> CreateAsync(User user, IDBClient db, HttpContext context)
        {
            var commandText = """
                INSERT INTO users (
                    user_firstname,
                    user_lastname,
                    role_id,
                    user_name,
                    normalized_user_name,
                    email,
                    phone_number,
                    date_of_birth,
                    sex,
                    marital_status,
                    street_address,
                    city_id,
                    state_id,
                    country_id,
                    zip_code,
                    two_factor_enabled,
                    lockout_enabled,
                    lockout_end,
                    security_stamp,
                    concurrency_stamp,
                    use_password_expiration,
                    password_expiration_date,
                    creator_id,
                    created_date
                )
                VALUES (
                    @firstName,
                    @lastName,
                    @roleId,
                    @userName,
                    @normalizedUserName,
                    @email,
                    @phoneNumber,
                    @dateOfBirth,
                    @sex,
                    @maritalStatus,
                    @streetAddress,
                    @cityId,
                    @stateId,
                    @countryId,
                    @zipCode,
                    @twoFactorEnabled,
                    @lockoutEnabled,
                    @lockoutEnd,
                    @securityStamp,
                    @concurrencyStamp,
                    @usePasswordExpiration,
                    @passwordExpirationDate,
                    @creatorId,
                    @createdDate
                );
                """;

            var parameters = new DbParameter[]
            {
                db.CreateParameter("firstName", user.FirstName, DbType.String),
                db.CreateParameter("lastName", user.LastName, DbType.String),
                db.CreateParameter("roleId", user.RoleId, DbType.Int16),
                db.CreateParameter("userName", user.UserName, DbType.String),
                db.CreateParameter("normalizedUserName", user.UserName.ToUpperInvariant(), DbType.String),
                db.CreateParameter("email", user.Email, DbType.String),
                db.CreateParameter("phoneNumber", user.PhoneNumber, DbType.String),
                db.CreateParameter("dateOfBirth", user.DateOfBirth, DbType.Date),
                db.CreateParameter("sex", user.Sex, DbType.Int16),
                db.CreateParameter("maritalStatus", user.MaritalStatus, DbType.Int16),
                db.CreateParameter("streetAddress", user.StreetAddress, DbType.String),
                db.CreateParameter("cityId", user.CityId, DbType.Int32),
                db.CreateParameter("stateId", user.StateId, DbType.Int16),
                db.CreateParameter("countryId", user.CountryId, DbType.Int16),
                db.CreateParameter("zipCode", user.ZipCode, DbType.String),
                db.CreateParameter("twoFactorEnabled", user.TwoFactorEnabled, DbType.Boolean),
                db.CreateParameter("lockoutEnabled", user.LockoutEnabled, DbType.Boolean),
                db.CreateParameter("lockoutEnd", (object?)user.LockoutEnd ?? DBNull.Value, DbType.DateTime),
                db.CreateParameter("securityStamp", Guid.NewGuid(), DbType.Guid),
                db.CreateParameter("concurrencyStamp", DateTime.UtcNow, DbType.DateTime),
                db.CreateParameter("usePasswordExpiration", user.UsePasswordExpiration, DbType.Boolean),
                db.CreateParameter("passwordExpirationDate", (object?)user.PasswordExpirationDate ?? DBNull.Value, DbType.DateTime),
                db.CreateParameter("creatorId", Extensions.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("createdDate", DateTime.UtcNow, DbType.DateTime)
            };


            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("User created successfully.")) : Results.Ok(CommonResult.Fail("Failed to create user."));
        }
        internal static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new BinaryDataReader(ms))
            {
                if (ms.Length == 0) return Results.BadRequest();
                if (ms.Length != context.Request.ContentLength) return Results.BadRequest();

                var listParameter = new List<DbParameter>();
                var commandText = """
                    UPDATE logins
                    SET usepasswordexpiration = @upx,
                        lockoutenabled = @le
                    """;

                listParameter.Add(db.CreateParameter("id", reader.ReadInt16(), DbType.Int16));
                listParameter.Add(db.CreateParameter("upx", reader.ReadBoolean(), DbType.Boolean));
                listParameter.Add(db.CreateParameter("le", reader.ReadBoolean(), DbType.Boolean));
                if (reader.ReadBoolean())
                {
                    listParameter.Add(db.CreateParameter("pwd", Password.HashPassword(reader.ReadString()), DbType.AnsiString));
                    commandText += ", passwordhash=@pwd";
                }

                commandText += " WHERE employeeid=@id";
                return await db.ExecuteNonQueryAsync(commandText, listParameter.ToArray()) ? 
                    Results.Ok(CommonResult.Ok("Login successfully updated")) :
                    Results.Problem("An error occured while updating data login, please try again later.");
            }
           
        }
        internal static async Task<IResult> DeleteAsync(int id, IDBClient db, HttpContext context)
        {
            var commandText = """
                UPDATE users SET is_deleted = true
                WHERE user_id = @userId;
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("userId", id, DbType.Int16));
            return success ? Results.Ok(CommonResult.Ok("User deleted successfully.")) : Results.Ok(CommonResult.Fail("Failed to delete user."));
        }
        internal static async Task<IResult> GetRoleOptionsAsync(IDBClient db)
        {
            var commandText = """
                SELECT role_id, role_name
                FROM roles
                ORDER BY role_name
                """;
            using (var builder = new Astro.Binaries.BinaryDataWriter())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await (reader.ReadAsync()))
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                    }
                }, commandText);
                return Results.File(builder.ToArray(), "application/octet-stream");
            }
        }
        internal static async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request, IDBClient db, HttpContext context)
        {
            var commandText = """
                UPDATE users
                SET password_hash = @password
                WHERE user_id = @id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("password", request.Password, DbType.String),
                db.CreateParameter("id", request.UserId, DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok()) : Results.Problem();
        }
    }
}
