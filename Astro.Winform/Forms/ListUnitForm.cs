using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using System.Data;

namespace Astro.Winform.Forms
{
    public partial class ListUnitForm : UserControl
    {
        private readonly BindingSource bs = new BindingSource();
        public ListUnitForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = bs;
            dataGridView1.ApplyGridStyle();
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            using (var stream = await WClient.GetStreamAsync("/data/units"))
            using (var reader = new Astro.Binaries.BinaryDataReader(stream))
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
            using (var reader = new Astro.Binaries.BinaryDataReader(stream))
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
