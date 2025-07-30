using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformApp.Data
{
    public static class GridHelpers
    {
        public static void InitializeDataGridColumns(DataGridView grid, DataTableColumnInfo[] columns, BindingSource? bs = null)
        {
            grid.AutoGenerateColumns = false;
            foreach (var column in columns)
            {
                var index = grid.Columns.Add("col_" + grid.Columns.Count, column.HeaderText);
                var gridColumn = grid.Columns[index];
                gridColumn.DataPropertyName = column.DataProperty;
                gridColumn.Width = column.ColumnWidth;
                gridColumn.DefaultCellStyle.Alignment = column.Alignment;
                gridColumn.DefaultCellStyle.Format = column.Format;
            }
            if (bs != null) grid.DataSource = bs;
        }
        public static void SetAlternateColumnBackColor(DataGridView grid)
        {
            for (int i = 0; i < grid.ColumnCount; i++)
            {
                if (i % 2 == 0)
                {
                    grid.Columns[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
                }
            }
        }
        /*
        public static void SetFilterSetting(DataTable table, ColumnFilterCollection filters)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (col.DataType.Equals(typeof(string)))
                {
                    filters.Add(col.ColumnName);
                }
            }
        }*/
    }
}
