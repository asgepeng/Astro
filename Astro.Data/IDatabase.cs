using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Data
{
    public interface IDatabase
    {
        DbParameter CreateParameter(string name, object? value);
        DbParameter CreateParameter(string name, object? value, DbType dbType);

        Task<bool> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters);
        Task<object?> ExecuteScalarAsync(string commandText, params DbParameter[] parameters);
        Task<int?> ExecuteScalarIntegerAsync(string commandText, params DbParameter[] parameters);
        Task<short?> ExecuteScalarInt16Async(string commandText, params DbParameter[] parameters);
        Task<bool> AnyRecordsAsync(string commandText, params DbParameter[] parameters);
        Task ExecuteReaderAsync(Func<DbDataReader, Task> handler, string commandText, params DbParameter[] parameters);

        bool ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        object? ExecuteScalar(string commandText, params DbParameter[] parameters);
        int? ExecuteScalarInteger(string commandText, params DbParameter[] parameters);
        bool AnyRecords(string commandText, params DbParameter[] parameters);
        void ExecuteReader(Action<DbDataReader> handler, string commandText, params DbParameter[] parameters);
    }
}
