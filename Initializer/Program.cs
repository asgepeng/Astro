using Astro.Data;
using Npgsql;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Koneksi ke database default "postgres"
        var connString = "Host=localhost;Database=postgres;Username=postgres;Password=Orogin@k-66171";
        var db = new NpgsqlDatabase(connString);

        // 2. Cek apakah database "astro" sudah ada
        var exist = await db.AnyRecordsAsync(
            "SELECT 1 FROM pg_database WHERE datname = 'astro'"
        );
        if (exist)
        {
            Console.WriteLine("Database 'astro' sudah ada.");
            return;
        }

        // 3. Buat database dan user
        var sql_ddl = """
        CREATE DATABASE astro ENCODING 'UTF8';
        CREATE ROLE astroboy LOGIN PASSWORD 'Orogin@k-66171';
        GRANT ALL PRIVILEGES ON DATABASE astro TO astroboy;
        """;
        await db.ExecuteNonQueryAsync(sql_ddl);
        db.ChangeDatabase("astro");
        await db.ExecuteNonQueryAsync("ALTER SCHEMA public OWNER TO astroboy;");

        // 4. Koneksi ke database baru "astro" dengan user baru
        db.ChangeUsernameAndPassword("astroboy", "Orogin@k-66171");

        // 5. Jalankan DDL dan DML
        sql_ddl = global::Astro.DbInitialializer.Properties.Resources.npgsql_ddl;
        await db.ExecuteNonQueryAsync(sql_ddl);
        await db.ExecuteNonQueryAsync(global::Astro.DbInitialializer.Properties.Resources.dml);
        var password = Astro.Security.Password.HashPassword("Power123...");
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@username", "administrator"),
            new NpgsqlParameter("@password", password),
            new NpgsqlParameter("@userid", 1)
        };
        var commandText = """
            INSERT INTO logins
            (login_name, login_password, user_id)
            VALUES
            (@username, @password, @userid)
            """;
        await db.ExecuteNonQueryAsync(commandText, parameters);
        Console.WriteLine("Create database succeeded.");
        Console.ReadKey();
    }

}