using Astro.Data;
using Astro.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Binaries
{
    internal static class BinaryBuilder
    {
        //data objects
        internal static async Task<byte[]> GetUser(this IDatabase db, short id)
        {
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
            using (var builder = new IO.Writer())
            {
                var addressInfo = new AddressInfo();

                await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                {
                    builder.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        addressInfo.City = reader.GetInt32(14);
                        addressInfo.State = reader.GetInt16(15);
                        addressInfo.Country = reader.GetInt16(16);

                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                        builder.WriteString(reader.GetString(2));
                        builder.WriteInt16(reader.GetInt16(3));
                        builder.WriteString(reader.GetString(4));
                        builder.WriteString(reader.GetString(5));
                        builder.WriteString(reader.GetString(6));
                        builder.WriteBoolean(reader.GetBoolean(7));
                        builder.WriteString(reader.GetString(8));
                        builder.WriteBoolean(reader.GetBoolean(9));
                        builder.WriteDateTime(reader.GetDateTime(10));
                        builder.WriteInt16(reader.GetInt16(11));
                        builder.WriteInt16(reader.GetInt16(12));
                        builder.WriteString(reader.GetString(13));
                        builder.WriteInt32(addressInfo.City);
                        builder.WriteInt16(addressInfo.State);
                        builder.WriteInt16(addressInfo.Country);
                        builder.WriteString(reader.GetString(17));
                        builder.WriteBoolean(reader.GetBoolean(18));
                        builder.WriteInt16(reader.GetInt16(19));
                        builder.WriteBoolean(reader.GetBoolean(20));
                        builder.WriteDateTime(reader.IsDBNull(21) ? null : reader.GetDateTime(21));
                        builder.WriteGuid(reader.IsDBNull(22) ? null : reader.GetGuid(22));
                        builder.WriteDateTime(reader.IsDBNull(23) ? null : reader.GetDateTime(23));
                        builder.WriteBoolean(reader.GetBoolean(24));
                        builder.WriteDateTime(reader.IsDBNull(25) ? null : reader.GetDateTime(25));
                    }
                }, commandText, db.CreateParameter("userId", id, DbType.Int16));

                commandText = """
                        select role_id, role_name
                        from roles
                        order by role_name
                        """;
                int iCount = 0;
                var iPos = builder.ReserveInt32();
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
                iPos = builder.ReserveInt32();
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
                    iPos = builder.ReserveInt32();
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                            iCount++;
                        }
                    }, commandText, db.CreateParameter("countryId", addressInfo.Country, DbType.Int16));

                    builder.WriteInt32(iCount, iPos);
                    commandText = """
                            SELECT city_id, city_name
                            from cities
                            where state_id = @stateId
                            """;
                    iCount = 0;
                    iPos = builder.ReserveInt32();
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt32(reader.GetInt32(0));
                            builder.WriteString(reader.GetString(1));
                            iCount++;
                        }
                    }, commandText, db.CreateParameter("stateId", addressInfo.State, DbType.Int16));
                    builder.WriteInt32(iCount, iPos);
                }
                else
                {
                    builder.WriteInt32(0);
                    builder.WriteInt32(0);
                }
                return builder.ToArray();
            }
        }
        internal static async Task<byte[]> GetRole(this IDatabase db, short id)
        {
            var commandText = """
                select role_id, role_name
                from roles
                where role_id = @id;
                """;
            using (var builder = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    builder.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));
                commandText = """
                        select
                            m.menu_id,
                            m.menu_title,
                            coalesce(rtm.allow_create, false) as allow_create,
                            coalesce(rtm.allow_read,   false) as allow_read,
                            coalesce(rtm.allow_update, false) as allow_update,
                            coalesce(rtm.allow_delete, false) as allow_delete
                        from menus as m
                        left join role_to_menus as rtm
                            on m.menu_id = rtm.menu_id and rtm.role_id = @id                        
                        """;
                var iCount = 0;
                var iPos = builder.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                        builder.WriteBoolean(reader.GetBoolean(2));
                        builder.WriteBoolean(reader.GetBoolean(3));
                        builder.WriteBoolean(reader.GetBoolean(4));
                        builder.WriteBoolean(reader.GetBoolean(5));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));

                builder.WriteInt32(iCount, iPos);
                return builder.ToArray();
            }
        }
        internal static async Task<byte[]> GetProduct(this IDatabase db, short id)
        {
            var commandText = """
                select product_id, product_name, product_description, product_sku, category_id, product_type,
                	is_active, stock, min_stock, max_stock, unit_id, price, cost_average, images
                from products
                where product_id = @id and is_deleted = false;
                """;
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt16(reader.GetInt16(4));
                        writer.WriteInt16(reader.GetInt16(5));
                        writer.WriteBoolean(reader.GetBoolean(6));
                        writer.WriteInt32(reader.GetInt32(7));
                        writer.WriteInt16(reader.GetInt16(8));
                        writer.WriteInt16(reader.GetInt16(9));
                        writer.WriteInt16(reader.GetInt16(10));
                        writer.WriteInt64(reader.GetInt64(11));
                        writer.WriteInt64(reader.GetInt64(12));
                        writer.WriteString(reader.GetString(13));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));

                commandText = """
                        select category_id, category_name
                        from categories
                        where is_deleted = false
                        """;
                var iCount = 0;
                var iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);

                commandText = """
                        select unit_id, unit_name
                        from units
                        order by unit_name
                        """;
                iCount = 0;
                iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);

                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetUnit(this IDatabase db, short id)
        {
            var commandText = """
                select unit_id, unit_name
                from units
                where unit_id = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new IO.Writer())
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                    data = writer.ToArray();
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return data.ToArray();
        }
        internal static async Task<byte[]> GetCategory(this IDatabase db, short id)
        {
            var commandText = """
                select c.category_id, c.category_name
                from categories c
                where c.is_deleted = false and c.category_id = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new IO.Writer())
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                    data = writer.ToArray();
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            return data;
        }
        //data tables
        internal static async Task<byte[]> GetUserDataTable(this IDatabase db)
        {
            var commandText = """
                    select u.user_id, concat(u.user_firstname, ' ', u.user_lastname) AS fullname, u.email, r.role_name,
                    case when u.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, u.created_date
                    from users as u
                    inner join roles as r on u.role_id = r.role_id
                    left join users AS c ON u.creator_id = c.user_id
                    where u.is_deleted = false
                    """;
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteString(reader.GetString(4));
                        writer.WriteDateTime(reader.GetDateTime(5));
                    }
                }, commandText);
                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetRoleDataTable(this IDatabase db)
        {
            var commandText = """
                select r.role_id, r.role_name, case when r.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, r.created_date
                FROM roles as r
                left join users as c on r.creator_id = c.user_id
                """;
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteDateTime(reader.GetDateTime(3));
                    }
                }, commandText);
                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetProductDataTable(this IDatabase db)
        {
            var commandText = """
                select p.product_id, p.product_name, p.product_sku, c.category_name,  p.stock, unt.unit_name, p.price, 
                   	concat(u.user_firstname, ' ', u.user_lastname) as creator, p.created_date
                from products as p
                inner join categories as c on p.category_id = c.category_id
                inner join units as unt on p.unit_id = unt.unit_id
                inner join users as u on p.creator_id = u.user_id
                where p.is_deleted = false
                """;
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt32(reader.GetInt32(4));
                        writer.WriteString(reader.GetString(5));
                        writer.WriteInt64(reader.GetInt64(6));
                        writer.WriteString(reader.GetString(7));
                        writer.WriteDateTime(reader.GetDateTime(8));
                    }
                }, commandText);
                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetUnitDataTable(this IDatabase db)
        {
            var commandText = """
                select u.unit_id, u.unit_name, u.created_date, concat(c.user_firstname, ' ', c.user_lastname) as created_by
                from units as u
                inner join users as c on u.creator_id = c.user_id
                order by u.unit_name
                """;
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteDateTime(reader.GetDateTime(2));
                        writer.WriteString(reader.GetString(3));
                    }
                }, commandText);
                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetCategoryDataTable(this IDatabase db)
        {
            var commandText = """
                    select c.category_id, c.category_name, c.created_date, concat(u.user_firstname, ' ', u.user_lastname) as created_by
                    from categories c
                    inner join users u on c.creator_id = u.user_id
                    where c.is_deleted = false
                    order by c.category_name
                    """;
            var data = Array.Empty<byte>();
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteDateTime(reader.GetDateTime(2));
                        writer.WriteString(reader.GetString(3));
                    }
                }, commandText);
                data = writer.ToArray();
            }
            return data;
        }
    }
}
