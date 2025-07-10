using Astro.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Utils
{
    public class BinaryBuilder : IDisposable
    {
        private bool disposedValue;
        private readonly MemoryStream ms;
        private readonly BinaryWriter writer;
        public BinaryBuilder()
        {
            ms = new MemoryStream();
            writer = new BinaryWriter(ms);
        }
        public void WriteBoolean(bool value)
        {
            writer.Write(value);
        }
        public void WriteInt16(short value)
        {
            writer.Write(value);
        }
        public void WriteInt32(int value)
        {
            writer.Write(value);
        }
        public void WriteInt64(Int64 value)
        {
            writer.Write(value);
        }
        public void WriteDouble(double value)
        {
            writer.Write(value);
        }
        public void WriteDateTime(DateTime value)
        {
            writer.Write(value.Ticks);
        }
        public void WriteString(string value)
        {
            int length = value.Length;
            writer.Write(length);
            writer.Write(Encoding.UTF8.GetBytes(value));
        }
        public byte[] ToArray()
        {
            return ms.ToArray();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ms.Close();
                    ms.Dispose();

                    writer.Close();
                    writer.Dispose();
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

    public class DataTableBuilder : IDisposable
    {
        private bool disposedValue;
        private DataTable table;
        private readonly long nullDateTime;
        private readonly BinaryReader reader;
        public DataTableBuilder(Stream stream)
        {
            reader = new BinaryReader(stream);
            nullDateTime = (new DateTime(1900, 1, 1).Ticks);
            table = new DataTable();
        }
        public virtual DataTable ToDataTable()
        {
            return table;
        }
        protected void AddColumn(string name, System.Type type) => this.table.Columns.Add(name, type);
        protected void AddRow(object[] values) => this.table.Rows.Add(values);
        protected int ReadInt32() => reader.ReadInt32();
        protected long ReadInt64() => reader.ReadInt64();
        protected double ReadDouble() => reader.ReadDouble();
        protected bool ReadBoolean() => reader.ReadBoolean();
        protected object ReadDateTime()
        {
            long longDate = reader.ReadInt64();
            if (longDate == nullDateTime || longDate == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return new DateTime(longDate);
            }
        }
        protected bool Read() => reader.BaseStream.Position < reader.BaseStream.Length;
        protected string ReadString()
        {
            int length = reader.ReadInt32();
            byte[] byteString = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(byteString);
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

    public class ListOptionBuilder : IDisposable
    {
        private bool disposedValue;
        private readonly BinaryReader reader;
        public ListOptionBuilder(Stream stream) => reader = new BinaryReader(stream);
        public ListOption ToListOption(Type idType)
        {
            var list = new ListOption();
            while (Read())
            {
                var option = new Option
                {
                    Id = idType == typeof(System.Int32) ? ReadInt32() : ReadInt16(),
                    Text = ReadString()
                };
                list.Add(option);
            }
            return list;
        }
        private bool Read() => reader.BaseStream.Position < reader.BaseStream.Length;
        private short ReadInt16() => reader.ReadInt16();
        private int ReadInt32() => reader.ReadInt32();
        private string ReadString()
        {
            var length = ReadInt32();
            var bytes = reader.ReadBytes(length);
            return System.Text.Encoding.UTF8.GetString(bytes);
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
