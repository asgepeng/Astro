using Astro.Models;
using Astro.ViewModels;
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
                var json = await HttpClientSingleton.GetAsync("/auth/permissions");
                var listMenu = ListMenu.Create(json);
                if (listMenu != null)
                {
                    foreach (var item in listMenu)
                    {
                        var parent = new ToolStripMenuItem() { Text = item.Title };
                        this.ms.Items.Add(parent);
                        foreach (var submenu in item.Items)
                        {
                            var child = new ToolStripMenuItem() { Text = submenu.Title, Tag = submenu.Id };
                            child.Click += this.HandleMenuClicked;
                            parent.DropDownItems.Add(child);
                        }
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
            if (My.Application.ApiToken.Trim() != "") await HttpClientSingleton.SignOutAsync();
            HttpClientSingleton.Dispose();
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
    }
}
