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
        private readonly BindingSource bs = new BindingSource();
        public ListUnitForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = bs;
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            using (var stream = await WClient.GetStreamAsync("/data/units"))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var result = reader.ReadByte();
                if (result == 0x00)
                {
                    this.bs.DataSource = null;
                    return;
                }
                if (stream.Length > 0)
                {
                    this.bs.DataSource = reader.ReadDataTable();
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
            var json = await WClient.DeleteAsync($"/data/units/{id}");
            await LoadDataAsync();
        }

        private async void HandleGridCellDoubleCLicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bs.Current is null) return;
            var id = (short)((DataRowView)this.bs.Current)[0];
            var form = new UnitForm() { Unit = new Models.Unit() };
            using (var stream = await WClient.GetStreamAsync($"/data/units/{id}"))
            using (var reader = new Astro.Streams.Reader(stream))
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
