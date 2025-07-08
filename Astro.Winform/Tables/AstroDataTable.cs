using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.DataTables
{
    internal abstract class AstroDataTable : DataTable
    {
        protected Int16 ReadInt16(BinaryReader reader)
        {
            return reader.ReadInt16();
        }
        protected Int32 ReadInt32(BinaryReader reader)
        {
            return reader.ReadInt32();
        }
        protected Int64 ReadInt64(BinaryReader reader)
        {
            return reader.ReadInt64();
        }
        protected double ReadDouble(BinaryReader reader)
        {
            return reader.ReadDouble();
        }
        protected static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            string value = Encoding.UTF8.GetString(reader.ReadBytes(length));
            return value;
        }
        protected static object ReadDateTime(BinaryReader reader)
        {
            long ticks = reader.ReadInt64();
            long defaultDate = (new DateTime(1900, 1, 1)).Ticks;
            return (ticks == 0 || ticks == defaultDate) ? DBNull.Value : new DateTime(ticks, DateTimeKind.Utc);
        }
        protected static bool AnyRows(BinaryReader reader) => reader.BaseStream.Position < reader.BaseStream.Length;
        internal abstract Task LoadAsync();
    }
}
