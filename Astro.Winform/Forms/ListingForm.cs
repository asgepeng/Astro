using Astro.DataTables;
using Astro.Winform.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformApp.Data;

namespace Astro.Winform.Forms
{
    public partial class ListingForm : Form
    {
        internal BindingSource BindingSource { get; } = new BindingSource();
        internal ListingData ListingType { get; } = ListingData.Users;
        internal ListingForm(ListingData type)
        {
            InitializeComponent();
            this.ListingType = type;
            InitializeGridColumns();
        }
        private async Task<Form> CreatePopupDialog(short id = 0)
        {
            var objectBuilder = new ObjectBuilder();
            switch (this.ListingType)
            {
                case ListingData.Users:
                    var userForm = new UserForm();
                    userForm.UserView = await objectBuilder.CreateUserViewModel(id);
                    return userForm;
                case ListingData.Roles:
                    var roleForm = new RoleForm();
                    roleForm.Role = await objectBuilder.CreateRoleViewModel(id);
                    return roleForm;
                default:
                    throw new NotSupportedException("Unsupported listing data type.");
            }
        }
        internal async Task LoadDataAsync()
        {
            var table = GetDataTable();
            await table.LoadAsync();
            this.BindingSource.DataSource = table;
        }
        internal async Task AddRecordAsync()
        {
            var form = await CreatePopupDialog();
            if (form != null)
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await this.LoadDataAsync();
                }
            }
        }
        internal async Task EditAsync()
        {
            var form = await CreatePopupDialog();
            if (form != null)
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await this.LoadDataAsync();
                }
            }
        }
        internal async Task DeleteAsync()
        {
            await this.LoadDataAsync();
        }
        private AstroDataTable GetDataTable()
        {
            switch (this.ListingType)
            {
                case ListingData.Users:
                    return new UserDataTable();
                case ListingData.Roles:
                    return new RoleDataTable();
                default:
                    throw new NotSupportedException("Unsupported listing data type.");
            }
        }
        private void InitializeGridColumns()
        {
            switch (this.ListingType)
            {
                case ListingData.Users:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Id", "id", 50, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Name", "fullname", 320, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Email Address", "email", 250, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Role Name", "role_name", 120),
                        new DataTableColumnInfo("Created By", "creator", 150),
                        new DataTableColumnInfo("Created Date", "created_date", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Roles:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Role Name", "name", 300),
                        new DataTableColumnInfo("Created By", "creator", 200),
                        new DataTableColumnInfo("Created Date", "created_date", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
            }
        }

        private async void HandleCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BindingSource.Current is null) return;

            var id = (short)((DataRowView)this.BindingSource.Current)[0];
            var dialog = await CreatePopupDialog(id);
            if (dialog.ShowDialog()== DialogResult.OK)
            {
                await this.LoadDataAsync();
            }
        }
    }
    internal enum ListingData
    {
        Users = 0,
        Roles = 1
    }
}
