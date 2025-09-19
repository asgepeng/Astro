using Astro.Binaries;
using Astro.Models;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using Astro.Winform.UserControls;
using Microsoft.VisualBasic;
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
    public partial class LoginDetailForm : XPanel
    {
        public LoginDetailForm()
        {
            InitializeComponent();
            this.Text = "Login Detail";
        }
        private short EmployeeID { get; set; }
        private async void LoginDetailForm_Load(object sender, EventArgs e)
        {
            this.comboBox1.DisplayMember = "Text";
            this.comboBox1.ValueMember = "Id";

            if (this.Tag != null)
            {
                using (var stream = await WClient.GetStreamAsync("/data/users/" + this.Tag.ToString()))
                using (var reader = new BinaryDataReader(stream))
                {
                    if (stream.Length == 0) return;

                    var dataFound = reader.ReadBoolean();
                    if (dataFound)
                    {
                        this.EmployeeID = reader.ReadInt16();
                        this.comboBox1.Items.Add(reader.ReadString());
                        this.comboBox1.SelectedIndex = 0;
                        this.comboBox1.Enabled = false;
                        this.Text = "Edit User Login";

                        this.checkBox1.Checked = reader.ReadBoolean();
                        this.checkBox2.Checked = reader.ReadBoolean();
                    }
                }
                this.resetPasswordLink.Visible = true;
                this.passwordLabel.Visible = false;
                this.passwordTextBox.Visible = false;
                this.confirmPassweordLabel.Visible = false;
                this.confirmPasswordTextBox.Visible = false;
                this.resetPasswordLink.LinkClicked += (sender, e) =>
                {
                    if (this.resetPasswordLink.Text == "Reset Password")
                    {
                        this.resetPasswordLink.Text = "Batalkan Reset Password";
                        this.passwordLabel.Visible = true;
                        this.passwordTextBox.Visible = true;
                        this.confirmPassweordLabel.Visible = true;
                        this.confirmPasswordTextBox.Visible = true;
                    }
                    else
                    {
                        this.resetPasswordLink.Text = "Reset Password";
                        this.passwordLabel.Visible = false;
                        this.passwordTextBox.Visible = false;
                        this.confirmPassweordLabel.Visible = false;
                        this.confirmPasswordTextBox.Visible = false;
                    }
                };
            }
            else
            {
                using (var stream = await WClient.PostStreamAsync("/data/employees", new byte[] { 0x00, 0x02 }))
                using (var reader = new BinaryDataReader(stream))
                {
                    if (stream.Length < 4) return;

                    var iCount = reader.ReadInt32();
                    MessageBox.Show(iCount.ToString());
                    for (int i = 0; i < iCount; i++)
                    {
                        this.comboBox1.Items.Add(new Option<short>()
                        {
                            Id = reader.ReadInt16(),
                            Text = reader.ReadString()
                        });
                    }
                }
            }
        }
        private bool IsValidInput()
        {
            if (this.comboBox1.SelectedItem is null)
            {
                this.comboBox1.ShowError("Pegawai belum dipilih");
                return false;
            }
            if (this.Tag != null && !this.confirmPasswordTextBox.Visible)
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(this.passwordTextBox.Text))
            {
                this.passwordTextBox.ShowError("Password baru tidak boleh kosong");
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.confirmPasswordTextBox.Text))
            {
                this.confirmPasswordTextBox.ShowError("Konfirmasi password tidak boleh kosong");
                return false;
            }
            if (this.passwordTextBox.Text != this.confirmPasswordTextBox.Text)
            {
                this.confirmPasswordTextBox.ShowError("Konfirmasi password tidak cocok");
                return false;
            }
            if (this.EmployeeID ==0) this.EmployeeID = ((Option<short>)this.comboBox1.SelectedItem).Id;
            return true;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if (!this.IsValidInput())
            {
                return;
            }
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            using (var writer = new BinaryDataWriter())
            {
                writer.WriteByte(0x01);
                writer.WriteInt16(this.EmployeeID);
                writer.WriteBoolean(this.checkBox1.Checked);
                writer.WriteBoolean(this.checkBox2.Checked);

                if (this.Tag != null && this.confirmPasswordTextBox.Visible)
                {
                    writer.WriteBoolean(true);
                }
                writer.WriteString(this.confirmPasswordTextBox.Text);

                var json = this.Tag != null ? await WClient.PutAsync("/data/users", writer.ToArray())
                    : await WClient.PostAsync("/data/users", writer.ToArray());
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
                        MessageBox.Show(commonResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
