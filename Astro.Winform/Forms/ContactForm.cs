using Astro.Data;
using Astro.Models;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using Astro.Winform.UserControls;
using System.Data;
using System.Data.Common;
using System.Text;
using WinformApp.Data;

namespace Astro.Winform.Forms
{
    public partial class ContactForm : XPanel
    {
        string[] phoneTypes = { "Home", "Mobile", "Whatsapp", "Chat" };
        private readonly IDBClient db;
        public ContactForm()
        {
            InitializeComponent();
            db = My.Application.CreateDBAccess();
        }
        public short ContactType { get; set; } = 0;
        private string GetPhoneType(short type)
        {
            if (type < phoneTypes.Length) return phoneTypes[type];
            return string.Empty;
        }
        private void CreateSubItem(ListViewItem item, string text)
        {
            var small = new Font("Segoe UI", 8.75F, FontStyle.Regular);
            item.SubItems.Add(text).Font = small;
        }
        private void FillAddressBox()
        {
            if (this.listView1.Items.Count > 0) this.listView1.Items.Clear();
            var group = this.listView1.Groups.Add("ad", "Addresses");
            foreach (var ad in this.Contact.Addresses)
            {
                var lvi = listView1.Items.Add(ad.StreetAddress, ad.Type);
                lvi.SubItems.Add(ad.Village.Name + " " + ad.District.Name + " " + ad.City.Name + " " + ad.StateOrProvince.Name + ", " + ad.ZipCode);
                lvi.SubItems.Add(ad.IsPrimary ? "Primary Address" : "Secondary");
                lvi.Group = group;
            }
            var phoneGroup = listView1.Groups.Add("pg", "Phones");
            foreach (var em in this.Contact.Phones)
            {
                var lvi = listView1.Items.Add(em.Number, em.Type + 2);
                lvi.SubItems.Add(GetPhoneType(em.Type));
                lvi.SubItems.Add(em.IsPrimary ? "Primary" : "Secondary");
                lvi.Group = phoneGroup;
            }
            var emailGroup = this.listView1.Groups.Add("ep", "Emails");
            foreach (var em in this.Contact.Emails)
            {
                var lvi = listView1.Items.Add(em.Address, 5);
                lvi.SubItems.Add(em.IsPrimary ? "Primary" : GetPhoneType(em.Type));
                lvi.Group = emailGroup;
            }
        }
        public Contact Contact { get; set; } = new Contact();
        private async void button1_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (string.IsNullOrWhiteSpace(this.contactNameTextBox.Text))
            {
                this.contactNameTextBox.ShowError("Nama supplier tidak bolek kosong.");
                return;
            }
            this.Contact.Name = this.contactNameTextBox.Text.Trim();
            var success = this.Contact.Id > 0 ? await this.UpdateAsync() : await this.CreateAsync();
            if (success)
            {
                mainForm.DialogResult = DialogResult.OK;
                mainForm.Close();
            }
            else
            {
                MessageBox.Show("GAGAL");
            }
        }
        private async Task<bool> CreateAsync()
        {
            var commandText = """
                INSERT INTO contacts
                    (name)
                VALUES
                    (@name)
                RETURNING contactid
                """;
            this.Contact.Id = await db.ExecuteScalarAsync<short>(commandText, db.CreateParameter("name", this.Contact.Name));
            if (this.Contact.Id == 0)
            {
                MessageBox.Show("Gagal menyimpan data supplier karena kesalahan yang tak diketahui.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            await UpdateAddressAsync();
            return true;
        }
        private async Task<bool> UpdateAsync()
        {
            var commandText = """
                UPDATE contacts
                SET name = @name
                WHERE contactid = @contactid
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactid", this.Contact.Id, DbType.Int16),
                db.CreateParameter("name", this.Contact.Name, DbType.AnsiString)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            if (!success) return false;

            await UpdateAddressAsync();
            return true;
        }
        private async Task UpdateAddressAsync()
        {
            if (this.Contact.Addresses.Count > 0 || this.Contact.Phones.Count > 0 || this.Contact.Emails.Count > 0)
            {
                var sb = new StringBuilder();
                var listParameter = new List<DbParameter>()
                {
                    db.CreateParameter("contactId", this.Contact.Id, DbType.Int16)
                };
                if (this.Contact.Id > 0)
                {
                    sb.AppendLine("""
                        DELETE FROM addresses WHERE contactid = @contactId;
                        DELETE FROM phones WHERE contactid = @contactId;
                        DELETE FROM emails WHERE contactid = @contactId;
                        """);
                }
                if (this.Contact.Addresses.Count > 0)
                {
                    sb.Append("""
                    INSERT INTO addresses 
                    (contactid, streetaddress, villageid, zipcode, addresstype, isprimary)
                    VALUES 
                    """);
                    for (int i = 0; i < this.Contact.Addresses.Count; i++)
                    {
                        var address = this.Contact.Addresses[i];
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @a").Append(i).Append(",").Append("@v").Append(i).Append(",@z").Append(i).Append(",@t").Append(i).Append(",@p").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("a" + i, address.StreetAddress, DbType.AnsiString));
                        listParameter.Add(db.CreateParameter("v" + i, address.Village.Id, DbType.Int64));
                        listParameter.Add(db.CreateParameter("z" + i, address.ZipCode, DbType.AnsiString));
                        listParameter.Add(db.CreateParameter("t" + i, address.Type, DbType.Int16));
                        listParameter.Add(db.CreateParameter("p" + i, address.IsPrimary, DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                if (this.Contact.Phones.Count > 0)
                {
                    sb.Append("""
                    INSERT INTO phones (contactid, phonenumber, phonetype, isprimary)
                    VALUES
                    """);
                    for (short i = 0; i < this.Contact.Phones.Count; i++)
                    {
                        var phone = this.Contact.Phones[i];
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @pn").Append(i).Append(", @pt").Append(i).Append(", @pp").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("pn" + i, phone.Number, DbType.String));
                        listParameter.Add(db.CreateParameter("pt" + i, phone.Type, DbType.Int16));
                        listParameter.Add(db.CreateParameter("pp" + i, phone.IsPrimary, DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                if (this.Contact.Emails.Count > 0)
                {
                    sb.Append("""
                    INSERT INTO emails (contactid, emailaddress, emailtype, isprimary)
                    values
                    """);
                    for (short i = 0; i < this.Contact.Emails.Count; i++)
                    {
                        var email = this.Contact.Emails[i];
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @ea").Append(i).Append(",@et").Append(i).Append(",@ep").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("ea" + i, email.Address.Trim(), DbType.String));
                        listParameter.Add(db.CreateParameter("et" + i, email.Type, DbType.Int16));
                        listParameter.Add(db.CreateParameter("ep" + i, email.IsPrimary, DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                await db.ExecuteNonQueryAsync(sb.ToString(), listParameter.ToArray());
            }
        }
        private async void ContactForm_Load(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm != null) mainForm.SetEnableControls(false);

            this.label1.Text = this.Text + " Name";
            if (this.Text == "Customer") this.tabControl1.TabPages[2].Text = "🧾 Account Receivable";
            if (this.Tag != null)
            {
                short.TryParse(Tag.ToString(), out short contactId);
                var commandText = """
                    SELECT contactid, name
                    FROM contacts
                    WHERE contactid = @contactId
                    AND isdeleted = false
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        this.Contact.Id = reader.GetInt16(0);
                        this.Contact.Name = reader.GetString(1);
                    }
                }, commandText, db.CreateParameter("contactId", contactId, DbType.Int16));
                commandText = """
                    SELECT a.addressid, a.streetaddress, v.villageid, v.name AS villagename, d.districtid, d.name AS districtname,
                    c.cityid, c.name AS cityname, s.stateid, s.name AS statename, a.addresstype, a.isprimary, a.zipcode
                    FROM addresses AS a
                    	INNER JOIN villages AS v ON a.villageid = v.villageid
                    	INNER JOIN districts AS d ON v.districtid = d.districtid
                    	INNER JOIN cities AS c on d.cityid = c.cityid
                    	INNER JOIN states AS s ON c.stateid = s.stateid	
                    WHERE a.contactid = @contactId
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        this.Contact.Addresses.Add(new Address()
                        {
                            Id = reader.GetInt16(0),
                            StreetAddress = reader.GetString(1),
                            Village = new Village()
                            {
                                Id = reader.GetInt64(2),
                                Name = reader.GetString(3)
                            },
                            District = new District()
                            {
                                Id = reader.GetInt32(4),
                                Name = reader.GetString(5)
                            },
                            City = new City()
                            {
                                Id = reader.GetInt32(6),
                                Name = reader.GetString(7)
                            },
                            StateOrProvince = new Province()
                            {
                                Id = reader.GetInt16(8),
                                Name = reader.GetString(9)
                            },
                            Type = reader.GetInt16(10),
                            IsPrimary = reader.GetBoolean(11),
                            ZipCode = reader.GetString(12)
                        });
                    }
                }, commandText, db.CreateParameter("contactId", this.Contact.Id, DbType.Int16));
                commandText = """
                    select phoneid, phonenumber, phonetype, isprimary
                    from phones as p
                    where contactid = @contactId
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        this.Contact.Phones.Add(new Phone()
                        {
                            Id = reader.GetInt32(0),
                            Number = reader.GetString(1),
                            Type = reader.GetInt16(2),
                            IsPrimary = reader.GetBoolean(3)
                        });
                    }
                }, commandText, db.CreateParameter("contactId", this.Contact.Id, DbType.Int16));
                commandText = """
                    SELECT emailid, emailaddress, emailtype, isprimary
                    FROM emails
                    WHERE contactid = @contactId
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                while (await reader.ReadAsync())
                {
                        this.Contact.Emails.Add(new Email()
                        {
                            Id = reader.GetInt32(0),
                            Address = reader.GetString(1),
                            Type = reader.GetInt16(2),
                            IsPrimary = reader.GetBoolean(3)
                        });
                    }
                }, commandText, db.CreateParameter("contactId", this.Contact.Id, DbType.Int16));
            }
            this.contactNameTextBox.Text = this.Contact.Name;
            FillAddressBox();
            if (mainForm != null) mainForm.SetEnableControls(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(
                  this.addButton,
                  new Point(0, this.addButton.Height)
              );
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var form = new EmailForm();
            if (this.Contact.Emails.Count == 0) form.DisablePrimary();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (this.Contact.Emails.Count > 0 && form.Email.IsPrimary)
                {
                    for (int i = 0; i < this.Contact.Emails.Count; i++)
                    {
                        this.Contact.Emails[i].IsPrimary = false;
                    }
                }
                this.Contact.Emails.Add(form.Email);
                this.FillAddressBox();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var form = new PhoneForm();
            if (this.Contact.Phones.Count == 0) form.DisablePrimary();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (this.Contact.Phones.Count > 0 && form.Phone.IsPrimary)
                {
                    for (int i = 0; i < this.Contact.Phones.Count; i++)
                    {
                        this.Contact.Phones[i].IsPrimary = false;
                    }
                }
                this.Contact.Phones.Add(form.Phone);
                this.FillAddressBox();
            }
        }

        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AddressForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (this.Contact.Addresses.Count == 0) form.Address.IsPrimary = true;
                else
                {
                    if (form.Address.IsPrimary)
                    {
                        for (int i = 0; i < this.Contact.Addresses.Count; i++)
                        {
                            this.Contact.Addresses[i].IsPrimary = false;
                        }
                    }
                }
                this.Contact.Addresses.Add(form.Address);
                FillAddressBox();
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;

            var selectedItem = this.listView1.SelectedItems[0];

            if (selectedItem.Group != null)
            {
                var groupItems = listView1.Items
                    .Cast<ListViewItem>()
                    .Where(i => i.Group == selectedItem.Group)
                    .ToList();

                int indexInGroup = groupItems.IndexOf(selectedItem);

                if (selectedItem.Group.Header == "Addresses")
                {
                    var addressForm = new AddressForm();
                    addressForm.Address = this.Contact.Addresses[indexInGroup].Clone();
                    if (addressForm.ShowDialog() == DialogResult.OK)
                    {
                        if (!this.Contact.Addresses[indexInGroup].IsPrimary && addressForm.Address.IsPrimary)
                        {
                            MessageBox.Show("Primary address will be changed.");
                            for (int i = 0; i < this.Contact.Addresses.Count; i++)
                            {
                                this.Contact.Addresses[i].IsPrimary = false;
                            }
                        }
                        this.Contact.Addresses[indexInGroup] = addressForm.Address;
                        this.FillAddressBox();
                    }
                }
                else if (selectedItem.Group.Header == "Emails")
                {
                    var emailForm = new EmailForm();
                    emailForm.Email = this.Contact.Emails[indexInGroup].Clone();
                    if (emailForm.ShowDialog() == DialogResult.OK)
                    {
                        if (!this.Contact.Emails[indexInGroup].IsPrimary && emailForm.Email.IsPrimary)
                        {
                            foreach (var email in this.Contact.Emails)
                            {
                                if (email.IsPrimary)
                                {
                                    email.IsPrimary = false;
                                }
                            }
                        }
                        this.Contact.Emails[indexInGroup] = emailForm.Email;
                        this.FillAddressBox();
                    }
                }
                else if (selectedItem.Group.Header == "Phones")
                {
                    var phoneForm = new PhoneForm();
                    phoneForm.Phone = this.Contact.Phones[indexInGroup].Clone();
                    if (phoneForm.ShowDialog() == DialogResult.OK)
                    {
                        if (!this.Contact.Phones[indexInGroup].IsPrimary && phoneForm.Phone.IsPrimary)
                        {
                            foreach (var phone in this.Contact.Phones)
                            {
                                if (phone.IsPrimary)
                                {
                                    phone.IsPrimary = false;
                                    break;
                                }
                            }
                        }
                        this.Contact.Phones[indexInGroup] = phoneForm.Phone;
                        this.FillAddressBox();
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var enabled = this.listView1.SelectedItems.Count > 0;
            this.editButton.Enabled = enabled;
            this.deleteButton.Enabled = enabled;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;

            var selectedItem = this.listView1.SelectedItems[0];
            if (selectedItem.Group != null)
            {
                var groupItems = listView1.Items
                    .Cast<ListViewItem>()
                    .Where(i => i.Group == selectedItem.Group)
                    .ToList();

                int indexInGroup = groupItems.IndexOf(selectedItem);
                if (MessageBox.Show("Are you sure want to delete this record?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (selectedItem.Group.Header == "Addresses")
                    {
                        if (this.Contact.Addresses[indexInGroup].IsPrimary)
                        {
                            MessageBox.Show("You cannot delete this record because it is primary");
                            return;
                        }
                        this.Contact.Addresses.RemoveAt(indexInGroup);
                    }
                    else if (selectedItem.Group.Header == "Phones")
                    {
                        if (this.Contact.Phones[indexInGroup].IsPrimary)
                        {
                            MessageBox.Show("You cannot delete this record because it is primary");
                            return;
                        }
                        this.Contact.Phones.RemoveAt(indexInGroup);
                    }
                    else if (selectedItem.Group.Header == "Emails")
                    {
                        if (this.Contact.Emails[indexInGroup].IsPrimary)
                        {
                            MessageBox.Show("You cannot delete this record because it is primary");
                            return;
                        }
                        this.Contact.Emails.RemoveAt(indexInGroup);
                    }
                    this.FillAddressBox();
                }
            }
        }
    }
}
