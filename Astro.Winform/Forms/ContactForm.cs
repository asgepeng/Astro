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

namespace Astro.Winform.Forms
{
    public partial class ContactForm : Form
    {
        string[] phoneTypes = { "Home", "Mobile", "Whatsapp", "Chat" };
        public ContactForm()
        {
            InitializeComponent();
        }
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
                var lvi = listView1.Items.Add(ad.StreetAddress, 0);
                lvi.SubItems.Add(ad.City.Name + " " + ad.StateOrProvince.Name + ", " + ad.ZipCode);
                lvi.SubItems.Add(ad.IsPrimary ? "Primary Address" : "Secondary");
            }
            var phoneGroup = listView1.Groups.Add("pg", "Phones");
            foreach (var em in this.Contact.Phones)
            {
                var lvi = listView1.Items.Add(em.Number, 1);
                lvi.SubItems.Add(GetPhoneType(em.Type));
                lvi.SubItems.Add(em.IsPrimary ? "Primary" : "Secondary");
                lvi.Group = phoneGroup;
            }
            var emailGroup = this.listView1.Groups.Add("ep", "Emails");
            foreach (var em in this.Contact.Emails)
            {
                var lvi = listView1.Items.Add(em.Address, 3);
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
            var json = string.Empty;
            if (this.Contact.Id == 0)
            {
                json = await HttpClientSingleton.PostAsync(this.Text == "Supplier" ? "/data/suppliers" : "/data/customers", this.Contact.ToString());
            }
            else
            {
                json = await HttpClientSingleton.PutAsync(this.Text == "Supplier" ? "/data/suppliers" : "/data/customers", this.Contact.ToString());
            }
            var commonResult = CommonResult.Create(json);
            if (commonResult != null)
            {
                if (commonResult.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(commonResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.Contact.Name;
            FillAddressBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(
                  this.button2,
                  new Point(0, this.button2.Height)
              );
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var form = new EmailForm();
            if (this.Contact.Emails.Count == 0) form.DisablePrimary();
            if (form.ShowDialog() == DialogResult.OK)
            {
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
                this.Contact.Addresses.Add(form.Address);
                FillAddressBox();
            }
        }
    }
}
