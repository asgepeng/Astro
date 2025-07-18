﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.IO
{
    public class Writer : IDisposable
    {
        private bool disposedValue;
        private readonly MemoryStream stream;
        private readonly BinaryWriter writer;
        public Writer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream, Encoding.UTF8);
        }
        public void WriteBoolean(bool value) => writer.Write(value);
        public void WriteByte(byte value) => writer.Write(value);
        public void WriteInt16(short value) => writer.Write(value);
        public void WriteUInt16(ushort value) => writer.Write(value);
        public void WriteInt32(int value) => writer.Write(value);
        public void WriteUInt32(uint value) => writer.Write(value);
        public void WriteInt32(int value, long pos)
        {
            long currentPos = writer.BaseStream.Position;
            writer.BaseStream.Position = pos;
            writer.Write(value);
            writer.BaseStream.Position = currentPos;
        }
        public void WriteInt64(long value) => writer.Write(value);
        public void WriteUInt64(ulong value) => writer.Write(value);
        public void WriteSingle(float value) => writer.Write(value);
        public void WriteDouble(double value) => writer.Write(value);
        public void WriteDecimal(decimal value) => writer.Write(value);
        public void WriteString(string value) => writer.Write(value ?? string.Empty);
        public void WriteDateTime(DateTime? value)
        {
            if (value.HasValue) writer.Write(value.Value.Ticks);
            else writer.Write(DateTime.MinValue.Ticks);
        }
        public void WriteGuid(Guid? guid)
        {
            if (guid.HasValue)
            {
                writer.Write(true);
                writer.Write(guid.Value.ToByteArray());
            }
            else writer.Write(false);
        }
        public long ReserveInt16()
        {
            WriteInt16(0);
            return writer.BaseStream.Position - 2;
        }
        public long ReserveInt32()
        {
            WriteInt32(0);
            return writer.BaseStream.Position - 4;
        }
        public long ReserveInt64()
        {
            WriteInt64(0L);
            return writer.BaseStream.Position - 8;
        }
        public byte[] ToArray()
        {
            writer.Flush();
            return stream.ToArray();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    writer.Dispose();
                    stream.Dispose();
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
