using Astro.Models;
using Astro.IO;
using Astro.Winform.Classes;
using Astro.Winform.Forms;
using Astro.Winform.Helpers;
using PointOfSale.Drawing;

namespace PointOfSale
{
    public partial class MainForm : Form
    {
        private VirtualControlCollection VirtualControls { get; } = new VirtualControlCollection();
        public MainForm()
        {
            InitializeComponent();
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            var form = new LoginForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (var stream = await WClient.GetStreamAsync("/auth/permissions"))
                using (var reader = new Reader(stream))
                {
                    var sectionLength = reader.ReadInt32();
                    while (sectionLength > 0)
                    {
                        var parent = new ToolStripMenuItem() { Tag = reader.ReadInt16(), Text = reader.ReadString() };
                        this.ms.Items.Add(parent);
                        int menuLength = reader.ReadInt32();
                        while (menuLength > 0)
                        {
                            var child = new ToolStripMenuItem() { Tag = reader.ReadInt16(), Text = reader.ReadString() };
                            reader.ReadInt32();

                            child.Click += this.HandleMenuClicked;
                            parent.DropDownItems.Add(child);
                            menuLength--;
                        }
                        sectionLength--;
                    }
                }

                this.WindowState = FormWindowState.Maximized;
                if (My.Application.User != null)
                {
                    this.Text = My.Application.User.Name + " - " + My.Application.User.Role.Name;
                }
            }
            else
            {
                this.Close();
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (My.Application.ApiToken.Trim() != "") await WClient.SignOutAsync();
            WClient.Dispose();
        }
        private async void HandleMenuClicked(object? sender, EventArgs e)
        {
            if (sender is null) return;

            var menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Tag is null) return;

            var menuId = (short)menuItem.Tag;
            switch (menuId)
            {
                case 1:
                    var objectBuilder = new ObjectBuilder();
                    var uvm = await objectBuilder.CreateUserViewModel(My.Application.User is null ? (short)0 : My.Application.User.Id);
                    var userDialog = new UserForm();
                    userDialog.UserView = uvm;
                    userDialog.ShowDialog();
                    break;
                case 2:
                    OpenOrCreateListingForm(ListingData.Users);
                    break;
                case 3:
                    OpenOrCreateListingForm(ListingData.Roles);
                    break;
                case 4:
                    var dialog = new ChangePasswordForm();
                    dialog.ShowDialog();
                    break;
                case 5:
                    this.Close();
                    break;
                case 6:
                    OpenOrCreateListingForm(ListingData.Products);
                    break;
                case 7:
                    OpenOrCreateListingForm(ListingData.Customers);
                    break;
                case 8:
                    OpenOrCreateListingForm(ListingData.Suppliers);
                    break;
                case 10:
                    OpenOrCreateListingForm(ListingData.Accounts);
                    break;
                case 11:
                    var soForm = new StockOpnameForm();
                    soForm.MdiParent = this;
                    soForm.Show();
                    soForm.WindowState = FormWindowState.Maximized;
                    break;
                case 12:
                    var pForm = new PurchaseForm();
                    pForm.MdiParent = this;
                    pForm.Show();
                    pForm.WindowState = FormWindowState.Maximized;
                    break;
            }
        }
        private void OpenOrCreateListingForm(ListingData type)
        {
            foreach (Form form in this.MdiChildren)
            {
                if (form is ListingForm lform)
                {
                    if (lform.ListingType == type)
                    {
                        lform.Activate();
                        return;
                    }
                }
            }
            var newForm = new ListingForm(type);
            newForm.MdiParent = this;
            newForm.Show();
            newForm.WindowState = FormWindowState.Maximized;
        }
        private async void HandleMdiChildActivate(object sender, EventArgs e)
        {
            var activeForm = this.ActiveMdiChild;
            if (activeForm != null && activeForm is ListingForm lform)
            {
                this.navigator.BindingSource = lform.BindingSource;
                this.searchTextBox.Text = lform.Filter;
                await lform.LoadDataAsync();
                return;
            }
            this.navigator.BindingSource = null;
        }

        private async void HandleNewButtonClicked(object sender, EventArgs e)
        {
            var activeForm = this.ActiveMdiChild;
            if (activeForm != null && activeForm is ListingForm lform)
            {
                await lform.AddRecordAsync();
            }
        }

        private async void HandleDeleteButtonClicked(object sender, EventArgs e)
        {
            var activeForm = this.ActiveMdiChild;
            if (activeForm != null && activeForm is ListingForm lform)
            {
                await lform.DeleteAsync();
            }
        }

        private void HandleCategoryButtonClicked(object sender, EventArgs e)
        {
            var form = new ListCategoryForm();
            form.ShowDialog();
        }

        private void unitButton_Click(object sender, EventArgs e)
        {
            var form = new ListUnitForm();
            form.ShowDialog();
        }

        private void ApplyFilter(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                var activeForm = this.ActiveMdiChild;
                if (activeForm is null) return;

                if (activeForm is ListingForm lform)
                {
                    lform.ApplyFilter(this.searchTextBox.Text);
                }
            }
        }

        private void OpenAccountProviderForm(object sender, EventArgs e)
        {
            var form = new ListAccountProviderForm();
            form.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var form = new ListingPopUpForm("SELECT * FROM products WHERE is_deleted = false");
            form.AddColumn("Product Name", "product_name", 300);
            form.AddColumn("SKU", "product_sku", 180);
            form.AddColumn("Price", "price", 100, DataGridViewContentAlignment.MiddleRight, "N0");
            form.ShowDialog();
        }

        private void sqlButton_Click(object sender, EventArgs e)
        {
            var form = new SqlExecuteForm();
            form.ShowDialog();
        }
    }
}
