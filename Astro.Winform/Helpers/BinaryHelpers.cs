using System.Text;

namespace Astro.Helpers
{
    public static class BinaryHelpers
    {
        public static void WriteString(BinaryWriter writer, string value)
        {
            int length = value.Length;
            writer.Write(length);
            writer.Write(Encoding.UTF8.GetBytes(value));
        }
        public static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            string value = Encoding.UTF8.GetString(reader.ReadBytes(length));
            return value;
        }
        public static object ReadDateTime(BinaryReader reader, object nullValue)
        {
            long ticks = reader.ReadInt64();
            long defaultDate = (new DateTime(1900, 1, 1)).Ticks;
            return (ticks == 0 || ticks == defaultDate) ? nullValue: new DateTime(ticks, DateTimeKind.Utc);
        }
        public static bool AnyRows(BinaryReader reader) => reader.BaseStream.Position < reader.BaseStream.Length;
    }
}
