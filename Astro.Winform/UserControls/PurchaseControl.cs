using Astro.Models.Transactions;
using Astro.Winform.Classes;
using Astro.Winform.Forms;
using Astro.Models;
using Astro.Winform.Helpers;
using Astro.Extensions;

namespace Astro.Winform.UserControls
{
    public partial class PurchaseControl : UserControl
    {
        public PurchaseControl()
        {
            InitializeComponent();
            this.grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new Font(this.grid.Font, FontStyle.Bold);
        }
        private void Calculate()
        {
            this.Purchase.Calculate();
            this.subTotalTextBox.Text = this.Purchase.SubTotal.ToString("N0");
            this.discountTextBox.Text = this.Purchase.Discount.ToString("N0");
            this.costTextBox.Text = this.Purchase.Cost.ToString("N0");
            this.grandTotalTextBox.Text = this.Purchase.GrandTotal.ToString("N0");
            this.grandTotalLabel.Text = this.Purchase.GrandTotal.ToString("N0");
        }
        private void SupplierButtonClicked(object sender, EventArgs e)
        {
            var commandText = """
                SELECT c.contact_id, c.contact_name, a.street_address
                FROM contacts AS c
                LEFT JOIN addresses AS a ON c.contact_id = a.owner_id AND a.is_primary = true
                WHERE c.contact_type = 0 AND c.is_deleted = false
                """;
            using (var dialog = new ListingPopUpForm(commandText))
            {
                dialog.AddColumn("Kode", "contact_id", 60, DataGridViewContentAlignment.MiddleCenter, "00000");
                dialog.AddColumn("Supplier", "contact_name", 200);
                dialog.AddColumn("Alamat", "street_address", 300);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Purchase.SupplierId = (short)(int)dialog.SelectedRow[0];
                    this.supplierIdTextBox.Text = this.Purchase.SupplierId.ToString("00000");
                    this.supplierNameTextBox.Text = dialog.SelectedRow[1].ToString();
                }
            }
        }
        public Purchase Purchase { get; set; } = new Purchase();

        private async void BarcodeTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(barcodeTextBox.Text))
                {
                    var sql = $"""
                        SELECT p.product_id, p.product_name, p.product_sku, i.stock, u.unit_name, i.buyprice
                        FROM products AS p
                        INNER JOIN inventories AS i ON p.product_id = i.product_id AND i.location_id = {this.Purchase.Location}
                        INNER JOIN units AS u ON p.unit_id = u.unit_id
                        WHERE p.is_deleted = false AND i.is_active = true
                        """;
                    using (var dialog = new ListingPopUpForm(sql))
                    {
                        dialog.Text = "Datfar Barang";

                        dialog.AddColumn("Kode", "product_id", 60, DataGridViewContentAlignment.MiddleCenter, "00000");
                        dialog.AddColumn("Nama Barang", "product_name", 300);
                        dialog.AddColumn("SKU", "product_sku", 120);
                        dialog.AddColumn("Stok", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0");
                        dialog.AddColumn("Satuan", "unit_name", 100);
                        dialog.AddColumn("Harga", "buyprice", 100, DataGridViewContentAlignment.MiddleRight, "N0");
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            var item = await GetPurchaseItemAsync((short)dialog.SelectedRow[0]);
                            if (item != null)
                            {
                                item.Quantity = 1;
                                this.Purchase.Items.Add(item);
                                this.purchaseItemBindingSource.ResetBindings(false);
                                this.Calculate();

                                this.grid.CurrentCell = this.grid.Rows[this.grid.Rows.Count - 1].Cells[3];
                                this.grid.BeginEdit(true);
                            }
                            else
                            {
                                MessageBox.Show("item null");
                            }
                        }
                    }
                }
                else
                {
                    var sku = this.barcodeTextBox.Text.Trim();
                    short.TryParse(sku, out short productId);
                    var item = await GetPurchaseItemAsync(productId, sku);
                    if (item != null)
                    {
                        this.barcodeTextBox.Clear();
                        item.Quantity = 1;
                        this.Purchase.Items.Add(item);
                        this.purchaseItemBindingSource.ResetBindings(false);
                        this.Calculate();

                        this.grid.CurrentCell = this.grid.Rows[this.grid.Rows.Count - 1].Cells[3];
                        this.grid.BeginEdit(true);
                    }
                    else
                    {
                        MessageBox.Show("item null");
                    }
                }
            }
        }

        private void ResetSupplierButtonClicked(object sender, EventArgs e)
        {
            this.Purchase.SupplierId = 0;
            this.supplierIdTextBox.Clear();
            this.supplierNameTextBox.Clear();
        }

        private void PurchaseDateChanged(object sender, EventArgs e)
        {
            this.Purchase.Date = this.dateTimePicker1.Value;
        }

        private async void BaseFormLoad(object sender, EventArgs e)
        {
            this.purchaseItemBindingSource.DataSource = this.Purchase.Items;
            using (var stream = await WClient.GetStreamAsync("/auth/stores"))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    this.locationComboBox.Items.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                }
                if (this.locationComboBox.Items.Count > 0) this.locationComboBox.SelectedIndex = 0;
                this.locationComboBox.DisplayMember = "Text";
                this.locationComboBox.ValueMember = "Id";
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    this.comboBox2.Items.Add(new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    });
                }
                if (this.comboBox2.Items.Count > 0) this.comboBox2.SelectedIndex = 0;
                this.comboBox2.ValueMember = "Id";
                this.comboBox2.DisplayMember = "Text";
            }
        }

        private void LocationComboBoxSelentedIndexChanged(object sender, EventArgs e)
        {
            if (this.locationComboBox.SelectedItem != null)
            {
                this.Purchase.Location = (short)((Option)locationComboBox.SelectedItem).Id;
            }
        }

        private async void NewProductButtonClicked(object sender, EventArgs e)
        {
            using (var form = new ProductForm())
            {
                var objectBuilder = new ObjectBuilder();
               
            }
        }
        private short GetLocationID()
        {
            if (this.locationComboBox.SelectedItem is null) return (short)0;
            return (short)((Option)this.locationComboBox.SelectedItem).Id;
        }
        private async Task<PurchaseItem?> GetPurchaseItemAsync(short productId, string sku = "")
        {
            var req = new PurchaseItemRequest()
            {
                Id = productId,
                Sku = sku,
                Location = GetLocationID()
            };
            using (var stream = await WClient.PostStreamAsync("/trans/purchases/get-item", req.ToString()))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                if (reader.ReadBoolean())
                {
                    var item = new PurchaseItem()
                    {
                        Id = reader.ReadInt16(),
                        Name = reader.ReadString(),
                        Sku = reader.ReadString(),
                        Quantity = reader.ReadInt32(),
                        Unit = reader.ReadString(),
                        Price = reader.ReadInt64()
                    };
                    return item;
                }
            }
            return null;
        }
        private void GridCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.Calculate();
            if (e.ColumnIndex == 3 && e.RowIndex == this.grid.RowCount - 1)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.grid.CurrentCell = this.grid.Rows[e.RowIndex].Cells[5];
                    this.grid.BeginEdit(true);
                });
            }
            if (e.ColumnIndex == 5 && e.RowIndex == this.grid.RowCount - 1)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.barcodeTextBox.Focus();
                });
            }
        }
        private void GridRowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            this.locationComboBox.Enabled = !(grid.Rows.Count > 0);
            this.button3.Enabled = grid.Rows.Count > 0;
            this.Calculate();
        }

        private void GridRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            this.locationComboBox.Enabled = !(grid.Rows.Count > 0);
            this.button3.Enabled = grid.Rows.Count > 0;
            this.Calculate();
        }

        private void GridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (this.purchaseItemBindingSource.Current is null) return;
            if (e.ColumnIndex == 9)
            {
                this.purchaseItemBindingSource.RemoveCurrent();
            }
        }
        private void ButtonCostClicked(object sender, EventArgs e)
        {
            using (var dialog = new CostsForm())
            {
                foreach (var item in this.Purchase.Costs)
                {
                    dialog.Costs.Add(item);
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.Purchase.Costs.Clear();
                    foreach (var item in dialog.Costs)
                    {
                        this.Purchase.Costs.Add(item);
                    }
                    this.Purchase.Cost = this.Purchase.Costs.GetTotal();
                    this.Calculate();
                }
            }
        }
        private async void SaveButtonClicked(object sender, EventArgs e)
        {
            if (this.Purchase is null) return;
            if (this.Purchase.Items.Count == 0) return;

            this.Purchase.Calculate();
            if (this.Purchase.TotalPaid < this.Purchase.GrandTotal)
            {
                if (!IsUserAllowedCreateAccountPayable())
                {
                    MessageBox.Show("Total pembayaran kurang dari Rp" + this.Purchase.GrandTotal.ToString("N0") + ".\n\nAnda tidak memiliki izin untuk membuat hutang pembelian.\n\nSilahkan hubungi administrator untuk mendapatkan izin tersebut.", "Transaksi tidak diijinkan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.Purchase.SupplierId == 0)
                {
                    MessageBox.Show("Silahkan pilih supplier terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (IsLimitexceeded(this.Purchase.AccountPayableAmount))
                {
                    MessageBox.Show("Total pembayaran kurang dari Rp" + this.Purchase.GrandTotal.ToString("N0") + ".\n\nAnda tidak memiliki izin untuk membuat hutang pembelian melebihi batas yang telah ditentukan.\n\nSilahkan hubungi administrator untuk mendapatkan izin tersebut.", "Transaksi tidak diijinkan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    if (MessageBox.Show("Transaki ini akan membuat Hutang sebesaar Rp" + this.Purchase.AccountPayableAmount.ToString("N0") + ".\n\nApakah anda yakin ingin melanjutkan?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }
            using (var stream = await WClient.PostStreamAsync("/trans/purchases", this.Purchase.ToByteArray()))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                var success = reader.ReadBoolean();
                if (success)
                {
                    MessageBox.Show(reader.ReadString(), reader.ReadString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.purchaseItemBindingSource.DataSource = null;

                    this.Purchase = new Purchase();
                    this.purchaseItemBindingSource.DataSource = this.Purchase.Items;
                    this.Calculate();
                }
                else
                {
                    MessageBox.Show(reader.ReadString(), reader.ReadString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private bool IsUserAllowedCreateAccountPayable()
        {
            return true;
        }
        private bool IsLimitexceeded(long apAmount)
        {
            return false;
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.FilterOnlyNumber();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            this.Purchase.TotalPaid = this.paidAmountTextBox.Text.ToInt64();
            if (this.Purchase.AccountPayableAmount < 0)
            {
                this.refundTextBox.Text = (-1 * this.Purchase.AccountPayableAmount).ToString("N0");
            }
            else
            {
                if (this.refundTextBox.Text.Length > 0) this.refundTextBox.Clear();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BarcodeTextBoxKeyDown(sender, new KeyEventArgs(Keys.Enter));
        }
    }
}
