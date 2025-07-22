using Astro.Models;
using Astro.Winform.Classes;
using Astro.Winform.Helpers;
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
    public partial class AccountProviderForm : Form
    {
        public AccountProviderForm()
        {
            InitializeComponent();
        }
        public AccountProvider Provider { get; set; } = new AccountProvider();
        private bool IsUpdate { get; set; } = false;
        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                toolTip1.ToolTipTitle = "Input required";
                toolTip1.Show("This field cannot be empty", this, new Point(this.textBox1.Location.X, this.textBox1.Location.Y), 2000);
                this.textBox1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                toolTip1.ToolTipTitle = "Input required";
                toolTip1.Show("This field cannot be empty", this, new Point(this.textBox2.Location.X, this.textBox2.Location.Y), 2000);
                this.textBox2.Focus();
                return;
            }
            if (this.comboBox1.SelectedIndex < 0)
            {
                toolTip1.Show("This field must be selected", this.comboBox1, 2000);
                this.comboBox1.Focus();
                return;
            }
            short.TryParse(this.textBox1.Text, out short providerId);

            if (!this.textBox1.ReadOnly) this.Provider.Id = providerId;
            this.Provider.Name = this.textBox2.Text;
            this.Provider.Type = (short)(this.comboBox1.SelectedIndex + 1);

            var json = this.IsUpdate ? await HttpClientSingleton.PutAsync("/data/account-providers", this.Provider.ToString()) : await HttpClientSingleton.PostAsync("/data/account-providers", this.Provider.ToString());
            var commonResult = CommonResult.Create(json);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void FilterOnlyNumber(object sender, KeyPressEventArgs e)
        {
            e.FilterOnlyNumber();
        }

        private void AccountProviderForm_Load(object sender, EventArgs e)
        {
            if (this.Provider.Id > 0)
            {
                this.textBox1.Text = this.Provider.Id.ToString("000");
                this.textBox1.ReadOnly = true;
                this.IsUpdate = true;
            }
            this.textBox2.Text = this.Provider.Name;
            this.comboBox1.SelectedIndex = this.Provider.Type - 1;
        }
    }
}
