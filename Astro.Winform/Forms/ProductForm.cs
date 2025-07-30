using Astro.Models;
using Astro.Text;
using Astro.ViewModels;
using Astro.Winform.Classes;
using Astro.Winform.Extensions;

namespace Astro.Winform.Forms
{
    public partial class ProductForm : Form
    {
        public ProductForm()
        {
            InitializeComponent();
        }
        public ProductViewModel? Model { get; set; } = null;
        private List<string> ImageURLs { get; } = new List<string>();
        private int SelectedImage = 0;
        private async Task DisplayImage(int index)
        {
            if (index < 0 || index >= this.ImageURLs.Count)
            {
                return;
            }
            using (var stream = await HttpClientSingleton.GetStreamAsync("/documents/download/" + this.ImageURLs[index]))
            {
                if (stream != null)
                {
                    this.productImage.Image?.Dispose();
                    this.productImage.Image = Image.FromStream(stream);
                }
            }
        }
        private async void ProductForm_Load(object sender, EventArgs e)
        {
            if (this.Model != null)
            {
                this.categoryComboBox.DataSource = this.Model.Categories;
                this.unitComboBox.DataSource = this.Model.Units;
                if (this.Model.Product != null)
                {
                    this.skuTextBox.Text = this.Model.Product.Sku;
                    this.nameTextBox.Text = this.Model.Product.Name;
                    this.descriptionTextBox.Text = this.Model.Product.Description;
                    this.stockTextBox.Text = this.Model.Product.Stock.ToString("N0");
                    this.basicpriceTextBox.Text = this.Model.Product.CostAverage.ToDecimalFormat();
                    this.priceTextBox.Text = this.Model.Product.Price.ToDecimalFormat();
                    this.minstockTextBox.Text = this.Model.Product.MinStock.ToString();
                    this.maxstockTextBox.Text = this.Model.Product.MaxStock.ToString();
                    this.categoryComboBox.SelectedItem = this.Model.Categories.FirstOrDefault(c => c.Id == this.Model.Product.Category);
                    this.unitComboBox.SelectedItem = this.Model.Units.FirstOrDefault(u => u.Id == this.Model.Product.Unit);

                    var arrImage = this.Model.Product.Images.Split(';');
                    foreach (var image in arrImage)
                    {
                        if (!string.IsNullOrWhiteSpace(image))
                        {
                            this.ImageURLs.Add(image);
                        }
                    }
                    await DisplayImage(0);
                }
            }
            this.unitComboBox.DisplayMember = "Text";
            this.unitComboBox.ValueMember = "Id";

            this.categoryComboBox.DisplayMember = "Text";
            this.categoryComboBox.ValueMember = "Id";
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            if (this.Model is null) return;

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

            var product = this.Model.Product != null ? this.Model.Product : new Product();
            product.Name = this.nameTextBox.Text.Trim();
            product.Description = this.descriptionTextBox.Text.Trim();
            product.Sku = this.skuTextBox.Text.Trim();
            product.Category = (short)((Option)this.categoryComboBox.SelectedItem).Id;
            product.Unit = (short)((Option)this.unitComboBox.SelectedItem).Id;
            int.TryParse(stockTextBox.Text, out int productStock);
            product.Stock = productStock;
            product.Price = priceTextBox.Text.ToInt64();
            product.CostAverage = basicpriceTextBox.Text.ToInt64();
            short.TryParse(minstockTextBox.Text, out short minStock);
            short.TryParse(maxstockTextBox.Text, out short maxStock);
            product.MinStock = minStock;
            product.MaxStock = maxStock;
            product.Images = string.Join(";", this.ImageURLs);

            var result = product.ID > 0 ? await HttpClientSingleton.PutAsync("/data/products", product.ToString()) : await HttpClientSingleton.PostAsync("/data/products", product.ToString());
            var commonResult = CommonResult.Create(result);
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
                var result = await HttpClientSingleton.UploadDocument("/documents/upload", bytes);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (this.Model != null)
                    {
                        this.ImageURLs.Add(result);
                        await this.DisplayImage(this.ImageURLs.Count - 1);
                    }
                }
            }
        }
    }
}
