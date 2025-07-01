using Astro.Data;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Security
{
    public class LoginManager
    {
        private readonly IDatabase db;
        public LoginManager(IDatabase iDatabase)
        {
            db = iDatabase;
        }
        public async Task<bool> SignInAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return false;

            var commandText = """
                SELECT "passwordhash", "passwordkey"
                FROM "logins"
                WHERE "username"=username
                """;
            var parameters = new Npgsql.NpgsqlParameter[]
            {
                new Npgsql.NpgsqlParameter("username", username)
            };
            var pwd = Array.Empty<byte>();
            var key = Array.Empty<byte>();
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                if (await reader.ReadAsync())
                {
                    pwd = (byte[])reader.GetValue(0);
                    key = (byte[])reader.GetValue(1);
                }
            }, commandText, parameters);
            if (pwd.Length == 0 || key.Length == 0) return false;

            var success = Astro.Security.Password.Match(password, pwd, key);
            if (!success)
            {
                commandText = """
                    UPDATE logins SET failedcount = failedcount + 1
                    WHERE "username"=username
                    """;
                await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter[] { new NpgsqlParameter("username", username) });
            }
            return success;
        }
        public async Task SignOutAsync()
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
