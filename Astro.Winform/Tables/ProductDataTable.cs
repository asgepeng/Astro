using Astro.DataTables;
using Astro.Models;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Astro.Winform.Tables
{
    internal class ProductDataTable : AstroDataTable
    {
        internal ProductDataTable()
        {
            Columns.Add("id", typeof(short)).Unique = true;
            Columns.Add("name", typeof(string));
            Columns.Add("sku", typeof(string));
            Columns.Add("category", typeof(string));
            Columns.Add("stock", typeof(int));
            Columns.Add("unit", typeof(string));
            Columns.Add("price", typeof(long));
            Columns.Add("creator", typeof(string));
            Columns.Add("created_date", typeof(DateTime));
        }
        internal short BranchId { get; set; } = 1;
        internal override async Task LoadAsync()
        {
            if (this.Rows.Count > 0) this.Rows.Clear();
            var requestData = Array.Empty<byte>();
            using (var writer = new Astro.Streams.Writer())
            {
                writer.WriteByte(0);
                writer.WriteInt16(this.BranchId);
                requestData = writer.ToArray();
            }
            using (var stream = await WClient.PostStreamAsync("/data/products", requestData))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
                        "📦 " + reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadInt32(),
                        reader.ReadString(),
                        reader.ReadInt64(),
                        reader.ReadString(),
                        reader.ReadDateTime()
                    };
                    Rows.Add(values);
                }
            }
        }
    }
}
