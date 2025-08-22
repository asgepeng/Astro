using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Data
{
    public class SqlDbClient : DBClient
    {
        private readonly string _connstring;
        public SqlDbClient(string connString) => _connstring = connString;
        public override DbParameter CreateParameter(string name, object? value) => new SqlParameter(name, value != null ? value : DBNull.Value);
        public override DbParameter CreateParameter(string name, object? value, DbType dbType) => new SqlParameter(name, dbType) { Value = value != null ? value : DBNull.Value };
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            var conn = new SqlConnection(_connstring);
            var cmd = new SqlCommand(commandText, conn);
            cmd.Parameters.AddRange(parameters);
            return (conn, cmd);
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters)
        {
            var conn = new SqlConnection(_connstring);
            var cmd = new SqlCommand(commandText, conn);
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.Add(CreateParameter(name, value));
            }
            return (conn, cmd);
        }
    }
}
