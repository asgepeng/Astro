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
    public partial class AccountForm : UserControl
    {
        public AccountForm()
        {
            InitializeComponent();
        }
        public List<AccountProvider> Providers { get; } = new List<AccountProvider>();
        public Account Account { get; set; } = new Account();
        private async void AccountForm_Load(object sender, EventArgs e)
        {
            using (var stream = await WClient.GetStreamAsync("/data/accounts/" + (Tag is null ? "0" :  Tag.ToString())))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var accountExists = reader.ReadBoolean();
                if (accountExists)
                {
                    this.Account.Id = reader.ReadInt16();
                    this.Account.AccountName = reader.ReadString();
                    this.Account.AccountNumber = reader.ReadString();
                    this.Account.Provider = reader.ReadInt16();
                    this.Account.AccountType = reader.ReadInt16();
                }
                var iCount = reader.ReadInt32();
                for (int i=0; i < iCount; i++)
                {
                    var provider = new AccountProvider()
                    {
                        Id = reader.ReadInt16(),
                        Name = reader.ReadString(),
                        Type = reader.ReadInt16()
                    };
                    this.Providers.Add(provider);
                }
            }
            this.label5.Text = this.Account.Id > 0 ? "Detil Rekening" : "Tambah Rekening";
            this.accountNameTextBox.Text = this.Account.AccountName;
            this.accountNumberTextBox.Text = this.Account.AccountNumber;
            this.accountTypeComboBox.SelectedIndex = this.Account.AccountType - 1;
            foreach (var item in this.Providers)
            {
                if (item.Type == this.Account.AccountType)
                {
                    this.accountProviderComboBox.Items.Add(item);
                    if (item.Id == this.Account.Provider)
                    {
                        this.accountProviderComboBox.SelectedIndex = this.accountProviderComboBox.Items.Count - 1;
                    }
                }
            }
            this.accountTypeComboBox.SelectedIndexChanged += new EventHandler(this.AccountTypeChanged);
        }
        private void AccountTypeChanged(object? sender, EventArgs e)
        {
            this.Account.AccountType = (short)(this.accountTypeComboBox.SelectedIndex + 1);
            this.accountProviderComboBox.Items.Clear();
            foreach (var item in this.Providers)
            {
                if (item.Type == this.Account.AccountType)
                {
                    this.accountProviderComboBox.Items.Add(item);
                    if (item.Id == this.Account.Provider)
                    {
                        this.accountProviderComboBox.SelectedIndex = this.accountProviderComboBox.Items.Count - 1;
                    }
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (string.IsNullOrWhiteSpace(this.accountNameTextBox.Text))
            {
                MessageBox.Show("Account name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.accountNameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.accountNumberTextBox.Text))
            {
                MessageBox.Show("Account name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.accountNumberTextBox.Focus();
                return;
            }
            if (this.accountTypeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Account type is not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.accountTypeComboBox.Focus();
                return;
            }
            if (this.accountProviderComboBox.SelectedItem is null)
            {
                MessageBox.Show("Account provider is not selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.accountProviderComboBox.Focus();
                return;
            }
            using (var writer = new Streams.Writer())
            {
                writer.WriteByte(0x01);
                writer.WriteInt16(this.Account.Id);
                writer.WriteString(this.accountNameTextBox.Text.Trim());
                writer.WriteString(this.accountNumberTextBox.Text.Trim());
                writer.WriteInt16((short)((AccountProvider)this.accountProviderComboBox.SelectedItem).Id);
                var json = this.Account.Id > 0 ? await WClient.PutAsync("/data/accounts", writer.ToArray()) : await WClient.PostAsync("/data/accounts", writer.ToArray());
                var commonResult = CommonResult.Create(json);
                if (commonResult != null)
                {
                    if (commonResult.Success)
                    {
                        mainForm.DialogResult = DialogResult.OK;
                        mainForm.Close();
                    }
                    else
                    {
                        MessageBox.Show(commonResult.Message);
                    }
                }
            }           
        }
    }
}
