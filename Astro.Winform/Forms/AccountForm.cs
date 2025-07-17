using Astro.Models;
using Astro.ViewModels;
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
    public partial class AccountForm : Form
    {
        public AccountForm()
        {
            InitializeComponent();
        }
        public AccountViewModel Model { get; set; } = new AccountViewModel();

        private void AccountForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Model.Account.AccountName;
            this.textBox2.Text = Model.Account.AccountNumber;
            this.comboBox1.SelectedIndex = Model.Account.AccountType - 1;
            foreach (var item in Model.Providers)
            {
                if (item.Type == Model.Account.AccountType)
                {
                    this.comboBox2.Items.Add(item);
                    if (item.Id == Model.Account.Provider)
                    {
                        this.comboBox2.SelectedIndex = this.comboBox2.Items.Count - 1;
                    }
                }
            }
            this.comboBox1.SelectedIndexChanged += new EventHandler(this.AccountTypeChanged);
        }
        private void AccountTypeChanged(object? sender, EventArgs e)
        {
            this.Model.Account.AccountType = (short)(this.comboBox1.SelectedIndex + 1);
            this.comboBox2.Items.Clear();
            foreach (var item in Model.Providers)
            {
                if (item.Type == Model.Account.AccountType)
                {
                    this.comboBox2.Items.Add(item);
                    if (item.Id == Model.Account.Provider)
                    {
                        this.comboBox2.SelectedIndex = this.comboBox2.Items.Count - 1;
                    }
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("Account name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBox1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                MessageBox.Show("Account name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBox1.Focus();
                return;
            }
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Account type is not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.comboBox1.Focus();
                return;
            }
            if (this.comboBox2.SelectedItem is null)
            {
                MessageBox.Show("Account provider is not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.comboBox2.Focus();
                return;
            }
            var account = this.Model.Account;
            account.AccountName = this.textBox1.Text.Trim();
            account.AccountNumber = this.textBox2.Text.Trim();
            account.AccountType = (short)(this.comboBox1.SelectedIndex + 1);
            account.Provider = ((AccountProvider)this.comboBox2.SelectedItem).Id;

            var json = account.Id > 0 ? await HttpClientSingleton.PutAsync("/data/accounts", account.ToString()) : await HttpClientSingleton.PostAsync("/data/accounts", account.ToString());
            var commonResult = CommonResult.Create(json);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(commonResult.Message);
                }
            }
        }
    }
}
