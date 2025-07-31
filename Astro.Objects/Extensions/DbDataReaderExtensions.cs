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
        public static void SetData(this System.Data.Common.DbDataReader reader, int ordinal, IO.Writer writer, DbType type)
        {
            actions[type].Invoke(reader, ordinal, writer);
        }
        public static object GetData(this IO.Reader reader, DbType type)
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
        private static readonly Dictionary<DbType, Action<DbDataReader, int, IO.Writer>> actions = new Dictionary<DbType, Action<DbDataReader, int, IO.Writer>>(
            new[]
            {
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Boolean, (r, o, w) => w.WriteBoolean(r.IsDBNull(o) ? default : r.GetBoolean(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Byte, (r, o, w) => w.WriteByte(r.IsDBNull(o) ? default : r.GetByte(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.SByte, (r, o, w) => w.WriteSByte(r.IsDBNull(o) ? default : (sbyte)r.GetByte(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int16, (r, o, w) => w.WriteInt16(r.IsDBNull(o) ? default : r.GetInt16(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt16, (r, o, w) => w.WriteUInt16(r.IsDBNull(o) ? default : (ushort)r.GetInt16(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int32, (r, o, w) => w.WriteInt32(r.IsDBNull(o) ? default : r.GetInt32(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt32, (r, o, w) => w.WriteUInt32(r.IsDBNull(o) ? default : (uint)r.GetInt32(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Int64, (r, o, w) => w.WriteInt64(r.IsDBNull(o) ? default : r.GetInt64(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.UInt64, (r, o, w) => w.WriteUInt64(r.IsDBNull(o) ? default : (ulong)r.GetInt64(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Single, (r, o, w) => w.WriteSingle(r.IsDBNull(o) ? default : r.GetFloat(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Double, (r, o, w) => w.WriteDouble(r.IsDBNull(o) ? default : r.GetDouble(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Decimal, (r, o, w) => w.WriteDecimal(r.IsDBNull(o) ? default : r.GetDecimal(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.String, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetString(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.StringFixedLength, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetString(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Guid, (r, o, w) => w.WriteGuid(r.IsDBNull(o) ? Guid.Empty : r.GetGuid(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTime, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : r.GetDateTime(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTime2, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : r.GetDateTime(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.DateTimeOffset, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : ((DateTimeOffset)r.GetValue(o)).DateTime)),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Binary, (r, o, w) => w.WriteBytes(r.IsDBNull(o) ? Array.Empty<byte>() : (byte[])r.GetValue(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Time, (r, o, w) => w.WriteInt64(r.IsDBNull(o) ? 0L : ((TimeSpan)r.GetValue(o)).Ticks)),
                new KeyValuePair<DbType, Action<DbDataReader, int, IO.Writer>>(DbType.Object, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetValue(o).ToString()))
            }
        );
        private static readonly Dictionary<DbType, Func<IO.Reader, object>> functions = new Dictionary<DbType, Func<IO.Reader, object>>(
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
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Binary, r => r.ReadBytes()),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Time, r => new TimeSpan(r.ReadInt64())),
                new KeyValuePair<DbType, Func<IO.Reader, object>>(DbType.Object, r => r.ReadString())
            }
        );
    }
}
