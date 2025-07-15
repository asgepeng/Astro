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
            this.Address.City = new City()
            {
                Id = selectedCity.Id,
                Name = selectedCity.Text
            };
            this.Address.ZipCode = this.zipCodeTextBox.Text.Trim();
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

            if (this.Address.StateOrProvince.Id == 0)
            {
                this.countryComboBox.DataSource = await ListOptionHelper.GetCountryOptionsAsync();
            }
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
