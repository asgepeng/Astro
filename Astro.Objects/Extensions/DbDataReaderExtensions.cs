using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Extensions
{
    public static class DbDataReaderExtensions
    {
        public static DbType GetDbType(this System.Data.Common.DbDataReader reader, int ordinal)
        {
            return TypeMappings.Get(reader.GetFieldType(ordinal));
        }
    }

    public static class TypeMappings
    {
        public static DbType Get(Type tp) => typeDict.TryGetValue(tp, out var type) ? type : DbType.Int32;
        public static void Copy(this System.Data.Common.DbDataReader reader, int ordinal, IO.Writer writer, DbType type)
        {
            actions[type].Invoke(reader, ordinal, writer);
        }
        public static object ReadCell(this IO.Reader reader, DbType type)
        {
            return functions[type].Invoke(reader);
        }
        public static Type ToType(this DbType dbType)
        {
            return dbType switch
            {
                DbType.Boolean => typeof(bool),
                DbType.Byte => typeof(byte),
                DbType.SByte => typeof(sbyte),
                DbType.Int16 => typeof(short),
                DbType.UInt16 => typeof(ushort),
                DbType.Int32 => typeof(int),
                DbType.UInt32 => typeof(uint),
                DbType.Int64 => typeof(long),
                DbType.UInt64 => typeof(ulong),
                DbType.Single => typeof(float),
                DbType.Double => typeof(double),
                DbType.Decimal => typeof(decimal),
                DbType.String => typeof(string),
                DbType.StringFixedLength => typeof(char),
                DbType.Guid => typeof(Guid),
                DbType.DateTime => typeof(DateTime),
                DbType.DateTime2 => typeof(DateTime),
                DbType.DateTimeOffset => typeof(DateTimeOffset),
                DbType.Binary => typeof(byte[]),
                DbType.Time => typeof(TimeSpan),
                DbType.Object => typeof(object),
                _ => throw new NotSupportedException($"DbType {dbType} is not supported.")
            };
        }

        private static readonly ConcurrentDictionary<Type, DbType> typeDict = new ConcurrentDictionary<Type, DbType>(
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
        private static readonly ConcurrentDictionary<DbType, Action<DbDataReader, int, IO.Writer>> actions = new ConcurrentDictionary<DbType, Action<DbDataReader, int, IO.Writer>>(
            new[]
            {
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Boolean, (reader, ordinal, writer) => { var val = reader.GetBoolean(ordinal); writer.WriteBoolean(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Byte, (reader, ordinal, writer) => { var val = reader.GetByte(ordinal); writer.WriteByte(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.SByte, (reader, ordinal, writer) => { var val = (sbyte)reader.GetByte(ordinal); writer.WriteSByte(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int16, (reader, ordinal, writer) => { var val = reader.IsDBNull(ordinal) ? (short)0 : reader.GetInt16(ordinal); writer.WriteInt16(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt16, (reader, ordinal, writer) => { var val = (ushort)reader.GetInt16(ordinal); writer.WriteUInt16(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int32, (reader, ordinal, writer) => { var val = reader.GetInt32(ordinal); writer.WriteInt32(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt32, (reader, ordinal, writer) => { var val = (uint)reader.GetInt32(ordinal); writer.WriteUInt32(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int64, (reader, ordinal, writer) => { var val = reader.GetInt64(ordinal); writer.WriteInt64(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt64, (reader, ordinal, writer) => { var val = (ulong)reader.GetInt64(0); writer.WriteUInt64(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Single, (reader, ordinal, writer) => { var val = reader.GetFloat(ordinal); writer.WriteSingle(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Double, (reader, ordinal, writer) => { var val = reader.GetDouble(ordinal); writer.WriteDouble(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Decimal, (reader, ordinal, writer) => { var val = reader.GetDecimal(ordinal); writer.WriteDecimal(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.String, (reader, ordinal, writer) => { var val = reader.GetString(ordinal); writer.WriteString(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.StringFixedLength, (reader, ordinal, writer) => { var val = reader.GetString(ordinal); writer.WriteString(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Guid, (reader, ordinal, writer) => { var val = reader.IsDBNull(ordinal) ? Guid.NewGuid() : reader.GetGuid(ordinal); writer.WriteGuid(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTime, (reader, ordinal, writer) => { var val = reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal); writer.WriteDateTime(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTime2, (reader, ordinal, writer) => { var val = reader.GetDateTime(ordinal); writer.WriteDateTime(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTimeOffset, (reader, ordinal, writer) => { var val = (DateTimeOffset)reader.GetValue(ordinal); writer.WriteDateTime(val.DateTime); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Binary, (reader, ordinal, writer) => { var val = (byte[])reader.GetValue(ordinal); writer.WriteByteArray(val); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Time, (reader, ordinal, writer) => { var val = (TimeSpan)reader.GetValue(ordinal); writer.WriteInt64(val.Ticks); }),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Object, (reader, ordinal, writer) => { var val = reader.GetValue(ordinal).ToString(); writer.WriteString(val); })
            }
        );
        private static readonly ConcurrentDictionary<DbType, Func<IO.Reader, object>> functions = new ConcurrentDictionary<DbType, Func<IO.Reader, object>>(
            new[]
            {
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Boolean, r => r.ReadBoolean()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Byte, r => r.ReadByte()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.SByte, r => r.ReadSByte()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Int16, r => r.ReadInt16()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.UInt16, r => r.ReadUInt16()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Int32, r => r.ReadInt32()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.UInt32, r => r.ReadUInt32()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Int64, r => r.ReadInt64()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.UInt64, r => r.ReadUInt64()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Single, r => r.ReadSingle()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Double, r => r.ReadDouble()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Decimal, r => r.ReadDecimal()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.String, r => r.ReadString()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.StringFixedLength, r => r.ReadString()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Guid, r => r.ReadGuid()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.DateTime, r => r.ReadDateTime()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.DateTime2, r => r.ReadDateTime()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.DateTimeOffset, r => new DateTimeOffset(r.ReadDateTime())),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Binary, r => r.ReadByteArray()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Time, r => new TimeSpan(r.ReadInt64())),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Object, r => r.ReadString())
            }
        );
    }
}
