using Astro.DataTables;
using Astro.Models;
using Astro.Winform.Classes;
using Astro.Winform.Helpers;
using Astro.Winform.Tables;
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
        private List<string> filters = new List<string>();

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
                    var userForm = new UserForm()
                    {
                        UserView = await objectBuilder.CreateUserViewModel(id)
                    };
                    return userForm;
                case ListingData.Roles:
                    var roleForm = new RoleForm() { Role = await objectBuilder.CreateRoleViewModel(id) };
                    return roleForm;
                case ListingData.Products:
                    var productForm = new ProductForm() { Model = await objectBuilder.CreateProductViewModel(id) };
                    return productForm;
                default:
                    throw new NotSupportedException("Unsupported listing data type.");
            }
        }
        internal string Filter { get; private set; } = string.Empty;
        internal void ApplyFilter(string filterValue)
        {
            if (string.IsNullOrWhiteSpace(filterValue))
            {
                this.Filter = string.Empty;
                this.BindingSource.RemoveFilter();
                return;
            }
            var search = filterValue.Replace("'", "''");
            var sb = new StringBuilder();
            for (int i=0; i < this.filters.Count; i++)
            {
                if (i > 0) sb.Append(" OR ");
                sb.Append(filters[i]).Append(" LIKE '%").Append(search).Append("%'");
            }
            this.Filter = filterValue;
            this.BindingSource.Filter = sb.ToString();
        }
        internal async Task LoadDataAsync()
        {
            var table = GetDataTable();
            await table.LoadAsync();
            this.BindingSource.DataSource = table;
            if (this.filters.Count == 0)
            {
                this.filters = table.GetStringColumns();
            }
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
            if (this.BindingSource.Current is null) return;
            var id = (short)((DataRowView)this.BindingSource.Current)[0];
            var dialog = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog != DialogResult.Yes) return;

            var url = "";
            switch (this.ListingType)
            {
                case ListingData.Users:
                    url = "/data/users/" + id.ToString();
                    break;
                case ListingData.Roles:
                    url = "/data/roles/" + id.ToString();
                    break;
                case ListingData.Products:
                    url = "/data/products/" + id.ToString();
                    break;
                default:
                    throw new NotSupportedException("Unsupported listing data type.");
            }
            var result = await HttpClientSingleton.DeleteAsync(url);
            var commonResult = CommonResult.Create(result);
            if (commonResult != null)
            {
                if (commonResult.Success)
                    MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(commonResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                case ListingData.Products:
                    return new ProductDataTable();
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
                        new DataTableColumnInfo("Role", "role_name", 120),
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
                case ListingData.Products:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Name", "name", 300),
                        new DataTableColumnInfo("SKU", "sku", 100),
                        new DataTableColumnInfo("Category", "category", 150),
                        new DataTableColumnInfo("Stock", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Unit", "unit", 100),
                        new DataTableColumnInfo("Price", "price", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Created By", "creator", 200),
                        new DataTableColumnInfo("Created Date", "created_date", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
            }
        }
        private bool flagDoubleClickHandled = false;
        private async void HandleCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BindingSource.Current is null) return;
            if (flagDoubleClickHandled) return;

            flagDoubleClickHandled = true;
            this.Cursor = Cursors.WaitCursor;
            var id = (short)((DataRowView)this.BindingSource.Current)[0];
            var dialog = await CreatePopupDialog(id);
            if (dialog.ShowDialog()== DialogResult.OK)
            {
                await this.LoadDataAsync();
            }
            this.Cursor = Cursors.Default;
            flagDoubleClickHandled = false;
        }
    }
    internal enum ListingData
    {
        Users = 0,
        Roles = 1,
        Products = 2
    }
}
