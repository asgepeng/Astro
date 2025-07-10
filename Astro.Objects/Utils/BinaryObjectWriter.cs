using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Utils
{
    public class BinaryObjectWriter : IDisposable
    {
        private bool disposedValue;
        private readonly MemoryStream ms;
        private readonly BinaryWriter writer;

        public BinaryObjectWriter()
        {
            ms = new MemoryStream();
            writer = new BinaryWriter(ms);
        }
        public void WriteBoolean(bool value) => writer.Write(value);
        public void WriteByte(byte value) => writer.Write(value);
        public void WriteInt16(short value) => writer.Write(value);
        public void WriteInt32(int value) => writer.Write(value);
        public void WriteInt32(int value, int pos)
        {
            writer.Seek(pos, SeekOrigin.Begin);
            writer.Write(value);
            writer.Seek(0, SeekOrigin.End);
        }
        public void WriteInt64(long value) => writer.Write(value);
        public void WriteFloat(float value) => writer.Write(value);
        public void WriteDouble(double value)=> writer.Write(value);
        public void WriteDateTime(DateTime? value)
        {
            var defaultvalue = (new DateTime(1900, 1, 1)).Ticks;
            var ticks = value.HasValue ? value.Value.Ticks : 0L;
            if (ticks == defaultvalue) ticks = 0;
            writer.Write(ticks);
        }
        public void WriteString(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var length = (short)bytes.Length;
            writer.Write(length);
            writer.Write(bytes);
        }
        public void WriteGuid(Guid? value)
        {
            var exists = value.HasValue;
            writer.Write(exists);
            if (exists) writer.Write(value.Value.ToByteArray());
        }
        public int Position => (int)writer.BaseStream.Position;
        public int ReserveInt32(int value)
        {
            writer.Write(value);
            return (int)writer.BaseStream.Position - 4;
        }
        public byte[] ToArray()
        {
            writer.Flush();
            return ms.ToArray();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ms.Dispose();
                    writer.Dispose();
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


    public class BinaryObjectReader : IDisposable
    {
        private bool disposedValue;
        private BinaryReader reader;
        public BinaryObjectReader(Stream stream) => reader = new BinaryReader(stream);

        public bool ReadBoolean() => reader.ReadBoolean();
        public byte ReadByte() => reader.ReadByte();
        public short ReadInt16() => reader.ReadInt16();
        public int ReadInt32() => reader.ReadInt32();
        public long ReadInt64() => reader.ReadInt64();
        public float ReadSingle() => reader.ReadSingle();
        public double ReadDouble() => reader.ReadDouble();
        public DateTime? ReadNullableDateTime()
        {
            var ticks = reader.ReadInt64();
            if (ticks == 0) return null;
            return new DateTime(ticks);
        }
        public DateTime ReadDateTime()
        {
            var ticks = reader.ReadInt64();
            if (ticks == 0) return DateTime.Now;
            return new DateTime(ticks);
        }
        public string ReadString()
        {
            var length = reader.ReadInt16();
            if (length == 0) return string.Empty;
            var bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
        public Guid? ReadGuid()
        {
            var exist = reader.ReadBoolean();
            if (exist)
            {
                var bytes = reader.ReadBytes(16);
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
