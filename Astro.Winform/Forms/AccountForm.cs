using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using DocumentFormat.OpenXml.InkML;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Astro.Winform.Forms
{
    public partial class AccountForm : UserControl
    {
        private readonly IDBClient db = My.Application.CreateDBAccess();
        public AccountForm()
        {
            InitializeComponent();
            this.accountProviderComboBox.DisplayMember = "Text";
            this.accountProviderComboBox.ValueMember = "Id";
        }
        private async Task GetAccountAsync()
        {
            if (this.Tag != null)
            {
                this.Account.Id = (short)((int)this.Tag);
            }
            if (this.Account.Id <= 0) return;

            var commandText = """
                SELECT a.accountname, a.accountnumber, a.providerid, p.providertype
                FROM accounts AS a
                INNER JOIN accountproviders AS p ON a.providerid = p.providerid
                WHERE a.accountid = @id AND a.isdeleted = false
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                if (await reader.ReadAsync())
                {
                    this.Account.AccountName = reader.GetString(0);
                    this.Account.AccountNumber = reader.GetString(1);
                    this.Account.Provider = reader.GetInt16(2);
                    this.Account.AccountType = reader.GetInt16(3);
                }
            }, commandText, db.CreateParameter("id", this.Account.Id, DbType.Int16));
        }
        private async Task GetProvidersAsync()
        {
            if (this.accountProviderComboBox.Items.Count > 0) this.accountProviderComboBox.Items.Clear();
            var commandText = """
                SELECT providerid, name
                FROM accountproviders
                WHERE providertype = @type
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var prov = new Option<short>()
                    {
                        Id = reader.GetInt16(0),
                        Text = reader.GetString(1)
                    };
                    this.accountProviderComboBox.Items.Add(prov);
                    if (prov.Id == this.Account.Provider)
                    {
                        this.accountProviderComboBox.SelectedIndex = this.accountProviderComboBox.Items.Count - 1;
                    }
                }
            }, commandText, db.CreateParameter("type", this.Account.AccountType, DbType.Int16));
        }
        private async Task<bool> CreateAsync()
        {
            var commandText = """
                INSERT INTO accounts
                    (accountname, accountnumber, providerid, creatorid)
                VALUES
                    (@accountname, @accountnumber, @providerid, @creatorid)
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("accountname", this.Account.AccountName, DbType.AnsiString),
                db.CreateParameter("accountnumber", this.Account.AccountNumber, DbType.AnsiString),
                db.CreateParameter("providerid", this.Account.Provider, DbType.Int16),
                db.CreateParameter("creatorid", My.Application.GetCurrentUserID(), DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private async Task<bool> UpdateAsync()
        {
            var commandText = """
                UPDATE accounts
                SET accountname =@accountname,
                    accountnumber = @accountnumber,
                    providerid = @providerid,
                    editorid = @editorid,
                    editeddate = current_timestamp
                WHERE accountid = @id;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("id", this.Account.Id, DbType.Int16),
                db.CreateParameter("accountname", this.Account.AccountName, DbType.AnsiString),
                db.CreateParameter("accountnumber", this.Account.AccountNumber, DbType.AnsiString),
                db.CreateParameter("providerid", this.Account.Provider, DbType.Int16),
                db.CreateParameter("editorid", My.Application.GetCurrentUserID(), DbType.Int16)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private void OnAccountProviderChanged(object? sender, EventArgs e)
        {
            if (this.accountProviderComboBox.SelectedItem is null)
            {
                this.Account.Provider = 0;
                return;
            }
            this.Account.Provider = ((Option<short>)this.accountProviderComboBox.SelectedItem).Id;
        }
        public Account Account { get; set; } = new Account();
        private async void HandleFormLoad(object sender, EventArgs e)
        {
            await GetAccountAsync();
            
            this.label5.Text = this.Account.Id > 0 ? "Detil Rekening" : "Tambah Rekening";
            this.accountNameTextBox.Text = this.Account.AccountName;
            this.accountNumberTextBox.Text = this.Account.AccountNumber;
            this.accountTypeComboBox.SelectedIndex = this.Account.AccountType - 1;
            await GetProvidersAsync();
            this.accountProviderComboBox.SelectedIndexChanged += this.OnAccountProviderChanged;
            this.accountTypeComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                this.Account.AccountType = (short)(this.accountTypeComboBox.SelectedIndex + 1);
                await GetProvidersAsync();
            };
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (string.IsNullOrWhiteSpace(this.accountNameTextBox.Text))
            {
                accountNameTextBox.ShowError("Nama rekening tidak boleh kosong");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.accountNumberTextBox.Text))
            {
                this.accountNumberTextBox.ShowError("Nomor rekening tidak boleh kosong");
                return;
            }
            if (this.Account.AccountType == 0)
            {
                this.accountTypeComboBox.ShowError("Tipe Rekening tidak boleh kosong");
                return;
            }
            if (this.Account.Provider == 0)
            {
                this.accountProviderComboBox.ShowError("Provider rekening belum dipilih");
                return;
            }
            this.Account.AccountName = this.accountNameTextBox.Text.Trim();
            this.Account.AccountNumber = this.accountNumberTextBox.Text.Trim();
            var success = this.Account.Id > 0 ? await UpdateAsync() : await CreateAsync();
            if (success)
            {
                mainForm.DialogResult = DialogResult.OK;
                mainForm.Close();

            }
        }
    }
}
