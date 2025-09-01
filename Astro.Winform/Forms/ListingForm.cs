using Astro.DataTables;
using Astro.Models;
using Astro.Winform.Classes;
using Astro.Winform.Helpers;
using Astro.Winform.Tables;
using PointOfSale;
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
        private readonly string sourceUrl;
        internal ListingForm(ListingData type)
        {
            InitializeComponent();
            this.ListingType = type;
            switch (this.ListingType)
            {
                case ListingData.Roles:
                    sourceUrl = "/data/roles";
                    break;
                case ListingData.Products:
                    sourceUrl = "/data/products";
                    break;
                case ListingData.Suppliers:
                    sourceUrl = "/data/suppliers";
                    break;
                case ListingData.Customers:
                    sourceUrl = "/data/customers";
                    break;
                case ListingData.Accounts:
                    sourceUrl = "/data/accounts";
                    break;
                case ListingData.Employee:
                    sourceUrl = "/data/employees";
                    break;
                default:
                    sourceUrl = "";
                    break;
            }
            InitializeGridColumns();
        }
        private async Task<Form> CreatePopupDialog(int id = 0)
        {
            var objectBuilder = new ObjectBuilder();
            switch (this.ListingType)
            {
                case ListingData.Users:
                    var userForm = new UserForm()
                    {
                        UserView = await objectBuilder.CreateUserViewModel((short)id)
                    };
                    return userForm;
                case ListingData.Products:
                    var productForm = new ProductForm() { Tag = id };
                    if (this.MdiParent != null)
                    {
                        var branch = ((MainForm)this.MdiParent).GetSelectedBranch();
                        if (branch != null)
                        {
                            productForm.BranchId = branch.Id;
                        }
                    }
                    throw new NotSupportedException("Unsupported listing data type.");
                case ListingData.Suppliers:
                case ListingData.Customers:
                    var text = ListingType == ListingData.Suppliers ? "Supplier" : "Customer";
                    var supplierForm = new ContactForm() { Text = text };
                    if (id > 0)
                    {
                        var contact = ListingType == ListingData.Suppliers ? await objectBuilder.CreateSupplier((short)id) : await objectBuilder.CreateCustomer((short)id);
                        if (contact != null) supplierForm.Contact = contact;
                    }

                    throw new NotSupportedException("Unsupported listing data type.");
                case ListingData.Accounts:
                    var accountForm = new AccountForm();
                    throw new NotSupportedException("Unsupported listing data type.");
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
            using (var stream = await WClient.GetStreamAsync(sourceUrl))
            using (var reader = new Streams.Reader(stream))
            {
                var result = reader.ReadByte();
                if (result == 0x00)
                {
                    this.BindingSource.DataSource = null;
                    return;
                }
                if (stream.Length > 0)
                {
                    this.BindingSource.DataSource = reader.ReadDataTable();
                }
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
                case ListingData.Suppliers:
                    url = "/data/suppliers/" + id.ToString();
                    break;
                case ListingData.Customers:
                    url = "/data/customers/" + id.ToString();
                    break;
                case ListingData.Accounts:
                    url = "/data/accounts/" + id.ToString();
                    break;
                default:
                    throw new NotSupportedException("Unsupported listing data type.");
            }
            var result = await WClient.DeleteAsync(url);
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
                    var table = new ProductDataTable();
                    if (this.MdiParent != null)
                    {
                        var branch = ((MainForm)this.MdiParent).GetSelectedBranch();
                        if (branch != null) table.BranchId = branch.Id;
                    }
                    return table;
                case ListingData.Suppliers:
                case ListingData.Customers:
                    return new ContactDataTable() { ContactType = ListingType == ListingData.Suppliers ? (short)0 : (short)1 };
                case ListingData.Accounts:
                    return new AccountDataTable();
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
                        new DataTableColumnInfo("Name", "fullname", 320, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Email Address", "email", 250, DataGridViewContentAlignment.MiddleLeft, ""),
                        new DataTableColumnInfo("Role", "role_name", 120),
                        new DataTableColumnInfo("Created By", "creator", 150),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Roles:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Role Name", "name", 300),
                        new DataTableColumnInfo("Created By", "creator", 200),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Products:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Name", "name", 300),
                        new DataTableColumnInfo("SKU", "sku", 100),
                        new DataTableColumnInfo("Category", "categoryname", 150),
                        new DataTableColumnInfo("Stock", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Unit", "unitname", 100),
                        new DataTableColumnInfo("Price", "price", 120, DataGridViewContentAlignment.MiddleRight, "N0"),
                        new DataTableColumnInfo("Created By", "fullname", 200),
                        new DataTableColumnInfo("Created Date", "createddate", 120, DataGridViewContentAlignment.MiddleRight, "dd/MM/yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Suppliers:
                case ListingData.Customers:
                    var headerText = ListingType == ListingData.Suppliers ? "Supplier Name" : "Customer Name";
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo(headerText, "name", 300),
                        new DataTableColumnInfo("Address", "address", 300),
                        new DataTableColumnInfo("Phone Number", "phonenumber", 120),
                        new DataTableColumnInfo("Created By", "creator", 180),
                        new DataTableColumnInfo("Created At", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Employee:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        //new DataTableColumnInfo("ID", "id", 60, DataGridViewContentAlignment.MiddleCenter, "00000"),
                        new DataTableColumnInfo("Nama Pegawai", "fullname", 300),
                        new DataTableColumnInfo("Alamat", "address", 300),
                        new DataTableColumnInfo("Phone Number", "phone", 120),
                        new DataTableColumnInfo("Jabatan", "rolename", 200),
                        new DataTableColumnInfo("Created By", "creator", 180),
                        new DataTableColumnInfo("Created At", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
                    }, this.BindingSource);
                    break;
                case ListingData.Accounts:
                    GridHelpers.InitializeDataGridColumns(this.dataGridView1, new DataTableColumnInfo[]
                    {
                        new DataTableColumnInfo("Account Name", "accountName", 200),
                        new DataTableColumnInfo("Account Number", "accountNumber", 200),
                        new DataTableColumnInfo("Account Type", "accountType", 120),
                        new DataTableColumnInfo("Provider Name", "providerName", 180),
                        new DataTableColumnInfo("Created By", "createdBy", 200),
                        new DataTableColumnInfo("Created Date", "createdDate", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm"),
                        new DataTableColumnInfo("Last Modified", "lastModified", 120, DataGridViewContentAlignment.MiddleRight, "dd-MM-yyyy HH:mm")
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
            var id = (int)((DataRowView)this.BindingSource.Current)[0];
            var dialog = await CreatePopupDialog(id);
            if (dialog.ShowDialog()== DialogResult.OK)
            {
                await this.LoadDataAsync();
            }
            this.Cursor = Cursors.Default;
            flagDoubleClickHandled = false;
        }
    }
    public enum ListingData
    {
        Users = 0,
        Roles = 1,
        Products = 2,
        Suppliers = 3,
        Customers = 4,
        Accounts = 5,
        Employee = 6
    }
}
