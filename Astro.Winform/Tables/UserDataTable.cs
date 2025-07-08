using Astro.Helpers;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.DataTables
{
    internal class UserDataTable : AstroDataTable
    {
        internal UserDataTable()
        {
            Columns.Add("id", typeof(short)).Unique = true;
            Columns.Add("fullname", typeof(string));
            Columns.Add("email", typeof(string));
            Columns.Add("role_name", typeof(string));
            Columns.Add("creator", typeof(string));
            Columns.Add("created_date", typeof(DateTime));
        }
        internal async override Task LoadAsync()
        {
            using (var reader = new BinaryReader(await HttpClientSingleton.GetStreamAsync("/data/users")))
            {
                while (AnyRows(reader))
                {
                    var values = new object[]
                    {
                        ReadInt16(reader),
                        ReadString(reader),
                        ReadString(reader),
                        ReadString(reader),
                        ReadString(reader),
                        ReadDateTime(reader)
                    };
                    Rows.Add(values);
                }
            }
        }
    }
}
