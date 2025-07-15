using Astro.DataTables;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Tables
{
    internal class ContactDataTable : AstroDataTable
    {
        internal ContactDataTable()
        {
            Columns.Add("id", typeof(short));
            Columns.Add("name", typeof(string));
            Columns.Add("address", typeof(string));
            Columns.Add("phone", typeof(string));
            Columns.Add("creator", typeof(string));
            Columns.Add("createdDate", typeof(DateTime));
        }
        internal short ContactType { get; set; } = 0;
        internal override async Task LoadAsync()
        {
            if (this.Rows.Count > 0) this.Rows.Clear();
            using (var stream = await HttpClientSingleton.GetStreamAsync(ContactType == 0 ? "/data/suppliers" : "/data/customers"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
                        "👤 " + reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadDateTime()
                    };
                    Rows.Add(values);
                }
            }
        }
    }
}
