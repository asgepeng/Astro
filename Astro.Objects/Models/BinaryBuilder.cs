using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Models
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
        public long WriteBoolean(bool value)
        {
            writer.Write(value);
            return writer.BaseStream.Position - 1;
        }
        public long WriteInt16(short value)
        {
            writer.Write(value);
            return writer.BaseStream.Position - 2;
        }
        public void WriteInt16(short value, long pos)
        {
            writer.BaseStream.Seek(pos, SeekOrigin.Begin);
            writer.Write(value);
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }
        public long WriteInt32(int value)
        {
            writer.Write(value);
            return writer.BaseStream.Position - 4;
        }
        public long WriteInt64(Int64 value)
        {
            writer.Write(value);
            return writer.BaseStream.Position - 8;
        }
        public void WriteInt64(Int64 value, Int64 pos)
        {
            ms.Seek(pos, SeekOrigin.Begin);
            writer.Write(value);
            ms.Seek(0, SeekOrigin.End);
        }
        public void WriteDouble(double value)
        {
            writer.Write(value);
        }
        public void WriteDateTime(DateTime value)
        {
            writer.Write(value.Ticks);
        }
        public void MoveFirst() => ms.Seek(0, SeekOrigin.Begin);
        public void MoveLast() => ms.Seek(0, SeekOrigin.End);
        public long GetLength() => ms.Length;
        public long GetPosition() => ms.Position;
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
        protected short ReadInt16() => reader.ReadInt16();
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
        private readonly BinaryReader reader;
        private bool disposedValue;


        public ListOptionBuilder(Stream stream)
        {
            this.reader = new BinaryReader(stream);
        }
        public ListOption GetListOption(Type idType)
        {
            var listOption = new ListOption();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var option = new Option
                {
                    Id = idType == typeof(short) ? (int)reader.ReadInt16() : reader.ReadInt32(),
                    Text = ReadString()
                };
                listOption.Add(option);
            }
            return listOption;
        }
        private string ReadString()
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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
