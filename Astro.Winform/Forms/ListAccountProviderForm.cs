using Astro.Models;
using Astro.Winform.Classes;
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

namespace Astro.Winform.Forms
{
    public partial class ListAccountProviderForm : Form
    {
        private readonly BindingSource bs;
        public ListAccountProviderForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            bs = new BindingSource();
        }
        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            using (var stream = await WClient.GetStreamAsync("/data/account-providers"))
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
            using (var reader = new Astro.Streams.Reader(stream))
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
