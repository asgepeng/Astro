using System.Data;
namespace Astro.Maps
{
    public static class Types
    {
        public static DbType GetDbType(this Type type) => typeDict[type];
        private static readonly Dictionary<Type, DbType> typeDict = new Dictionary<Type, DbType>(
            new[]
            {
                new KeyValuePair<Type, DbType>(typeof(bool), DbType.Boolean),
                new KeyValuePair<Type, DbType>(typeof(byte), DbType.Byte),
                new KeyValuePair<Type, DbType>(typeof(sbyte), DbType.SByte),
                new KeyValuePair<Type, DbType>(typeof(short), DbType.Int16),
                new KeyValuePair<Type, DbType>(typeof(ushort), DbType.UInt16),
                new KeyValuePair<Type, DbType>(typeof(int), DbType.Int32),
                new KeyValuePair<Type, DbType>(typeof(uint), DbType.UInt32),
                new KeyValuePair<Type, DbType>(typeof(long), DbType.Int64),
                new KeyValuePair<Type, DbType>(typeof(ulong), DbType.UInt64),
                new KeyValuePair<Type, DbType>(typeof(float), DbType.Single),
                new KeyValuePair<Type, DbType>(typeof(double), DbType.Double),
                new KeyValuePair<Type, DbType>(typeof(decimal), DbType.Decimal),
                new KeyValuePair<Type, DbType>(typeof(string), DbType.String),
                new KeyValuePair<Type, DbType>(typeof(char), DbType.StringFixedLength),
                new KeyValuePair<Type, DbType>(typeof(Guid), DbType.Guid),
                new KeyValuePair<Type, DbType>(typeof(DateTime), DbType.DateTime),
                new KeyValuePair<Type, DbType>(typeof(DateTimeOffset), DbType.DateTimeOffset),
                new KeyValuePair<Type, DbType>(typeof(byte[]), DbType.Binary),
                new KeyValuePair<Type, DbType>(typeof(TimeSpan), DbType.Time),
                new KeyValuePair<Type, DbType>(typeof(object), DbType.Object)
            }
        );
        public static Action<IDataReader, int, BinaryWriter>[] myActions = new Action<IDataReader, int, BinaryWriter>[]
        {
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? Array.Empty<byte>() : (byte[])reader.GetValue(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(bool) : reader.GetBoolean(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(decimal) : reader.GetDecimal(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? DateTime.MinValue.Ticks : reader.GetDateTime(ordinal).Ticks),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? DateTime.MinValue.Ticks : reader.GetDateTime(ordinal).Ticks),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(decimal) : reader.GetDecimal(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(double) : reader.GetDouble(ordinal)),
            (reader, ordinal, writer) =>
            {
                if (reader.IsDBNull(ordinal))
                {
                    writer.Write(false);
                }
                else
                {
                    writer.Write(true);
                    writer.Write(reader.GetGuid(ordinal).ToByteArray());
                }
            },
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(short) : reader.GetInt16(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(int) : reader.GetInt32(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(long) : reader.GetInt64(ordinal)),
            (reader, ordinal, writer) =>
            {
                if (reader.IsDBNull(ordinal))
                {
                    writer.Write("");
                }
                else
                {
                    var obj = reader.GetValue(ordinal);
                    var strObj = obj?.ToString();
                    writer.Write(strObj is null ? "" : strObj);
                }
            },
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(sbyte) : (sbyte)reader.GetValue(ordinal)),
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(float) : (sbyte)reader.GetFloat(ordinal)),
            (reader, ordinal, writer) =>
            {
                if (reader.IsDBNull(ordinal))
                {
                    writer.Write("");
                }
                else
                {
                    writer.Write(reader.GetString(ordinal));
                }
            },
            (reader, ordinal, writer) => writer.Write(reader.IsDBNull(ordinal) ? default(float) : (sbyte)reader.GetFloat(ordinal)),
        };
    }
}
