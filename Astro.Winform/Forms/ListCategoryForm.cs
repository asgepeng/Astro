using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using Astro.Winform.UserControls;
using System.Data;

namespace Astro.Winform.Forms
{
    public partial class ListCategoryForm : XPanel
    {
        private readonly BindingSource bs = new BindingSource();
        public ListCategoryForm()
        {
            InitializeComponent();
            this.Text = "Kategori Produk";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = bs;
            dataGridView1.InsertIconColumn("""
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="black" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <rect x="3" y="6" width="6" height="6"/>
                  <line x1="12" y1="8" x2="21" y2="8"/>
                  <line x1="12" y1="12" x2="21" y2="12"/>
                  <rect x="3" y="15" width="6" height="6"/>
                  <line x1="12" y1="18" x2="21" y2="18"/>
                </svg>
                """.ToImage(20, 20));
            dataGridView1.ApplyGridStyle();
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            using (var stream = await WClient.PostStreamAsync("/data/categories", new byte[] { 0x00, 0x00 }))
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
            var form = new CategoryForm()
            {
                Category = new Models.Category()
            };
            if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
        }

        private async void HandleDeleteButtonClicked(object sender, EventArgs e)
        {
            if (this.bs.Current is null) return;

            var id = (short)((DataRowView)this.bs.Current)[0];
            var json = await WClient.DeleteAsync($"/data/categories/{id}");
            await LoadDataAsync();
        }

        private async void HandleGridCellDoubleCLicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (bs.Current is null) return;
            var id = (short)((DataRowView)this.bs.Current)[0];
            var form = new CategoryForm() { Category = new Models.Category() };
            using (var stream = await WClient.GetStreamAsync($"/data/categories/{id}"))
            using (var reader = new Astro.Binaries.BinaryDataReader(stream))
            {
                if (reader.Read())
                {
                    var dataExists = reader.ReadBoolean();
                    if (dataExists)
                    {
                        form.Category.Id = reader.ReadInt16();
                        form.Category.Name = reader.ReadString();
                    }
                }
            }
            if (form.ShowDialog() == DialogResult.OK) await LoadDataAsync();
        }
    }
}
