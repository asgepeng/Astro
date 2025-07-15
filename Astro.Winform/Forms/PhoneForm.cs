using Astro.Models;
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
    public partial class PhoneForm : Form
    {
        public PhoneForm()
        {
            InitializeComponent();
        }
        public Phone Phone { get; set; } = new Phone();
        public void DisablePrimary()
        {
            this.checkBox1.Enabled = false;
            this.checkBox1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Phone type is not selected.", "Phone Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(zipCodeTextBox.Text))
            {
                MessageBox.Show("Phone number cannot be empty.", "Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                zipCodeTextBox.Focus();
                return;
            }
            this.Phone.Type = (short)typeComboBox.SelectedIndex;
            this.Phone.Number = zipCodeTextBox.Text.Trim();
            this.Phone.IsPrimary = this.checkBox1.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
