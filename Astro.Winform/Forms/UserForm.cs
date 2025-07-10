using Astro.Models;
using Astro.ViewModels;
using Astro.Winform.Classes;
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

namespace Astro.Winform.Forms
{
    public partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
        }
        public UserViewModel? UserView { get; set; }

        private void UserForm_Load(object sender, EventArgs e)
        {
            if (this.UserView != null)
            {
                this.firstnameTextBox.Text = this.UserView.User.FirstName;
                this.lastnameTextBox.Text = this.UserView.User.LastName;
                this.emailTextBox.Text = this.UserView.User.Email;
                this.maritalComboBox.SelectedIndex = this.UserView.User.MaritalStatus;
                this.dobTextBox.Value = this.UserView.User.DateOfBirth;
                this.phoneTextBox.Text = this.UserView.User.PhoneNumber;
                this.sexComboBox.SelectedIndex = this.UserView.User.Sex;
                this.streetAddressTextBox.Text = this.UserView.User.StreetAddress;
                this.useExpirationCheckBox.Checked = this.UserView.User.UsePasswordExpiration;
                this.passwordExpirationDate.Enabled = this.UserView.User.UsePasswordExpiration;
                this.zipCodeTextBox.Text = this.UserView.User.ZipCode;
                if (this.passwordExpirationDate.Enabled)
                {
                    this.passwordExpirationDate.Value = this.UserView.User.PasswordExpirationDate.HasValue ? this.UserView.User.PasswordExpirationDate.Value : new DateTime(2999, 12, 31);
                }
                else
                {
                    this.passwordExpirationDate.Value = new DateTime(1900, 1, 1);
                }
                this.lockoutEnableCheckBox.Checked = this.UserView.User.LockoutEnabled;

                for (int i = 0; i < this.UserView.Roles.Count; i++)
                {
                    var item = this.UserView.Roles[i];
                    this.roleComboBox.Items.Add(item);
                    if (this.UserView.User.RoleId == item.Id) roleComboBox.SelectedIndex = i;
                }
                roleComboBox.DisplayMember = "Text";
                roleComboBox.ValueMember = "Id";

                for (int i = 0; i < this.UserView.Countries.Count; i++)
                {
                    var country = this.UserView.Countries[i];
                    this.countryComboBox.Items.Add(country);
                    if (country.Id == this.UserView.User.CountryId) this.countryComboBox.SelectedIndex = i;
                }
                countryComboBox.DisplayMember = "Text";
                countryComboBox.ValueMember = "Id";

                for (int i = 0; i < this.UserView.States.Count; i++)
                {
                    stateComboBox.Items.Add(this.UserView.States[i]);
                    if (this.UserView.States[i].Id == this.UserView.User.StateId) this.stateComboBox.SelectedIndex = i;
                }
                stateComboBox.DisplayMember = "Text";
                stateComboBox.ValueMember = "Id";

                for (int i = 0; i < this.UserView.Cities.Count; i++)
                {
                    cityComboBox.Items.Add(this.UserView.Cities[i]);
                    if (this.UserView.Cities[i].Id == this.UserView.User.CityId) cityComboBox.SelectedIndex = i;
                }
                cityComboBox.DisplayMember = "Text";
                cityComboBox.ValueMember = "Id";

                countryComboBox.SelectedIndexChanged += new EventHandler(this.GetStatesAsync);
                stateComboBox.SelectedIndexChanged += new EventHandler(this.GetCitiesAsync);
            }
        }
        private async void GetStatesAsync(object? sender, EventArgs e)
        {
            if (this.countryComboBox.SelectedItem == null) return;

            var countryId = (short)((Option)this.countryComboBox.SelectedItem).Id;
            var listOption = await ListOptionHelper.GetStateOptionsAsync(countryId);
            this.stateComboBox.Items.Clear();
            this.cityComboBox.Items.Clear();
            foreach (var item in listOption)
            {
                this.stateComboBox.Items.Add(item);
            }
        }
        private async void GetCitiesAsync(object? sender, EventArgs e)
        {
            if (this.stateComboBox.SelectedItem is null) return;
            var stateId = (short)((Option)this.stateComboBox.SelectedItem).Id;
            var listOption = await ListOptionHelper.GetCityOptionsAsync(stateId);
            this.cityComboBox.Items.Clear();
            foreach (var item in listOption)
            {
                this.cityComboBox.Items.Add(item);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (this.UserView is null) return;
            if (this.roleComboBox.SelectedItem is null) return;

            if (this.cityComboBox.SelectedItem is null || this.stateComboBox.SelectedItem is null || this.countryComboBox.SelectedItem is null) return;

            var user = this.UserView.User;
            user.FirstName = this.firstnameTextBox.Text.Trim();
            user.LastName = this.lastnameTextBox.Text.Trim();
            user.Email = this.emailTextBox.Text.Trim();
            user.PhoneNumber = this.phoneTextBox.Text.Trim();
            user.RoleId = (short)((Option)this.roleComboBox.SelectedItem).Id;
            user.UsePasswordExpiration = this.useExpirationCheckBox.Checked;
            user.LockoutEnabled = this.lockoutEnableCheckBox.Checked;
            user.StreetAddress = this.streetAddressTextBox.Text.Trim();
            user.CityId = (int)((Option)this.cityComboBox.SelectedItem).Id;
            user.StateId = (short)((Option)this.stateComboBox.SelectedItem).Id;
            user.CountryId = (short)((Option)this.countryComboBox.SelectedItem).Id;
            user.ZipCode = this.zipCodeTextBox.Text.Trim();
            user.MaritalStatus = (short)this.maritalComboBox.SelectedIndex;
            user.DateOfBirth = this.dobTextBox.Value;
            user.Sex = (short)this.sexComboBox.SelectedIndex;

            if (user.Id > 0)
            {
                var json = await HttpClientSingleton.PutAsync("/data/users", user.ToString());
                var commonResult = CommonResult.Create(json);
                if (commonResult != null)
                {
                    if (commonResult.Success)
                    {
                        MessageBox.Show(commonResult.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            else
            {
                var json = await HttpClientSingleton.PostAsync("/data/users", user.ToString());
                var commonResult = CommonResult.Create(json);
                if (commonResult != null)
                {
                    if (commonResult.Success)
                    {
                        MessageBox.Show(commonResult.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }
    }
}
