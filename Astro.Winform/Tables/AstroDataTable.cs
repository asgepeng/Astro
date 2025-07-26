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
        internal abstract Task LoadAsync();
        internal List<string> GetStringColumns()
        {
            var stringColumns = new List<string>();
            foreach (DataColumn column in this.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    stringColumns.Add(column.ColumnName);
                }
            }
            return stringColumns;
        }
    }
}
