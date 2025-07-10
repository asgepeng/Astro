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
    public partial class RoleForm : Form
    {
        public RoleForm()
        {
            InitializeComponent();
        }
        public Role? Role { get; set; }

        private void RoleForm_Load(object sender, EventArgs e)
        {
            if (Role != null)
            {
                this.textBox1.Text = Role.Name;
                this.permissionBindingSource.DataSource = Role.Permissions;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (this.Role is null) return;

            this.Role.Name = this.textBox1.Text.Trim();
            MessageBox.Show(this.Role.ToString());
            var json = await HttpClientSingleton.PostAsync("/data/roles", this.Role.ToString());
            var commonResult = CommonResult.Create(json);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    MessageBox.Show(commonResult.Message);
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
