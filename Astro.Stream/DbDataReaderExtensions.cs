using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
        public static void SetData(this System.Data.Common.DbDataReader reader, int ordinal, Streams.Writer writer, DbType type)
        {
            actions[type].Invoke(reader, ordinal, writer);
        }
        public static object GetData(this Streams.Reader reader, DbType type)
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
        private static readonly Dictionary<DbType, Action<DbDataReader, int, Streams.Writer>> actions = new Dictionary<DbType, Action<DbDataReader, int, Streams.Writer>>(
            new[]
            {
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Boolean, (r, o, w) => w.WriteBoolean(r.IsDBNull(o) ? default : r.GetBoolean(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Byte, (r, o, w) => w.WriteByte(r.IsDBNull(o) ? default : r.GetByte(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.SByte, (r, o, w) => w.WriteSByte(r.IsDBNull(o) ? default : (sbyte)r.GetByte(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Int16, (r, o, w) => w.WriteInt16(r.IsDBNull(o) ? default : r.GetInt16(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.UInt16, (r, o, w) => w.WriteUInt16(r.IsDBNull(o) ? default : (ushort)r.GetInt16(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Int32, (r, o, w) => w.WriteInt32(r.IsDBNull(o) ? default : r.GetInt32(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.UInt32, (r, o, w) => w.WriteUInt32(r.IsDBNull(o) ? default : (uint)r.GetInt32(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Int64, (r, o, w) => w.WriteInt64(r.IsDBNull(o) ? default : r.GetInt64(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.UInt64, (r, o, w) => w.WriteUInt64(r.IsDBNull(o) ? default : (ulong)r.GetInt64(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Single, (r, o, w) => w.WriteSingle(r.IsDBNull(o) ? default : r.GetFloat(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Double, (r, o, w) => w.WriteDouble(r.IsDBNull(o) ? default : r.GetDouble(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Decimal, (r, o, w) => w.WriteDecimal(r.IsDBNull(o) ? default : r.GetDecimal(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.String, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetString(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.StringFixedLength, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetString(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Guid, (r, o, w) => w.WriteGuid(r.IsDBNull(o) ? null : r.GetGuid(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.DateTime, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : r.GetDateTime(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.DateTime2, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : r.GetDateTime(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.DateTimeOffset, (r, o, w) => w.WriteDateTime(r.IsDBNull(o) ? DateTime.MinValue : ((DateTimeOffset)r.GetValue(o)).DateTime)),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Binary, (r, o, w) => w.WriteBytes(r.IsDBNull(o) ? Array.Empty<byte>() : (byte[])r.GetValue(o))),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Time, (r, o, w) => w.WriteInt64(r.IsDBNull(o) ? 0L : ((TimeSpan)r.GetValue(o)).Ticks)),
                new KeyValuePair<DbType, Action<DbDataReader, int, Streams.Writer>>(DbType.Object, (r, o, w) => w.WriteString(r.IsDBNull(o) ? string.Empty : r.GetValue(o).ToString()))
            }
        );
        private static readonly Dictionary<DbType, Func<Streams.Reader, object>> functions = new Dictionary<DbType, Func<Streams.Reader, object>>(
            new[]
            {
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Boolean, r => r.ReadBoolean()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Byte, r => r.ReadByte()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.SByte, r => r.ReadSByte()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Int16, r => r.ReadInt16()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.UInt16, r => r.ReadUInt16()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Int32, r => r.ReadInt32()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.UInt32, r => r.ReadUInt32()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Int64, r => r.ReadInt64()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.UInt64, r => r.ReadUInt64()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Single, r => r.ReadSingle()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Double, r => r.ReadDouble()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Decimal, r => r.ReadDecimal()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.String, r => r.ReadString()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.StringFixedLength, r => r.ReadString()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Guid, r => r.ReadGuid()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.DateTime, r => r.ReadDateTime()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.DateTime2, r => r.ReadDateTime()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.DateTimeOffset, r => new DateTimeOffset(r.ReadDateTime())),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Binary, r => r.ReadBytes()),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Time, r => new TimeSpan(r.ReadInt64())),
                new KeyValuePair<DbType, Func<Streams.Reader, object>>(DbType.Object, r => r.ReadString())
            }
        );
        /*
        public static readonly Func<IDataReader, int, object>[] myFunctions = new Func<IDataReader, int, object>[]
        {
            (reader, ordinal) => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal),                   // 0  DbType.AnsiString
            (reader, ordinal) => reader.IsDBNull(ordinal) ? Array.Empty<byte>() : (byte[])reader.GetValue(ordinal),     // 1  DbType.Binary
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (byte)0 : reader.GetByte(ordinal),                          // 2  DbType.Byte
            (reader, ordinal) => reader.IsDBNull(ordinal) ? false : reader.GetBoolean(ordinal),                         // 3  DbType.Boolean
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0m : reader.GetDecimal(ordinal),                            // 4  DbType.Currency
            (reader, ordinal) => reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal),            // 5  DbType.Date
            (reader, ordinal) => reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal),            // 6  DbType.DateTime
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0m : reader.GetDecimal(ordinal),                            // 7  DbType.Decimal
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0d : reader.GetDouble(ordinal),                             // 8  DbType.Double
            (reader, ordinal) => reader.IsDBNull(ordinal) ? Guid.Empty : reader.GetGuid(ordinal),                       // 9  DbType.Guid
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (short)0 : reader.GetInt16(ordinal),                        // 10 DbType.Int16
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal),                               // 11 DbType.Int32
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0L : reader.GetInt64(ordinal),                              // 12 DbType.Int64
            (reader, ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal),                            // 13 DbType.Object
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (sbyte)0 : (sbyte)reader.GetByte(ordinal),                  // 14 DbType.SByte
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0f : reader.GetFloat(ordinal),                              // 15 DbType.Single
            (reader, ordinal) => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal),                   // 16 DbType.String
            (reader, ordinal) => reader.IsDBNull(ordinal) ? TimeSpan.Zero : (TimeSpan)reader.GetValue(ordinal),         // 17 DbType.Time
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (ushort)0 : (ushort)reader.GetInt16(ordinal),               // 18 DbType.UInt16
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (uint)0 : (uint)reader.GetInt32(ordinal),                   // 19 DbType.UInt32
            (reader, ordinal) => reader.IsDBNull(ordinal) ? (ulong)0 : (ulong)reader.GetInt64(ordinal),                 // 20 DbType.UInt64
            (reader, ordinal) => reader.IsDBNull(ordinal) ? 0m : reader.GetDecimal(ordinal),                            // 21 DbType.VarNumeric
            (reader, ordinal) => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal),                   // 22 DbType.AnsiStringFixedLength
            (reader, ordinal) => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal),                   // 23 DbType.StringFixedLength
            (reader, ordinal) => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal),                   // 24 DbType.Xml (as string)
            (reader, ordinal) => reader.IsDBNull(ordinal) ? DateTime.MinValue : (DateTime)reader.GetValue(ordinal),     // 25 DbType.DateTime2
            (reader, ordinal) => reader.IsDBNull(ordinal) ? DateTimeOffset.MinValue : (DateTimeOffset)reader.GetValue(ordinal), // 26 DbType.DateTimeOffset
            (reader, ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal),                            // 27 (Reserved/Unknown)
        };
        
public static Action<IDataReader, int, Streams.Writer>[] myActions = new Action<IDataReader, int, Streams.Writer>[]
{
    (reader, ordinal, writer) => writer.WriteString(reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal)),
    (reader, ordinal, writer) => writer.WriteBytes(reader.IsDBNull(ordinal) ? Array.Empty<byte>() : (byte[])reader.GetValue(ordinal)),
    (reader, ordinal, writer) => writer.WriteBoolean(reader.IsDBNull(ordinal) ? default(bool) : reader.GetBoolean(ordinal)),
    (reader, ordinal, writer) => writer.WriteDecimal(reader.IsDBNull(ordinal) ? default(decimal) : reader.GetDecimal(ordinal)),
    (reader, ordinal, writer) => writer.WriteDateTime(reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal)),
    (reader, ordinal, writer) => writer.WriteDateTime(reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal)),
    (reader, ordinal, writer) => writer.WriteDecimal(reader.IsDBNull(ordinal) ? default(decimal) : reader.GetDecimal(ordinal)),
    (reader, ordinal, writer) => writer.WriteDouble(reader.IsDBNull(ordinal) ? default(double) : reader.GetDouble(ordinal)),
    (reader, ordinal, writer) => writer.WriteGuid(reader.IsDBNull(ordinal) ? null : reader.GetGuid(ordinal)),
    (reader, ordinal, writer) => writer.WriteInt16(reader.IsDBNull(ordinal) ? default(short) : reader.GetInt16(ordinal)),
    (reader, ordinal, writer) => writer.WriteInt32(reader.IsDBNull(ordinal) ? default(int) : reader.GetInt32(ordinal)),
    (reader, ordinal, writer) => writer.WriteInt64(reader.IsDBNull(ordinal) ? default(long) : reader.GetInt64(ordinal)),
    (reader, ordinal, writer) => writer.WriteString(reader.IsDBNull(ordinal) ? string.Empty : reader.GetValue(ordinal).ToString()),
    (reader, ordinal, writer) => writer.WriteSByte(reader.IsDBNull(ordinal) ? default(sbyte) : (sbyte)reader.GetValue(ordinal)),
    (reader, ordinal, writer) => writer.WriteSingle(reader.IsDBNull(ordinal) ? default(float) : (sbyte)reader.GetFloat(ordinal)),
    (reader, ordinal, writer) => writer.WriteString(reader.IsDBNull(ordinal) ? default(string) : reader.GetString(ordinal)),
};
AnsiString = 0,
Binary = 1,
Byte = 2,
Boolean = 3,
Currency = 4,
Date = 5,
DateTime = 6,
Decimal = 7,
Double = 8,
Guid = 9,
Int16 = 10,
Int32 = 11,
Int64 = 12,
Object = 13,
SByte = 14,
Single = 15,
String = 16,
Time = 17,
UInt16 = 18,
UInt32 = 19,
UInt64 = 20,
VarNumeric = 21,
AnsiStringFixedLength = 22,
//
// Summary:
//     A fixed-length string of Unicode characters.
StringFixedLength = 23,
//
// Summary:
//     A parsed representation of an XML document or fragment.
Xml = 25,
//
// Summary:
//     Date and time data. Date value range is from January 1,1 AD through December
//     31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999 with an accuracy
//     of 100 nanoseconds.
DateTime2 = 26,
//
// Summary:
//     Date and time data with time zone awareness. Date value range is from January
//     1,1 AD through December 31, 9999 AD. Time value range is 00:00:00 through 23:59:59.9999999
//     with an accuracy of 100 nanoseconds. Time zone value range is -14:00 through
//     +14:00.
DateTimeOffset = 27
 */
    }
}
