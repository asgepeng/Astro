using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.DataTables
{
    internal class RoleDataTable : AstroDataTable
    {
        internal RoleDataTable()
        {
            Columns.Add("id", typeof(short)).Unique = true;
            Columns.Add("name", typeof(string));
            Columns.Add("creator", typeof(string));
            Columns.Add("created_date", typeof(DateTime));
        }
        internal async override Task LoadAsync()
        {
            using (var reader = new IO.Reader(await HttpClientSingleton.GetStreamAsync("/data/roles")))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
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
