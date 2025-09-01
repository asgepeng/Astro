using Astro.Models;
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
            if (stateComboBox.SelectedItem is null)
            {
                MessageBox.Show("Country is not selected", "Country empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                stateComboBox.Focus();
                return;
            }
            if (cityComboBox.SelectedItem is null)
            {
                MessageBox.Show("State is not selected", "State empty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cityComboBox.Focus();
                return;
            }
            if (districtComboBox.SelectedItem is null)
            {
                MessageBox.Show("City cannot be empty.", "City", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                districtComboBox.Focus();
                return;
            }
            if (villageComboBox.SelectedItem is null)
            {
                MessageBox.Show("Desa / Kelurahan belum dipilih.", "City", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                villageComboBox.Focus();
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
            this.Address.StateOrProvince = (Province)stateComboBox.SelectedItem;
            this.Address.City = (City)cityComboBox.SelectedItem;
            this.Address.District = (District)districtComboBox.SelectedItem;
            this.Address.Village = (Village)villageComboBox.SelectedItem;
            this.Address.ZipCode = this.zipCodeTextBox.Text.Trim();
            this.Address.IsPrimary = this.primaryCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void AddressForm_Load(object sender, EventArgs e)
        {
            this.stateComboBox.DisplayMember= "Name";
            this.stateComboBox.ValueMember = "Id";
            this.cityComboBox.DisplayMember = "Name";
            this.cityComboBox.ValueMember = "Id";
            this.districtComboBox.DisplayMember = "Name";
            this.districtComboBox.ValueMember = "Id";
            this.villageComboBox.DisplayMember = "Name";
            this.villageComboBox.ValueMember = "Id";

            this.typeComboBox.SelectedIndex = this.Address.Type;
            this.streetTextBox.Text = this.Address.StreetAddress;

            using (var stream = await WClient.GetStreamAsync("/data/regions/states/360"))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var countryCount = reader.ReadInt32();
                for (int i = 0; i < countryCount; i++)
                {
                    var state = new Province()
                    {
                        Id = reader.ReadInt16(),
                        Name = reader.ReadString()
                    };
                    this.stateComboBox.Items.Add(state);
                    if (state.Id == this.Address.StateOrProvince.Id) this.stateComboBox.SelectedIndex = i;
                }
            }
            if (this.Address.StateOrProvince.Id != 0)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/cities/" + this.Address.StateOrProvince.Id.ToString()))
                using (var reader = new Streams.Reader(stream))
                {
                    var stateCount = reader.ReadInt32();
                    for (int i = 0; i < stateCount; i++)
                    {
                        var city = new City()
                        {
                            Id = reader.ReadInt32(),
                            Name = reader.ReadString()
                        };
                        this.cityComboBox.Items.Add(city);
                        if (city.Id == this.Address.City.Id) this.cityComboBox.SelectedIndex = i;
                    }
                }
            }
            if (this.Address.City.Id != 0)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/districts/" + this.Address.City.Id.ToString()))
                using (var reader = new Streams.Reader(stream))
                {
                    var districtCount = reader.ReadInt32();
                    for (int i = 0; i < districtCount; i++)
                    {
                        var district = new District()
                        {
                            Id = reader.ReadInt32(),
                            Name = reader.ReadString()
                        };
                        this.districtComboBox.Items.Add(district);
                        if (district.Id == this.Address.City.Id) this.districtComboBox.SelectedIndex = i;
                    }
                }
            }
            if (this.Address.District.Id != 0)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/villages/" + this.Address.District.Id.ToString()))
                using (var reader = new Streams.Reader(stream))
                {
                    var villageCount = reader.ReadInt32();
                    for (int i = 0; i < villageCount; i++)
                    {
                        var village = new Village()
                        {
                            Id = reader.ReadInt64(),
                            Name = reader.ReadString()
                        };
                        this.villageComboBox.Items.Add(village);
                        if (village.Id == this.Address.City.Id) this.villageComboBox.SelectedIndex = i;
                    }
                }
            }

            this.zipCodeTextBox.Text = this.Address.ZipCode;
            this.primaryCheckBox.Checked = this.Address.IsPrimary;
            this.primaryCheckBox.Visible = !this.Address.IsPrimary;

            stateComboBox.SelectedIndexChanged += StateChanged;
            cityComboBox.SelectedIndexChanged += CityChanged;
            districtComboBox.SelectedIndexChanged += DistrictChanged;
        }
        private async void StateChanged(object? sender, EventArgs e)
        {
            this.cityComboBox.Items.Clear();
            if (this.stateComboBox.SelectedItem is null)
            {
                return;
            }
            if (this.stateComboBox.SelectedItem is Province province)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/cities/" + province.Id.ToString()))
                using (var reader = new Astro.Streams.Reader(stream))
                {
                    var cityCount = reader.ReadInt32();
                    for (int i = 0; i < cityCount; i++)
                    {
                        var city = new City()
                        {
                            Id = reader.ReadInt32(),
                            Name = reader.ReadString()
                        };
                        this.cityComboBox.Items.Add(city);
                    }
                }
                this.districtComboBox.Items.Clear();
                this.villageComboBox.Items.Clear();
            }
        }
        private async void CityChanged(object? sender, EventArgs e)
        {
            this.districtComboBox.Items.Clear();
            if (this.cityComboBox.SelectedItem is null)
            {
                return;
            }

            if (this.cityComboBox.SelectedItem is City city)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/districts/" + city.Id.ToString()))
                using (var reader = new Astro.Streams.Reader(stream))
                {
                    var districtCount = reader.ReadInt32();
                    for (int i = 0; i < districtCount; i++)
                    {
                        var district = new District()
                        {
                            Id = reader.ReadInt32(),
                            Name = reader.ReadString()
                        };
                        this.districtComboBox.Items.Add(district);
                    }
                }
                this.villageComboBox.Items.Clear();
            }
        }
        private async void DistrictChanged(object? sender, EventArgs e)
        {
            villageComboBox.Items.Clear();
            if (districtComboBox.SelectedItem is null)
            {
                return;
            }            
            if (districtComboBox.SelectedItem is District district)
            {
                using (var stream = await WClient.GetStreamAsync("/data/regions/villages/" + district.Id.ToString()))
                using (var reader = new Astro.Streams.Reader(stream))
                {
                    var villageCount = reader.ReadInt32();
                    for (int i = 0; i < villageCount; i++)
                    {
                        var village = new Village()
                        {
                            Id = reader.ReadInt64(),
                            Name = reader.ReadString()
                        };
                        this.villageComboBox.Items.Add(village);
                    }
                }
            }
        }
    }
}
