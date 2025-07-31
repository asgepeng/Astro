using Astro.DataTables;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Tables
{
    internal class AccountDataTable : AstroDataTable
    {
        internal AccountDataTable()
        {
            Columns.Add("id", typeof(short));
            Columns.Add("accountName", typeof(string));
            Columns.Add("accountNumber", typeof(string));
            Columns.Add("accountType", typeof(string));
            Columns.Add("providerName", typeof(string));
            Columns.Add("createdBy", typeof(string));
            Columns.Add("createdDate", typeof(DateTime));
            Columns.Add("lastModified", typeof(DateTime));
        }
        internal override async Task LoadAsync()
        {
            if (this.Rows.Count > 0) this.Rows.Clear();
            using (var stream = await WClient.GetStreamAsync("/data/accounts"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
                        "💳 " + reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadString(),
                        reader.ReadDateTime(),
                        reader.ReadDateTimeOrDBNull()
                    };
                    this.Rows.Add(values);
                }
            }
        }
    }
}
