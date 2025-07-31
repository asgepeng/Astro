using Astro.DataTables;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal override async Task LoadAsync()
        {
            if (this.Rows.Count > 0) this.Rows.Clear();
            using (var stream = await WClient.GetStreamAsync("/data/products"))
            using (var reader = new IO.Reader(stream))
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
