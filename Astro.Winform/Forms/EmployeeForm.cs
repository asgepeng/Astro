using Astro.Models;
using Astro.Streams;
using Astro.Winform.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Astro.Winform.Forms
{
    public partial class EmployeeForm : UserControl
    {
        public EmployeeForm()
        {
            InitializeComponent();
        }
        private List<Village> Villages { get; } = new List<Village>();
        private List<Province> States { get; } = new List<Province>();
        private List<City> Cities { get; } = new List<City>();
        private List<District> Districts { get; } = new List<District>();

        private async void LoadEmployeeData(object sender, EventArgs e)
        {
            this.provinceComboBox.DisplayMember = "Name";
            this.provinceComboBox.ValueMember = "Id";
            this.cityComboBox.DisplayMember = "Name";
            this.cityComboBox.ValueMember = "Id";
            this.districtComboBox.DisplayMember = "Name";
            this.districtComboBox.ValueMember = "Id";
            this.villageComboBox.DisplayMember = "Name";
            this.villageComboBox.ValueMember = "Id";

            if (this.Tag != null)
            {
                using (var stream = await WClient.GetStreamAsync("/data/employees/" + Tag.ToString()))
                using (var reader = new Streams.Reader(stream))
                {
                    if (stream.Length == 0) return;
                    long villageId = 0;
                    int cityId = 0;
                    int districtId = 0;
                    short stateId = 0;
                    short roleId = 0;

                    var exists = reader.ReadBoolean();
                    if (exists)
                    {
                        formLabel.Text = "Detil Pegawai";
                        saveButton.Text = "Simpan Perubahan";
                        reader.ReadInt16();
                        this.fullnameTextBox.Text = reader.ReadString();
                        this.bopTextBox.Text = reader.ReadString();
                        this.bodDatePicker.Value = reader.ReadDateTime();
                        this.sexComboBox.SelectedIndex = reader.ReadInt16();
                        this.maritalComboBox.SelectedIndex = reader.ReadInt16();
                        this.emailTextBox.Text = reader.ReadString();
                        this.phoneTextBox.Text = reader.ReadString();
                        this.streetTextBox.Text = reader.ReadString();
                        this.zipCodeTextBox.Text = reader.ReadString();

                        villageId = reader.ReadInt64();
                        districtId = reader.ReadInt32();
                        cityId = reader.ReadInt32();
                        stateId = reader.ReadInt16();
                        roleId = reader.ReadInt16();
                    }
                    else
                    {
                        formLabel.Text = "Tambah Pegawai";
                        saveButton.Text = "Tambahkan";
                    }
                    await LoadStatesAsync(stateId);
                    if (stateId > 0) await LoadCitiesAsync(stateId, cityId);
                    if (cityId > 0) await LoadDistrictsAsync(cityId, districtId);
                    if (districtId > 0) await LoadVillagesAsync(districtId, villageId);
                }
            }
            else
            {
                formLabel.Text = "Tambah Pegawai";
                saveButton.Text = "Tambahkan";
            }
        }
        private async Task LoadStatesAsync(short stateId)
        {
            using (var ms = await WClient.GetStreamAsync("/data/regions/states/360"))
            using (var stateReader = new Streams.Reader(ms))
            {
                var stateCount = stateReader.ReadInt32();
                for (int i = 0; i < stateCount; i++)
                {
                    var item = new Province()
                    {
                        Id = stateReader.ReadInt16(),
                        Name = stateReader.ReadString()
                    };
                    this.provinceComboBox.Items.Add(item);
                    if (stateId == item.Id) this.provinceComboBox.SelectedIndex = i;
                }
            }
        }
        private async Task LoadCitiesAsync(short stateId, int cityId)
        {
            using (var ms = await WClient.GetStreamAsync("/data/regions/cities/" + stateId.ToString()))
            using (var cityReader = new Streams.Reader(ms))
            {
                var cityCount = cityReader.ReadInt32();
                for (int i = 0; i < cityCount; i++)
                {
                    var item = new City()
                    {
                        Id = cityReader.ReadInt32(),
                        Name = cityReader.ReadString()
                    };
                    cityComboBox.Items.Add(item);
                    if (cityId == item.Id) this.cityComboBox.SelectedIndex = i;
                }
            }
        }
        private async Task LoadDistrictsAsync(int cityId, int districtId)
        {
            using (var ms = await WClient.GetStreamAsync("/data/regions/districts/" + cityId.ToString()))
            using (var districtReader = new Streams.Reader(ms))
            {
                var districtCount = districtReader.ReadInt32();
                for (int i = 0; i < districtCount; i++)
                {
                    var item = new District()
                    {
                        Id = districtReader.ReadInt32(),
                        Name = districtReader.ReadString()
                    };
                    districtComboBox.Items.Add(item);
                    if (districtId == item.Id) this.districtComboBox.SelectedIndex = i;
                }
            }
        }
        private async Task LoadVillagesAsync(int districtId, long villageId)
        {
            using (var ms = await WClient.GetStreamAsync("/data/regions/villages/" + districtId.ToString()))
            using (var villageReader = new Streams.Reader(ms))
            {
                var villageCount = villageReader.ReadInt32();
                for (int i = 0; i < villageCount; i++)
                {
                    var item = new Village()
                    {
                        Id = villageReader.ReadInt64(),
                        Name = villageReader.ReadString()
                    };
                    villageComboBox.Items.Add(item);
                    if (villageId == item.Id) this.villageComboBox.SelectedIndex = i;
                }
            }
        }
    }
}
