using Astro.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.IO
{
    public class Reader : IDisposable
    {
        private bool disposedValue;
        private readonly BinaryReader reader;
        public Reader(Stream stream) => reader = new BinaryReader(stream, Encoding.UTF8);
        public bool ReadBoolean() => reader.ReadBoolean();
        public byte ReadByte() => reader.ReadByte();
        public sbyte ReadSByte() => reader.ReadSByte();
        public short ReadInt16() => reader.ReadInt16();
        public ushort ReadUInt16() => reader.ReadUInt16();
        public int ReadInt32() => reader.ReadInt32();
        public uint ReadUInt32() => reader.ReadUInt32();
        public long ReadInt64() => reader.ReadInt64();
        public ulong ReadUInt64() => reader.ReadUInt64();
        public float ReadSingle() => reader.ReadSingle();
        public double ReadDouble() => reader.ReadDouble();
        public decimal ReadDecimal()=> reader.ReadDecimal();
        public string ReadString() => reader.ReadString();
        public DateTime ReadDateTime()
        {
            var ticks = ReadInt64();
            return ticks == 0 ? DateTime.MinValue : new DateTime(ticks, DateTimeKind.Utc);
        }
        public object ReadDateTimeOrDBNull()
        {
            long ticks = reader.ReadInt64();
            long defaultDate = (new DateTime(1900, 1, 1)).Ticks;
            return (ticks == 0 || ticks == defaultDate) ? DBNull.Value : new DateTime(ticks, DateTimeKind.Utc);
        }
        public DateTime? ReadNullableDateTime()
        {
            long ticks = reader.ReadInt64();
            return ticks == 0 ? (DateTime?)null : new DateTime(ticks);
        }
        public bool Read() => reader.BaseStream.Position < reader.BaseStream.Length;
        public byte[] ReadBytes()
        {
            var length = reader.ReadInt32();
            return reader.ReadBytes(length);
        }
        public System.Data.DataTable ReadDataTable()
        {
            var table = new DataTable();
            var colCount = ReadByte();
            var types = new DbType[colCount];

            for (int i = 0; i < colCount; i++)
            {
                var colName = ReadString();
                types[i] = (DbType)ReadByte();
                table.Columns.Add(colName, types[i].ToType());
            }
            try
            {
                var rowCount = ReadInt32();
                for (int i = 0; i < rowCount; i++)
                {
                    var values = new object[colCount];
                    for (int j = 0; j < colCount; j++)
                    {
                        values[j] = this.GetData(types[j]);
                    }
                    table.Rows.Add(values);
                }
            }
            catch { }
            
            return table;
        }
        public Guid? ReadGuid()
        {
            if (reader.ReadBoolean())
            {
                byte[] bytes = reader.ReadBytes(16);
                return new Guid(bytes);
            }
            return null;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    reader.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
