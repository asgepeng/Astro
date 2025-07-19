using Astro.Models;
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
    public partial class AddressForm : Form
    {
        public AddressForm()
        {
            InitializeComponent();
        }
        public Address Address { get; set; } = new Address();

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Address type is not selected.", "Address Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(streetTextBox.Text))
            {
                MessageBox.Show("Street address cannot be empty.", "Street address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                streetTextBox.Focus();
                return;
            }
            if (countryComboBox.SelectedItem is null)
            {
                MessageBox.Show("Country is not selected", "Country empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                countryComboBox.Focus();
                return;
            }
            if (stateComboBox.SelectedItem is null)
            {
                MessageBox.Show("State is not selected", "State empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                stateComboBox.Focus();
                return;
            }
            if (cityComboBox.SelectedItem is null)
            {
                MessageBox.Show("City cannot be empty.", "City", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cityComboBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(zipCodeTextBox.Text))
            {
                MessageBox.Show("Zip or Postal code cannot be empty.", "Zip / Postal Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                zipCodeTextBox.Focus();
                return;
            }

            this.Address.Type = (short)typeComboBox.SelectedIndex;
            this.Address.StreetAddress = streetTextBox.Text.Trim();
            var selectedCity = (Option)this.cityComboBox.SelectedItem;
            var selectedState = (Option)this.stateComboBox.SelectedItem;
            var selectedCountry = (Option)this.countryComboBox.SelectedItem;
            this.Address.City = new City()
            {
                Id = selectedCity.Id,
                Name = selectedCity.Text
            };
            this.Address.StateOrProvince = new Province()
            {
                Id = (short)selectedState.Id,
                Name = selectedState.Text
            };
            this.Address.Country = new Country()
            {
                Id = (short)selectedCountry.Id,
                Name = selectedCountry.Text
            };
            this.Address.ZipCode = this.zipCodeTextBox.Text.Trim();
            this.Address.IsPrimary = this.primaryCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void AddressForm_Load(object sender, EventArgs e)
        {
            this.countryComboBox.DisplayMember= "Text";
            this.countryComboBox.ValueMember = "Id";
            this.stateComboBox.DisplayMember = "Text";
            this.stateComboBox.ValueMember = "Id";
            this.cityComboBox.DisplayMember = "Text";
            this.cityComboBox.ValueMember = "Id";

            this.typeComboBox.SelectedIndex = this.Address.Type;
            this.streetTextBox.Text = this.Address.StreetAddress;

            if (this.Address.StateOrProvince.Id == 0)
            {
                this.countryComboBox.DataSource = await ListOptionHelper.GetCountryOptionsAsync();
            }
            else
            {
                this.countryComboBox.DataSource = await ListOptionHelper.GetCountryOptionsAsync();
                this.stateComboBox.DataSource = await ListOptionHelper.GetStateOptionsAsync(this.Address.Country.Id);
                this.cityComboBox.DataSource = await ListOptionHelper.GetCityOptionsAsync(this.Address.StateOrProvince.Id);

                this.countryComboBox.Text = this.Address.Country.Name;
                this.stateComboBox.Text = this.Address.StateOrProvince.Name;
                this.cityComboBox.Text = this.Address.City.Name;
            }
            this.zipCodeTextBox.Text = this.Address.ZipCode;
            this.primaryCheckBox.Checked = this.Address.IsPrimary;
            this.primaryCheckBox.Visible = !this.Address.IsPrimary;

            countryComboBox.SelectedIndexChanged += CountryChanged;
            stateComboBox.SelectedIndexChanged += StateChanged;
        }
        private async void CountryChanged(object? sender, EventArgs e)
        {
            if (this.countryComboBox.SelectedItem is Option country)
            {
                this.stateComboBox.DataSource = await ListOptionHelper.GetStateOptionsAsync((short)country.Id);
                this.stateComboBox.SelectedIndex = -1;
                this.cityComboBox.DataSource = null;
            }
        }
        private async void StateChanged(object? sender, EventArgs e)
        {
            if (this.stateComboBox.SelectedItem is Option state)
            {
                this.cityComboBox.DataSource = await ListOptionHelper.GetCityOptionsAsync((short)state.Id);
                this.cityComboBox.SelectedIndex = -1;
            }
            else
            {
                this.cityComboBox.DataSource = null;
            }
        }
    }
}
