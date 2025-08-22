using Npgsql;
using System.Data.Common;
using System.Data;

namespace Astro.Data
{
    public sealed class PgDbClient : DBClient
    {
        private string connectionString;
        public PgDbClient(string constring)
        {
            this.connectionString = constring;
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            var conn = new NpgsqlConnection(this.connectionString);
            var cmd = new NpgsqlCommand(commandText, conn);
            cmd.Parameters.AddRange(parameters);
            return (conn, cmd);
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters)
        {
            var dbparams = new NpgsqlParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                dbparams[i] = new NpgsqlParameter(parameters[i].Item1, parameters[i].Item2);
            }
            return CreateConnection(commandText, dbparams);
        }
        public void ChangeDatabase(string database)
        {
            var builder = new NpgsqlConnectionStringBuilder(this.connectionString);
            builder.Database = database;
            this.connectionString = builder.ToString();
        }
        public void ChangeUsernameAndPassword(string username, string password)
        {
            var builde = new NpgsqlConnectionStringBuilder(this.connectionString);
            builde.Username = username;
            builde.Password = password;
            this.connectionString = builde.ToString();
        }

        public override DbParameter CreateParameter(string name, object? value)
        {
            return new NpgsqlParameter(name, value ?? DBNull.Value);
        }

        public override DbParameter CreateParameter(string name, object? value, DbType dbType)
        {
            return new NpgsqlParameter(name, dbType)
            {
                Value = value ?? DBNull.Value
            };
        }
    }
}
