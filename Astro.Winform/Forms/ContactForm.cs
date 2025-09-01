using Astro.Models;
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
using WinformApp.Data;

namespace Astro.Winform.Forms
{
    public partial class ContactForm : UserControl
    {
        string[] phoneTypes = { "Home", "Mobile", "Whatsapp", "Chat" };
        public ContactForm()
        {
            InitializeComponent();            
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
            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                MessageBox.Show("Supplier name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox1.Focus();
                return;
            }
            this.Contact.Name = this.textBox1.Text.Trim();
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            using (var writer = new Streams.Writer())
            {
                writer.WriteByte(0x01);
                writer.WriteInt32(this.Contact.Id);
                writer.WriteString(this.Contact.Name);
                writer.WriteInt32(this.Contact.Addresses.Count);
                for (int i = 0; i < this.Contact.Addresses.Count; i++)
                {
                    var address = this.Contact.Addresses[i];
                    writer.WriteString(address.StreetAddress.Trim());
                    writer.WriteInt64(address.Village.Id);
                    writer.WriteString(address.ZipCode.Trim());
                    writer.WriteInt16(address.Type);
                    writer.WriteBoolean(address.IsPrimary);
                }
                writer.WriteInt32(this.Contact.Phones.Count);
                for (int i = 0; i < this.Contact.Phones.Count; i++)
                {
                    var phone = this.Contact.Phones[i];
                    writer.WriteString(phone.Number.Trim());
                    writer.WriteInt16(phone.Type);
                    writer.WriteBoolean(phone.IsPrimary);
                }
                writer.WriteInt32(this.Contact.Emails.Count);
                for (int i = 0; i < this.Contact.Emails.Count; i++)
                {
                    var email = this.Contact.Emails[i];
                    writer.WriteString(email.Address.Trim());
                    writer.WriteInt16(email.Type);
                    writer.WriteBoolean(email.IsPrimary);
                }
                var endpoint = this.ContactType == 0 ? "/data/suppliers" : "/data/customers";
                var result = this.Contact.Id > 0 ? await WClient.PutAsync(endpoint, writer.ToArray()) : await WClient.PostAsync(endpoint, writer.ToArray());
                var commonResult = CommonResult.Create(result);
                if (commonResult != null)
                {
                    if (commonResult.Success)
                    {
                        mainForm.DialogResult = DialogResult.OK;
                        mainForm.Close();
                    }
                    else
                    {
                        MessageBox.Show(commonResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
                using (var stream = await WClient.GetStreamAsync("/data/suppliers/" + Tag.ToString()))
                using (var reader = new Astro.Streams.Reader(stream))
                {
                    var supplierExist = reader.ReadBoolean();
                    if (supplierExist)
                    {
                        Contact.Id = reader.ReadInt16();
                        Contact.Name = reader.ReadString();
                    }
                    var iCount = reader.ReadInt32();
                    while (iCount > 0)
                    {
                        Contact.Addresses.Add(new Address()
                        {
                            Id = reader.ReadInt32(),
                            StreetAddress = reader.ReadString(),
                            Village = new Village()
                            {
                                Id = reader.ReadInt64(),
                                Name = reader.ReadString()
                            },
                            District = new District()
                            {
                                Id = reader.ReadInt32(),
                                Name = reader.ReadString()
                            },
                            City = new City()
                            {
                                Id = reader.ReadInt32(),
                                Name = reader.ReadString()
                            },
                            StateOrProvince = new Province()
                            {
                                Id = reader.ReadInt16(),
                                Name = reader.ReadString()
                            },
                            Type = reader.ReadInt16(),
                            IsPrimary = reader.ReadBoolean(),
                            ZipCode = reader.ReadString()
                        });
                        iCount--;
                    }
                    iCount = reader.ReadInt32();
                    while (iCount > 0)
                    {
                        Contact.Phones.Add(new Phone()
                        {
                            Id = reader.ReadInt32(),
                            Number = reader.ReadString(),
                            Type = reader.ReadInt16(),
                            IsPrimary = reader.ReadBoolean()
                        });
                        iCount--;
                    }
                    iCount = reader.ReadInt32();
                    while (iCount > 0)
                    {
                        Contact.Emails.Add(new Email()
                        {
                            Id = reader.ReadInt32(),
                            Address = reader.ReadString(),
                            Type = reader.ReadInt16(),
                            IsPrimary = reader.ReadBoolean()
                        });
                        iCount--;
                    }
                }
            }
            this.textBox1.Text = this.Contact.Name;
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
