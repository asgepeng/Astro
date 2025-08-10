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

namespace Astro.Server.Api
{
    internal static class Users
    {
        internal static void MapUserEndPoints(this WebApplication app)
        {
            app.MapGet("/data/users", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/users/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/users", CreateAsync).RequireAuthorization();
            app.MapPut("/data/users", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/users/{id}", DeleteAsync).RequireAuthorization();
            app.MapGet("/data/users/role-options", GetRoleOptionsAsync).RequireAuthorization();
            app.MapPost("/data/users/reset-password", ResetPasswordAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
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
        internal static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            var isWinformApp = context.Request.IsDesktopAppRequest();
            if (isWinformApp)
            {
                var data = await db.GetUser(id);
                return Results.File(data, "application/octet-stream");
            }

            var commandText = """
                SELECT
                    u.user_id,
                    u.user_firstname,
                    u.user_lastname,
                    u.role_id,
                    u.user_name,
                    u.normalized_user_name,
                    u.email,
                    u.email_confirmed,
                    u.phone_number,
                    u.phone_number_confirmed,
                    u.date_of_birth,
                    u.sex,
                    u.marital_status,
                    u.street_address,
                    u.city_id,
                    u.state_id,
                    u.country_id,
                    u.zip_code,
                    u.two_factor_enabled,
                    u.access_failed_count,
                    u.lockout_enabled,
                    u.lockout_end,
                    u.security_stamp,
                    u.concurrency_stamp,
                    u.use_password_expiration,
                    u.password_expiration_date
                FROM
                    users AS u
                WHERE u.user_id = @userId
                    AND u.is_deleted = false
                """;
            User? user = null;
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user = User.Create(reader);
                }
            }, commandText, db.CreateParameter("userId", id, DbType.Int16));
            if (user is null) return Results.NotFound(CommonResult.Fail("User not found."));

            var model = new UserViewModel(user);
            commandText = """
                select role_id, role_name
                from roles
                order by role_name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    model.Roles.Add(new Option()
                    {
                        Id = (int)reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText);
            commandText = """
                SELECT country_id, country_name
                from countries
                order by country_name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    model.Countries.Add(new Option()
                    {
                        Id = (int)reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText);

            if (user.CityId > 0)
            {
                commandText = """
                select c.city_id, st.state_id, cnt.country_id
                from cities AS c
                inner join states as st on c.state_id = st.state_id
                inner join countries AS cnt ON st.country_id = cnt.country_id
                where c.city_id = @city_id
                """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        model.AddressInfo.City = reader.GetInt32(0);
                        model.AddressInfo.State = reader.GetInt16(1);
                        model.AddressInfo.Country = reader.GetInt16(2);
                    }
                }, commandText, db.CreateParameter("city_id", user.CityId, DbType.Int16));
                commandText = """
                SELECT state_id, state_name, country_id
                FROM states
                """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        model.States.Add(new Option()
                        {
                            Id = (int)reader.GetInt16(0),
                            Text = reader.GetString(1)
                        });
                    }
                }, commandText);
                commandText = """
                SELECT city_id, city_name
                from cities
                where state_id = @state_id
                """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        model.Cities.Add(new Option()
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.GetString(1)
                        });
                    }
                }, commandText, db.CreateParameter("state_id", model.AddressInfo.State, DbType.Int16));
            }
            return Results.Ok(model);
        }
        internal static async Task<IResult> CreateAsync(User user, IDatabase db, HttpContext context)
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
        internal static async Task<IResult> UpdateAsync(User user, IDatabase db, HttpContext context)
        {
            var commandText = """
            UPDATE users
            SET
                user_firstname = @firstName,
                user_lastname = @lastName,
                role_id = @roleId,
                user_name = @userName,
                normalized_user_name = @normalizedUserName,
                email = @email,
                phone_number = @phoneNumber,
                date_of_birth = @dateOfBirth,
                sex = @sex,
                marital_status = @maritalStatus,
                street_address = @streetAddress,
                city_id = @cityId,
                state_id = @stateId,
                country_id = @countryId,
                zip_code = @zipCode,
                two_factor_enabled = @twoFactorEnabled,
                lockout_enabled = @lockoutEnabled,
                lockout_end = @lockoutEnd,
                security_stamp = @securityStamp,
                concurrency_stamp = @concurrencyStamp,
                use_password_expiration = @usePasswordExpiration,
                password_expiration_date = @passwordExpirationDate,
                editor_id = @editorId,
                edited_date = @editedDate
            WHERE user_id = @userId;
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
                db.CreateParameter("editorId", Extensions.Application.GetUserID(context), DbType.Int16),
                db.CreateParameter("editedDate", DateTime.UtcNow, DbType.DateTime),
                db.CreateParameter("userId", user.Id, DbType.Int16)
            };



            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("User updated successfully.")) : Results.Ok(CommonResult.Fail("Failed to update user."));
        }
        internal static async Task<IResult> DeleteAsync(int id, IDatabase db, HttpContext context)
        {
            var commandText = """
                UPDATE users SET is_deleted = true
                WHERE user_id = @userId;
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("userId", id, DbType.Int16));
            return success ? Results.Ok(CommonResult.Ok("User deleted successfully.")) : Results.Ok(CommonResult.Fail("Failed to delete user."));
        }
        internal static async Task<IResult> GetRoleOptionsAsync(IDatabase db)
        {
            var commandText = """
                SELECT role_id, role_name
                FROM roles
                ORDER BY role_name
                """;
            using (var builder = new IO.Writer())
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
        internal static async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request, IDatabase db, HttpContext context)
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
