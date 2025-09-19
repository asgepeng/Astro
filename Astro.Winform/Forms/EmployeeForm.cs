using Astro.Models;
using Astro.Winform.Classes;
using Astro.Binaries;
using Astro.Extensions;
using Astro.Winform.Extensions;
using Astro.Data;
using DocumentFormat.OpenXml.InkML;
using System.Data.Common;
using System.Data;

namespace Astro.Winform.Forms
{
    public partial class EmployeeForm : UserControl
    {
        private readonly IDBClient db = My.Application.CreateDBAccess();
        public EmployeeForm()
        {
            InitializeComponent();
        }
        public short EmployeeID { get; private set; } = 0;

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
            this.roleComboBox.DisplayMember = "Name";
            this.roleComboBox.ValueMember = "Id";

            long villageId = 0;
            int cityId = 0;
            int districtId = 0;
            short stateId = 0;
            short roleId = 0;

            if (this.Tag != null)
            {
                var commandText = """
                    SELECT e.employeeid, e.fullname, e.placeofbirth, e.dateofbirth, e.sex, e.maritalstatus, e.email, e.phone, 
                        e.streetaddress, e.zipcode, e.villageid, v.districtid, d.cityid, c.stateid, e.roleid, e.hireddate, e.isactive, 
                        e.payrolldate, e.notes
                    FROM employees AS e
                    LEFT JOIN villages AS v ON e.villageid = v.villageid
                    LEFT JOIN districts AS d ON v.districtid = d.districtid
                    LEFT JOIN cities AS c ON d.cityid = c.cityid
                    LEFT JOIN states AS s ON c.stateid = s.stateid
                    WHERE e.employeeid = @employeeid AND e.isdeleted = false
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        formLabel.Text = "Detil Pegawai";
                        saveButton.Text = "Simpan Perubahan";
                        this.EmployeeID = reader.GetInt16(0);
                        this.fullnameTextBox.Text = reader.GetString(1);
                        this.bopTextBox.Text = reader.GetString(2);
                        this.bodDatePicker.Value = reader.GetDateTime(3);
                        this.sexComboBox.SelectedIndex = reader.GetInt16(4);
                        this.maritalComboBox.SelectedIndex = reader.GetInt16(5);
                        this.emailTextBox.Text = reader.GetString(6);
                        this.phoneTextBox.Text = reader.GetString(7);
                        this.streetTextBox.Text = reader.GetString(8);
                        this.zipCodeTextBox.Text = reader.GetString(9);

                        villageId = reader.GetInt64(10);
                        districtId = reader.GetInt32(11);
                        cityId = reader.GetInt32(12);
                        stateId = reader.GetInt16(13);
                        roleId = reader.GetInt16(14);

                        this.hiredDateTextBox.Text = reader.GetDateTime(15).ToString("dd-MM-yyyy");
                        this.activeCheckBox.Checked = reader.GetBoolean(16);
                    }
                    else
                    {
                        formLabel.Text = "Tambah Pegawai";
                        saveButton.Text = "Tambahkan";
                    }
                }, commandText, db.CreateParameter("employeeid", (short)Tag, System.Data.DbType.Int16));
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
            await LoadRolesAsync(roleId);

            this.provinceComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (provinceComboBox.SelectedItem is null) return;

                if (this.cityComboBox.Items.Count > 0) this.cityComboBox.Items.Clear();
                if (this.districtComboBox.Items.Count > 0) this.districtComboBox.Items.Clear();
                if (this.villageComboBox.Items.Count > 0) this.villageComboBox.Items.Clear();

                var curState = (Province)this.provinceComboBox.SelectedItem;
                await LoadCitiesAsync(curState.Id, 0);
            };
            this.cityComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (this.cityComboBox.SelectedItem is null) return;

                if (this.districtComboBox.Items.Count > 0) this.districtComboBox.Items.Clear();
                if (this.villageComboBox.Items.Count > 0) this.villageComboBox.Items.Clear();

                var curCity = (City)this.cityComboBox.SelectedItem;
                await LoadDistrictsAsync(curCity.Id, 0);
            };
            this.districtComboBox.SelectedIndexChanged += async (sender, e) =>
            {
                if (this.districtComboBox.SelectedItem is null) return;

                if (this.villageComboBox.Items.Count > 0) this.villageComboBox.Items.Clear();
                var curDistrict = (District)this.districtComboBox.SelectedItem;
                await LoadVillagesAsync(curDistrict.Id, 0);
            };
        }
        private async Task LoadStatesAsync(short stateId)
        {
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var state = new Province()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1)
                    };
                    this.provinceComboBox.Items.Add(state);
                    if (state.Id == stateId) this.provinceComboBox.SelectedIndex = this.provinceComboBox.Items.Count - 1;
                }
            }, """
            SELECT stateid, name
            FROM states
            WHERE countryid = 360
            ORDER BY name
            """);
        }
        private async Task LoadCitiesAsync(short stateId, int cityId)
        {
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
                    if (city.Id == cityId) this.cityComboBox.SelectedIndex = cityComboBox.Items.Count - 1;
                }
            }, """
            SELECT cityid, name
            FROM cities
            WHERE stateid = @stateid
            """, db.CreateParameter("stateid", stateId, System.Data.DbType.Int16));
        }
        private async Task LoadDistrictsAsync(int cityId, int districtId)
        {
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
            }, """
            SELECT districtid, name
            FROM districts
            WHERE cityid = @cityid
            ORDER BY name
            """, db.CreateParameter("cityid", cityId));
        }
        private async Task LoadVillagesAsync(int districtId, long villageId)
        {
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
            }, """
            SELECT villageid, name
            FROM villages
            WHERE districtid = @districtid
            ORDER BY name
            """, db.CreateParameter("districtid", districtId, System.Data.DbType.Int32));
        }
        private async Task LoadRolesAsync(short roleId)
        {
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var role = new Role()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1)
                    };
                    this.roleComboBox.Items.Add(role);
                    if (role.Id == roleId) this.roleComboBox.SelectedIndex = this.roleComboBox.Items.Count - 1;
                }
            }, """
            SELECT roleid, name
            FROM roles
            ORDER BY name
            """);
        }
        private async void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.fullnameTextBox.Text))
            {
                this.fullnameTextBox.ShowError("Nama pegawai tidak boleh kosong");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.bopTextBox.Text))
            {
                this.bopTextBox.ShowError("Tempat lahir pegawai tidak boleh kosong");
                return;
            }
            if (bodDatePicker.Value < DateTime.Today.AddYears(-40) || bodDatePicker.Value > DateTime.Today.AddYears(-17))
            {
                this.bodDatePicker.ShowError($"Tanggal lahir harus antara {(DateTime.Today.AddYears(-40).ToString("dd-MM-yyyy"))} dan {DateTime.Today.AddYears(-17).ToString("dd-MM-yyyy")}");
                return;
            }
            if (sexComboBox.SelectedIndex == -1)
            {
                this.sexComboBox.ShowError("Jenis kelamin belum dipilih");
                return;
            }
            if (maritalComboBox.SelectedIndex == -1)
            {
                this.maritalComboBox.ShowError("Status perkawinan belum dipilih");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.emailTextBox.Text))
            {
                this.emailTextBox.ShowError("Alamat email tidak boleh kosong");
                return;
            }
            if (!this.emailTextBox.Text.IsValidEmailFormat())
            {
                this.emailTextBox.ShowError("Format alamat email tidak valid");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.phoneTextBox.Text))
            {
                this.phoneTextBox.ShowError("Nomor telepon tidak boleh kosong");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.streetTextBox.Text))
            {
                this.streetTextBox.ShowError("Alamat rumah tidak boleh kosong");
                return;
            }
            if (this.villageComboBox.SelectedItem is null)
            {
                this.villageComboBox.ShowError("Desa / Kelurahan tidak boleh kosong");
                return;
            }
            if (this.roleComboBox.SelectedItem is null)
            {
                this.roleComboBox.ShowError("Jabatan tidak boleh kosong");
                return;
            }

            var mainForm = this.FindForm();
            if (mainForm is null) return;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("employeeid", this.EmployeeID, DbType.Int16),
                db.CreateParameter("fullname", this.fullnameTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("placeofbirth", bopTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("dateofbirth", bodDatePicker.Value.ToUniversalTime(), DbType.DateTime),
                db.CreateParameter("sex", (short)this.sexComboBox.SelectedIndex, DbType.Int16),
                db.CreateParameter("maritalstatus", (short)this.maritalComboBox.SelectedIndex, DbType.Int16),
                db.CreateParameter("email", this.emailTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("phone", phoneTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("streetaddress", streetTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("zipcode", zipCodeTextBox.Text.Trim(), DbType.AnsiString),
                db.CreateParameter("villageid", ((Village)this.villageComboBox.SelectedItem).Id, DbType.Int64),
                db.CreateParameter("roleid", ((Role)this.roleComboBox.SelectedItem).Id, DbType.Int16),
                db.CreateParameter("isactive", activeCheckBox.Checked, DbType.Boolean),
                db.CreateParameter("userid", My.Application.GetCurrentUserID(), DbType.Int16)
            };
            var commandText =  this.EmployeeID == 0 ? """
                INSERT INTO employees 
                (
                    fullname, placeofbirth, dateofbirth, sex, maritalstatus, streetaddress,
                    villageid, zipcode, phone, email, hireddate, roleid, isactive, terminationdate, payrolldate,
                    payrollmethod, notes, creatorid
                ) 
                VALUES 
                (
                    @fullname, @placeofbirth, @dateofbirth, @sex, @maritalstatus, @streetaddress,
                    @villageid, @zipcode, @phone, @email, CURRENT_TIMESTAMP, @roleid, @isactive, NULL, 10,
                    1,'', @userid
                );
                """ : """
                UPDATE employees
                SET fullname = @fullname,
                    placeofbirth = @placeofbirth,
                    dateofbirth = @dateofbirth,
                    sex = @sex,
                    maritalstatus = @maritalstatus,
                    email = @email,
                    phone = @phone,
                    streetaddress = @streetaddress,
                    zipcode = @zipcode,
                    villageid = @villageid,
                    roleid = @roleid,
                    isactive = @isactive,
                    editorid = @userid
                WHERE employeeid = @employeeid
                """;
            if (await db.ExecuteNonQueryAsync(commandText, parameters))
            {
                mainForm.DialogResult = DialogResult.OK;
                mainForm.Close();
            }
        }
    }
}
