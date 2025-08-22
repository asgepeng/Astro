using Astro.Data;
using DocumentFormat.OpenXml.Bibliography;
using Npgsql;
using System.Data.Common;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Koneksi ke database default "postgres"
        var connString = "Host=localhost;Database=postgres;Username=postgres;Password=Orogin@k-66171";
        var db = new PgDbClient(connString);

        // 2. Cek apakah database "astro" sudah ada
        var exist = await db.HasRecordsAsync(
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
        await db.ExecuteNonQueryAsync("INSERT INTO logins (employeeid, passwordhash) VALUES (1, @password)",
            db.CreateParameter("password", passwordHash, System.Data.DbType.String));

        var locCommand = """
            INSERT INTO locations (name, streetaddress, creatorid)
            VALUES ('MAESTRO BENDO', 'Jl. Cimalaya No 20', 1),
                ('MAESTRO ALBAIK', 'Dsn Besole Darungan', 1),
                ('MAESTRO ALBAIK', 'Jl. Asahan Tanjungsari', 1),
                ('MAESTRO FOTOKOPI', 'dsn Besole Ds Darungan', 1),
                ('MAESTRO AGRO', 'Dsn Besole Darungan', 1);
            """;
        await db.ExecuteNonQueryAsync(locCommand);
        var databases = new string[] {"maestro_bendo", "maestro_albaik", "maestro_besole" , "maestro_fotokopi", "maestro_agro"};
        var dictProduct = new Dictionary<string, short>();
        var dictUnit = await db.FetchDictionaryAsync<string, short>("SELECT name, unitid FROM units");
        var dictCategory = await db.FetchDictionaryAsync<string, short>("SELECT name, categoryid FROM categories");
        var dictCustomer = new Dictionary<string, short>();

        for (short i = 0; i < databases.Length; i++)
        {
            Console.WriteLine("DATABASE: " + databases[i]);
            var mysql = new MySqlDbClient("server=cloud-id1.hostddns.us;Database=" + databases[i] + ";Uid=maestro_user;Pwd=Orogin@k-66171;SslMode=none");
            var listUnit = await mysql.FetchListAsync<string>("SELECT DISTINCT unit FROM items WHERE NOT unit = ''");
            var insertUnit = "INSERT INTO units (name) VALUES (@name) RETURNING unitid;";

            foreach (var unit in listUnit)
            {
                if (string.IsNullOrWhiteSpace(unit)) continue;
                if (dictUnit.ContainsKey(unit)) continue;

                var unitId = await db.ExecuteScalarAsync<short>(insertUnit, db.CreateParameter("name", unit, System.Data.DbType.String));
                if (unitId != default(short))
                {
                    dictUnit[unit] = unitId;
                }
            }
            var listCategory = await mysql.FetchListAsync<string>("SELECT `name` FROM itemgroups ORDER BY `name`");
            var insertCategory = "INSERT INTO categories (name) VALUES (@name) RETURNING categoryid;";
            foreach (var category in listCategory)
            {
                if (string.IsNullOrWhiteSpace(category)) continue;
                if (dictCategory.ContainsKey(category)) continue;

                var categoryId = await db.ExecuteScalarAsync<short>(insertCategory, db.CreateParameter("name", category, System.Data.DbType.String));
                if (categoryId != default(short))
                {
                    dictCategory[category] = categoryId;
                }
            }
            var commandText = """
            SELECT i.id, i.name, i.description, g.name AS categoryName, i.unit, i.buyprice, i.basicprice, i.qty, i.price1, i.minstock, i.maxstock 
            FROM items AS i 
            INNER JOIN itemgroups AS g ON `i`.`group`= g.id;
            """;
            var insertCommand = """
            INSERT INTO products (name, description, sku, categoryid, unitid, producttype, creatorid)
            VALUES (@name, @description, @sku, @category, @unit, 0, 1) RETURNING productid;
            """;
            var inventoryCommand = """
            INSERT INTO inventories (locationid, productid, isactive, buyprice, cogs, price, stock, minstock, maxstock, creatorid)
            VALUES (@location, @product, true, @buyprice, @cogs, @price, @stock, @minstock, @maxstock, 1);
            """;
            var locationID = (short)i + 1;
            await mysql.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var categoryID = dictCategory.TryGetValue(reader.GetString(3), out var catId) ? catId : (short)0;
                    var unitID = dictUnit.TryGetValue(reader.GetString(4), out var unitId) ? unitId : (short)0;
                    var sku = reader.GetString(0).Trim();
                    var productID = (short)0;
                    if (dictProduct.ContainsKey(sku))
                    {
                        productID = dictProduct[sku];
                    }
                    else
                    {
                        var productparams = new DbParameter[]
                        {
                            db.CreateParameter("name", reader.GetString(1), System.Data.DbType.String),
                            db.CreateParameter("description", reader.GetString(2), System.Data.DbType.String),
                            db.CreateParameter("sku", reader.GetString(0), System.Data.DbType.String),
                            db.CreateParameter("category", categoryID, System.Data.DbType.Int16),
                            db.CreateParameter("unit", unitID, System.Data.DbType.Int16)
                        };
                        productID = await db.ExecuteScalarAsync<short>(insertCommand, productparams);
                        dictProduct[sku] = productID;
                    }

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
            INSERT INTO contacts (name, contacttype, creatorid)
            VALUES (@name, @type, 1) RETURNING contactid
            """;
            var insertAddressCommand = """
            INSERT INTO addresses (contactid, addresstype, streetaddress, villageid, zipcode, isprimary)
            VALUES (@contactId, 1, @street, 3572012006, 66124, true);
            """;
            var insertPhoneCommand = """
            INSERT INTO phones (contactid, phonetype, phonenumber, isprimary)
            VALUES (@contactId, @type, @phone, @primary);
            """;
            var insertEmailCommand = """
            INSERT INTO emails (contactid, emailtype, emailaddress, isprimary)
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
                    var phone = reader.GetString(3);
                    var wa = reader.GetString(4);
                    var email = reader.GetString(5);

                    if (string.IsNullOrWhiteSpace(contactName)) continue;
                    if (dictCustomer.ContainsKey(phone)) continue;

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
                            dictCustomer[phone] = contactId;
                        }
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
        }
        
        Console.WriteLine("Create database succeeded.");
        Console.ReadKey();
    }

}