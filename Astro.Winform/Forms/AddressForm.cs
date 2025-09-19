using Astro.Models;
using Astro.Winform.Classes;
using System.Data;
using Astro.Binaries;
using Astro.Winform.Extensions;
using Astro.Data;

namespace Astro.Winform.Forms
{
    public partial class AddressForm : Form
    {
        private readonly IDBClient db;
        public AddressForm()
        {
            InitializeComponent();
            db = My.Application.CreateDBAccess();
        }
        public Address Address { get; set; } = new Address();

        private void button1_Click(object sender, EventArgs e)
        {
            if (typeComboBox.SelectedIndex < 0)
            {
                typeComboBox.ShowError("Tipe alamat belum dipilih.");
                return;
            }
            if (string.IsNullOrWhiteSpace(streetTextBox.Text))
            {
                streetTextBox.ShowError("Alamat tidak boleh kosong.");
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

            await LoadStatesAsync(this.Address.StateOrProvince.Id);
            if (this.Address.StateOrProvince.Id > 0) await LoadCitiesAsync(this.Address.StateOrProvince.Id, this.Address.City.Id);
            if (this.Address.City.Id > 0) await LoadDistrictsAsync(this.Address.City.Id, this.Address.District.Id);
            if (this.Address.District.Id > 0) await LoadVillagesAsync(this.Address.District.Id, this.Address.Village.Id);


            this.zipCodeTextBox.Text = this.Address.ZipCode;
            this.primaryCheckBox.Checked = this.Address.IsPrimary;
            this.primaryCheckBox.Visible = !this.Address.IsPrimary;

            stateComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (this.stateComboBox.SelectedItem is null) return;

                this.cityComboBox.Items.Clear();
                this.districtComboBox.Items.Clear();
                this.villageComboBox.Items.Clear();

                this.Address.StateOrProvince = (Province)this.stateComboBox.SelectedItem;
                await LoadCitiesAsync(this.Address.StateOrProvince.Id, 0);
                
            };
            cityComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (this.cityComboBox.SelectedItem is null) return;

                this.districtComboBox.Items.Clear();
                this.villageComboBox.Items.Clear();

                this.Address.City = (City)this.cityComboBox.SelectedItem;
                await this.LoadDistrictsAsync(this.Address.City.Id, 0);
            };
            districtComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (this.districtComboBox.SelectedItem is null) return;

                this.villageComboBox.Items.Clear();
                this.Address.District = (District)this.districtComboBox.SelectedItem;
                await this.LoadVillagesAsync(this.Address.District.Id, 0);
            };
            villageComboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (this.villageComboBox.SelectedItem is null) return;

                this.Address.Village = (Village)this.villageComboBox.SelectedItem;
            };
        }
        private async Task LoadStatesAsync(short stateId)
        {
            var commandText = """
                SELECT stateid, name
                FROM states
                WHERE countryid = 360
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var state = new Province()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1)
                    };
                    this.stateComboBox.Items.Add(state);
                    if (state.Id == stateId) this.stateComboBox.SelectedIndex = this.stateComboBox.Items.Count - 1;
                }
            }, commandText);
        }
        private async Task LoadCitiesAsync(short stateId, int cityId)
        {
            var commandText = """
                SELECT cityid, name
                FROM cities
                WHERE stateid = @stateid
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var city = new City()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    this.cityComboBox.Items.Add(city);
                    if (city.Id == cityId) this.cityComboBox.SelectedIndex = this.cityComboBox.Items.Count - 1;
                }
            }, commandText, db.CreateParameter("stateid", stateId, DbType.Int16));
        }
        private async Task LoadDistrictsAsync(int cityId, int districtId)
        {
            var commandText = """
                SELECT districtid, name
                FROM districts
                WHERE cityid = @cityid
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var district = new District()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    };
                    this.districtComboBox.Items.Add(district);
                    if (district.Id == districtId) this.districtComboBox.SelectedIndex = this.districtComboBox.Items.Count - 1;
                }
            }, commandText, db.CreateParameter("cityid", cityId, DbType.Int32));
        }
        private async Task LoadVillagesAsync(int districtId, long villageId)
        {
            var commandText = """
                SELECT villageid, name
                FROM villages
                WHERE districtid = @districtid
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var village = new Village()
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1)
                    };
                    this.villageComboBox.Items.Add(village);
                    if (village.Id == villageId) this.villageComboBox.SelectedIndex = this.villageComboBox.Items.Count - 1;
                }
            }, commandText, db.CreateParameter("districtid", districtId, DbType.Int32));
        }
    }
}
