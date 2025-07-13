using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform.Forms
{
    public partial class ListUnitForm : Form
    {
        private readonly DataTable table;
        private readonly BindingSource bs = new BindingSource();
        public ListUnitForm()
        {
            InitializeComponent();

            table = new DataTable();
            table.Columns.Add("id", typeof(short));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("dateAdd", typeof(DateTime));
            table.Columns.Add("addedBy", typeof(string));

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = bs;
            bs.DataSource = table;
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            if (table.Rows.Count > 0) table.Rows.Clear();
            using (var stream = await HttpClientSingleton.GetStreamAsync("/data/units"))
            using (var reader = new IO.Reader(stream))
            {
                while (reader.Read())
                {
                    var values = new object[]
                    {
                        reader.ReadInt16(),
                        "📑 " + reader.ReadString(),
                        reader.ReadDateTime(),
                        reader.ReadString()
                    };
                    table.Rows.Add(values);
                }
            }
            this.Cursor = Cursors.Default;
        }

        private async void HandleFormLoad(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void HandleSaveButtonClicked(object sender, EventArgs e)
        {
            var form = new UnitForm()
            {
                Unit = new Models.Unit()
            };
            if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
        }

        private async void HandleDeleteButtonClicked(object sender, EventArgs e)
        {
            if (this.bs.Current is null) return;

            var id = (short)((DataRowView)this.bs.Current)[0];
            var json = await HttpClientSingleton.DeleteAsync($"/data/units/{id}");
            await LoadDataAsync();
        }

        private async void HandleGridCellDoubleCLicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bs.Current is null) return;
            var id = (short)((DataRowView)this.bs.Current)[0];
            var form = new UnitForm() { Unit = new Models.Unit() };
            using (var stream = await HttpClientSingleton.GetStreamAsync($"/data/units/{id}"))
            using (var reader = new IO.Reader(stream))
            {
                if (reader.Read())
                {
                    var dataExists = reader.ReadBoolean();
                    if (dataExists)
                    {
                        form.Unit.Id = reader.ReadInt16();
                        form.Unit.Name = reader.ReadString();
                    }
                }
            }
            if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
        }
    }
}
