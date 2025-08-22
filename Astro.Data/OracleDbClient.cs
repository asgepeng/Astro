using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;

namespace Astro.Data
{
    public class OracleDbClient : DBClient
    {
        private readonly string _connstring;
        public OracleDbClient(string connstring) => _connstring = connstring;
        public override DbParameter CreateParameter(string name, object? value) => new OracleParameter(name, value != null ? value : DBNull.Value);
        public override DbParameter CreateParameter(string name, object? value, DbType dbType) => new OracleParameter(name, dbType) { Value = value != null ? value : DBNull.Value };
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            var conn = new OracleConnection(_connstring);
            var cmd = new OracleCommand(commandText, conn);
            cmd.Parameters.AddRange(parameters);
            return (conn, cmd);
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters)
        {
            var orcparameters = new OracleParameter[parameters.Length];
            var index = 0;
            foreach (var (name, value) in parameters)
            {
                orcparameters[index] = new OracleParameter(name, value != null ? DBNull.Value : value);
                index++;
            }
            return CreateConnection(commandText, orcparameters);
        }
    }
}
