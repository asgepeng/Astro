using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Data.Common;
using System.Globalization;
using Npgsql;
using System.Data;
using Org.BouncyCastle.Tls;

namespace Astro.Data
{
    public interface IDBClient
    {
        DbParameter CreateParameter(string name, object? value);
        DbParameter CreateParameter(string name, object? value, DbType dbType);

        Task<bool> ExecuteNonQueryAsync(string commandText, params DbParameter[] parameters);
        Task ExecuteReaderAsync(Func<DbDataReader, Task> handler, string commandText, params DbParameter[] parameters);
        Task<T?> ExecuteScalarAsync<T>(string commandText, params DbParameter[] parameters);
        Task<bool> HasRecordsAsync(string commandText, params DbParameter[] parameters);     
        Task<List<T>> FetchListAsync<T>(string commandText, params DbParameter[] parameters);
        Task<Dictionary<TKey, TValue>> FetchDictionaryAsync<TKey, TValue>(string commandText, params DbParameter[] parameters)
            where TKey : notnull where TValue : notnull;

        bool ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        void ExecuteReader(Action<DbDataReader> handler, string commandText, params DbParameter[] parameters);
        T? ExecuteScalar<T>(string commandText, params DbParameter[] parameters);
        bool HasRecords(string commandText, params DbParameter[] parameters);
        List<T> FetchList<T>(string commandText, params DbParameter[] parameters);
        Dictionary<TKey, TValue>FetchDictionary<TKey, TValue>(string commandText, params DbParameter[] parameters)
            where TKey : notnull where TValue : notnull;

    }
    public abstract class DBClient : IDBClient
    {
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params DbParameter[] parameters);
        protected abstract (DbConnection conn, DbCommand cmd) CreateConnection(string commandText, params (string, object)[] parameters);

        public bool ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            var (conn, cmd) = CreateConnection(commandText, parameters);
            using (conn) using (cmd)
            {
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception)
                {
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
                        return default(T);

                    return (T)Convert.ChangeType(res, typeof(T))!;
                }
                catch
                {
                    return default(T);
                }
            }
        }
        public bool HasRecords(string commandText, params DbParameter[] parameters)
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
        public List<T> FetchList<T>(string commandText, params DbParameter[] parameters)
        {
            var list = new List<T>();
            ExecuteReader(reader =>
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
        public Dictionary<TKey, TValue> FetchDictionary<TKey, TValue>(string commandText, params DbParameter[] parameters) where TKey : notnull where TValue : notnull
        {
            var dict = new Dictionary<TKey, TValue>();
            ExecuteReader(reader =>
            {
                while (reader.Read())
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
                    var logPath = Path.Combine(AppContext.BaseDirectory, "sql.log");
                    System.IO.File.AppendAllText(logPath, commandText + "\n\n" +  ex.Message);
                    return false;
                }
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
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
        public async Task<bool> HasRecordsAsync(string commandText, params DbParameter[] parameters)
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
        public async Task<List<T>> FetchListAsync<T>(string commandText, params DbParameter[] parameters)
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
        public async Task<Dictionary<TKey, TValue>> FetchDictionaryAsync<TKey, TValue>(string commandText, params DbParameter[] parameters) where TKey : notnull where TValue : notnull
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

        public abstract DbParameter CreateParameter(string name, object? value);
        public abstract DbParameter CreateParameter(string name, object? value, DbType dbType);

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
}
