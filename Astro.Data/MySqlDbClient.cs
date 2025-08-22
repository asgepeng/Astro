using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Data
{
    public class MySqlDbClient : DBClient
    {
        private string connectionString;
        public MySqlDbClient(string constring) => connectionString = constring;
        public override DbParameter CreateParameter(string name, object? value) => new MySqlParameter(name, value is null ? DBNull.Value : value);
        public override DbParameter CreateParameter(string name, object? value, DbType dbType) => new MySqlParameter(name, dbType) { Value = value is null ? DBNull.Value : value };
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            var conn = new MySqlConnection(this.connectionString);
            var cmd = new MySqlCommand(commandText, conn);
            cmd.Parameters.AddRange(parameters);
            return (conn, cmd);
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters)
        {
            var dbparams = new MySqlParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                dbparams[i] = new MySqlParameter(parameters[i].Item1, parameters[i].Item2);
            }
            return CreateConnection(commandText, dbparams);
        }
        public void ChangeDatabase(string database)
        {
            var builder = new MySqlConnectionStringBuilder(this.connectionString);
            builder.Database = database;
            this.connectionString = builder.ToString();
        }
        public void ChangeUsernameAndPassword(string username, string password)
        {
            var builde = new MySqlConnectionStringBuilder(this.connectionString);
            builde.UserID = username;
            builde.Password = password;
            this.connectionString = builde.ToString();
        }
    }
}
