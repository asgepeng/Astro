using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Utils
{
    public abstract class BinaryToObjectBuilder<T> : IDisposable
    {
        private bool disposedValue;
        private readonly BinaryReader reader;
        public BinaryToObjectBuilder(Stream stream)
        {
            reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }
        public T Create()
        {
            return CreateObject(reader);
        }
        protected abstract T CreateObject(BinaryReader reader);
        protected bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }
        protected Int16 ReadInt16()
        {
            return reader.ReadInt16();
        }
        protected Int32 ReadInt32()
        {
            return reader.ReadInt32();
        }
        protected long ReadInt64()
        {
            return reader.ReadInt64();
        }
        protected double ReadDouble()
        {
            return reader.ReadDouble();
        }
        protected string ReadString()
        {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }
        protected DateTime ReadDateTime()
        {
            long ticks = reader.ReadInt64();
            if (ticks == 0 || ticks == (new DateTime(1900, 1, 1)).Ticks)
            {
                return DateTime.MinValue; 
            }
            return new DateTime(ticks, DateTimeKind.Utc);
        }
        protected DateTime? ReadNullableDateTime()
        {
            long ticks = reader.ReadInt64();
            if (ticks == 0 || ticks == (new DateTime(1900, 1, 1)).Ticks)
            {
                return null;
            }
            return new DateTime(ticks, DateTimeKind.Utc);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    reader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BinaryToObjectBuilder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
