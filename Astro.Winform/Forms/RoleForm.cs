using Astro.Models;
using Astro.ViewModels;
using Astro.Winform.Classes;
using ExCSS;
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

        private async void HandleFormLoad(object sender, EventArgs e)
        {
            using (var stream = await WClient.GetStreamAsync("/data/roles/" + (Tag is null ? "0" : Tag.ToString())))
            using (var r = new Astro.Streams.Reader(stream))
            {
                Role = new Role();
                var roleExists = r.ReadBoolean();
                if (roleExists)
                {
                    Role.Id = r.ReadInt16();
                    Role.Name = r.ReadString();
                }
                var iCount = r.ReadInt32();
                while (iCount > 0)
                {
                    Role.Permissions.Add(new Permission()
                    {
                        Id = r.ReadInt16(),
                        Name = "☰ " + r.ReadString(),
                        AllowCreate = r.ReadBoolean(),
                        AllowRead = r.ReadBoolean(),
                        AllowEdit = r.ReadBoolean(),
                        AllowDelete = r.ReadBoolean()
                    });
                    iCount--;
                }
            }
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
