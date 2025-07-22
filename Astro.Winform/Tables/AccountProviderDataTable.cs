using Astro.DataTables;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Winform.Tables
{
    internal class AccountProviderDataTable : AstroDataTable
    {
        internal AccountProviderDataTable()
        {
            this.Columns.Add("id", typeof(Int16));
            this.Columns.Add("name", typeof(String));
            this.Columns.Add("type", typeof(string));
        }
        internal override async Task LoadAsync()
        {
            if (this.Rows.Count > 0) this.Rows.Clear();
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/account-providers"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
                        "💰 " + reader.ReadString(),
                        reader.ReadString()
                    };
                    this.Rows.Add(values);
                }
            }
        }
    }
}
