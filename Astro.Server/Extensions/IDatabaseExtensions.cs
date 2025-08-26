using Astro.Data;
using Astro.Models;
using System.Data.Common;
using System.Data;
using Microsoft.AspNetCore.Http;
using Astro.Server.Extensions;
using Astro.Server.Memory;

namespace Astro.Server.Binaries
{
    internal static class IDatabaseExtensions
    {
        //data objects
        internal static async Task<byte[]> GetUser(this IDBClient db, short id)
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
            using (var builder = new Streams.Writer())
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
                SELECT l.location_id, l.name, l.street_address
                FROM userlocations AS ul
                INNER JOIN locations AS l ON ul.location_id = l.location_id
                WHERE ul.user_id = @userId
                """;
                var iCount = 0;
                var iPos = builder.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                        builder.WriteString(reader.GetString(2));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("userId", id, DbType.Int16));
                builder.WriteInt32(iCount, iPos);

                commandText = """
                        select role_id, role_name
                        from roles
                        order by role_name
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
        internal static async Task<byte[]> GetRole(this IDBClient db, short id)
        {
            var commandText = """
                select roleid, name
                from roles
                where roleid = @id;
                """;
            using (var builder = new Streams.Writer())
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
                            m.menuid,
                            m.title,
                            coalesce(rtm.allowcreate, false) as allowcreate,
                            coalesce(rtm.allowread,   false) as allowread,
                            coalesce(rtm.allowupdate, false) as allowupdate,
                            coalesce(rtm.allowdelete, false) as allowdelete
                        from menus as m
                        left join rolemenus as rtm
                            on m.menuid = rtm.menuid and rtm.roleid = @id                        
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
        internal static async Task<byte[]> GetProduct(this IDBClient db, short id, HttpContext context)
        {
            var commandText = """
                SELECT p.productid, p.name, p.description, p.sku, p.categoryid, p.producttype,
                	i.isactive, i.stock, i.minstock, i.maxstock, p.unitid, i.price, i.cogs, p.images
                FROM products AS p
                INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                where p.productid = @id and p.isdeleted = false;
                """;
            using (var writer = new Streams.Writer())
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
                }, commandText, db.CreateParameter("id", id, DbType.Int16), db.CreateParameter("location", context.Request.GetLocationID(), DbType.Int16));

                commandText = """
                        select categoryid, name
                        from categories
                        where isdeleted = false
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
                        select unitid, name
                        from units
                        order by name
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
        internal static async Task<byte[]> GetUnit(this IDBClient db, short id)
        {
            var commandText = """
                select unitid, name
                from units
                where unitid = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetCategory(this IDBClient db, short id)
        {
            var commandText = """
                select c.categoryid, c.name
                from categories c
                where c.isdeleted = false and c.categoryid = @id
                """;
            var data = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async reader =>
            {
                using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetContact(this IDBClient db, short id)
        {
            var commandText = """
                SELECT contactid, name
                FROM contacts
                WHERE contactid = @contactId
                AND isdeleted = false
                """;
            using (var writer = new Streams.Writer())
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
                SELECT a.addressid, a.streetaddress, v.villageid, v.name AS villagename, d.districtid, d.name AS districtname,
                c.cityid, c.name AS cityname, s.stateid, s.name AS statename, a.addresstype, a.isprimary, a.zipcode
                FROM addresses AS a
                	INNER JOIN villages AS v ON a.villageid = v.villageid
                	INNER JOIN districts AS d ON v.districtid = d.districtid
                	INNER JOIN cities AS c on d.cityid = c.cityid
                	INNER JOIN states AS s ON c.stateid = s.stateid	
                WHERE a.contactid = @contactId
                """;
                var iPos = writer.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        //address id: int, streetaddress: string
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));

                        //village id: int64, villagename : string
                        writer.WriteInt64(reader.GetInt64(2));
                        writer.WriteString(reader.GetString(3));

                        //district id: int, districtname : string
                        writer.WriteInt32(reader.GetInt32(4));
                        writer.WriteString(reader.GetString(5));

                        //city id: int, cityname : string
                        writer.WriteInt32(reader.GetInt32(6));
                        writer.WriteString(reader.GetString(7));

                        //state id: short, statename : string
                        writer.WriteInt16(reader.GetInt16(8));
                        writer.WriteString(reader.GetString(9));

                        //address type, isprimary, zipcode
                        writer.WriteInt16(reader.GetInt16(10));
                        writer.WriteBoolean(reader.GetBoolean(11));
                        writer.WriteString(reader.GetString(12));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                commandText = """
                    select phoneid, phonenumber, phonetype, isprimary
                    from phones as p
                    where contactid = @contactId
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
                    select emailid, emailaddress, emailtype, isprimary
                    from emails
                    where contactid = @contactId
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
        internal static async Task<byte[]> GetAccountProviderAsync(this IDBClient db, short id)
        {
            var commandText = """
                select provider_id, provider_name, provider_type
                from account_providers
                where provider_id = @id;
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetAccount(this IDBClient db, short id)
        {
            var commandText = """
                select a.accountid, a.accountname, a.accountnumber, a.providerid, p.providertype
                from accounts as a
                inner join accountproviders as p on a.providerid = p.providerid
                where a.accountid = @id and a.isdeleted = false
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
                    SELECT providerid, name, providertype
                    FROM accountproviders
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
        internal static async Task<byte[]> GetUserPermissions(this IDBClient db, short roleID)
        {
            var commandText = """
                SELECT s.sectionid, s.title, m.menuid, m.title AS menu_title, m.icon, rtm.allowcreate, rtm.allowread, rtm.allowupdate, rtm.allowdelete
                FROM rolemenus AS rtm 
                INNER JOIN menus m ON rtm.menuid = m.menuid 
                INNER JOIN sections s ON m.sectionid = s.sectionid
                WHERE m.disabled = false AND rtm.roleid = @roleId
                ORDER BY s.sectionid, m.menuid
                """;
            var listMenu = new ListMenu();
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
                        writer.WriteString(reader.GetString(4));
                        writer.WriteBoolean(reader.GetBoolean(5));
                        writer.WriteBoolean(reader.GetBoolean(6));
                        writer.WriteBoolean(reader.GetBoolean(7));
                        writer.WriteBoolean(reader.GetBoolean(8));
                        iMenuCount++;
                    }
                    writer.WriteInt32(iMenuCount, iMenuPos);
                    writer.WriteInt32(iSectionCount, iSectionPos);

                }, commandText, db.CreateParameter("roleId", roleID, DbType.Int16));
                return writer.ToArray();
            }
        }

        //data tables
        internal static async Task<byte[]> GetUserDataTable(this IDBClient db)
        {
            var commandText = """
                    SELECT e.employeeid, e.fullname, e.email, r.name, CASE WHEN e.creatorid = 0 then 'System' else c.fullname end as creator, e.createddate
                    FROM employees AS e
                    INNER JOIN roles AS r on e.roleid = r.roleid
                    LEFT JOIN employees AS c ON e.creatorid = c.employeeid
                    where e.isdeleted = false
                    """;
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetRoleDataTable(this IDBClient db)
        {
            var commandText = """
                SELECT r.roleid, r.name, CASE WHEN r.creatorid = 0 then 'System' else c.fullname end as creator, r.createddate
                FROM roles AS r
                LEFT JOIN employees AS c on r.creatorid = c.employeeid
                """;
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetProductDataTable(this IDBClient db, short locationId)
        {
            var commandText = """
                SELECT p.productid, p.name, p.sku, c.name AS category_name, i.stock, u.name AS unit_name,
                i.price, e.fullname, p.createddate
                FROM products AS p
                INNER JOIN categories AS c ON p.categoryid = c.categoryid
                INNER JOIN units AS u ON p.unitid = u.unitid
                INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                INNER JOIN employees AS e ON p.creatorid = e.employeeid
                WHERE p.isdeleted = false
                """;
            using (var writer = new Streams.Writer())
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
                }, commandText, db.CreateParameter("location", locationId, DbType.Int16));
                return writer.ToArray();
            }
        }
        internal static async Task<byte[]> GetUnitDataTable(this IDBClient db)
        {
            var commandText = """
                SELECT u.unitid, u.name, u.createddate, c.fullname
                FROM units AS u
                INNER JOIN employees AS c on u.creatorid = c.employeeid
                ORDER BY u.name
                """;
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetCategoryDataTable(this IDBClient db)
        {
            var commandText = """
                    SELECT c.categoryid, c.name, c.createddate, u.fullname
                    FROM categories AS c
                    INNER JOIN employees AS u on c.creatorid = u.employeeid
                    WHERE c.isdeleted = false
                    ORDER BY c.name
                    """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetContactDataTable(this IDBClient db, short contactType)
        {
            var commandText = """
                SELECT c.contactid, 
                       c.name, 
                       COALESCE(ca.streetaddress || ' ' || v.name || ' ' || d.name || ' ' || cty.name || ', ' || ca.zipcode, '') AS address,
                       COALESCE(p.phonenumber, '') as phonenumber, 
                       creator.fullname, 
                       c.createddate
                FROM contacts c
                LEFT JOIN addresses ca 
                       ON c.contactid = ca.contactid AND ca.isprimary = true
                LEFT JOIN villages v 
                       ON ca.villageid = v.villageid
                LEFT JOIN districts d 
                       ON v.districtid = d.districtid
                LEFT JOIN cities cty 
                       ON d.cityid = cty.cityid
                LEFT JOIN phones p 
                       ON c.contactid = p.contactid AND p.isprimary = true
                INNER JOIN employees creator 
                       ON c.creatorid = creator.employeeid
                WHERE c.isdeleted = false 
                  AND c.contacttype = @contactType;
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetAccountProviderTableAsync(this IDBClient db)
        {
            var commandText = """
                select providerid, name, case providertype when 1 then 'Bank' when 2 then 'E-Wallet' when 3 then 'E-Money' else '-' end as providertype
                from accountproviders
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
        internal static async Task<byte[]> GetAccountDataTable(this IDBClient db)
        {
            var commandText = """
                SELECT acc.accountid, acc.accountname, acc.accountnumber, 
                CASE ap.providertype WHEN 1 THEN 'Bank' WHEN 2 THEN 'E-Wallet' WHEN 3 THEN 'E-Money' ELSE '-' END AS accounttype, 
                ap.name, u.fullname, acc.createddate, acc.editeddate
                FROM accounts AS acc
                INNER JOIN accountproviders AS ap ON acc.providerid = ap.providerid
                INNER JOIN employees AS u ON acc.creatorid = u.employeeid
                where acc.isdeleted = false
                """;
            var data = Array.Empty<byte>();
            using (var writer = new Streams.Writer())
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
