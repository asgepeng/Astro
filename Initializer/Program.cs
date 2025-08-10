using Astro.Data;
using Npgsql;
using System.Data.Common;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Koneksi ke database default "postgres"
        var connString = "Host=localhost;Database=postgres;Username=postgres;Password=Orogin@k-66171";
        var db = new NpgsqlDatabase(connString);

        // 2. Cek apakah database "astro" sudah ada
        var exist = await db.AnyRecordsAsync(
            "SELECT 1 FROM pg_database WHERE datname = 'pos'"
        );
        if (exist)
        {
            Console.WriteLine("Database 'pos' sudah ada.");
            return;
        }

        // 3. Buat database dan user
        var sql_ddl = """
        CREATE DATABASE pos ENCODING 'UTF8';
        CREATE ROLE posuser LOGIN PASSWORD 'Orogin@k-66171';
        GRANT ALL PRIVILEGES ON DATABASE pos TO posuser;
        """;
        await db.ExecuteNonQueryAsync(sql_ddl);
        db.ChangeDatabase("pos");
        await db.ExecuteNonQueryAsync("ALTER SCHEMA public OWNER TO posuser;");

        // 4. Koneksi ke database baru "astro" dengan user baru
        db.ChangeUsernameAndPassword("posuser", "Orogin@k-66171");

        // 5. Jalankan DDL dan DML
        sql_ddl = global::Astro.DbInitialializer.Properties.Resources.ddl;
        await db.ExecuteNonQueryAsync(sql_ddl);
        await db.ExecuteNonQueryAsync(global::Astro.DbInitialializer.Properties.Resources.dml);

        var passwordHash = Astro.Models.Password.HashPassword("Power123...");
        await db.ExecuteNonQueryAsync("UPDATE users SET password_hash = @password WHERE user_id = 1;",
            db.CreateParameter("password", passwordHash, System.Data.DbType.String));

        var locCommand = """
            INSERT INTO locations (name, street_address, creator_id)
            VALUES ('MAESTRO BENDO', 'Jl. Cimalaya No 20', 1),
                ('MAESTRO ALBAIK', 'Dsn Besole Darungan', 1),
                ('MAESTRO ALBAIK', 'Jl. Asahan Tanjungsari', 1),
                ('MAESTRO FOTOKOPI', 'dsn Besole Ds Darungan', 1),
                ('MAESTRO AGRO', 'Dsn Besole Darungan', 1);
            """;
        await db.ExecuteNonQueryAsync(locCommand);

        var mysql = new MySqlDatabase("Server=cloud-id1.hostddns.us;Database=maestro_albaik;Uid=maestro_user;Pwd=Orogin@k-66171");
        var listUnit = await mysql.CreateListAsync<string>("SELECT DISTINCT unit FROM items WHERE NOT unit = ''");
        var dictUnit = await db.CreateDictionyAsync<string, short>("SELECT unit_name, unit_id FROM units");
        var insertUnit = "INSERT INTO units (unit_name) VALUES (@name) RETURNING unit_id;";

        foreach (var unit in listUnit)
        {
            if (string.IsNullOrWhiteSpace(unit)) continue;
            if (dictUnit.ContainsKey(unit)) continue;

            var unitId = await db.ExecuteScalarAsync<short>(insertUnit, db.CreateParameter("name", unit, System.Data.DbType.String));
            if (unitId != default(short))
            {
                dictUnit[unit]= unitId;
                Console.WriteLine($"Unit ID: {unitId}, Name: {unit}");
            }
        }
        var listCategory = await mysql.CreateListAsync<string>("SELECT `name` FROM itemgroups ORDER BY `name`");
        var dictCategory = await db.CreateDictionyAsync<string, short>("SELECT category_name, category_id FROM categories");
        var insertCategory = "INSERT INTO categories (category_name) VALUES (@name) RETURNING category_id;";
        foreach (var category in listCategory)
        {
            if (string.IsNullOrWhiteSpace(category)) continue;
            if (dictCategory.ContainsKey(category)) continue;

            var categoryId = await db.ExecuteScalarAsync<short>(insertCategory, db.CreateParameter("name", category, System.Data.DbType.String));
            if (categoryId != default(short))
            {
                dictCategory[category] = categoryId;
                Console.WriteLine($"ID: {categoryId}, Name: {category}");
            }
        }
        var commandText = """
            SELECT i.id, i.name, i.description, g.name AS categoryName, i.unit, i.buyprice, i.basicprice, i.qty, i.price1, i.minstock, i.maxstock 
            FROM items AS i 
            INNER JOIN itemgroups AS g ON `i`.`group`= g.id;
            """;
        var insertCommand = """
            INSERT INTO products (product_name, product_description, product_sku, category_id, unit_id, product_type, creator_id)
            VALUES (@name, @description, @sku, @category, @unit, 1, 1) RETURNING product_id;
            """;
        var inventoryCommand = """
            INSERT INTO inventories (location_id, product_id, is_active, buyprice, cogs, price, stock, min_stock, max_stock, creator_id)
            VALUES (@location, @product, true, @buyprice, @cogs, @price, @stock, @minstock, @maxstock, 1);
            """;
        var locationID = (short)1;
        await mysql.ExecuteReaderAsync(async reader =>
        {
            while (await reader.ReadAsync())
            {
                var categoryID = dictCategory.TryGetValue(reader.GetString(3), out var catId) ? catId : (short)0;
                var unitID = dictUnit.TryGetValue(reader.GetString(4), out var unitId) ? unitId : (short)0;
                var productparams = new DbParameter[]
                {
                    db.CreateParameter("name", reader.GetString(1), System.Data.DbType.String),
                    db.CreateParameter("description", reader.GetString(2), System.Data.DbType.String),
                    db.CreateParameter("sku", reader.GetString(0), System.Data.DbType.String),
                    db.CreateParameter("category", categoryID, System.Data.DbType.Int16),
                    db.CreateParameter("unit", unitID, System.Data.DbType.Int16)
                };
                var productID = await db.ExecuteScalarAsync<short>(insertCommand, productparams);
                if (productID != default(short))
                {
                    long buyprice = (long)reader.GetDouble(5);
                    long basicPrice = (long)reader.GetDouble(6);
                    int stock = reader.GetInt32(7);
                    long price = (long)reader.GetDouble(8);
                    int minstock = reader.GetInt32(9);
                    int maxstock = reader.GetInt32(10);

                    var invParams = new DbParameter[]
                    {
                        db.CreateParameter("location", locationID, System.Data.DbType.Int16),
                        db.CreateParameter("product", productID, System.Data.DbType.Int16),
                        db.CreateParameter("buyprice", buyprice, System.Data.DbType.Int64),
                        db.CreateParameter("cogs", basicPrice, System.Data.DbType.Int64),
                        db.CreateParameter("stock", stock, System.Data.DbType.Int32),
                        db.CreateParameter("price", price, System.Data.DbType.Int64),
                        db.CreateParameter("minstock", minstock, System.Data.DbType.Int32),
                        db.CreateParameter("maxstock", maxstock, System.Data.DbType.Int32)
                    };
                    await db.ExecuteNonQueryAsync(inventoryCommand, invParams);
                }
            }
        }, commandText);

        var insertCustomerCommand = """
            INSERT INTO contacts (contact_name, contact_type, creator_id)
            VALUES (@name, @type, 1) RETURNING contact_id
            """;
        var insertAddressCommand = """
            INSERT INTO addresses (owner_id, address_type, street_address, city_id, zip_code, is_primary)
            VALUES (@contactId, 1, @street, 3572, '66124', true);
            """;
        var insertPhoneCommand = """
            INSERT INTO phones (owner_id, phone_type, phone_number, is_primary)
            VALUES (@contactId, @type, @phone, @primary);
            """;
        var insertEmailCommand = """
            INSERT INTO emails (owner_id, email_type, email_address, is_primary)
            VALUES (@contactId, 1, @email, true);
            """;
        var contactSelectCommand = """
            SELECT s.id, s.name, s.address, s.telp, s.whatsapp, s.email, 0 AS ctype FROM suppliers AS s
            UNION ALL
            SELECT s.id, s.name, s.address, s.telp, s.whatsapp, s.email, 1 AS ctype FROM customers AS s
            """;
        await mysql.ExecuteReaderAsync(async reader =>
        {
            while (await reader.ReadAsync())
            {
                var contactType = (short)reader.GetInt32(6);
                var contactName = reader.GetString(1).Trim();
                if (string.IsNullOrWhiteSpace(contactName)) continue;

                var contactId = await db.ExecuteScalarAsync<short>(insertCustomerCommand, db.CreateParameter("name", contactName), db.CreateParameter("type", contactType, System.Data.DbType.Int16));
                if (contactId != default(short))
                {
                    var address = reader.GetString(2);
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        var addressParams = new DbParameter[]
                        {
                            db.CreateParameter("contactId", contactId, System.Data.DbType.Int16),
                            db.CreateParameter("street", address, System.Data.DbType.String)
                        };
                        await db.ExecuteNonQueryAsync(insertAddressCommand, addressParams);
                    }
                    var phone = reader.GetString(3);
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        if (!phone.StartsWith("0")) phone = phone.Insert(0, "0");
                        var phoneParams = new DbParameter[]
                        {
                            db.CreateParameter("contactId", contactId, System.Data.DbType.Int16),
                            db.CreateParameter("type", 0, System.Data.DbType.Int16),
                            db.CreateParameter("phone", phone, System.Data.DbType.String),
                            db.CreateParameter("primary", true, System.Data.DbType.Boolean)
                        };
                        await db.ExecuteNonQueryAsync(insertPhoneCommand, phoneParams);
                    }
                    var wa = reader.GetString(4);
                    if (!string.IsNullOrWhiteSpace(wa))
                    {
                        var phoneParams = new DbParameter[]
                        {
                            db.CreateParameter("contactId", contactId, System.Data.DbType.Int16),
                            db.CreateParameter("type", 1, System.Data.DbType.Int16),
                            db.CreateParameter("phone", wa, System.Data.DbType.String),
                            db.CreateParameter("primary", string.IsNullOrWhiteSpace(phone), System.Data.DbType.Boolean)
                        };
                        await db.ExecuteNonQueryAsync(insertPhoneCommand, phoneParams);
                    }
                    var email = reader.GetString(5);
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var emailParams = new DbParameter[]
                        {
                            db.CreateParameter("contactId", contactId, System.Data.DbType.Int16),
                            db.CreateParameter("email", email)
                        };
                        await db.ExecuteNonQueryAsync(insertEmailCommand, emailParams);
                    }
                }
            }
        }, contactSelectCommand);

        Console.WriteLine("Create database succeeded.");
        Console.ReadKey();
    }

}