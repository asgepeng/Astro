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

        private void HandleFormLoad(object sender, EventArgs e)
        {
            if (Role != null)
            {
                this.rolenameTextBox.Text = Role.Name;
                this.permissionBindingSource.DataSource = Role.Permissions;
                this.saveButton.Text = Role.Id > 0 ? "Update" : "Create";
            }
        }

        private async void HandleSaveButtonClicked(object sender, EventArgs e)
        {
            if (this.Role is null) return;

            this.Role.Name = this.rolenameTextBox.Text.Trim();
            var json = this.Role.Id > 0 ? await WClient.PutAsync("/data/roles", this.Role.ToString()) :await WClient.PostAsync("/data/roles", this.Role.ToString());
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
