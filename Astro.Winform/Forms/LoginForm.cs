using Astro.Models;
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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        void DisableControls(bool disabled)
        {
            if (disabled) this.Cursor = Cursors.WaitCursor;
            else this.Cursor = Cursors.Default;

            foreach (Control control in this.Controls)
            {
                control.Enabled = !disabled;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.DisableControls(true);
            if (this.usernameTextBox.Text.Trim() == "")
            {
                MessageBox.Show("Username tidak boleh kosong", "Username", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.usernameTextBox.Focus();
                return;
            }
            if (this.passwordTextBox.Text == "")
            {
                MessageBox.Show("Password tidak boleh kosong", "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.passwordTextBox.Focus();
                return;
            }

            var success = await WClient.SignInAsync(usernameTextBox.Text.Trim(), passwordTextBox.Text);
            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.DisableControls(false);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.usernameTextBox.Text = "administrator";
            this.passwordTextBox.Text = "Power123...";
            this.button1.PerformClick();
        }
    }
}
