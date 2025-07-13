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
    public class CString
    {
        public string Server { get; set; } = "";
        public string Database { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public abstract class DatabaseClient : IDatabase
    {
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters);
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters);
     
        public abstract DbParameter CreateParameter(string name, object value);
        public abstract DbParameter CreateParameter(string name, DbType type, object value);

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
                    File.WriteAllText(AppContext.BaseDirectory + "db.executeReaderAsync.txt", ex.ToString());
                }
            }
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
        public object? ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch { return null; }
            }
        }
        public async Task<object?> ExecuteScalarAsync(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    await conn.OpenAsync();
                    return await cmd.ExecuteScalarAsync();
                }
                catch { return null; }
            }
        }
        public int? ExecuteScalarInteger(string commandText, params DbParameter[] parameters)
        {
            var obj = ExecuteScalar(commandText, parameters);
            if (obj is null) return null;
            return Convert.ToInt32(obj);
        }
        public async Task<int?> ExecuteScalarIntegerAsync(string commandText, params DbParameter[] parameters)
        {
            var obj = await ExecuteScalarAsync(commandText, parameters);
            if (obj is null) return null;
            return Convert.ToInt32(obj);
        }
        public async Task<short?> ExecuteScalarInt16Async(string commandText, params DbParameter[] parameters)
        {
            var obj = await ExecuteScalarAsync(commandText, parameters);
            if (obj is null) return null;
            return Convert.ToInt16(obj);
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
    }
    public sealed class MySqlDatabase : DatabaseClient
    {
        private readonly string connectionString;
        public MySqlDatabase(string connstring)
        {
            this.connectionString = connstring;
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public override DbParameter CreateParameter(string name, DbType type, object value)
        {
            return new MySqlParameter(name, type)
            {
                Value = value
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

        public override DbParameter CreateParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }

        public override DbParameter CreateParameter(string name, DbType type, object value)
        {
            return new NpgsqlParameter(name, type)
            {
                Value = value
            };
        }
    }
    public sealed class SqlDatabase : DatabaseClient
    {
        public override DbParameter CreateParameter(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override DbParameter CreateParameter(string name, DbType type, object value)
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
