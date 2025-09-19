using Astro.Models.Transactions;
using Astro.Winform.Classes;
using Astro.Winform.Forms;
using Astro.Models;
using Astro.Winform.Helpers;
using Astro.Extensions;
using Astro.Binaries;
using DocumentFormat.OpenXml.InkML;
using Org.BouncyCastle.Ocsp;
using System.Data.Common;
using System.Data;
using Astro.Data;
using MySqlX.XDevAPI.Common;

namespace Astro.Winform.UserControls
{
    public partial class PurchaseControl : UserControl
    {
        private Point _mousePoint;
        private bool _closeButtonHit;
        private readonly IDBClient db = My.Application.CreateDBAccess();
        public PurchaseControl()
        {
            this.InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            this.Text = "Pembelian";
            this.grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.grid.ColumnHeadersDefaultCellStyle.Font = new Font(this.grid.Font, FontStyle.Bold);
            this.purchaseItemBindingSource.DataSource = this.Purchase.Items;
        }
        public Image? Icon { get; set; }
        #region Protected Area
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var x = 15;
            var y = 10;
            if (Icon != null)
            {
                var imgRext = new Rectangle(x, y, 32, 32);
                e.Graphics.DrawImage(this.Icon, imgRext);
                x += 37;
            }
            var rect = new Rectangle(x, y, this.Width - 10, 32);
            TextFormatFlags flags = TextFormatFlags.EndEllipsis;
            TextFormatFlags centerFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(e.Graphics, this.Text, My.Application.TitleFont, rect, My.Application.TitleForeColor, flags);
            var closeButton = new Rectangle(e.ClipRectangle.Width - 40, 0, 40, 40);
            var closeColor = SystemColors.ControlText;
            if (closeButton.Contains(_mousePoint))
            {
                e.Graphics.FillRectangle(Brushes.Red, closeButton);
                closeColor = SystemColors.Window;
            }
            TextRenderer.DrawText(e.Graphics, "\uE8BB", My.Application.EmojiFont, closeButton, closeColor, centerFlags);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mousePoint = e.Location;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.X >= this.Width - 40 && e.Y <= 40 && _closeButtonHit)
            {
                var mainForm = this.FindForm();
                if (mainForm != null) mainForm.Close();
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.X >= this.Width - 40 && e.Y <= 40)
            {
                _closeButtonHit = true;
            }
            Invalidate();
        }
        #endregion Protected Area
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
                SELECT c.contactid, c.name, a.streetaddress
                FROM contacts AS c
                LEFT JOIN addresses AS a ON c.contactid = a.contactid AND a.isprimary = true
                WHERE c.contacttype = 0 AND c.isdeleted = false
                """;
            using (var dialog = new ListingPopUpForm(commandText))
            {
                dialog.AddColumn("Kode", "contactid", 60, DataGridViewContentAlignment.MiddleCenter, "00000");
                dialog.AddColumn("Supplier", "name", 200);
                dialog.AddColumn("Alamat", "streetaddress", 300);
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
                        SELECT p.productid, p.name, p.sku, i.stock, u.name AS unitname, i.buyprice
                        FROM products AS p
                        LEFT JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @locationid
                        INNER JOIN units AS u ON p.unitid = u.unitid
                        WHERE p.isdeleted = false AND i.isactive = true
                        """;
                    using (var dialog = new ListingPopUpForm(sql))
                    {
                        dialog.Text = "Datfar Barang";

                        dialog.AddColumn("Kode", "productid", 60, DataGridViewContentAlignment.MiddleCenter, "00000");
                        dialog.AddColumn("Nama Barang", "name", 300);
                        dialog.AddColumn("SKU", "sku", 120);
                        dialog.AddColumn("Stok", "stock", 80, DataGridViewContentAlignment.MiddleRight, "N0");
                        dialog.AddColumn("Satuan", "unitname", 100);
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
            this.Purchase.Date = this.dateTimePicker1.Value.ToUniversalTime();
        }

        private async void BaseFormLoad(object sender, EventArgs e)
        {
            this.comboBox2.ValueMember = "Id";
            this.comboBox2.DisplayMember = "Text";

            this.purchaseItemBindingSource.DataSource = this.Purchase.Items;
            if (My.Application.User != null)
            {
                this.comboBox2.Items.Add(new Option<short>()
                {
                    Id = My.Application.User.Id,
                    Text = "Tunai - " + My.Application.User.Name
                });
            }
            var commandText = """
                SELECT ac.accountid, CONCAT(p.name, ' - ', ac.accountname) AS txt
                FROM accounts AS ac
                INNER JOIN accountproviders AS p ON ac.providerid = p.providerid
                WHERE ac.isdeleted = false
                ORDER BY ac.accountid
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    this.comboBox2.Items.Add(new Option<short>()
                    {
                        Id = reader.GetInt16(0),
                        Text = reader.GetString(1)
                    });
                }
            }, commandText);
        }
        private void NewProductButtonClicked(object sender, EventArgs e)
        {
            using (var form = new ProductForm())
            {

            }
        }
        private async Task<PurchaseItem?> GetPurchaseItemAsync(short productId, string sku = "")
        {
            PurchaseItem? item = null;
            var commandText = """
                SELECT p.productid, p.name, p.sku, u.name AS unitname, i.buyprice
                FROM products AS p
                INNER JOIN units AS u ON p.unitid = u.unitid
                INNER JOIN inventories AS i ON p.productid = i.productid  AND i.locationid = @location
                WHERE p.isdeleted = false AND (p.productid = @id OR p.sku = @sku)
                ORDER BY p.sku LIMIT 1
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("location", My.Application.GetCurrentLocationID(), DbType.Int16),
                db.CreateParameter("id", productId, DbType.Int16),
                db.CreateParameter("sku", sku)
            };
            await db.ExecuteReaderAsync(async reader =>
            {
                if (await reader.ReadAsync())
                {
                    item = new PurchaseItem()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1),
                        Sku = reader.GetString(2),
                        Quantity = 1,
                        Unit = reader.GetString(3),
                        Price = reader.GetInt64(4)
                    };
                }
            }, commandText, parameters);
            return item;
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
            this.button3.Enabled = grid.Rows.Count > 0;
            this.Calculate();
        }

        private void GridRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
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
                    this.Calculate();
                }
            }
        }
        private async void SaveButtonClicked(object sender, EventArgs e)
        {
            if (this.Purchase is null) return;
            if (this.Purchase.Items.Count == 0) return;

            this.Purchase.Calculate();
            var paidAmount = this.paidAmountTextBox.Text.ToInt64();
            if (paidAmount < this.Purchase.GrandTotal)
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
            if (paidAmount > this.Purchase.GrandTotal) paidAmount = this.Purchase.GrandTotal;
            if (paidAmount > 0)
            {
                var accountType = this.comboBox2.SelectedIndex == 0 ? (short)1 : (short)2;
                if (this.comboBox2.SelectedItem is null) return;
                var payment = new Payment()
                {
                    AccountType = accountType,
                    Amount = paidAmount,
                    AccountId = ((Option<short>)this.comboBox2.SelectedItem).Id
                };
                this.Purchase.Payments.Add(payment);
            }
            this.Purchase.Id = Guid.NewGuid();
            this.Purchase.InvoiceNumber = "INV/20332/25/2025-09-08";
            if (await this.CreateAsync())
            {
                var form = this.FindForm();
                if (form is null) return;

                form.DialogResult = DialogResult.OK;
                form.Close();
            }
        }
        private async Task<bool> CreateAsync()
        {
            this.Purchase.Calculate();
            var creator = My.Application.GetCurrentUserID();
            var parameters = new DbParameter[]
            {
                db.CreateParameter("id", this.Purchase.Id, DbType.Guid),
                db.CreateParameter("invoicenumber", this.Purchase.InvoiceNumber, DbType.AnsiString),
                db.CreateParameter("location", My.Application.GetCurrentLocationID(), DbType.Int16),
                db.CreateParameter("purchasedate", this.Purchase.Date, DbType.DateTime),
                db.CreateParameter("supplierid", this.Purchase.SupplierId, DbType.Int16),
                db.CreateParameter("subtotal", this.Purchase.SubTotal, DbType.Int64),
                db.CreateParameter("discount", this.Purchase.Discount, DbType.Int32),
                db.CreateParameter("cost", this.Purchase.Cost, DbType.Int32),
                db.CreateParameter("grandtotal", this.Purchase.GrandTotal, DbType.Int32),
                db.CreateParameter("tax", this.Purchase.Tax, DbType.Int32),
                db.CreateParameter("totalpaid", this.Purchase.TotalPaid, DbType.Int64),
                db.CreateParameter("status", this.Purchase.GetStatusCode(), DbType.Int16),
                db.CreateParameter("notes", "", DbType.AnsiString),
                db.CreateParameter("creator", creator, DbType.Int16)
            };
            var commandText = this.Purchase.GenerateSql();
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            if (!success) return false;

            if (this.Purchase.Payments.Count > 0)
            {
                var iCount = 0;
                commandText = """
                    INSERT INTO cashflows
                        (cashflowid, cashflowdate, refid, reftype, accountid, accounttype, amount, creatorid)
                    VALUES
                        (@paymentid, @purchasedate, @purchaseid, 1, @accountid, @accounttype, @amount, @creator)
                    """;
                foreach (var payment in this.Purchase.Payments)
                {
                    var paymentParams = new DbParameter[]
                    {
                        db.CreateParameter("paymentid", payment.Id, DbType.Guid),
                        db.CreateParameter("purchasedate", this.Purchase.Date, DbType.DateTime),
                        db.CreateParameter("purchaseid", this.Purchase.Id, DbType.Guid),
                        db.CreateParameter("accountid", payment.AccountId, DbType.Int16),
                        db.CreateParameter("accountType", payment.AccountType, DbType.Int16),
                        db.CreateParameter("amount", payment.Amount * -1, DbType.Int64),
                        db.CreateParameter("creator", creator, DbType.Int16)
                    };
                    success = await db.ExecuteNonQueryAsync(commandText, paymentParams);
                    if (success) iCount++;
                }
                if (iCount == this.Purchase.Payments.Count)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Gagal menyimpan pembayaran");
                    return false;
                }
            }
            return true;
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
            var amount = this.paidAmountTextBox.Text.ToInt64();
            var refund = amount - this.Purchase.GrandTotal;
            if (refund > 0) this.refundTextBox.Text = refund.ToString("N0");
            else this.refundTextBox.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BarcodeTextBoxKeyDown(sender, new KeyEventArgs(Keys.Enter));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Purchase.Calculate();
            this.textBox7.Text = this.Purchase.Items.GenerateSql();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
    }
}
