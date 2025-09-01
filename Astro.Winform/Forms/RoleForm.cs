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
    public partial class RoleForm : UserControl
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
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (this.Role is null) return;

            this.Role.Name = this.rolenameTextBox.Text.Trim();
            using (var writer = new Streams.Writer())
            {
                if (this.Role.Id > 0) writer.WriteInt16(this.Role.Id);
                else writer.WriteByte(0x01);
                writer.WriteString(this.rolenameTextBox.Text.Trim());
                writer.WriteInt32(this.Role.Permissions.Count);
                foreach (var item in this.Role.Permissions)
                {
                    writer.WriteInt16(item.Id);
                    writer.WriteBoolean(item.AllowCreate);
                    writer.WriteBoolean(item.AllowRead);
                    writer.WriteBoolean(item.AllowEdit);
                    writer.WriteBoolean(item.AllowDelete);
                }
                var json = this.Role.Id > 0 ? await WClient.PutAsync("/data/roles", writer.ToArray()) : await WClient.PostAsync("/data/roles", writer.ToArray());
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
                        MessageBox.Show(commonResult.Message);
                    }
                }
            }
        }
    }
}
