using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Globalization;
using Npgsql;
using System.Data;

namespace Astro.Data
{
    public interface IDatabase
    {
        DbParameter CreateParameter(string name, object? value);
        DbParameter CreateParameter(string name, object? value, DbType dbType);

        Task<bool> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters);
        Task<T?> ExecuteScalarAsync<T>(string commandText, params DbParameter[] parameters);
        Task<bool> AnyRecordsAsync(string commandText, params DbParameter[] parameters);
        Task ExecuteReaderAsync(Func<DbDataReader, Task> handler, string commandText, params DbParameter[] parameters);
        Task<List<T>> CreateListAsync<T>(string commandText, params DbParameter[] parameters);
        Task<Dictionary<TKey, TValue>> CreateDictionyAsync<TKey, TValue>(string commandText, params DbParameter[] parameters)
            where TKey : notnull where TValue : notnull;

        bool ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        T? ExecuteScalar<T>(string commandText, params DbParameter[] parameters);
        bool AnyRecords(string commandText, params DbParameter[] parameters);
        void ExecuteReader(Action<DbDataReader> handler, string commandText, params DbParameter[] parameters);
        List<T> CreateList<T>(string commandText, params DbParameter[] parameters);

    }
    public abstract class DatabaseClient : IDatabase
    {
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters);
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters);
     
        public bool AnyRecords(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
                catch { return false; }
            }
        }
        public async Task<bool> AnyRecordsAsync(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        return reader.HasRows;
                    }
                }
                catch { return false; }
            }
        }
        public bool ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    File.WriteAllText(AppContext.BaseDirectory + "db.executeNonQuery.txt", commandText + "\r\n\r\n" +  ex.ToString());
                    return false; 
                }
            }
        }
        public void ExecuteReader(Action<DbDataReader> handler, string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        handler(reader);
                    }
                }
                catch { }
            }
        }
        public List<T> CreateList<T>(string commandText, params DbParameter[] parameters)
        {
            var list = new List<T>();
            ExecuteReader(async reader =>
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        var item = (T)Convert.ChangeType(reader.GetValue(0), typeof(T))!;
                        if (item is not null)
                        {
                            list.Add(item);
                        }
                    }
                }
            }, commandText, parameters);
            return list;
        }
        public async Task ExecuteReaderAsync(Func<DbDataReader, Task> handler, string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await handler(reader);
                    }
                }
                catch(Exception ex) 
                {
                    File.WriteAllText(AppContext.BaseDirectory + "db.executeReaderAsync.txt", commandText + "\n" + ex.ToString());
                }
            }
        }
        public async Task<List<T>> CreateListAsync<T>(string commandText, params DbParameter[] parameters)
        {
            var list = new List<T>();
            await ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull(0))
                    {
                        var item  = (T)Convert.ChangeType(reader.GetValue(0), typeof(T))!;
                        if (item is not null)
                        {
                            list.Add(item);
                        }
                    }
                }
            }, commandText, parameters);
            return list;
        }
        public async Task<Dictionary<TKey, TValue>> CreateDictionyAsync<TKey, TValue>(string commandText, params DbParameter[] parameters) where TKey : notnull where TValue : notnull
        {
            var dict = new Dictionary<TKey, TValue>();
            await ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var okey = !reader.IsDBNull(0) ? (TKey)Convert.ChangeType(reader.GetValue(0), typeof(TKey))! : default!;
                    var ovalue = !reader.IsDBNull(1) ? (TValue)Convert.ChangeType(reader.GetValue(1), typeof(TValue))! : default!;
                    if (okey is not null && ovalue is not null)
                    {
                        dict.Add(okey, ovalue);
                    }
                }
            }, commandText, parameters);
            return dict;
        }
        public byte[] ExportToExcel(string commandText, params (string, object)[] parameters)
        {
            var data = Array.Empty<byte>();
            var filename = AppContext.BaseDirectory + DateTime.Now.Ticks.ToString();
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                using (var stream = new MemoryStream())
                using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Tambahkan stylesheet untuk format tanggal
                    WorkbookStylesPart stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = GenerateStylesheet();
                    stylesPart.Stylesheet.Save();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    SheetData sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    if (document.WorkbookPart is null) throw new ArgumentException("document.WorkbookPar is null");
                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Data"
                    };
                    sheets.Append(sheet);

                    // Header row
                    Row headerRow = new Row();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnName = reader.GetName(i);
                        Cell cell = new Cell()
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(columnName)
                        };
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);

                    // Data rows
                    while (reader.Read())
                    {
                        Row newRow = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Type type = reader.GetFieldType(i);
                            Cell cell = new Cell();
                            object? value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                            if (value is null)
                            {
                                type = typeof(string);
                                value = "";
                            }

                            if (type == typeof(DateTime) && value != DBNull.Value)
                            {
                                double oaDate = ((DateTime)value).ToOADate();
                                cell.CellValue = new CellValue(oaDate.ToString(CultureInfo.InvariantCulture));
                                cell.DataType = CellValues.Number;
                                cell.StyleIndex = 1; // Gaya tanggal
                            }
                            else if (type == typeof(decimal))
                            {
                                cell.CellValue = new CellValue(((decimal)value).ToString(CultureInfo.InvariantCulture));
                                cell.DataType = CellValues.Number;
                            }
                            else if (type == typeof(int))
                            {
                                cell.CellValue = new CellValue(((int)value).ToString());
                                cell.DataType = CellValues.Number;
                            }
                            else if (type == typeof(long))
                            {
                                cell.CellValue = new CellValue(((long)value).ToString());
                                cell.DataType = CellValues.Number;
                            }
                            else
                            {
                                cell.CellValue = new CellValue(value?.ToString() ?? "");
                                cell.DataType = CellValues.String;
                            }

                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }

                    workbookPart.Workbook.Save();
                    data = stream.ToArray();
                }
            }
            return data;
        }
        public T? ExecuteScalar<T>(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn)
            using (cmd)
            {
                try
                {
                    conn.Open();
                    object? res = cmd.ExecuteScalar();
                    if (res is null || res == DBNull.Value)
                        return default;

                    return (T)Convert.ChangeType(res, typeof(T))!;
                }
                catch
                {
                    return default;
                }
            }
        }

        public async Task<T?> ExecuteScalarAsync<T>(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    object? result = await cmd.ExecuteScalarAsync();
                    if (result is null || result == DBNull.Value)
                    {
                        return default;
                    }
                    return (T?)Convert.ChangeType(result, typeof(T));
                }
                catch { return default; }
            }
        }
        //FUNCTION
        private Stylesheet GenerateStylesheet()
        {
            return new Stylesheet(
                new NumberingFormats(
                    new NumberingFormat()
                    {
                        NumberFormatId = 164, // Custom formats must be >= 164
                        FormatCode = "yyyy-mm-dd HH:Mm:ss"
                    }
                ),
                new Fonts(new Font()),
                new Fills(new Fill()),
                new Borders(new Border()),
                new CellFormats(
                    new CellFormat(), // default
                    new CellFormat()
                    {
                        NumberFormatId = 164,
                        ApplyNumberFormat = true
                    }
                )
            );
        }

        public abstract DbParameter CreateParameter(string name, object? value);
        public abstract DbParameter CreateParameter(string name, object? value, DbType dbType);
    }
    public sealed class MySqlDatabase : DatabaseClient
    {
        private readonly string connectionString;
        public MySqlDatabase(string connstring)
        {
            this.connectionString = connstring;
        }

        public override DbParameter CreateParameter(string name, object? value)
        {
            return new MySqlParameter(name, value ?? DBNull.Value);
        }

        public override DbParameter CreateParameter(string name, object? value, DbType dbType)
        {
            return new MySqlParameter(name, dbType)
            {
                Value = value ?? DBNull.Value
            };
        }

        protected override (DbConnection, DbCommand) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            var conn = new MySqlConnection(this.connectionString);
            var cmd = new MySqlCommand(commandText, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
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
    }
    public sealed class NpgsqlDatabase : DatabaseClient
    {
        private string connectionString;
        public NpgsqlDatabase(string constring)
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
            for (int i=0; i < parameters.Length; i++)
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
    public sealed class SqlDatabase : DatabaseClient
    {
        public override DbParameter CreateParameter(string name, object? value)
        {
            throw new NotImplementedException();
        }

        public override DbParameter CreateParameter(string name, object? value, DbType dbType)
        {
            throw new NotImplementedException();
        }

        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }
        protected override (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
