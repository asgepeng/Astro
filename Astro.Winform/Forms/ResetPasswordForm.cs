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
    public partial class ResetPasswordForm : Form
    {
        public ResetPasswordForm()
        {
            InitializeComponent();
        }
        public ResetPasswordRequest Request { get; set; } = new ResetPasswordRequest();

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("New password must not be empty", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox1.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                MessageBox.Show("Password confirmation must not be empty", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox2.Focus();
                return;
            }
            if (this.textBox1.Text.Equals(this.textBox2.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                MessageBox.Show("New password and password confirmation does not match", "Matching", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Request.Password = this.textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
