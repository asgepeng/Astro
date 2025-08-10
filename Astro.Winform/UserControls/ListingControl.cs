using Astro.DataTables;
using Astro.Winform.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformApp.Data;

namespace Astro.Forms.Controls
{
    public partial class ListingControl : UserControl
    {
        private readonly BindingSource bs = new BindingSource();
        private AstroDataTable table;
        public ListingControl()
        {
            InitializeComponent();
            GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
            {
                //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                new DataTableColumnInfo("Name", "name", 300),
                new DataTableColumnInfo("SKU", "sku", 100),
                new DataTableColumnInfo("Category", "category", 150),
                new DataTableColumnInfo("Stock", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0"),
                new DataTableColumnInfo("Unit", "unit", 100),
                new DataTableColumnInfo("Price", "price", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                new DataTableColumnInfo("Created By", "creator", 200),
                new DataTableColumnInfo("Created Date", "created_date", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
            }, this.bs);
            table = new ProductDataTable();
        }
        private async Task ReloadDataTable()
        {
            await this.table.LoadAsync();
            if (this.bs.DataSource is null) this.bs.DataSource = this.table;
        }
        private async void ListingControl_Load(object sender, EventArgs e)
        {
            await ReloadDataTable();
        }
    }
}
