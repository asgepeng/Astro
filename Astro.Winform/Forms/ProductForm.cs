using Astro.Models;
using Astro.Text;
using Astro.ViewModels;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;
using System.Reflection;

namespace Astro.Winform.Forms
{
    public partial class ProductForm : UserControl
    {
        public ProductForm()
        {
            InitializeComponent();

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
            var productId = this.Tag != null ? this.Tag.ToString() : "0";
            this.DisableAllControls(true);
            using (var stream = await WClient.GetStreamAsync("/data/products/" + productId))
            using (var reader = new Astro.Streams.Reader(stream))
            {
                if (stream is null || stream.Length == 0) return;
                if (reader.ReadBoolean())
                {
                    this.saveButton.Text = "Simpan Perubahan";
                    this.titleLabel.Text = "Detil Produk";

                    Product = Product.Create(reader);

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

                    var arrImage = Product.Images.Trim().Split(';');
                    foreach (var image in arrImage)
                    {
                        if (!string.IsNullOrWhiteSpace(image))
                        {
                            this.ImageURLs.Add(image.Trim());
                        }
                    }
                }

                var iCount = reader.ReadInt32();
                while (iCount > 0)
                {
                    var categoryItem = new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    };
                    this.categoryComboBox.Items.Add(categoryItem);
                    if (Product != null && Product.Category == categoryItem.Id) this.categoryComboBox.SelectedIndex = this.categoryComboBox.Items.Count - 1;
                    iCount--;
                }
                iCount = reader.ReadInt32();
                while (iCount > 0)
                {
                    var unitItem = new Option()
                    {
                        Id = reader.ReadInt16(),
                        Text = reader.ReadString()
                    };
                    this.unitComboBox.Items.Add(unitItem);
                    if (Product != null && Product.Unit == unitItem.Id) this.unitComboBox.SelectedIndex = this.unitComboBox.Items.Count - 1;
                    iCount--;
                }
                if (this.ImageURLs.Count > 0)
                {
                    await DisplayImage();
                }
                this.DisableAllControls(false);
                this.nameTextBox.Focus();
            }
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            var mainForm = this.FindForm();
            if (mainForm is null) return;

            if (string.IsNullOrWhiteSpace(this.nameTextBox.Text))
            {
                MessageBox.Show("Product name is required", "Product name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.skuTextBox.Text))
            {
                MessageBox.Show("Sku is required", "Sku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                skuTextBox.Focus();
                return;
            }
            if (categoryComboBox.SelectedItem is null)
            {
                MessageBox.Show("Product category is not selected", "Category", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                categoryComboBox.Focus();
                return;
            }
            if (unitComboBox.SelectedItem is null)
            {
                MessageBox.Show("Unit not yet selected", "Unit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                unitComboBox.Focus();
                return;
            }
            short.TryParse(minstockTextBox.Text, out short minStock);
            short.TryParse(maxstockTextBox.Text, out short maxStock);

            var productId = this.Product != null ? this.Product.ID : (short)0;
            using (var writer = new Astro.Streams.Writer())
            {
                writer.WriteByte(0x01);
                writer.WriteInt16(productId);
                writer.WriteString(this.nameTextBox.Text.Trim());
                writer.WriteString(this.descriptionTextBox.Text.Trim());
                writer.WriteString(this.skuTextBox.Text.Trim());
                writer.WriteInt16((short)((Option)this.categoryComboBox.SelectedItem).Id);
                writer.WriteInt16((short)typeComboBox.SelectedIndex);
                writer.WriteInt16((short)((Option)this.unitComboBox.SelectedItem).Id);
                writer.WriteString(string.Join(";", this.ImageURLs));
                writer.WriteBoolean(this.taxableCheckBox.Checked);
                writer.WriteDecimal(this.taxFactorNumericUpDown.Value);

                writer.WriteInt16(minStock);
                writer.WriteInt16(maxStock);
                writer.WriteInt64(this.priceTextBox.Text.ToInt64());
                writer.WriteBoolean(this.isactiveCheckBox.Checked);

                var result = productId == 0 ? await WClient.PostAsync("/data/products", writer.ToArray()) : await WClient.PutAsync("/data/products", writer.ToArray());
                var commonResult = CommonResult.Create(result);
                if (commonResult != null)
                {
                    if (commonResult.Success)
                    {
                        if (short.TryParse(commonResult.Message, out short newProductID))
                        {
                            this.Tag = newProductID;
                        }
                        else
                        {
                            this.Tag = null;
                        }
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
