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
    public partial class ChangePasswordForm : Form
    {
        public ChangePasswordForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.passwordTextBox.Text))
            {

                MessageBox.Show("Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passwordTextBox.Focus();
                return;
            }
            if (string.IsNullOrEmpty(newPasswordTextBox.Text))
            {
                MessageBox.Show("New Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                newPasswordTextBox.Focus();
                return;
            }
            if (string.IsNullOrEmpty(confirmPasswordTextBox.Text))
            {
                MessageBox.Show("Confirm Password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmPasswordTextBox.Focus();
                return;
            }
            if (newPasswordTextBox.Text != confirmPasswordTextBox.Text)
            {
                MessageBox.Show("New Password and Confirm Password do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                confirmPasswordTextBox.Focus();
                return;
            }
            var request = new ChangePasswordRequest(this.passwordTextBox.Text, this.confirmPasswordTextBox.Text);
            var response = await WClient.PostAsync("/auth/change-password", request.ToString());
            var result = CommonResult.Create(response);
            if (result != null)
            {
                MessageBox.Show(result.Message, "Change password", MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                if (result.Success)
                {
                    this.Close();
                }
            }
        }

        private void CloseForm(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
