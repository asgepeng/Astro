using Alaska.Data;
using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using Astro.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System.Data.Common;
using System.Text;
using System.Data;

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
                    u.password_expiration_date
                FROM
                    users AS u
                WHERE u.user_id = @userId
                    AND u.is_deleted = false
                """;
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            if (isWinformApp)
            {
                using (var builder = new BinaryObjectWriter())
                {
                    var addressInfo = new AddressInfo();
                    await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                    {
                        if (await reader.ReadAsync())
                        {
                            addressInfo.City = reader.GetInt32(14); // city_id
                            addressInfo.State = reader.GetInt16(15); // state_id
                            addressInfo.Country = reader.GetInt16(16); // country_id

                            builder.WriteBoolean(true);
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
                            builder.WriteInt16(reader.GetInt16(11)); // sex
                            builder.WriteInt16(reader.GetInt16(12)); // marital_status
                            builder.WriteString(reader.GetString(13)); // street_address 
                            builder.WriteInt32(addressInfo.City); // city_id
                            builder.WriteInt16(addressInfo.State);
                            builder.WriteInt16(addressInfo.Country);
                            builder.WriteString(reader.GetString(17)); // zip_code
                            builder.WriteBoolean(reader.GetBoolean(18)); // two_factor_enabled
                            builder.WriteInt16(reader.GetInt16(19)); // access_failed_count
                            builder.WriteBoolean(reader.GetBoolean(20)); // lockout_enabled
                            builder.WriteDateTime(reader.IsDBNull(21) ? null : reader.GetDateTime(21)); // lockout_end (nullable)
                            builder.WriteGuid(reader.IsDBNull(22) ? null : reader.GetGuid(22)); // security_stamp
                            builder.WriteDateTime(reader.IsDBNull(23) ? null : reader.GetDateTime(23)); // concurrency_stamp
                            builder.WriteBoolean(reader.GetBoolean(24)); // use_password_expiration
                            builder.WriteDateTime(reader.IsDBNull(25) ? null : reader.GetDateTime(25)); // password_expiration_date (nullable)
                        }
                        else
                        {
                            builder.WriteBoolean(false); // User not found
                        }
                    }, commandText, new NpgsqlParameter("userId", id));

                    commandText = """
                        select role_id, role_name
                        from roles
                        order by role_name
                        """;
                    int iCount = 0;
                    var iPos = builder.ReserveInt32(iCount);  //siapkan buffer untuk panjang data roles
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                            iCount++;
                        }
                    }, commandText);
                    builder.WriteInt32(iCount, iPos);
                    
                    iCount = 0;
                    iPos = builder.ReserveInt32(iCount);  //siapkan buffer untuk panjang data countries
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
                            iCount++;
                        }
                    }, commandText);
                    builder.WriteInt32(iCount, iPos);

                    if (addressInfo.City > 0)
                    {
                        commandText = """
                            SELECT state_id, state_name
                            FROM states
                            where country_id = @countryId
                            """;
                        iCount = 0;
                        iPos = builder.ReserveInt32(iCount);  //siapkan buffer untuk panjang data states
                        await db.ExecuteReaderAsync(async reader =>
                        {
                            while (await reader.ReadAsync())
                            {
                                builder.WriteInt16(reader.GetInt16(0)); // state_id
                                builder.WriteString(reader.GetString(1)); // state_name
                                iCount++;
                            }
                        }, commandText, new NpgsqlParameter("countryId", addressInfo.Country));
                        builder.WriteInt32(iCount, iPos);
                        commandText = """
                            SELECT city_id, city_name
                            from cities
                            where state_id = @stateId
                            """;
                        iCount = 0;
                        iPos = builder.ReserveInt32(iCount);  //siapkan buffer untuk panjang data cities
                        await db.ExecuteReaderAsync(async reader =>
                        {
                            while (await reader.ReadAsync())
                            {
                                builder.WriteInt32(reader.GetInt32(0)); // city_id
                                builder.WriteString(reader.GetString(1)); // city_name
                                iCount++;
                            }
                        }, commandText, new NpgsqlParameter("stateId", addressInfo.State));
                        builder.WriteInt32(iCount, iPos);
                    }
                    else
                    {
                        builder.WriteInt32(0);
                        builder.WriteInt32(0);
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

            var parameters = new[]
            {
                new NpgsqlParameter("firstName",               user.FirstName),
                new NpgsqlParameter("lastName",                user.LastName),
                new NpgsqlParameter("roleId",                  user.RoleId),
                new NpgsqlParameter("userName",                user.UserName),
                new NpgsqlParameter("normalizedUserName",      user.UserName.ToUpperInvariant()),
                new NpgsqlParameter("email",                   user.Email),
                new NpgsqlParameter("phoneNumber",             user.PhoneNumber),
                new NpgsqlParameter("dateOfBirth",             user.DateOfBirth),
                new NpgsqlParameter("sex",                     user.Sex),
                new NpgsqlParameter("maritalStatus",           user.MaritalStatus),
                new NpgsqlParameter("streetAddress",           user.StreetAddress),
                new NpgsqlParameter("cityId",                  user.CityId),
                new NpgsqlParameter("stateId",                 user.StateId),
                new NpgsqlParameter("countryId",               user.CountryId),
                new NpgsqlParameter("zipCode",                 user.ZipCode),
                new NpgsqlParameter("twoFactorEnabled",        user.TwoFactorEnabled),
                new NpgsqlParameter("lockoutEnabled",          user.LockoutEnabled),
                new NpgsqlParameter("lockoutEnd",              (object?)user.LockoutEnd ?? DBNull.Value),
                new NpgsqlParameter("securityStamp",           DbType.Guid) { Value = Guid.NewGuid() },
                new NpgsqlParameter("concurrencyStamp",        (object?)user.ConcurrencyStamp ?? DBNull.Value),
                new NpgsqlParameter("usePasswordExpiration",   user.UsePasswordExpiration),
                new NpgsqlParameter("passwordExpirationDate",  (object?)user.PasswordExpirationDate ?? DBNull.Value),
                new NpgsqlParameter("creatorId",               AppHelpers.GetUserID(context)),
                new NpgsqlParameter("createdDate",             DateTime.UtcNow)
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

            var parameters = new[]
            {
                new NpgsqlParameter("firstName",               user.FirstName),
                new NpgsqlParameter("lastName",                user.LastName),
                new NpgsqlParameter("roleId",                  user.RoleId),
                new NpgsqlParameter("userName",                user.UserName),
                new NpgsqlParameter("normalizedUserName",      user.UserName.ToUpperInvariant()),
                new NpgsqlParameter("email",                   user.Email),
                new NpgsqlParameter("phoneNumber",             user.PhoneNumber),
                new NpgsqlParameter("dateOfBirth",             user.DateOfBirth),
                new NpgsqlParameter("sex",                     user.Sex),
                new NpgsqlParameter("maritalStatus",           user.MaritalStatus),
                new NpgsqlParameter("streetAddress",           user.StreetAddress),
                new NpgsqlParameter("cityId",                  user.CityId),
                new NpgsqlParameter("stateId",                 user.StateId),
                new NpgsqlParameter("countryId",               user.CountryId),
                new NpgsqlParameter("zipCode",                 user.ZipCode),
                new NpgsqlParameter("twoFactorEnabled",        user.TwoFactorEnabled),
                new NpgsqlParameter("lockoutEnabled",          user.LockoutEnabled),
                new NpgsqlParameter("lockoutEnd",              (object?)user.LockoutEnd ?? DBNull.Value),
                new NpgsqlParameter("securityStamp",           DbType.Guid) { Value = Guid.NewGuid() },
                new NpgsqlParameter("concurrencyStamp",        (object?)user.ConcurrencyStamp ?? DBNull.Value),
                new NpgsqlParameter("usePasswordExpiration",   user.UsePasswordExpiration),
                new NpgsqlParameter("passwordExpirationDate",  (object?)user.PasswordExpirationDate ?? DBNull.Value),
                new NpgsqlParameter("editorId",                AppHelpers.GetUserID(context)),
                new NpgsqlParameter("editedDate",              DateTime.UtcNow),
                new NpgsqlParameter("userId",                  user.Id)
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
