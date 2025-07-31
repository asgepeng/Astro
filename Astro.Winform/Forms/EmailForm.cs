using Astro.Extensions;
using Astro.Models;
using Astro.Text;
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
    public partial class EmailForm : Form
    {
        public EmailForm()
        {
            InitializeComponent();
        }
        public Email Email { get; set; } = new Email();
        public void DisablePrimary()
        {
            this.checkBox1.Enabled = false;
            this.checkBox1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Email type is not selected.", "Email Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(zipCodeTextBox.Text))
            {
                MessageBox.Show("Email address cannot be empty.", "Email address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                zipCodeTextBox.Focus();
                return;
            }
            if (!zipCodeTextBox.Text.IsValidEmailFormat())
            {
                MessageBox.Show("Please enter a valid email format", "Invalid format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                zipCodeTextBox.Focus();
                return;
            }
            this.Email.Type = (short)typeComboBox.SelectedIndex;
            this.Email.Address = zipCodeTextBox.Text.Trim();
            this.Email.IsPrimary = this.checkBox1.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void EmailForm_Load(object sender, EventArgs e)
        {
            this.typeComboBox.SelectedIndex = this.Email.Type;
            this.zipCodeTextBox.Text = this.Email.Address;
            this.checkBox1.Checked = this.Email.IsPrimary;
            this.checkBox1.Visible = !this.Email.IsPrimary;
        }
    }
}
