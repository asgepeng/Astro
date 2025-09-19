using Astro.Models;
using Astro.Text;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using Astro.Binaries;
using Astro.Data;
using System.Data.Common;
using System.Data;
using Astro.Extensions;

namespace Astro.Winform.Forms
{
    public partial class ProductForm : UserControl
    {
        private readonly IDBClient db;
        public ProductForm()
        {
            InitializeComponent();
            db = My.Application.CreateDBAccess();
        }
        private void DisableAllControls(bool disabled)
        {
            foreach (Control control in this.Controls)
            {
                if (control.GetType().Equals(typeof(TextBox)) || control.GetType().Equals(typeof(ComboBox)) || control.GetType().Equals(typeof(Button)) || control.GetType().Equals(typeof(Label)))
                {
                    control.Enabled = !disabled;
                }
            }
        }
        public short BranchId { get; set; } = 1;
        public Product? Product { get; private set; }
        private List<string> ImageURLs { get; } = new List<string>();
        private int SelectedImage = 0;
        private async Task DisplayImage()
        {
            if (SelectedImage < 0 || SelectedImage >= this.ImageURLs.Count)
            {
                return;
            }
            using (var stream = await WClient.GetStreamAsync("/documents/download/" + this.ImageURLs[SelectedImage]))
            {
                if (stream != null && stream.Length > 0)
                {
                    try
                    {
                        this.productImage.Image?.Dispose();
                        this.productImage.Image = Image.FromStream(stream);
                    }
                    catch (Exception)
                    {
                        this.ImageURLs.RemoveAt(SelectedImage);
                        this.SelectedImage = 0;
                    }
                }
            }
        }
        private async Task PreviousImage()
        {
            if (SelectedImage > 0) SelectedImage--; ;
            await DisplayImage();
        }
        private async Task NextImage()
        {
            if (SelectedImage < this.ImageURLs.Count - 1) SelectedImage++;
            await DisplayImage();
        }
        private async void ProductForm_Load(object sender, EventArgs e)
        {
            this.unitComboBox.DisplayMember = "Text";
            this.unitComboBox.ValueMember = "Id";

            this.categoryComboBox.DisplayMember = "Text";
            this.categoryComboBox.ValueMember = "Id";
            this.DisableAllControls(true);
            await this.GetByIdAsync();
            this.DisableAllControls(false);
        }
        private async Task GetByIdAsync()
        {
            short id = 0, unitID = 0, categoryID = 0;
            if (this.Tag != null)
            {
                short.TryParse(this.Tag.ToString(), out id);
            }
            string commandText = "";
            if (id > 0)
            {
                commandText = """
                SELECT p.productid, p.name, p.description, p.sku, p.categoryid, p.producttype,
                	i.isactive, i.stock, i.minstock, i.maxstock, p.unitid, i.price, i.cogs, p.images, p.taxable, p.taxfactor, p.parentid
                FROM products AS p
                INNER JOIN inventories AS i ON p.productid = i.productid AND i.locationid = @location
                WHERE p.productid = @id and p.isdeleted = false;
                """;
                var parameters = new DbParameter[]
                {
                db.CreateParameter("id", id, System.Data.DbType.Int16),
                db.CreateParameter("location", My.Application.GetCurrentLocationID(), System.Data.DbType.Int16)
                };
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        this.Product = Product.Create(reader);
                        this.skuTextBox.Text = Product.Sku;
                        this.nameTextBox.Text = Product.Name;
                        this.descriptionTextBox.Text = Product.Description;
                        this.stockTextBox.Text = Product.Stock.ToString("N0");
                        this.basicpriceTextBox.Text = Product.COGs.ToDecimalFormat();
                        this.priceTextBox.Text = Product.Price.ToDecimalFormat();
                        this.marginTextBox.Text = Product.Margin.ToString("N0");
                        this.minstockTextBox.Text = Product.MinStock.ToString();
                        this.maxstockTextBox.Text = Product.MaxStock.ToString();
                        this.typeComboBox.SelectedIndex = Product.Type;
                        this.isactiveCheckBox.Checked = Product.Active;
                        this.taxableCheckBox.Checked = Product.Taxable;
                        this.taxFactorNumericUpDown.Value = Product.TaxFactor;

                        unitID = this.Product.Unit;
                        categoryID = this.Product.Category;

                        var arrImage = Product.Images.Trim().Split(';');
                        foreach (var image in arrImage)
                        {
                            if (!string.IsNullOrWhiteSpace(image))
                            {
                                this.ImageURLs.Add(image.Trim());
                            }
                        }
                    }
                }, commandText, parameters);
            }            

            commandText = """
                SELECT unitid, name
                FROM units
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var unit = new Option<short>()
                    {
                        Id = reader.GetInt16(0),
                        Text = reader.GetString(1)
                    };
                    this.unitComboBox.Items.Add(unit);
                    if (unitID == unit.Id) this.unitComboBox.SelectedIndex = this.unitComboBox.Items.Count - 1;
                }
            }, commandText);
            commandText = """
                SELECT categoryid, name
                FROM categories
                ORDER BY name
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var cat = new Option<short>()
                    {
                        Id = reader.GetInt16(0),
                        Text = reader.GetString(1)
                    };
                    this.categoryComboBox.Items.Add(cat);
                    if (categoryID == cat.Id) this.categoryComboBox.SelectedIndex = this.categoryComboBox.Items.Count - 1;
                }
            }, commandText);
            if (this.ImageURLs.Count > 0)
            {
                await DisplayImage();
            }
        }
        private async Task<bool> CreateAsync()
        {
            if (this.Product is null) return false;
            if (await this.Product.IsBarcodeUsed(db)) return false;

            var commandText = """
            INSERT INTO products (
                name,
                description,
                sku,
                categoryid,
                producttype,
                unitid,
                images,
                taxable,
                taxfactor,
                creatorid,
                createddate
            )
            VALUES (
                @name,
                @description,
                @sku,
                @categoryid,
                @producttype,
                @unitid,
                @images,
                @taxable,
                @taxfactor,
                @creatorid,
                @createddate
            ) RETURNING productid;           
            """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("productid", this.Product.ID, DbType.Int16),
                db.CreateParameter("name", this.Product.Name, DbType.AnsiString),
                db.CreateParameter("description", this.Product.Description, DbType.AnsiString),
                db.CreateParameter("sku", this.Product.Sku, DbType.AnsiString),
                db.CreateParameter("categoryid", this.Product.Category, DbType.Int16),
                db.CreateParameter("producttype", this.Product.Type, DbType.Int16),
                db.CreateParameter("unitid", this.Product.Unit, DbType.Int16),
                db.CreateParameter("images", this.Product.Images, DbType.AnsiString),
                db.CreateParameter("taxable", this.Product.Taxable, DbType.Boolean),
                db.CreateParameter("taxfactor", this.Product.TaxFactor, DbType.Decimal),
                db.CreateParameter("creatorid", My.Application.GetCurrentUserID(), DbType.Int16),
                db.CreateParameter("createddate", DateTime.UtcNow, DbType.DateTime)
            };
            this.Product.ID = await db.ExecuteScalarAsync<short>(commandText, parameters);
            if (this.Product.ID <= 0)
            {
                MessageBox.Show("Gagal menyimpan data produk");
                return false;
            }
            commandText = """
                INSERT INTO inventories
                (locationid, productid, isactive, stock, minstock, maxstock, price, cogs)
                VALUES
                (@location, @productid, @isactive, 0, @minstock, @maxstock, @price, 0);
                """;
            parameters = new DbParameter[]
            {
                db.CreateParameter("location", My.Application.GetCurrentLocationID(), DbType.Int16),
                db.CreateParameter("productid", this.Product.ID, DbType.Int16),
                db.CreateParameter("isactive", this.Product.Active, DbType.Boolean),
                db.CreateParameter("minstock", this.Product.MinStock, DbType.Int16),
                db.CreateParameter("maxstock", this.Product.MaxStock, DbType.Int16),
                db.CreateParameter("price", this.Product.Price, DbType.Int64)
            };
            return await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        async Task<bool> UpdateAsync()
        {
            if (this.Product is null) return false;
            if (await this.Product.IsBarcodeUsed(db))
            {
                MessageBox.Show("Barcode '" + this.Product.Sku + "' sudah digunakan di produk lain, silakan gunakan kode barcode lainnya");
                return false;
            }
            var commandText = """
                UPDATE products
                SET name = @name,
                    description = @description,
                    sku = @sku,
                    categoryid = @categoryid,
                    producttype = @producttype,
                    unitid = @unitid,
                    images = @images,
                    taxable = @taxable,
                    taxfactor = @taxfactor
                WHERE productid = @productid AND isdeleted = false;
                UPDATE inventories
                SET isactive = @isactive,
                    minstock = @minstock,
                    maxstock = @maxstock,
                    price = @price
                WHERE productid = @productid AND locationid = @location
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("productid", this.Product.ID, DbType.Int16),
                db.CreateParameter("name", this.Product.Name, DbType.String),
                db.CreateParameter("description", this.Product.Description ?? string.Empty, DbType.String),
                db.CreateParameter("sku", this.Product.Sku ?? string.Empty, DbType.String),
                db.CreateParameter("categoryid", this.Product.Category, DbType.Int16),
                db.CreateParameter("producttype", this.Product.Type, DbType.Int16),
                db.CreateParameter("taxable", this.Product.Taxable, DbType.Boolean),
                db.CreateParameter("taxfactor", this.Product.TaxFactor, DbType.Decimal),
                db.CreateParameter("isactive", this.Product.Active, DbType.Boolean),
                db.CreateParameter("stock", this.Product.Stock, DbType.Int32),
                db.CreateParameter("minstock", this.Product.MinStock, DbType.Int16),
                db.CreateParameter("maxstock", this.Product.MaxStock, DbType.Int16),
                db.CreateParameter("unitid", this.Product.Unit, DbType.Int16),
                db.CreateParameter("price", this.Product.Price, DbType.Int64),
                db.CreateParameter("images", this.Product.Images ?? string.Empty, DbType.String),
                db.CreateParameter("location", My.Application.GetCurrentLocationID(), DbType.Int16)
            };

            return await db.ExecuteNonQueryAsync(commandText, parameters);
        }
        private async void loginButton_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (string.IsNullOrWhiteSpace(this.nameTextBox.Text))
            {
                this.nameTextBox.ShowError("Nama produk tidak boleh kosong");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.skuTextBox.Text))
            {
                this.skuTextBox.ShowError("SKU tidak boleh kosong");
                return;
            }
            if (categoryComboBox.SelectedItem is null)
            {
                this.categoryComboBox.ShowError("Kategori produk belum dipilih");
                return;
            }
            if (unitComboBox.SelectedItem is null)
            {
                unitComboBox.ShowError("Satuan produk belum dipilih");
                return;
            }
            short.TryParse(minstockTextBox.Text, out short minStock);
            short.TryParse(maxstockTextBox.Text, out short maxStock);

            if (this.Product is null) this.Product = new Product();
            this.Product.Name = this.nameTextBox.Text.Trim();
            this.Product.Sku = this.skuTextBox.Text.Trim();
            this.Product.Description = this.descriptionTextBox.Text.Trim();
            this.Product.Category = ((Option<short>)this.categoryComboBox.SelectedItem).Id;
            this.Product.Type = (short)this.typeComboBox.SelectedIndex;
            this.Product.Unit = ((Option<short>)this.unitComboBox.SelectedItem).Id;
            this.Product.MinStock = minstockTextBox.Text.ToInt16();
            this.Product.MaxStock = maxstockTextBox.Text.ToInt16();
            this.Product.Images = string.Join(";", this.ImageURLs);
            this.Product.Price = this.priceTextBox.Text.ToInt64();
            this.Product.Taxable = this.taxableCheckBox.Checked;
            this.Product.Active = this.isactiveCheckBox.Checked;
            this.Product.TaxFactor = this.taxFactorNumericUpDown.Value;

            if (this.Product.ID > 0)
            {
                if (await UpdateAsync())
                {
                    mainForm.DialogResult = DialogResult.OK;
                    mainForm.Close();
                }
            }
            else
            {
                if (await CreateAsync())
                {
                    mainForm.DialogResult = DialogResult.OK;
                    mainForm.Close();
                }
            }
        }
        private void BasicPriceTextBoxEnter(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            tb.Text = tb.Text.ToInt64().ToString();
        }
        private void BasicPriceTextBoxLeave(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            tb.Text = tb.Text.ToInt64().ToDecimalFormat();
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Title = "Select image";
            openDialog.Multiselect = false;
            openDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var bytes = File.ReadAllBytes(openDialog.FileName);
                var result = await WClient.UploadDocument("/documents/upload", bytes);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (this.Product != null)
                    {
                        this.ImageURLs.Add(result);
                        this.SelectedImage = this.ImageURLs.Count - 1;
                        await this.DisplayImage();
                    }
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await PreviousImage();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await NextImage();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.productImage is null) return;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = """
                    JPEG Image (*.jpg)|*.jpg|
                    PNG Image (*.png)|*.png|
                    Bitmap Image (*.bmp)|*.bmp|
                    GIF Image (*.gif)|*.gif
                    """;
                saveDialog.Title = "Simpan Gambar";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var ext = Path.GetExtension(saveDialog.FileName).ToLower();
                    var format = System.Drawing.Imaging.ImageFormat.Png;

                    switch (ext)
                    {
                        case ".jpg":
                        case ".jpeg":
                            format = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case ".bmp":
                            format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case ".gif":
                            format = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case ".png":
                            format = System.Drawing.Imaging.ImageFormat.Png; break;
                        default:
                            MessageBox.Show("Ekstensi tidak dikenali. Menyimpan sebagai PNG.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }

                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            this.productImage.Image.Save(stream, this.productImage.Image.RawFormat);
                            System.IO.File.WriteAllBytes(saveDialog.FileName, stream.ToArray());
                            MessageBox.Show("Gambar berhasil disimpan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menyimpan gambar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            if (this.ImageURLs.Count > 0) SelectedImage = this.ImageURLs.Count - 1;
            await DisplayImage();
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            if (this.ImageURLs.Count > 0 && SelectedImage > 0) SelectedImage = 0;
            await DisplayImage();
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            if (this.ImageURLs.Count > 0 && this.SelectedImage >= 0 && this.SelectedImage < this.ImageURLs.Count)
            {
                this.ImageURLs.RemoveAt(this.SelectedImage);
                if (this.ImageURLs.Count == 0)
                {
                    this.productImage.Image?.Dispose();
                    this.productImage.Image = null;
                    this.SelectedImage = 0;
                }
                else if (this.SelectedImage >= this.ImageURLs.Count)
                {
                    this.SelectedImage = this.ImageURLs.Count - 1;
                }
                await DisplayImage();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void priceTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
