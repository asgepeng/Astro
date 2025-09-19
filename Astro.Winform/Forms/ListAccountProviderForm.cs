using Astro.Models;
using Astro.Winform.Classes;
using Astro.Binaries;

using System.Data;
using Astro.Winform.Extensions;

namespace Astro.Winform.Forms
{
    public partial class ListAccountProviderForm : UserControl
    {
        private readonly BindingSource bs;
        public ListAccountProviderForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            bs = new BindingSource();
            dataGridView1.DataSource = bs;
            dataGridView1.InsertIconColumn("""
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                     viewBox="0 0 24 24" role="img" aria-label="Ikon Bank">
                  <!-- Atap -->
                  <polygon points="4,8 12,3 20,8" fill="#1E3A8A"/>
                  <!-- Badan gedung -->
                  <rect x="4" y="8" width="16" height="10" fill="#3B82F6"/>
                  <!-- Pilar -->
                  <rect x="6" y="10" width="2" height="6" fill="#BFDBFE"/>
                  <rect x="11" y="10" width="2" height="6" fill="#BFDBFE"/>
                  <rect x="16" y="10" width="2" height="6" fill="#BFDBFE"/>
                  <!-- Alas -->
                  <rect x="3" y="18" width="18" height="2" fill="#1E3A8A"/>
                  <!-- Simbol lingkaran (bisa dianggap logo bank/uang) -->
                  <circle cx="12" cy="13" r="1.5" fill="#FACC15"/>
                </svg>
                """.ToImage(24, 24));
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            using (var stream = await WClient.PostStreamAsync("/data/account-providers", new byte[] { 0x00, 0x00 }))
            using (var reader = new BinaryDataReader(stream))
            {
                if (stream.Length == 0) return;

                var result = reader.ReadByte();
                if (result == 0x00)
                {
                    this.bs.DataSource = null;
                    return;
                }
                var table = reader.ReadDataTable();
                this.bs.DataSource = table;
            }
            this.Cursor = Cursors.Default;
        }

        private async void ListAccountProviderForm_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var form = new AccountProviderForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (this.bs.Current is null) return;

            var id = (short)((DataRowView)this.bs.Current)[0];
            var json = await WClient.DeleteAsync("/data/account-providers/" + id.ToString());
            var commonResult = CommonResult.Create(json);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    await LoadDataAsync();
                }
            }
        }

        private async void HandleGridDoubleClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (this.bs.Current is null) return;

            var form = new AccountProviderForm();
            var id = (short)((DataRowView)this.bs.Current)[0];
            using (var stream = await WClient.GetStreamAsync("/data/account-providers/" + id.ToString()))
            using (var reader = new Astro.Binaries.BinaryDataReader(stream))
            {
                if (reader.ReadBoolean())
                {
                    form.Provider.Id = reader.ReadInt16();
                    form.Provider.Name = reader.ReadString();
                    form.Provider.Type = reader.ReadInt16();
                }
            }
            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadDataAsync();
            }
        }
    }
}
