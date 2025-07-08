using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformApp.Data
{
    public class DataTableColumnInfo
    {
        public string HeaderText { get; }
        public string DataProperty { get; }
        public int ColumnWidth { get; } = 100;
        public DataGridViewContentAlignment Alignment { get; } = DataGridViewContentAlignment.MiddleLeft;
        public string Format { get; } = "";
        public DataTableColumnInfo(string header, string columnName, int columnWidth, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, string format = "")
        {
            this.HeaderText = header;
            this.DataProperty = columnName;
            this.ColumnWidth = columnWidth;
            this.Alignment = alignment;
            this.Format = format;
        }
    }
}
