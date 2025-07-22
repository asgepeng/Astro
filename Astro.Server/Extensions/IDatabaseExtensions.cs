using Astro.Data;
using Astro.Models;
using System.Data.Common;
using System.Data;

namespace Astro.Server.Binaries
{
    internal static class IDatabaseExtensions
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
        internal static async Task<byte[]> GetContact(this IDatabase db, short id)
        {
            var commandText = """
                select contact_id, contact_name
                from contacts
                where contact_id = @contactId
                and is_deleted = false
                """;
            using (var writer = new IO.Writer())
            {
                bool contactExists = false;
                await db.ExecuteReaderAsync(async reader =>
                {
                    contactExists = reader.HasRows;
                    writer.WriteBoolean(contactExists);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                if (!contactExists) return writer.ToArray();

                commandText = """
                select a.address_id, a.street_address, a.city_id, c.city_name, c.state_id, s.state_name, cn.country_id, cn.country_name, a.address_type, a.is_primary, a.zip_code
                from addresses as a
                    inner join cities as c on a.city_id = c.city_id
                    inner join states as s on c.state_id = s.state_id
                    inner join countries as cn on s.country_id = cn.country_id
                where a.owner_id = @contactId
                """;
                var iPos = writer.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt32(reader.GetInt32(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteInt16(reader.GetInt16(4));
                        writer.WriteString(reader.GetString(5));
                        writer.WriteInt16(reader.GetInt16(6));
                        writer.WriteString(reader.GetString(7));
                        writer.WriteInt16(reader.GetInt16(8));
                        writer.WriteBoolean(reader.GetBoolean(9));
                        writer.WriteString(reader.GetString(10));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                commandText = """
                    select phone_id, phone_number, phone_type, is_primary
                    from phones as p
                    where owner_id = @contactId
                    """;

                iCount = 0;
                iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        writer.WriteBoolean(reader.GetBoolean(3));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                iCount = 0;
                iPos = writer.ReserveInt32();
                commandText = """
                    select email_id, email_address, email_type, is_primary
                    from emails
                    where owner_id = @contactId
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        writer.WriteBoolean(reader.GetBoolean(3));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetAccountProviderAsync(this IDatabase db, short id)
        {
            var commandText = """
                select provider_id, provider_name, provider_type
                from account_providers
                where provider_id = @id;
                """;
            var data = Array.Empty<byte>();
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                    }
                }, commandText, db.CreateParameter("id", id));
                data = writer.ToArray();
            }
            return data;
        }
        internal static async Task<byte[]> GetAccount(this IDatabase db, short id)
        {
            var commandText = """
                select a.account_id, a.account_name, a.account_number, a.provider_id, p.provider_type
                from accounts as a
                inner join account_providers as p on a.provider_id = p.provider_id
                where a.account_id = @id and a.is_deleted = false
                """;
            var data = Array.Empty<byte>();
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    writer.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteString(reader.GetString(2));
                        writer.WriteInt16(reader.GetInt16(3));
                        writer.WriteInt16(reader.GetInt16(4));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));

                var iCount = 0;
                var iPos = writer.ReserveInt32();
                commandText = """
                    select provider_id, provider_name, provider_type
                    from account_providers
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        iCount++;
                    }
                }, commandText);
                writer.WriteInt32(iCount, iPos);
                data = writer.ToArray();
            }
            return data;
        }
        internal static async Task<byte[]> GetUserPermissions(this IDatabase db, short roleID)
        {
            var commandText = """
                SELECT s.section_id, s.section_title, m.menu_id, m.menu_title, rtm.allow_create, rtm.allow_read, rtm.allow_update, rtm.allow_delete
                FROM role_to_menus rtm INNER JOIN
                    menus m ON rtm.menu_id = m.menu_id INNER JOIN
                    sections s ON m.section_id = s.section_id
                WHERE m.is_disabled = false AND rtm.role_id = @roleId
                ORDER BY s.section_id, m.menu_id
                """;
            var listMenu = new ListMenu();
            var data = Array.Empty<byte>();
            using (var writer = new IO.Writer())
            {
                await db.ExecuteReaderAsync(async (DbDataReader reader) =>
                {
                    var iSectionPos = writer.ReserveInt32();
                    var iMenuPos = 0L;

                    var iSectionCount = 0;
                    var iMenuCount = 0;

                    var currentSectionId = 0;

                    while (await reader.ReadAsync())
                    {
                        var sectionId = reader.GetInt16(0);
                        if (sectionId != currentSectionId)
                        {
                            currentSectionId = sectionId;
                            if (iMenuPos > 0)
                            {
                                writer.WriteInt32(iMenuCount, iMenuPos);
                                iMenuCount = 0;
                            }
                            writer.WriteInt16(sectionId);
                            writer.WriteString(reader.GetString(1));

                            iMenuPos = writer.ReserveInt32();
                            iSectionCount++;
                        }
                        writer.WriteInt16(reader.GetInt16(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteBoolean(reader.GetBoolean(4));
                        writer.WriteBoolean(reader.GetBoolean(5));
                        writer.WriteBoolean(reader.GetBoolean(6));
                        writer.WriteBoolean(reader.GetBoolean(7));
                        iMenuCount++;
                    }
                    writer.WriteInt32(iMenuCount, iMenuPos);
                    writer.WriteInt32(iSectionCount, iSectionPos);

                }, commandText, db.CreateParameter("roleId", roleID));
                return writer.ToArray();
            }
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
                order by p.product_id
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
        internal static async Task<byte[]> GetContactDataTable(this IDatabase db, short contactType)
        {
            var commandText = """
                select c.contact_id, c.contact_name, COALESCE(ca.street_address || ' ' || ct.city_name || ' ' || s.state_name || ', ' || ca.zip_code, '') AS address,
                   	COALESCE(p.phone_number, '') as phone_number, concat(creator.user_firstname, ' ', creator.user_lastname) as creator, c.created_date
                from contacts as c
                    left join addresses as ca on c.contact_id = ca.owner_id and ca.is_primary = true
                    left join cities as ct on ca.city_id = ct.city_id
                    left join states as s on ct.state_id = s.state_id
                    left join phones as p on c.contact_id = p.owner_id and p.is_primary = true
                    inner join users as creator on c.creator_id = creator.user_id
                where 
                    c.is_deleted = false and c.contact_type = @contactType
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
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteString(reader.GetString(4));
                        writer.WriteDateTime(reader.GetDateTime(5));
                    }
                }, commandText, db.CreateParameter("contactType", contactType));
                data = writer.ToArray();
            }
            return data;
        }
        internal static async Task<byte[]> GetAccountProviderTableAsync(this IDatabase db)
        {
            var commandText = """
                select provider_id, provider_name, case provider_type when 1 then 'Bank' when 2 then 'E-Wallet' when 3 then 'E-Money' else '-' end as provider_type
                from account_providers
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
                        writer.WriteString(reader.GetString(2));
                    }
                    data = writer.ToArray();
                }, commandText);
            }
            return data;
        }
        internal static async Task<byte[]> GetAccountDataTable(this IDatabase db)
        {
            var commandText = """
                select acc.account_id, acc.account_name, acc.account_number, 
                case ap.provider_type when 1 then 'Bank' when 2 then 'E-Wallet' when 3 then 'E-Money' else '-' end as accounttype, 
                ap.provider_name, concat(u.user_firstname, ' ', u.user_lastname) as creator, acc.created_date, acc.edited_date
                from accounts AS acc
                inner join account_providers AS ap on acc.provider_id = ap.provider_id
                inner join users as u on acc.creator_id = u.user_id
                where acc.is_deleted = false
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
                        writer.WriteString(reader.GetString(2));
                        writer.WriteString(reader.GetString(3));
                        writer.WriteString(reader.GetString(4));
                        writer.WriteString(reader.GetString(5));
                        writer.WriteDateTime(reader.GetDateTime(6));
                        writer.WriteDateTime(reader.IsDBNull(7) ? null : reader.GetDateTime(7));
                    }
                }, commandText);
                data = writer.ToArray();
            }
            return data;
        }
    }
}
