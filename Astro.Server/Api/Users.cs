using Alaska.Data;
using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            app.MapGet("data/users/role-options", GetRoleOptionsAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            var winFormApp = AppHelpers.IsWinformApp(context.Request);
            var commandText = """
                    select u.user_id, concat(u.user_firstname, ' ', u.user_lastname) AS fullname, u.email, r.role_name,
                    case when u.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, u.created_date
                    from users as u
                    inner join roles as r on u.role_id = r.role_id
                    left join users AS c ON u.creator_id = c.user_id
                    where u.is_deleted = false
                    """;
            if (winFormApp)
            {
                var data = Array.Empty<byte>();
                using (var builder = new BinaryBuilder())
                {
                    await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0)); // user_id
                            builder.WriteString(reader.GetString(1)); // fullname
                            builder.WriteString(reader.GetString(2)); // email
                            builder.WriteString(reader.GetString(3)); // role_name
                            builder.WriteString(reader.GetString(4)); // creator
                            builder.WriteDateTime(reader.GetDateTime(5)); // created_date
                        }
                    }, commandText);
                    data = builder.ToArray();
                }
                return Results.File(data, "application/octet-stream");
            }
            else
            {
                var builder = new StringBuilder();
                builder.Append("<table class=\"data-table\"><thead><tr><th>User ID</th><th>Full Name</th><th>Email</th><th>Role</th><th>Creator</th><th>Created Date</th></tr></thead><tbody>");
                await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.Append("<tr>");
                        builder.AppendFormat("<td>{0}</td>", reader.GetInt16(0));
                        builder.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(1))).Append("</td>");
                        builder.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(2))).Append("</td>");
                        builder.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(3))).Append("</td>");
                        builder.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(4))).Append("</td>");
                        builder.Append("<td>").Append(reader.GetDateTime(5).ToString("dd/MM/yyyy HH:mm")).Append("</td>");
                        builder.Append("</tr>");
                    }
                }, commandText);
                builder.Append("</tbody></table>");
                return Results.Content(builder.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(int id, IDatabase db, HttpContext context)
        {
            if (id <= 0) return Results.Ok();

            User? user = null;
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
                    u.password_expiration_date,
                FROM
                    users AS u
                WHERE u.user_id = @userId
                    AND u.is_deleted = false
                """;
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            if (isWinformApp)
            {
                using (var builder = new BinaryBuilder())
                {
                    var addressInfo = new AddressInfo();
                    await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                    {
                        if (await reader.ReadAsync())
                        {
                            addressInfo.City = reader.GetInt32(14); // city_id
                            addressInfo.State = reader.GetInt16(15); // state_id
                            addressInfo.Country = reader.GetInt16(16); // country_id

                            builder.WriteInt16(reader.GetInt16(0)); // user_id
                            builder.WriteString(reader.GetString(1)); // user_firstname
                            builder.WriteString(reader.GetString(2)); // user_lastname
                            builder.WriteInt16(reader.GetInt16(3)); // role_id
                            builder.WriteString(reader.GetString(4)); // user_name
                            builder.WriteString(reader.GetString(5)); // normalized_user_name
                            builder.WriteString(reader.GetString(6)); // email
                            builder.WriteBoolean(reader.GetBoolean(7)); // email_confirmed
                            builder.WriteString(reader.GetString(8)); // phone_number
                            builder.WriteBoolean(reader.GetBoolean(9)); // phone_number_confirmed
                            builder.WriteDateTime(reader.GetDateTime(10)); // date_of_birth
                            builder.WriteInt16(reader.GetByte(11)); // sex
                            builder.WriteInt16(reader.GetByte(12)); // marital_status
                            builder.WriteString(reader.GetString(13)); // street_address 
                            builder.WriteInt32(addressInfo.City); // city_id
                            builder.WriteInt16(addressInfo.State);
                            builder.WriteInt16(addressInfo.Country);
                            builder.WriteString(reader.GetString(17)); // zip_code
                            builder.WriteBoolean(reader.GetBoolean(18)); // two_factor_enabled
                            builder.WriteInt16(reader.GetInt16(19)); // access_failed_count
                            builder.WriteBoolean(reader.GetBoolean(20)); // lockout_enabled
                            long lockoutEndTicks = reader.IsDBNull(21) ? 0 : reader.GetDateTime(21).Ticks;
                            builder.WriteInt64(lockoutEndTicks); // lockout_end (nullable)
                            builder.WriteString(reader.GetString(22)); // security_stamp
                            builder.WriteString(reader.GetString(23)); // concurrency_stamp
                            builder.WriteBoolean(reader.GetBoolean(24)); // use_password_expiration
                            long passwordExpirationDate = reader.IsDBNull(25) ? 0 : reader.GetDateTime(25).Ticks;
                            builder.WriteInt64(passwordExpirationDate); // password_expiration_date (nullable)
                        }
                    }, commandText, new NpgsqlParameter("userId", id));
                    var lengthPosition = builder.WriteInt64((long)0);  //siapkan buffer untuk panjang data roles
                    commandText = """
                        select role_id, role_name
                        from roles
                        order by role_name
                        """;
                    long itemCount = 0;
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                            itemCount++;
                        }
                    }, commandText);
                    builder.WriteInt64(itemCount, lengthPosition);
                    lengthPosition = builder.WriteInt64(0L);  //siapkan buffer untuk panjang data countries
                    itemCount = 0;
                    commandText = """
                        SELECT country_id, country_name
                        from countries
                        order by country_name
                        """;
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                            itemCount++;
                        }
                    }, commandText);
                    builder.WriteInt64(itemCount, lengthPosition);
                    if (addressInfo.City > 0)
                    {
                        commandText = """
                            SELECT state_id, state_name
                            FROM states
                            where state_id = @stateId
                            """;
                        lengthPosition = builder.WriteInt64(0L);  //siapkan buffer untuk panjang data states
                        await db.ExecuteReaderAsync(async reader =>
                        {
                            while (await reader.ReadAsync())
                            {
                                builder.WriteInt16(reader.GetInt16(0)); // state_id
                                builder.WriteString(reader.GetString(1)); // state_name
                                builder.WriteInt16(reader.GetInt16(2)); // country_id
                            }
                        }, commandText, new NpgsqlParameter("stateId", addressInfo.State));
                        builder.WriteInt64(itemCount, lengthPosition);
                        commandText = """
                            SELECT city_id, city_name
                            from cities
                            where state_id = @state_id
                            """;
                        lengthPosition = builder.WriteInt64(0L);  //siapkan buffer untuk panjang data cities
                        await db.ExecuteReaderAsync(async reader =>
                        {
                            while (await reader.ReadAsync())
                            {
                                builder.WriteInt32(reader.GetInt32(0)); // city_id
                                builder.WriteString(reader.GetString(1)); // city_name
                            }
                        }, commandText, new NpgsqlParameter("state_id", addressInfo.State));
                    }
                    else
                    {
                        builder.WriteInt64(0L); // No states data
                        builder.WriteInt64(0L); // No cities data
                    }
                    return Results.File(builder.ToArray(), "application/octet-stream");
                }
            }
            
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    user = User.Create(reader);
                }
            }, commandText, new NpgsqlParameter("userId", id));
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
                }, commandText, new NpgsqlParameter("city_id", user.CityId));
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
                }, commandText, new NpgsqlParameter("state_id", model.AddressInfo.State));
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
                    email_confirmed,
                    phone_number,
                    phone_number_confirmed,
                    password_hash,
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
                    @emailConfirmed,
                    @phoneNumber,
                    @phoneNumberConfirmed,
                    @passwordHash,
                    @dateOfBirth,
                    @sex,
                    @maritalStatus,
                    @streetAddress,
                    @cityId,
                    @zipCode,
                    @twoFactorEnabled,
                    @accessFailedCount,
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

            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("firstName", user.FirstName),
                new Npgsql.NpgsqlParameter("lastName", user.LastName),
                new Npgsql.NpgsqlParameter("roleId", user.RoleId),
                new Npgsql.NpgsqlParameter("userName", user.UserName),
                new Npgsql.NpgsqlParameter("normalizedUserName", user.UserName.ToUpperInvariant()),
                new Npgsql.NpgsqlParameter("email", user.Email),
                new Npgsql.NpgsqlParameter("emailConfirmed", user.EmailConfirmed),
                new Npgsql.NpgsqlParameter("phoneNumber", user.PhoneNumber),
                new Npgsql.NpgsqlParameter("phoneNumberConfirmed", user.PhoneNumberConfirmed),
                new Npgsql.NpgsqlParameter("passwordHash", BCrypt.Net.BCrypt.HashPassword("YourDefaultPasswordHere")),
                new Npgsql.NpgsqlParameter("dateOfBirth", user.DateOfBirth),
                new Npgsql.NpgsqlParameter("sex", user.Sex),
                new Npgsql.NpgsqlParameter("maritalStatus", user.MaritalStatus),
                new Npgsql.NpgsqlParameter("streetAddress", user.StreetAddress),
                new Npgsql.NpgsqlParameter("cityId", user.CityId),
                new Npgsql.NpgsqlParameter("zipCode", user.ZipCode),
                new Npgsql.NpgsqlParameter("twoFactorEnabled", user.TwoFactorEnabled),
                new Npgsql.NpgsqlParameter("accessFailedCount", user.AccessFailedCount),
                new Npgsql.NpgsqlParameter("lockoutEnabled", user.LockoutEnabled),
                new Npgsql.NpgsqlParameter("lockoutEnd", (object?)user.LockoutEnd ?? DBNull.Value),
                new Npgsql.NpgsqlParameter("securityStamp", user.SecurityStamp.ToString()),
                new Npgsql.NpgsqlParameter("concurrencyStamp", (object?)user.ConcurrencyStamp ?? DBNull.Value),
                new Npgsql.NpgsqlParameter("usePasswordExpiration", user.UsePasswordExpiration),
                new Npgsql.NpgsqlParameter("passwordExpirationDate", (object?)user.PasswordExpirationDate ?? DBNull.Value),
                new NpgsqlParameter("creatorId", AppHelpers.GetUserID(context)),
                new NpgsqlParameter("createdDate", DateTime.UtcNow)
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
                email_confirmed = @emailConfirmed,
                phone_number = @phoneNumber,
                phone_number_confirmed = @phoneNumberConfirmed,
                date_of_birth = @dateOfBirth,
                sex = @sex,
                marital_status = @maritalStatus,
                street_address = @streetAddress,
                city_id = @cityId,
                zip_code = @zipCode,
                two_factor_enabled = @twoFactorEnabled,
                access_failed_count = @accessFailedCount,
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

            var parameters = new[]
            {
                new Npgsql.NpgsqlParameter("firstName", user.FirstName),
                new Npgsql.NpgsqlParameter("lastName", user.LastName),
                new Npgsql.NpgsqlParameter("roleId", user.RoleId),
                new Npgsql.NpgsqlParameter("userName", user.UserName),
                new Npgsql.NpgsqlParameter("normalizedUserName", user.UserName.ToUpperInvariant()),
                new Npgsql.NpgsqlParameter("email", user.Email),
                new Npgsql.NpgsqlParameter("emailConfirmed", user.EmailConfirmed),
                new Npgsql.NpgsqlParameter("phoneNumber", user.PhoneNumber),
                new Npgsql.NpgsqlParameter("phoneNumberConfirmed", user.PhoneNumberConfirmed),
                new Npgsql.NpgsqlParameter("dateOfBirth", user.DateOfBirth),
                new Npgsql.NpgsqlParameter("sex", user.Sex),
                new Npgsql.NpgsqlParameter("maritalStatus", user.MaritalStatus),
                new Npgsql.NpgsqlParameter("streetAddress", user.StreetAddress),
                new Npgsql.NpgsqlParameter("cityId", user.CityId),
                new Npgsql.NpgsqlParameter("zipCode", user.ZipCode),
                new Npgsql.NpgsqlParameter("twoFactorEnabled", user.TwoFactorEnabled),
                new Npgsql.NpgsqlParameter("accessFailedCount", user.AccessFailedCount),
                new Npgsql.NpgsqlParameter("lockoutEnabled", user.LockoutEnabled),
                new Npgsql.NpgsqlParameter("lockoutEnd", (object?)user.LockoutEnd ?? DBNull.Value),
                new Npgsql.NpgsqlParameter("securityStamp", user.SecurityStamp.ToString()),
                new Npgsql.NpgsqlParameter("concurrencyStamp", (object?)user.ConcurrencyStamp ?? DBNull.Value),
                new Npgsql.NpgsqlParameter("usePasswordExpiration", user.UsePasswordExpiration),
                new Npgsql.NpgsqlParameter("passwordExpirationDate", (object?)user.PasswordExpirationDate ?? DBNull.Value),
                new Npgsql.NpgsqlParameter("editorId", AppHelpers.GetUserID(context)),
                new Npgsql.NpgsqlParameter("editedDate", DateTime.UtcNow),
                new Npgsql.NpgsqlParameter("userId", user.Id)
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
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter("userId", id));
            return success ? Results.Ok(CommonResult.Ok("User deleted successfully.")) : Results.Ok(CommonResult.Fail("Failed to delete user."));
        }

        internal static async Task<IResult> GetRoleOptionsAsync(IDatabase db)
        {
            var commandText = """
                SELECT role_id, role_name
                FROM roles
                ORDER BY role_name
                """;
            using (var builder = new BinaryBuilder())
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
    }
}
